$DefaultInventoryName = "DespairInventory";

function testInventory(%c)
{
    %client = %c;
    %inventory = Inventory_Create($DefaultInventoryName);
    %emptyHand = InventorySlotItemData_Create("Hand");
    %emptyPocket = InventorySlotItemData_Create("Pocket");
    %inventory.add(InventorySpace_Create("Hands",2,%emptyHand,2,"handsSelect","inventoryMove","handsEquip","InventoryDrop"));
    %inventory.add(InventorySpace_Create("Pocket",2,%emptyPocket,2,"inventorySelect","inventoryMove","empty","InventoryDrop"));
    Inventory_Push(%client.player,%inventory);

    return %Inventory;
}

datablock PlayerData(HandPlayer)
{
    shapeFile = "base/data/shapes/empty.dts";
    boundingBox = vectorScale("20 20 20", 4);
};

function gunImage::onFire(%this,%obj,%slot)
{
	if(%obj.isHandBot)
    {
        %player = %obj.getObjectMount();
        
        if(%player.rightHandBot == %obj)
        {
            %obj.playThread(2, shiftAway);
        }
        else
        {
            %obj.playThread(3, leftrecoil);
        }
    }
		
	Parent::onFire(%this,%obj,%slot);	
}


function empty()
{

}

function InventoryDrop(%client,%space,%slot)
{
    %player = %client.player;
    if(isObject(%player) && Inventory_GetTop(%player).name $= $DefaultInventoryName)
    {   
        %word = Inventory_GetSelectedSpace(%player);
        %space = getWord(%word,0);
        %slot = getWord(%word,1);
        %item = %space.getValue(%slot);
        if(isObject(%item))
        {
            %space.removeSlot(%slot);

            %zScale = getWord (%player.getScale (), 2);
            %muzzlepoint = VectorAdd (%player.getPosition (), "0 0" SPC 1.5 * %zScale);
            %muzzlevector = %player.getEyeVector ();
            %muzzlepoint = VectorAdd (%muzzlepoint, %muzzlevector);
            %playerRot = rotFromTransform (%player.getTransform ());
            %thrownItem = new Item ("")
            {
                dataBlock = %item;
            };
            %thrownItem.setScale (%player.getScale ());
            MissionCleanup.add (%thrownItem);
            %thrownItem.setTransform (%muzzlepoint @ " " @ %playerRot);
            %thrownItem.setVelocity (VectorScale (%muzzlevector, 20 * %zScale));
            %thrownItem.schedulePop ();
            %thrownItem.miniGame = %client.miniGame;
            %thrownItem.bl_id = %client.getBLID ();
            %thrownItem.setCollisionTimeout (%player);
        }
    }
}

function inventoryMove(%client,%space,%slot,%type,%val)
{
    %player = %client.player;
    if(!isObject(%player))
    {
        return;
    }

    if(%val)
    {
        %item = %space.getValue(%slot);
        //main fire
        if(%type)
        {
            if(%item !$= "")
            {
                //item selected for move
                %player.moveitemBufferItem = %item;
                %player.moveitemBufferSpace = %space;
                %player.moveitemBufferSlot = %slot;
            }
        }
        //secondary fire
        else
        {
            %bufferItem = %player.moveitemBufferItem;
            %bufferSpace = %player.moveitemBufferSpace;
            %bufferSlot = %player.moveitemBufferSlot;
            //is there an item to move?
            if(%bufferItem !$= "" && %bufferItem == %bufferSpace.getValue(%bufferSlot))
            {
                

                //swap slots
                %success = %space.CanEquip(%bufferItem,%item.size) && %bufferSpace.CanEquip(%item,%bufferItem.size);
                if(%success)
                {
                    %bufferSpace.removeSlot(%bufferSlot,true);
                    %bufferSpace.setSlot(%bufferSlot,%item,true);
                    %space.removeSlot(%slot,true);
                    %space.setSlot(%slot,%bufferItem,true);
                    %player.moveitemBufferItem = "";
                }
            }
        }
    }
}

function inventorySelect(%client,%space,%slot,%open)
{
    %player = %client.player;
    if(%open)
    {
        %player.itemsDisabled = true;
    }
    else
    {
        %player.itemsDisabled = false;
    }
}

function handsSelect(%client,%space,%slot,%open)
{
    %player = %client.player;
    if(%open)
    {
        %player.itemsDisabled = true;
    }
    else
    {
        %player.itemsDisabled = false;

        //call to get hands back
        //handsEquip(%client,"Hands",0,true);
    }
}

