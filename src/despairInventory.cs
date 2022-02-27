function testInventory(%c)
{
    %client = %c;
    %inventory = Inventory_Create();
    %empty = InventorySlotItemData_Create("Empty");
    %inventory.add(InventorySpace_Create("Hands",2,%empty,2,"inventorySelect","empty","handsEquip","empty"));
    Inventory_Push(%client.player,%inventory);

    return %Inventory;
}

datablock PlayerData(HandPlayer)
{
    shapeFile = "base/data/shapes/empty.dts";
};

function empty()
{

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

        //call to get hands back
        handsEquip(%client,"Hands",0,true);
    }
}

function handsEquip(%client,%space,%slot,%equip)
{
    %player = %client.player;

    if(!isObject(%player.rightHandBot))
    {
        %bot = new AiPlayer(){dataBlock = "HandPlayer";isHandBot = true;client = %client;};
        %player.mountObject(%bot,0);
        %player.rightHandBot = %bot;

        %bot = new AiPlayer(){dataBlock = "HandPlayer";isHandBot = true;client  = %client;};
        %player.mountObject(%bot,1);
        %player.leftHandBot = %bot;
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

package DespairInventory
{
    function Armor::onTrigger(%this, %obj, %triggerNum, %val)
    {
        %client = %obj.client;
        if(isObject(%client))
        {
            if(%client.getClassName() $= "GameConnection")
            {
                if(!%obj.itemsDisabled)
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
            talk(%object SPC %player);
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
};
deactivatePackage("DespairInventory");
activatePackage("DespairInventory");

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
		%raycast = containerRayCast (%start, %end, %mask, %obj);
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
            talk(%raycast);
			if (%raycast)
			{
				%eyeTarget = posFromRaycast (%raycast);
				%eyeTargetVec = VectorSub (%eyeTarget, %start);
				%eyeToMuzzle = VectorSub (%start, % );
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