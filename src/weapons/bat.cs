datablock ItemData(BatItem)
{
	shapeFile = $Despair::Path @ "res/shapes/weapons/bat.dts";
	image = BatImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0.47 0.35 0.2 1";
	uiName = "Baseball Bat";
	canDrop = true;

	itemPropsClass = "MeleeProps";
	itemPropsAlways = true;
};

function BatItem::onAdd(%this, %obj)
{
	parent::onAdd(%this, %obj);
	if(%obj.itemProps.bloody)
		%obj.playThread(0, "blood");
	else
		%obj.playThread(0, "root");
}

datablock ShapeBaseImageData(BatImage)
{
	className = "WeaponImage";
	item = BatItem;

	shapeFile = $Despair::Path @ "res/shapes/weapons/bat.dts";
	doColorShift = true;
	colorShiftColor = "0.47 0.35 0.2 1";

	isWeapon = true;

	useCustomStates = true;
	type = "blunt";

	armReady = false;

	fireManual = true;

	windUp = 0.3;
	fireDelay = 0.6;
	fireScript = "onFire";
	meleeRange = 4;

	damage = 25;

	stateName[0]					= "Activate";
	stateAllowImageChange[0]		= 1;
	stateSequence[0]				= "root";
	stateTimeoutValue[0]			= 0.01;
	stateTransitionOnTimeOut[0]		= "CheckBlood";

	stateName[1]					= "CheckBlood";
	stateTransitionOnAmmo[1]		= "Blood";
	stateSequence[1]				= "root";

	stateName[2]					= "Blood";
	stateSequence[2]				= "blood";
};

function BatImage::onMount(%image, %player, %slot)
{
	%props = %player.getItemProps();
	%player.setImageAmmo(%slot, %props.bloody);
	if(!%player.updateBloody)
	{
		%player.playThread(1, "1hpre1");
		%player.schedule(32, stopThread, 1);
	}
	%player.updateBloody = 0;
}

function BatImage::onUnMount(%image, %player, %slot)
{
	cancel(%player.windUpSchedule);
}

function BatImage::onWindUp(%image, %player)
{
	%player.swingType = getRandom(1, 2);
	%player.playThread(1, "1hpre" @ %player.swingType);
	%windUp = %image.windUp*1000;
	%player.lastFireTime = $Sim::Time + %windUp;
	%player.windUpSchedule = %image.schedule(%windUp, onFire, %player);
}

function BatImage::onFire(%image, %player)
{
	cancel(%player.windUpSchedule);
	%player.swingType = getRandom(1, 2);
	%player.playThread(1, "1hswing" @ %player.swingType);
	%player.lastFireTime = $Sim::Time;
	fireMelee(%image, %player);
}

function BatImage::onMeleeHit(%image, %player, %object, %position, %normal)
{
	if (!isObject(%object))
		return;

	%damage = %image.damage;
	%props = %player.getItemProps();
	if (%object.getType() & $TypeMasks::PlayerObjectType)
	{
		sprayBloodWide(%position, VectorScale(%normal, -10));
		if(!%props.bloody)
		{
			%props.bloody = %object.health - %damage <= 0 || getRandom() < 0.3; //low chance to get bloody
			if(%props.bloody)
			{
				%player.updateBloody = 1;
				%player.unMountImage(0); %player.mountImage(%image, 0); //update blood
			}
		}

		if(%props.bloody && getRandom() < 0.6) //Another random chance to get bloody hand
		{
			%player.bloody["rhand"] = true;
			%player.bloody = true;
			if (isObject(%player.client))
				%player.client.applyBodyParts();
		}

		return %object.damage(%player, %position, %damage, %image.type);
	}
	if(%object.getType() & $TypeMasks::FxBrickObjectType && %object.getDataBlock().isDoor)
	{
		ServerPlay3D(WoodHitSound, %position);
		return %object.doorDamage(1);
	}
}