function handsEquip(%client,%space,%slot,%equip)
{
    %player = %client.player;

    if(!isObject(%player.rightHandBot))
    {
        %player.rightHandBot = makeHandBot(%player,0);

        %player.leftHandBot = makeHandBot(%player,1);
    }

    %rightImage = %space.getValue(0).image;
    %leftImage = %space.getValue(1).image;

    if(%leftImage.armReady)
    {
        if(%rightImage.armReady)
        {
            %player.playThread(0,"ArmReadyBoth");
        }
        else
        {
            %player.playThread(0,"ArmReadyLeft");
        }
    }
    else if(%rightImage.armReady)
    {   
        %player.playThread(0,"ArmReadyRight");
    }
    else
    {
        %player.playThread(0,"root");
    }

    if(%rightImage !$= "")
    {
        %player.rightHandBot.mountImage(%rightImage,0);
    }
    else
    {
        %player.rightHandBot.unmountImage(0);
    }

    if(%leftImage !$= "")
    {
        %player.leftHandBot.mountImage(%leftImage,0);
    }
    else
    {
        %player.leftHandBot.unmountImage(0);
    }
    
}

function makeHandBot(%player,%slot)
{
    %client = %player.client;
    if(isObject(%client))
    {
        %bot = new AiPlayer(){dataBlock = "HandPlayer";isHandBot = true;};
        %bot.kill();
        //if i set this before the bot is killed it will kill the client lol
        %bot.client = %client;
        %player.mountObject(%bot,%slot);
    }
    return %bot;
}

function getMountedObjectNode(%obj,%target)
{
    %count = %Obj.getmountedObjectCount();
    for(%i = 0; %i < %count; %i++)
    {
        %currObj = %obj.getMountedObject(%i);
        if(%target == %currObj)
        {
            return %obj.getMountedObjectNode(%i);
        }
    }
    return -1;
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
                if(!%obj.itemsDisabled && Inventory_GetTop(%obj).name $= $DefaultInventoryName)
                {   
                    
                    if(%triggerNum == 0)
                    {
                        %bot = %obj.leftHandBot;
                    }
                    
                    if(%triggerNum == 4)
                    {
                        %bot = %obj.rightHandBot;
                    }

                    if(%bot !$= "")
                    {
                        %bot.setImageTrigger(0,%val);
                        return;
                    }
                    //try to pickup an item
                    else
                    {

                    }
                }
            }
        }
        Parent::onTrigger(%this, %obj, %triggerNum, %val);
    }

    function Player::PlayThread(%player,%slot,%thread)
    {
        %object = %player.getObjectMount();
        if(isObject(%object) && %player.isHandBot)
        {
            %object.playThread(%slot,%thread);
        }
        else
        {
            Parent::PlayThread(%player,%slot,%thread);
        }
    }

    function Player::getMuzzleVector(%player,%slot)
    {
        %object = %player.getObjectMount();
        if(isObject(%object) && %player.isHandBot)
        {
            if(%object.isFirstPerson() && !%player.getMountedImage(%slot).melee)
            {
                return %player.getCorrectedAimVector(%slot);
            }
            else
            {
                return %object.getMuzzleVector(%slot);
            }
        }
        else
        {
            return Parent::getMuzzleVector(%player,%slot);
        }
    }

    function Player::getForwardVector(%player)
    {
        %object = %player.getObjectMount();
        if(isObject(%object) && %player.isHandBot)
        {
            return %object.getForwardVector();
        }
        else
        {
            return Parent::getForwardVector(%player);
        }
    }

    function Player::getTransform(%player)
    {
        %object = %player.getObjectMount();
        if(isObject(%object) && %player.isHandBot)
        {
            return %object.getTransform();
        }
        else
        {
            return Parent::getTransform(%player);
        }
    }

    function Player::getEyeTransform(%player)
    {
        %object = %player.getObjectMount();
        if(isObject(%object) && %player.isHandBot)
        {
            return %object.getEyeTransform();
        }
        else
        {
            return Parent::getEyeTransform(%player);
        }
    }

    function Player::getEyePoint(%player)
    {
        %object = %player.getObjectMount();
        if(isObject(%object) && %player.isHandBot)
        {
            return %object.getEyePoint();
        }
        else
        {
            return Parent::getEyePoint(%player);
        }
    }

    function Player::getEyeVector(%player)
    {
        %object = %player.getObjectMount();
        if(isObject(%object) && %player.isHandBot)
        {
            return %object.getEyeVector();
        }
        else
        {
            return Parent::getEyeVector(%player);
        }
    }

    function Player::isFirstPerson(%player)
    {
        %object = %player.getObjectMount();
        if(isObject(%object) && %player.isHandBot)
        {
            return %object.isFirstPerson();
        }
        else
        {
            return Parent::isFirstPerson(%player);
        }
    }

    function Armor::onCollision (%this, %obj, %col, %vec, %speed)
    {
        if (%obj.getState () $= "Dead")
        {
            return;
        }
        if (%col.getDamagePercent () >= 1)
        {
            return;
        }
        %colClassName = %col.getClassName ();
        if (%colClassName $= "Item")
        {
            %client = %obj.client;
            %colData = %col.getDataBlock ();
            %i = 0;
            while (%i < %this.maxTools)
            {
                if (%obj.tool[%i] == %colData)
                {
                    %obj.pickup (%col);
                    break;
                }
                %i += 1;
            }
            
        }

        parent::onCollision (%this, %obj, %col, %vec, %speed);
    }

    //getting items
    function ItemData::onPickup(%this, %obj, %user, %amount)
    {
        if(Inventory_GetTop(%user).name $= $DefaultInventoryName)
        {
            if (%obj.canPickup == 0)
            {
                return;
            }
            %player = %user;
            %client = %player.client;
            %data = %player.getDataBlock ();
            if (!isObject (%client))
            {
                return;
            }
            %mg = %client.miniGame;
            if (isObject (%mg))
            {
                if (%mg.WeaponDamage == 1)
                {
                    if (getSimTime () - %client.lastF8Time < 5000)
                    {
                        return;
                    }
                }
            }
            %canUse = 1;
            if (miniGameCanUse (%player, %obj) == 1)
            {
                %canUse = 1;
            }
            if (miniGameCanUse (%player, %obj) == 0)
            {
                %canUse = 0;
            }
            if (!%canUse)
            {
                if (isObject (%obj.spawnBrick))
                {
                    %ownerName = %obj.spawnBrick.getGroup ().name;
                }
                %msg = %ownerName @ " does not trust you enough to use this item.";
                if ($lastError == $LastError::Trust)
                {
                    %msg = %ownerName @ " does not trust you enough to use this item.";
                }
                else if ($lastError == $LastError::MiniGameDifferent)
                {
                    if (isObject (%client.miniGame))
                    {
                        %msg = "This item is not part of the mini-game.";
                    }
                    else 
                    {
                        %msg = "This item is part of a mini-game.";
                    }
                }
                else if ($lastError == $LastError::MiniGameNotYours)
                {
                    %msg = "You do not own this item.";
                }
                else if ($lastError == $LastError::NotInMiniGame)
                {
                    %msg = "This item is not part of the mini-game.";
                }
                commandToClient (%client, 'CenterPrint', %msg, 1);
                return;
            }
            
            %success = Inventory_GetTop(%user).EquipTo("Hands",%this);
            if(!%success)
            {
                %success = Inventory_GetTop(%user).EquipTo("pocket",%this);
            }

            if(%success)
            {
                if (%obj.isStatic ())
                {
                    %obj.Respawn ();
                }
                else 
                {
                    %obj.delete ();
                }
            }
        }
        else
        {
            %success = Parent::onPickup(%this,%obj,%user,%amount);
        }

        return %success;
    }

    function Player::RemoveBody (%obj)
    {
        if(isObject(%obj.rightHandBot))
        {
            %obj.rightHandBot.delete();
            %obj.leftHandBot.delete();
        }
        return Parent::RemoveBody (%obj);
    }
};
deactivatePackage("DespairInventory");
activatePackage("DespairInventory");

