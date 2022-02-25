function Inventory_Create(%display)
{
    %list = new ScriptObject()
	{
		superClass = "List";
        class = "Inventory";

        display = %display;
	};
    %list.subscribedPlayers = new SimSet();
    return %list;
}

function InventorySlotItemData_Create(%name,%unselectable,%icon,%color)
{
    %dbName = %name @ "Item";
    %function = "datablock ItemData(" @ %dbName @ "){category = \"Tools\";uiName = %name;iconName = %icon;oColorShift = %color !$= \"\";colorShiftColor = %color;unselectable = %unselectable;};";
    eval(%function);
    %db = %dbName;
    if(isObject(%dbName))
    {
        return %dbName.getId();
    }
    return "";
}

function InventorySpace_Create(%name,%capacity,%emptySlotItem,%emptySlotMin,%select,%drop,%use,%equip)
{
    %slot = new ScriptObject(%name)
	{
        //inheritance
		superClass = "List";
        class = "InventorySpace";

        //display and capacity
        capacity = %capacity;
        emptySlotItem = %emptySlotItem;
        emptySlotMin = %emptySlotMin;
        
        //callbacks
        select = %select;
        drop = %drop;
        use = %primUse;
        equip = %equip;
	};

    return %slot;
}

function InventorySpace::CurrentCapacity(%space)
{
    //count all the size of the held items
    %capacity = %space.capacity;
    %count = %space.getCount();
    for(%i = 0; %i < %count; %i++)
    {
        %capacity -= %space.getValue(%i).size;
    }

    return %capacity;
}

function InventorySpace::CanEquip(%space,%item)
{
    %size = %item.size;
    return %space.CurrentCapacity() >= %size;
}

function InventorySpace::Equip(%space,%item)
{
    if(%space.CanEquip(%item) && isObject(%item))
    {
        %space.add(%item);

        %space.inventory.Display(false);
        %slot = %space.getcount() - 1;

        %callback = %space.equip;
        talk(%callback SPC "e");
        if(isFunction(%callback))
        {
            %players = %space.inventory.subscribedPlayers;
            %count = %players.getCount();
            for(%i = 0; %i < %count; %i++)
            {
                %client = %players.getObject(%i).client;
                if(isObject(%client))
                {
                    call(%callback,%client,%space,%slot,1);
                }
            }
        }
    }
}

function InventorySpace::Unequip(%space,%item)
{
    %item = %space.FindValue(%item);
    if(%item >= 0)
    {
        %slot = %space.getcount() - 1;

        %callback = %space.equip;
        if(isFunction(%callback))
        {
            %players = %space.inventory.subscribedPlayers;
            %count = %players.getCount();
            for(%i = 0; %i < %count; %i++)
            {
                %client = %players.getObject(%i).client;
                if(isObject(%client))
                {
                    call(%callback,%client,%space,%slot,0);
                }
            }
        }

        %space.remove(%item);
        %space.inventory.Display(false);
    }
}

function Inventory_Push(%player,%inventory)
{
    if(!isObject(%player.inventoryStack))
    {
        %player.InventoryStack = List_NewList();
    }
    //add player to the update list
    %inventory.subscribedPlayers.add(%player);

    %stack = %player.inventoryStack;
    %stack.add(%inventory);

    %client = %player.client;
    if(isObject(%client))
    {
        %client.DisplayInventory(true);  
    }
}

function Inventory_Pop(%player)
{
    if(!isObject(%player.inventoryStack))
    {
        %player.InventoryStack = List_NewList();
    }

    //remove player from the update list
    Inventory_GetTop(%player).subscribedPlayers.remove(%player);

    %stack = %player.inventoryStack;
    %stack.get(%stack.getCount() - 1);

    %client = %player.client;
    if(isObject(%client))
    {
        %client.DisplayInventory(true);  
    }
}

function Inventory_GetTop(%player)
{
    if(!isObject(%player.inventoryStack))
    {
        %player.InventoryStack = List_NewList();
    }

    %stack = %player.inventoryStack;
    return %stack.getValue(%stack.getCount() - 1);
}

function Inventory_DisplayItem(%client,%item,%slot)
{
    %unselectable = %item.unselectable;
    if(%unselectable)
    {
        messageClient(%client, 'MsgItemPickup', "", %slot, %item.getName(), true);
    }
    else
    {
        
        messageClient(%client, 'MsgItemPickup', "", %slot, %item.getId(), true);
    }
}

function Inventory::Add(%list,%Value,%row,%tag)
{
    %value.inventory = %list;
    return Parent::Add(%list,%Value,%row,%tag);
}

