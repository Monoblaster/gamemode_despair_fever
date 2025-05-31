datablock StaticShapeData(PlaneShapeGlowData)
{
	shapeFile = $Despair::Path @ "res/shapes/plane_glow.dts";
};

datablock AudioProfile(SnoringLoopSound)
{
	fileName = $Despair::Path @ "res/sounds/snoring.ogg";
	description = audioClosestLooping3D;
	preload = true;
};

datablock AudioProfile(SlipSound)
{
	fileName = $Despair::Path @ "res/sounds/slip.wav";
	description = audioClosest3d;
	preload = true;
};

datablock AudioProfile(SlipSound1)
{
	fileName = $Despair::Path @ "res/sounds/slip1.wav";
	description = audioClosest3d;
	preload = true;
};

datablock AudioProfile(SlipSound2)
{
	fileName = $Despair::Path @ "res/sounds/slip2.wav";
	description = audioClosest3d;
	preload = true;
};

datablock AudioProfile(SlipSound3)
{
	fileName = $Despair::Path @ "res/sounds/slip3.wav";
	description = audioClosest3d;
	preload = true;
};

function Player::setSleepCam(%this)
{
	%client = %this.client;
	if (!isObject(%client))
		return;
	%camera = %client.camera;
	if (!isObject(%camera))
		return;
	if (!isObject($KOScreenShape))
	{
		$KOScreenShape = new StaticShape()
		{
			datablock = PlaneShapeGlowData;
			scale = "1 1 1";
			position = "0 0 -400"; //units below ground level, woo
		};
		$KOScreenShape.setNodeColor("ALL", "0 0 0 1");
	}
	%camera = %client.camera;
	//aim the camera at the target
	%pos = vectorAdd($KOScreenShape.position, "0.2 0 0");
	%delta = vectorSub($KOScreenShape.position, %pos);
	%deltaX = getWord(%delta, 0);
	%deltaY = getWord(%delta, 1);
	%deltaZ = getWord(%delta, 2);
	%deltaXYHyp = vectorLen(%deltaX SPC %deltaY SPC 0);

	%rotZ = mAtan(%deltaX, %deltaY) * -1; 
	%rotX = mAtan(%deltaZ, %deltaXYHyp);

	%aa = eulerRadToMatrix(%rotX SPC 0 SPC %rotZ); //this function should be called eulerToAngleAxis...

	%camera.setTransform(%pos SPC %aa);
	%camera.setFlyMode();
	%camera.mode = "Observer";
	%client.setControlObject(%camera);
	%camera.setControlObject(%client.dummyCamera);
}

function Player::KnockOut(%this, %duration)
{
	cancel(%this.wakeUpSchedule);
	if(%this.getState() $= "Dead")
		return;
	if($DespairTrial !$= "")
		return;
	%this.changeDataBlock(PlayerCorpseArmor);
	%client = %this.client;
	if (isObject(%client) && isObject(%client.camera))
	{
		%this.setSleepCam();
	}

	%this.setArmThread(land);
	%this.setImageTrigger(0, 0);
	%this.playThread(0, "death1");
	%this.playThread(1, "root");
	%this.playThread(2, "root");
	%this.playThread(3, "root");
	%this.setActionThread("root");

	%this.spawnFiber();

	%this.unconscious = true;
	//%this.setShapeNameDistance(0);
	%this.isBody = true;
	if (%duration > 0)
	{
		%this.knockoutStart = getSimTime();
		%this.knockoutLength = %duration;
	}
	
	if(%this.statusEffect[$SE_passiveSlot] $= "fresh")
	{
		%this.freshSleep = true;
		%this.removeStatusEffect($SE_passiveSlot);
	}
	%this.setStatusEffect($SE_sleepSlot, "sleeping");

	if(%this.character.trait["Snorer"])
	{
		%this.stopAudio(0);
		%this.playAudio(0, SnoringLoopSound);
	}
	%this.KnockOutTick(%duration);
	%this.lastKO = $Sim::Time;
	if(isObject(%client = %this.client))
		RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") is unconscious!", "\c2");
}

