datablock ItemData(BurgerItem)
{
	shapeFile = $Despair::Path @ "res/shapes/food/Burger.dts";
	mass = 1;
	density = 0.4;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Burger";

	iconName = $Despair::Path @ "res/shapes/food/Icon_Cheeseburger";

	image = BurgerImage;
	canDrop = true;
	
	waitForKiller = false;
};

datablock ShapeBaseImageData(BurgerImage)
{
	shapeFile = $Despair::Path @ "res/shapes/food/Burger.dts";

	emap = true;
	mountPoint = 0;

	className = "WeaponImage";
	item = BurgerItem;

	armReady = true;

	stateName[0]					= "Activate";
	stateTransitionOnTimeOut[0]		= "Ready";
	stateAllowImageChange[0]		= true;
	stateTimeoutValue[0]			= 0.5;

	stateName[1]					= "Ready";
	stateTransitionOnTriggerDown[1]	= "Fire";
	stateAllowImageChange[1]		= true;

	stateName[2]					= "Fire";
	stateTransitionOnTimeout[2]		= "Ready";
	stateAllowImageChange[2]		= true;
	stateScript[2]					= "onEat";
	stateTimeoutValue[2]			= 1;
};

function BurgerImage::onEat(%this, %obj, %slot)
{
    if ($Sim::Time - %obj.lastEat < 120) //can only eat every 2 mins
    {
        if(isObject(%obj.client))
            %obj.client.chatMessage("\c5You don't feel like eating at the moment.");
        return;
    }
	if(%obj.character.trait["Glutton"])
	{	
		%obj.health = getMin(%obj.health + 25, %obj.maxHealth);
		%obj.unMountImage(0);
		%obj.removeTool(%obj.currTool);
		%obj.lastEat = $Sim::Time;
		serverPlay3d("EatSound", %obj.getEyePoint());
		%obj.addMood(5, "Eating burger gives you extra energy!");
		%obj.health = getMin(%obj.health + 25, %obj.maxHealth);
		return;
	}
	%obj.unMountImage(0);
	%obj.removeTool(%obj.currTool);
    %obj.lastEat = $Sim::Time;
	serverPlay3d("EatSound", %obj.getEyePoint());
	%obj.addMood(5, "Mmm, that was delicious!");
	%obj.health = getMin(%obj.health + 25, %obj.maxHealth);
	
}