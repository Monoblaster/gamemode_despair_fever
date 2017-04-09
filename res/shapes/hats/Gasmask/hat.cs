datablock ItemData(HatGasmaskItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Gasmask.dts";
	image = HatGasmaskImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Gasmask";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = true;
	hidehair = true;
};
datablock ShapeBaseImageData(HatGasmaskImage)
{
	item = HatGasmaskItem;
	shapeFile = "./Gasmask.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
