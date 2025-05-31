datablock AudioProfile(BodyPickUpSound1)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodypickup1.wav";
	description = AudioQuiet3d;
	preload = true;
};
datablock AudioProfile(BodyPickUpSound2)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodypickup2.wav";
	description = AudioQuiet3d;
	preload = true;
};
datablock AudioProfile(BodyPickUpSound3)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodypickup3.wav";
	description = AudioQuiet3d;
	preload = true;
};

datablock AudioProfile(BodyDropSound1)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodyDrop1.wav";
	description = AudioQuiet3d;
	preload = true;
};
datablock AudioProfile(BodyDropSound2)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodyDrop2.wav";
	description = AudioQuiet3d;
	preload = true;
};
datablock AudioProfile(BodyDropSound3)
{
	fileName = $Despair::Path @ "res/sounds/gore/bodyDrop3.wav";
	description = AudioQuiet3d;
	preload = true;
};

datablock AudioProfile(BodyChokeSound)
{
	fileName = $Despair::Path @ "res/sounds/gore/choking.wav";
	description = AudioQuietLooping3d;
	preload = true;
};

function Player::findCorpseRayCast(%obj, %doPlayer)
{
	%a = %obj.getEyePoint();
	%b = vectorAdd(vectorScale(%obj.getEyeVector(), 5), %a);
	%ray = containerRayCast(%a, %b, $TypeMasks::fxBrickObjectType | $TypeMasks::playerObjectType, %obj);
	if(%ray)
		%b = getWords(%ray, 1, 3);
	%center = vectorScale(vectorAdd(%a, %b), 0.5); //Get middle of raycast
	%length = vectorDist(%a, %b) / 2;

	if(%ray && %ray.getType() & $TypeMasks::playerObjectType)
	{
		if(%ray.isBody || %doPlayer)
			return %ray;
	}

	%maxdist = 1; //how fatass our fat raycast is
	initContainerRadiusSearch(%center, %length + %maxdist, $TypeMasks::CorpseObjectType); //Scale radius search so it searches the entirety of raycast
	while (isObject(%col = containerSearchNext()))
	{
		if (!%col.isDead)
			continue;
		%p = %col.getHackPosition();
		%ab = vectorSub(%b, %a);
		%ap = vectorSub(%p, %a);

		%project = vectorDot(%ap, %ab) / vectorDot(%ab, %ab); //Projection, aka "check against closest point on raycast" or something.

		if (%project < 0 || %project > 1)
			continue;

		%j = vectorAdd(%a, vectorScale(%ab, %project));
		%distance = vectorDist(%p, %j);
		if (%distance <= %maxdist) //Give 'em the corpse!
		{
			return %col;
		}
	}
	return 0;
}

