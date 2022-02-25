function testInventory(%c)
{
    %client = %c;
    %inventory = Inventory_Create();
    %empty = InventorySlotItemData_Create("Empty");
    %space = InventorySpace_Create("Hands",2,%empty,2,"empty","empty","empty","handsEquip");
    talk(%space);
    %inventory.add(%space);
    Inventory_Push(%client.player,%inventory);

    return %Inventory;
}

function createOffHandImages()
{
    %group = DataBlockGroup;
    %count = %group.getCount();
    for(%i = 0; %i < %count; %i++)
    {
        %dataBlock = %group.getObject(%i);
        if(%dataBlock.getClassName() $= "ShapeBaseImageData")
        {
            %newDatablock = "datablock ShapeBaseImageData(" @ %datablock.getName() @ "OffHand" @ ":" @ %datablock.getName() @ "){mountPoint = 1;};";
            eval(%newDatablock);
        }
    }
}

function empty()
{

}

function handsEquip(%client,%space,%slot,%equip)
{
    %player = %client.player;
    %dominant = "";
    %nonDominant = "offHand";

    switch(%slot)
    {
        case 0:
            if(%equip)
            {
                //equip dominant hand
                %player.mountImage(%space.getValue(%slot).image,0);
            }
            else
            {
                %player.unmountImage(0);
            }
        case 1:
            if(%equip)
            {
                //equip nondominant hand
                %player.mountImage(%space.getValue(%slot).image @ "OffHand",1);
            }
            else
            {
                %player.unmountImage(1);
            }
    }
}

function altAttackLoop(%player)
{
    cancel(%player.altAttackLoop);
    %player.setImageTrigger(1,true);
    %player.altAttackLoop = schedule(33,%player,"altAttackLoop",%player);
}

package DespairInventory
{
    function Armor::onTrigger(%this, %obj, %triggerNum, %val)
    {
        %client = %obj.client;
        if(isObject(%client))
        {
            if(%client.getClassName() $= "GameConnection")
            {
                if(%triggerNum == 4 && %obj.currTool == -1)
                {
                    switch(%val)
                    {
                        case 1:
                            altAttackLoop(%obj);
                        case 0:
                            cancel(%obj.altAttackLoop);
                    }
                    
                }
            }
        }

        Parent::onTrigger(%this, %obj, %triggerNum, %val);
    }

};
activatePackage("DespairInventory");