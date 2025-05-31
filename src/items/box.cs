datablock itemData(KillerBoxItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/Box.dts";
	image = KillerBoxImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0.6 0.6 0.6 1";
	uiName = "Box";
	canDrop = true;
};

datablock ShapeBaseImageData(KillerBoxImage)
{
	item = KillerBoxItem;
	shapeFile = $Despair::Path @ "res/shapes/items/Box.dts";
	doColorShift = true;
	colorShiftColor = "0.6 0.6 0.6 1";
	offset = "0 0 0";
	rotation = eulerToMatrix("0 0 0");

	armReady = true;

	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.5;
	stateTransitionOnTimeout[0]		= "Ready";
	stateSound[0]					= "";
	
	stateName[1] = "Ready";
	stateAllowImageChange[1] = true;
	stateTransitionOnTriggerDown[1] = "Use";

	stateName[2] = "Use";
	stateScript[2] = "onUse";
	stateAllowImageChange[2] = false;
	stateTransitionOnTriggerUp[2] = "Ready";
};

function KillerBoxImage::onMount(%this, %obj, %slot)
{
	fixArmReady(%obj);
	if (isObject(%obj.client))
	{
		if(%obj.client.killer)
			commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>This box contains something...\nClick to dump its contents! Rightclick to dispose of it.");
		else
			commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>This box looks suspicious...");
	}
}

function KillerBoxImage::onUnMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'ClearCenterPrint');
}

function KillerBoxImage::onUse(%this, %obj, %slot)
{
	if (isObject(%obj.client) && !%obj.client.killer)
		return;
	%count = 3;
	%loot[0] = "HatSkimaskItem CoatItem LockpickItem CleanSprayItem KnifeItem BananaItem";
	%loot[1] = "TaserItem DisguiseItem FlashbangItem";
	%gotRare = false;

	%chance = GameCharacters.getCount() * 0.045; //10 players = 45% chance for rare shit
	while(%count-- >= 0)
	{
		if(getRandom() <= %chance && !%gotRare)
		{
			%index = 1;

			if(getRandom() > 0.1) //10% chance after the initial check to get MORE rare shit!!!
				%gotRare = true;
		}
		else
			%index = 0;

		%data = getWord(%loot[%index], %i = getRandom(0, getWordCount(%loot[%index])-1));
		%loot[%index] = removeWord(%loot[%index], %i);
		%item = new Item() {
			dataBlock = %data;
			position = %obj.getPosition();
		};
		%item.setCollisionTimeout(%obj);
		%item.setTransform(%obj.getEyePoint());
		%targVel = vectorSpread(VectorScale(%obj.getEyeVector(), 5), 0.25);
		%item.setVelocity(VectorAdd(%obj.getVelocity(), %targVel));
		%item.schedulePop();
	}

	%obj.removeTool(%obj.currTool);

	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>You dump the contents and discard the box.", 2);
}

function KillerBoxImage::onRightClick(%this, %obj, %slot)
{
	if(%obj.getImageState(0) !$= "Ready")
		return;
	if (isObject(%obj.client) && !%obj.client.killer)
		return;
	%obj.removeTool(%obj.currTool);
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "\c5You dispose of the box.");
}