function Player::KnockOutTick(%this, %ticks, %done)
{
	cancel(%this.wakeUpSchedule);
	if (%this.getState() $= "Dead" || !%this.unconscious)
		return;

	if (isObject(%killer = %this.carryPlayer) && %killer.choking && %killer.health > 0) //not in crit TODO: reward killers who kill when in crit by "second wind"
	{
		%choking = true;
	}
	else
	{
		%done += 1;
	}

	if (%done >= %ticks)
	{
		%this.WakeUp();
		return;
	}
	if (isObject(%this.client))
	{
		%this.client.centerPrint("\c6" @ %ticks - %done SPC "seconds left until you wake up.", 2);
		if(%choking)
		{
			%high = -1;
			%choice[%high++] = "can't breathe";
			%choice[%high++] = "no air";
			%choice[%high++] = "choking";
			%choice[%high++] = "can't move";
			%choice[%high++] = "help";
			%choice[%high++] = "please";
			%choice[%high++] = "gasp";
			%choice[%high++] = "it hurts";
			%choice[%high++] = "my neck";
			%choice[%high++] = "no";
			%choice[%high++] = "dying";

			%dream = %choice[getRandom(%high)];
			messageClient(%this.client, '', '   \c1... %1 ...', %dream);
			if(getRandom() < 0.4)
			{
				%this.forcedEmote = true;
				serverCmdMe(%this.client, "gasp");
			}
		}
		else if (getRandom() < 0.1)
		{
			%dream = getDreamText();
			if (getRandom() < 0.15) //less chance for a random character name to appear
			{
				%character = GameCharacters.getObject(getRandom(0, GameCharacters.getCount()-1));
				if (isObject(%character))
					%dream = %character.name;
			}

			if(%this.character.trait["Medium"])
			{
				if($lastDeadText !$= "" && getRandom() < 0.3)
					%dream = $lastDeadText;
				%dream = scrambleText(%dream,	"...");
				//$lastDeadText = "";
			}
			messageClient(%this.client, '', '   \c1... %1 ...', %dream);
		}
	}
	if(getRandom() < 0.01)
		%this.spawnFiber();
	%this.wakeUpSchedule = %this.schedule(1000, KnockOutTick, %ticks, %done);
}

function Player::WakeUp(%this)
{
	cancel(%this.wakeUpSchedule);
	if (%this.getState() $= "Dead" || !%this.unconscious)
		return;
	%this.knockoutStart = "";
	%client = %this.client;
	if (isObject(%client) && isObject(%client.camera))
	{
		%client.camera.setMode("Player", %this);
		%client.camera.setControlObject(%client);
		%client.setControlObject(%this);
	}
	%this.setArmThread(look);
	%this.unconscious = false;
	%this.isBody = false;

	//%this.setShapeNameDistance($defaultMinigame.shapeNameDistance);
	%this.changeDataBlock(PlayerDespairArmor);
	%this.playThread(0, "root");

	%this.removeStatusEffect($SE_sleepSlot);
	if(!%this.currResting)
	{
		%pos = %this.getPosition();
		%ray = containerRayCast(%pos, vectorSub(%pos, "0 0 1"), $TypeMasks::FxBrickObjectType, %this);
		if(!%this.character.trait["Heavy Sleeper"] && (!%ray || %ray.getName() !$= "_bed"))
			%this.setStatusEffect($SE_passiveSlot, "sore back");
		else if(%this.character.trait["Paranoid"])
			%this.setStatusEffect($SE_passiveSlot, "drowsy");
		else if(%this.freshSleep)
			%this.setStatusEffect($SE_passiveSlot, "shining");
		else
			%this.addMood(4, "You slept undisturbed.");
		%this.freshSleep = "";
	}
	%this.currResting = false;

	if(%this.character.trait["Snorer"])
		%this.stopAudio(0);

	%client.updateBottomPrint();
	RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") woke up!", "\c2");
}

function Player::Slip(%this, %ticks)
{
	cancel(%this.wakeUpSchedule);
	if(%this.getState() $= "Dead")
		return;
	if(%this.unconscious || %this.health <= 0)
		return;

	if(%ticks <= 0 && %this.isSlipping)
	{
		if(vectorLen(%this.getVelocity()) < 1)
		{
			%this.isSlipping = false;
			%this.setArmThread(look);
			%this.changeDataBlock(PlayerDespairArmor);
			%this.setActionThread("sit");
			%this.playThread(0, "root");

			if(getRandom() < 0.1)
				%this.spawnFiber();

			%this.client.camera.schedule(500, setMode, "Player", %this);
			%this.client.camera.schedule(500, setControlObject, %client);
			%this.client.schedule(500, setControlObject, %this);
			return;
		}
	}
	else if(!%this.isSlipping)
	{
		%this.isSlipping = true;
		%this.lastSlip = $Sim::Time;
		%this.client.setControlObject(%cam = %this.client.camera);
		%cam.setMode("CORPSE", %this);
		%this.changeDatablock(PlayerCorpseArmor);
		%this.setArmThread(land);
		%this.setImageTrigger(0, 0);
		%this.playThread(0, "death1");
		%this.playThread(1, "root");
		%this.playThread(2, "root");
		%this.playThread(3, "root");
		%this.setActionThread("root");

		%vel = getWords(vectorScale(%this.getForwardVector(), 4), 0, 1) SPC 3;
		%vel = vectorAdd(%this.getVelocity(), %vel);
		%this.setVelocity(%vel);

		serverPlay3d(SlipSound @ getRandom(1, 3), %this.getPosition());

		if ($Sim::Time - %this.lastMoodChange > 30)
			%this.addMood(-3, "You got pranked!");

		if(isObject(%this.getMountedImage(0)) && getRandom() < 0.3)
			%this.dropTool(%this.currTool);
	}
	%this.wakeUpSchedule = %this.schedule(1000, Slip, %ticks--);
	if(isObject(%client = %this.client))
		RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") slipped!", "\c2");
}