function Player::carryTick(%this)
{
	cancel(%this.carrySchedule);
	%player = %this.carryPlayer;
	if (!isObject(%player) || %player.getState() $= "Dead")
	{
		%this.lastTosser = %player;
		%this.carryEnd = $Sim::Time;
		%this.carryPlayer = 0;
		%player.carryObject = 0;
		if(%player.startCarrying)
		{
			%player.startCarrying = false;
			%player.lastNormal = "";
			ServerPlay3D("BodyDropSound" @ getRandom(1, 3), %this.getPosition());
		}
		if(%player.choking)
		{
			%this.stopAudio(0);
			%player.choking = "";
		}
		return;
	}
	%eyePoint = %player.getEyePoint();
	%normal = %player.getAimVector();
	if(%player.lastNormal $= "")
		%player.lastNormal = %normal;
	if(!%player.startCarrying)
	{
		if(vectorLen(vectorSub(%player.lastNormal, %normal)) > 0.05)
		{
			ServerPlay3D("BodyPickUpSound" @ getRandom(1, 3), %this.getPosition());
			if (%this.bloody["chest_front"] || %this.bloody["chest_back"])
			{
				%player.bloody["rhand"] = true;
				%player.bloody["lhand"] = true;
				%player.bloodyWriting = 2;
			}
			%player.applyAppearance();
			%player.playThread(2, "armReadyBoth");
			%player.startCarrying = true;
			%player.lastNormal = "";
		}
		else
		{
			%this.carrySchedule = %this.schedule(100, "carryTick");
			return;
		}
	}

	if($Sim::Time - %this.lastFiber > 2 && getRandom() < 0.005)
		%this.spawnFiber();
	if($Sim::Time - %player.lastFiber > 2 && getRandom() < 0.005)
		%player.spawnFiber();

	if (!%this.isBody)
	{
		%this.lastTosser = %player;
		%this.carryEnd = $Sim::Time;
		%this.carryPlayer = 0;
		%player.carryObject = 0;
		%player.startCarrying = false;
		%player.lastNormal = "";
		%player.playThread(2, "root");
		ServerPlay3D("BodyDropSound" @ getRandom(1, 3), %this.getPosition());
		if(%player.choking)
		{
			%this.stopAudio(0);
			%player.choking = "";
		}
		return;
	}

	if(%this.isDead && getRandom() < 0.45 && %this.pools < 300) //blood
	{
		updateCorpseBloodPool(%this.getPosition(), %this);
		%this.pools++;
	}

	if (%player.getMountedImage(0))
	{
		%player = %this.carryPlayer;
		%this.lastTosser = %player;
		%this.carryPlayer = 0;
		%player.carryObject = 0;
		%player.startCarrying = false;
		%player.lastNormal = "";
		ServerPlay3D("BodyDropSound" @ getRandom(1, 3), %this.getPosition());
		if(%player.choking)
		{
			%this.stopAudio(0);
			%player.choking = "";
		}
		return;
	}

	%eyeVector = getWords(%normal, 0, 1) SPC (getWord(%normal, 2) * 0.5) - 0.5;

	%center = %this.getPosition();
	%target = vectorAdd(%eyePoint, vectorScale(%eyeVector, 3));

	%maxdist = 4;
	if (vectorDist(%center, %target) > 4 || (%player.choking && vectorLen(%player.getVelocity()) > 4) || getWord(%center, 2) - getWord(%player.getPosition(), 2) < -1)
	{
		%this.lastTosser = %player;
		%this.carryEnd = $Sim::Time;
		%this.carryPlayer = 0;
		%player.carryObject = 0;
		%player.startCarrying = false;
		%player.lastNormal = "";
		%player.playThread(2, "root");
		ServerPlay3D("BodyDropSound" @ getRandom(1, 3), %this.getPosition());
		if(%player.choking)
		{
			%this.stopAudio(0);
			%player.choking = "";
		}
		return;
	}

	if(%player.choking && $Sim::Time - %player.choking > 6)
	{
		if(%this.isDead || $deathCount >= $maxDeaths || %player.noWeapons)
		{
			%this.stopAudio(0);
			%player.choking = "";
			%player.spawnFiber(); //Guaranteed killer fibers
			%player.startCarrying = false;
		}
		else
		{
			%this.health = $Despair::CritThreshold;
			%this.damage(%player, %this.getPosition(), 5, "choking");
			%this.pools = 1000;
			%player.startCarrying = false;
		}
	}

	%vel = vectorScale(vectorSub(%target, %center), 4);

	%this.setVelocity(%vel);

	%rot = getWords(%player.getTransform(), 3, 7);
	if(%rot !$= getWords(%this.getTransform(), 3, 7))
		%this.setTransform(getWords(%this.getTransform(), 0, 2) SPC %rot);
	%this.carrySchedule = %this.schedule(1, "carryTick");
}

