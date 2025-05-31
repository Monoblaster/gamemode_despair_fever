datablock AudioProfile(HammerSwingSound1)
{
	fileName = $Despair::Path @ "res/sounds/weapons/HammerSwing1.wav";
	description = audioQuiet3D;
	preload = true;
};
datablock AudioProfile(HammerSwingSound2)
{
	fileName = $Despair::Path @ "res/sounds/weapons/HammerSwing2.wav";
	description = audioQuiet3D;
	preload = true;
};
datablock AudioProfile(HammerSwingSound3)
{
	fileName = $Despair::Path @ "res/sounds/weapons/HammerSwing3.wav";
	description = audioQuiet3D;
	preload = true;
};
datablock ItemData(HandHammerItem)
{
	category = "DespairWeapon";
	classname = "DespairWeapon";

	shapeFile = $Despair::Path @ "res/shapes/weapons/handhammer.dts";
	iconName = $Despair::Path @ "res/shapes/weapons/icon_sword";
	image = HandHammerImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0.75 0.75 0.75 1";
	uiName = "Handhammer";
	canDrop = true;

	itemPropsClass = "MeleeProps";
	itemPropsAlways = true;
};

function HandHammerItem::onAdd(%this, %obj)
{
	parent::onAdd(%this, %obj);
	if(%obj.itemProps.bloody)
		%obj.playThread(0, "blood");
	else
		%obj.playThread(0, "root");
}

datablock ShapeBaseImageData(HandHammerImage)
{
	className = "WeaponImage";
	item = HandHammerItem;

	shapeFile = $Despair::Path @ "res/shapes/weapons/handhammer.dts";
	doColorShift = true;
	colorShiftColor = "0.75 0.75 0.75 1";

	useCustomStates = true;
	type = "blunt";

	armReady = false;

	fireManual = true;

	windUp = 0.4;
	fireDelay = 0.5;
	fireScript = "onFire";
	meleeRange = 3;

	damage = 28;

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

function HandHammerImage::onMount(%image, %player, %slot)
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

function HandHammerImage::onUnMount(%image, %player, %slot)
{
	cancel(%player.windUpSchedule);
}

function HandHammerImage::onWindUp(%image, %player)
{
	%player.swingType = getRandom(1, 2);
	%player.playThread(1, "1hpre" @ %player.swingType);
	%windUp = %image.windUp*1000;
	%player.lastFireTime = $Sim::Time + %windUp;
	%player.windUpSchedule = %image.schedule(%windUp, onFire, %player);
}

function HandHammerImage::onFire(%image, %player)
{
	cancel(%player.windUpSchedule);
	%player.swingType = getRandom(1, 2);
	%player.playThread(1, "1hswing" @ %player.swingType);
	%player.lastFireTime = $Sim::Time;
	fireMelee(%image, %player);
	%player.playAudio(1, "HammerSwingSound" @ getRandom(1, 3));
}
function HandHammerImage::onMeleeHit(%image, %player, %object, %position, %normal)
{
	if (!isObject(%object))
		return;

	%damage = %image.damage;
	%props = %player.getItemProps();
	if (%object.getType() & $TypeMasks::PlayerObjectType)
	{
		sprayBloodStab(%position, VectorScale(%normal, -10), %object);
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
			%player.bloodyWriting = 2;
			%player.bloody["rhand"] = true;
			if (isObject(%player.client))
				%player.client.applyBodyParts();
		}

		ServerPlay3D("BluntHitSound" @ getRandom(1, 3), %position);
		if(%obj.unconscious)
			%damage *= 2;
		return %object.damage(%player, %position, %damage, %image.type);
	}
}