function serverCmdSleep(%this, %bypass)
{
	if(!isObject(%pl = %this.player) || %this.miniGame != $defaultMinigame || $DespairTrial)
		return;

	if(%this.killer)
	{
		%this.setControlObject(%cam = %this.camera);
		%cam.setMode("CORPSE", %pl);
		%pl.changeDatablock(PlayerCorpseArmor);
		%pl.setArmThread(land);
		%pl.setImageTrigger(0, 0);
		%pl.playThread(0, "root");
		%pl.playThread(1, "root");
		%pl.playThread(2, "root");
		%pl.playThread(3, "death1");
		%pl.setActionThread("root");
		%pl.unconscious = 1;
		%pl.currResting = 1;
		%pl.isBody = true;
		%pl.spawnFiber();
		%this.chatMessage("\c6You are faking sleep. Press any key to get up.");
		if(%this.character.trait["Snorer"])
		{
			%pl.stopAudio(0);
			%pl.playAudio(0, SnoringLoopSound);
		}
		RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") is faking sleep!", "\c2");
		return;
	}

	%se = %pl.statusEffect[$SE_sleepSlot];
	if (%se !$= "sleepy" && %se !$= "tired" && %se !$= "exhausted")
	{
		%this.chatMessage("\c6You can't sleep yet - you don't feel tired!");
		return;
	}

	%mood = getMoodName(%pl.mood);

	switch$ (%mood)
	{
		case "overjoyed":
			%sec = $Despair::SleepOverjoyed;
		case "happy":
			%sec = $Despair::SleepHappy;
		case "sad":
			%sec = $Despair::SleepSad;
		case "depressed":
			%sec = $Despair::SleepDepressed;
		default:
			%sec = $Despair::SleepDefault;
	}

	%sec = %se $= "exhausted" ? $Despair::SleepDepressed : %sec;
	%pos = %pl.getPosition();

	if(!%this.character.trait["Heavy Sleeper"])
	{
		%ray = containerRayCast(%pos, vectorSub(%pos, "0 0 1"), $TypeMasks::FxBrickObjectType, %this);
		if(!%ray || %ray.getName() !$= "_bed")
		{
			%sec += 10;
			%cold = "\n<color:FF0000>on the cold floor";
		}
	}
	if (%bypass)
	{
		if (%pl.unconscious)
			return;
		%pl.KnockOut(%sec);
		%this.updateBottomPrint();
		RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") is sleeping!", "\c2");
		return;
	}
	%message = "Are you sure you want to sleep" @ %cold @ "?\n<color:0000FF>You will be unconscious for<color:000000>" SPC %sec SPC "<color:0000FF>seconds!\n(The surface and your mood affect sleeping times)";
	commandToClient(%this, 'messageBoxYesNo', "Sleep Prompt", %message, 'SleepAccept');
}

function serverCmdSleepAccept(%this)
{
	serverCmdSleep(%this, 1);
}

function loadDreamList()
{
	%file = new FileObject();
	%fileName = $Despair::Path @ "data/dreams.txt";

	if (!%file.openForRead(%fileName))
	{
		error("ERROR: Failed to open '" @ %fileName @ "' for reading");
		%file.delete();
		return;
	}

	deleteVariables("$Despair::DreamListItem_*");
	%max = -1;

	while (!%file.isEOF())
	{
		%line = %file.readLine();
		$Despair::DreamListItem[%max++] = %line;
	}

	%file.close();
	%file.delete();

	$Despair::DreamListMax = %max;
}

if (!$Despair::LoadedDreamList)
{
	$Despair::LoadedDreamList = true;
	loadDreamList();
}

function getDreamText()
{
	%text = $Despair::DreamListItem[getRandom($Despair::DreamListMax)];
	return %text;
}