function Player::getEyeTransformWithoutHeadZ(%this)
{
    %angle = mAcos(getWord(%this.getEyeVector(), 2)) - 1.5708;
    %axisA = getWords(%this.getTransform(), 3, 6);
    %axisB = vectorCross(%this.getForwardVector(), %this.getUpVector()) SPC %angle;

    return MatrixMultiply(%this.getEyePoint() SPC %axisB, "0 0 0" SPC %axisA);
}

function Player::getCorrectedAimVector(%this, %slot)
{
    %pullInD = 6.0;
    %maxAdjD = 500;

    %aheadVec = "0 " @ %maxAdjD @ " 0";
    %eyeMat = %this.getEyeTransformWithoutHeadZ();
    %eyePos = getWords(%eyeMat, 0, 2);
    %aheadVec = MatrixMulVector(%eyeMat, %aheadVec);
    %aheadPoint = vectorAdd(%eyePos, %aheadVec);

    %muzzlePos = %this.getMuzzlePoint(%slot);

    %ray = containerRayCast(%eyePos, %aheadPoint, $TypeMasks::All, %this.getObjectMount(),%this.getObjectMount().getObjectMount());
    if(%ray)
    {
        %collidePoint = getWords(%ray, 1, 3);
        %collideVector = vectorSub(%collidePoint, %eyePos);
    }
    else
    {
        %collidePoint = %aheadPoint;
        %collideVector = vectorSub(%collidePoint, %eyePos);
    }

    %len = vectorLen(%collideVector);
    if(%len < %pullInD && %len > 0.2)
    {
        %mid = %pullInD;
        %collideVector = vectorScale(%collideVector, %mid/%len);
        %collidePoint = vectorAdd(%eyePos, %collideVector);
    }

    %muzzleToCollide = vectorSub(%collidePoint, %muzzlePos);
    %len = vectorLen(%muzzleToCollide);
    if(%len > 0.2)
    {
        %muzzleToCollide = vectorScale(%muzzleToCollide, 1/%len);
        return %muzzleToCollide;
    }
    return %this.getMuzzleVector(%slot);
}

