function Player::addTool(%this, %data, %props, %ignoreProps, %stealth)
{
	if(%stealth $= "") %stealth = 1;
	%data = %data.getID();
	%maxTools = %this.getDataBlock().maxTools;

	for (%i = 0; %i < %maxTools; %i++)
	{
		if (!%this.tool[%i])
			break;

		if (!%data.customPickupMultiple && %this.tool[%i] == %data)
		{
			if (!%ignoreProps && isObject(%props))
				%props.delete();
			return -1;
		}
	}

	if (%i == %maxTools)
	{
		if (!%ignoreProps && isObject(%props))
			%props.delete();
		return -1;
	}

	%this.tool[%i] = %data;
	%this.itemProps[%i] = %props;

	if (isObject(%props))
		%props.onOwnerChange(%this);

	if (isObject(%this.client))
	{
		messageClient(%this.client, 'MsgItemPickup', '', %i, %data, %stealth);

		if (%this.currTool == %i)
			serverCmdUseTool(%this.client, %i);
	}

	return %i;
}

function Player::setTool(%this, %index, %data, %props, %ignoreProps, %stealth)
{
	if(%stealth $= "") %stealth = 1;
	%data = %data.getID();

	if (!%ignoreProps && isObject(%this.itemProps[%index]))
		%this.itemProps[%index].delete();

	%this.tool[%index] = %data;
	%this.itemProps[%index] = %props;

	if (isObject(%props))
		%props.onOwnerChange(%this);

	if (isObject(%this.client))
	{
		messageClient(%this.client, 'MsgItemPickup', '', %index, %data, %stealth);

		if (%this.currTool == %index)
			serverCmdUseTool(%this.client, %index);
	}

	return %index;
}


function Player::removeTool(%this, %index, %ignoreProps, %stealth)
{
	if(%stealth $= "") %stealth = 1;
	%this.tool[%index] = 0;

	if (!%ignoreProps && isObject(%this.itemProps[%index]))
		%this.itemProps[%index].delete();

	if (isObject(%this.client))
		messageClient(%this.client, 'MsgItemPickup', '', %index, 0, %stealth);

	if (%this.currTool == %index)
		%this.unMountImage(0);
}