package DespairCorpses
{
	function Armor::onTrigger(%this, %obj, %slot, %state)
	{
		if(!isObject(%obj.getMountedImage(0)))
		{
			%item = %obj.carryObject;
			if(%slot == 0)
			{
				if (isObject(%item) && isEventPending(%item.carrySchedule) && %item.carryPlayer $= %obj)
				{
					%time = $Sim::Time - %item.carryStart;
					cancel(%item.carrySchedule);
					%item.lastTosser = %obj;
					%item.carryEnd = $Sim::Time;
					%item.carryPlayer = 0;
					if(%obj.choking)
					{
						%item.stopAudio(0);
						%obj.choking = "";
					}
					%obj.carryObject = 0;
					if(%obj.startCarrying)
					{
						ServerPlay3D("BodyDropSound" @ getRandom(1, 3), %item.getPosition());
						%obj.startCarrying = false;
						%obj.lastNormal = "";
						%obj.playThread(2, "root");
					}
				}
				if(%state && isObject(%col = %obj.findCorpseRayCast()))
				{
					if(isObject(%obj.client))
						%obj.client.examineObject(%col);

					if (isEventPending(%col.carrySchedule) && isObject(%col.carryPlayer))
					{
						%col.carryPlayer.playThread(2, "root");
						if(%col.caryPlayer.choking)
						{
							%col.stopAudio(0);
							%col.caryPlayer.choking = "";
						}
					}
					if(%col.isDead && !(%obj.client.killer && !%col.suicide && $investigationStart $= ""))
						return;
					%obj.carryObject = %col;
					%col.carryPlayer.carryObject = 0;
					%col.carryPlayer = %obj;
					%col.carryStart = $Sim::Time;
					%col.carryTick();
					return; //so we don't call anything else
				}
			}
			else if (%slot == 4 && isObject(%item) && isEventPending(%item.carrySchedule) && %item.carryPlayer $= %obj && %item.unconscious)
			{
				if(%state && !%item.isDead && $deathCount < $maxDeaths && !%obj.noWeapons)
				{
					%item.playAudio(0, BodyChokeSound);
					%obj.choking = $Sim::Time;
				}
				else
				{
					%item.stopAudio(0);
					%obj.choking = "";
				}
			}
		}
		Parent::onTrigger(%this, %obj, %slot, %state);
	}

	function Player::dropTool(%obj, %index)
	{
		%item = %obj.tool[%index];
		if(isObject(%target = %obj.findCorpseRayCast()))
		{
			%item = %obj.tool[%obj.currTool];
			if(!isObject(%item))
				return;
			%itemName = %item.getName();
			%props = %obj.itemProps[%obj.currTool];
			%slot = -1;
			if(%item.className $= "DespairWeapon")
			{
				if(%target.tool[%target.weaponSlot] == nameToID(noWeaponIcon))
					%slot = %target.setTool(%target.weaponSlot, %item, %props, 1, 2);
				else
					return;
			}
			else if(%item.className $= "Hat")
			{
				if(%item.onPickup("", %target))
					%slot = %target.hatSlot;
				else
					return;
			}
			else if(!%item.isIcon && (%slot = %target.addTool(%item, %props, 1, 2)) != -1)
			{
				%obj.removeTool(%obj.currTool, 1, 2);
				%obj.itemProps[%obj.currTool] = "";
			}
			if(%slot >= 0)
			{
				if(isFunction(%itemName, "onDrop"))
					%item.onDrop(%obj, %slot);
				%obj.playThread(2, "activate2");
				%target.playThread(2, "plant");
				ServerPlay3D("BodyPickUpSound" @ getRandom(1, 3), %target.getPosition());
				if(isObject(%target.client))
					messageClient(%target.client, 'MsgItemPickup', '', %slot, %target.tool[%slot], 0);
				if(isObject(%obj.client))
					messageClient(%obj.client, 'MsgItemPickup', '', %slot, %obj.tool[%slot], 0);

				if(getRandom() < 0.2)
					%obj.spawnFiber();
				if(getRandom() < 0.2)
					%target.spawnFiber();
			}
			return;
		}
		parent::dropTool(%obj, %index);
	}
};
activatePackage(DespairCorpses);