function HandPlayer::Damage (%data, %this, %sourceObject, %position, %damage, %damageType)
{
    Armor::Damage (%data, %this, %sourceObject, %position, %damage, %damageType);
}

function HandPlayer::onDisabled(%this, %obj, %state)
{

}

//overwrite
function WeaponImage::onFire (%this, %obj, %slot)
{
	%obj.hasShotOnce = 1;
	if (%this.minShotTime > 0)
	{
		if (getSimTime () - %obj.lastFireTime < %this.minShotTime)
		{
			return;
		}
		%obj.lastFireTime = getSimTime ();
	}
	%client = %obj.client;
	if (isObject (%client.miniGame))
	{
		if (getSimTime () - %client.lastF8Time < 3000)
		{
			return;
		}
	}
	%projectile = %this.Projectile;
	%dataMuzzleVelocity = %projectile.muzzleVelocity;
	if (%this.melee)
	{
		%initPos = %obj.getEyeTransform ();
		%muzzlevector = %obj.getMuzzleVector (%slot);
		%start = %initPos;
		%vec = VectorScale (%muzzlevector, 20);
		%end = VectorAdd (%start, %vec);
		%mask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::StaticObjectType;
        if (%obj.isMounted ())
        {
            %mount = %obj.getObjectMount ();
        }
        else 
        {
            %mount = 0;
        }
		%raycast = containerRayCast (%start, %end, %mask, %obj,%mount);
		if (%raycast)
		{
			%hitPos = posFromRaycast (%raycast);
			%eyeDiff = VectorLen (VectorSub (%start, %hitPos));
			%muzzlepoint = %obj.getMuzzlePoint (%slot);
			%muzzleDiff = VectorLen (VectorSub (%muzzlepoint, %hitPos));
			%ratio = %eyeDiff / %muzzleDiff;
			%dataMuzzleVelocity *= %ratio;
		}
	}
	else 
	{
		%initPos = %obj.getMuzzlePoint (%slot);
		%muzzlevector = %obj.getMuzzleVector (%slot);
		if (%obj.isFirstPerson ())
		{
			%start = %obj.getEyePoint ();
			%eyeVec = VectorScale (%obj.getEyeVector (), 5);
			%end = VectorAdd (%start, %eyeVec);
			%mask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::StaticShapeObjectType | $TypeMasks::StaticObjectType;
			if (%obj.isMounted ())
			{
				%mount = %obj.getObjectMount ();
			}
			else 
			{
				%mount = 0;
			}
			%raycast = containerRayCast (%start, %end, %mask, %obj, %mount);
			if (%raycast)
			{
				%eyeTarget = posFromRaycast (%raycast);
				%eyeTargetVec = VectorSub (%eyeTarget, %start);
				%eyeToMuzzle = VectorSub (%start, %initPos);
				if (VectorLen (%eyeTargetVec) < 3.1)
				{
					%muzzlevector = %obj.getEyeVector ();
					%initPos = %start;
				}
			}
		}
	}
	%inheritFactor = %projectile.velInheritFactor;
	%objectVelocity = %obj.getVelocity ();
	%eyeVector = %obj.getEyeVector ();
	%rawMuzzleVector = %obj.getMuzzleVector (%slot);
	%dot = VectorDot (%eyeVector, %rawMuzzleVector);
	if (%dot < 0.6)
	{
		if (VectorLen (%objectVelocity) < 14)
		{
			%inheritFactor = 0;
		}
	}
	%gunVel = VectorScale (%dataMuzzleVelocity, getWord (%obj.getScale (), 2));
	%muzzleVelocity = VectorAdd (VectorScale (%muzzlevector, %gunVel), VectorScale (%objectVelocity, %inheritFactor));
	if (!isObject (%projectile))
	{
		error ("ERROR: WeaponImage::onFire() - " @ %this.getName () @ " has invalid projectile \'" @ %projectile @ "\'");
		return 0;
	}
	%p = new (%this.projectileType) ("")
	{
		dataBlock = %projectile;
		initialVelocity = %muzzleVelocity;
		initialPosition = %initPos;
		sourceObject = %obj;
		sourceSlot = %slot;
		client = %obj.client;
	};
	%p.setScale (%obj.getScale ());
	MissionCleanup.add (%p);
	return %p;
}