function Inventory_GetSelectedSpace(%player)
{
    %tool = %player.currTool;
    %space = %player.slotSpace[%tool];
    %item = %player.slotIndex[%tool];

    return %space SPC %item;
}

function Inventory::Display(%inventory,%silent)
{
    %players = %inventory.subscribedPlayers;
    %count = %players.getCount();
    for(%i = 0; %i < %count; %i++)
    {
        %client = %players.getObject(%i).client;
        if(isObject(%client))
        {
            %client.DisplayInventory(%silent);  
        }
    }
}

function GameConnection::DisplayInventory(%client,%silent)
{
    %player = %client.player;
    if(isObject(%player))
    {
        %inventory = Inventory_GetTop(%player);
        %callback = %inventory.display;
        
        if(!isFunction(%callback))
        {
            %callback = "Inventory_DefaultDisplay";
        }

        messageClient(%client, 'MsgItemPickup', "", -1, 0, %silent);
        call(%callback,%client,%inventory,%silent);
    }
}

package Inventory
{
    function ServerCmdUseTool(%client, %slot)
    {
        %player = %client.player;
        if(isObject(%player))
        {
            %player.currTool = %slot;
            %word = Inventory_GetSelectedSpace(%player);
            %space = getWord(%word,0);
            %slot2 = getWord(%word,1);

            %callback = %space.select;
            if(isFunction(%callback))
            {
                call(%callback,%client,%space,%slot2,1);
                return;
            }
        }
        Parent::ServerCmdUseTool(%client, %slot);
    }

    function ServerCmdUnUseTool(%client)
    {
        %player = %client.player;
        if(isObject(%player))
        {
            %word = Inventory_GetSelectedSpace(%player);
            %space = getWord(%word,0);
            %slot = getWord(%word,1);

            %player.currTool = -1;

            %callback = %space.select;
            if(isFunction(%callback))
            {
                call(%callback,%client,%space,%slot,0);
                return;
            }
        }

        Parent::ServerCmdUnUseTool(%client);
    }

    function ServerCmdDropTool(%client, %position)
    {
        %player = %client.player;
        if(isObject(%player))
        {
            %word = Inventory_GetSelectedSpace(%player);
            %space = getWord(%word,0);
            %slot = getWord(%word,1);

            %callback = %space.drop;
            if(isFunction(%callback))
            {
                call(%callback,%client,%space,%slot);
                return;
            }
        }

        Parent::ServerCmdDropTool(%client, %position);
    }

    function Armor::onTrigger(%this, %obj, %triggerNum, %val)
    {
        %client = %obj.client;
        if(isObject(%client))
        {
            if(%client.getClassName() $= "GameConnection")
            {
                %word = Inventory_GetSelectedSpace(%obj);
                %space = getWord(%word,0);
                %slot = getWord(%word,1);
                %callback = %space.use;

                switch(%triggerNum)
                {
                    case 0:
                        %type = 0;
                    case 4:
                        %type = 1;
                }

                if(isFunction(%callback))
                {
                    call(%callback,%client,%space,%slot,%type,%val);
                    return;
                }
            }
        }
        
        Parent::onTrigger(%this, %obj, %triggerNum, %val);
    }

};
activatePackage("Inventory");

//DISPLAY FUNCTIONS

function Inventory_DefaultDisplay(%client,%inventory,%silent)
{
    %player = %client.player;

    %slot = 0;
    %count = %inventory.getCount();
    for(%i = 0; %i < %count; %i++)
    {
        %space = %inventory.getValue(%i);
        %slotCount = %space.getCount();
        for(%j = 0; %j < %slotCount; %j++)
        {
            %item = %space.getValue(%j);
            Inventory_DisplayItem(%client,%item,%slot,%silent);

            //sets lookup values for later
            %player.slotSpace[%slot] = %space;
            %player.slotIndex[%slot] = %j;
            %slot++;
        }

        %emptyCount = getMin(%space.emptySlotMin,%space.CurrentCapacity());
        %emptyItem = %space.emptySlotItem;
        for(%j = 0; %j < %emptyCount; %j++)
        {
            Inventory_DisplayItem(%client,%emptyItem,%slot,%silent);
            
            //sets lookup values for later
            %player.slotSpace[%slot] = %space;
            %player.slotIndex[%slot] = "";
            %slot++;
        }
    }

    %maxTools = %client.player.getDatablock().maxTools;
    for(%i = %slot; %i < %maxTools; %i++)
    {
        messageClient(%client, 'MsgItemPickup', "", %i, 0, true);
        //sets lookup values for later
        %player.slotSpace[%i] = "";
        %player.slotIndex[%i] = "";
    }
}