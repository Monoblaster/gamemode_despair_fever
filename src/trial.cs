datablock AudioProfile(ObjectionSound)
{
	fileName = $Despair::Path @ "res/sounds/objection.wav";
	description = audio2D;
	preload = true;
};
datablock StaticShapeData(DespairStand)
{
	shapeFile = $Despair::Path @ "res/shapes/stand.dts";
	canDismount = false;
};
datablock StaticShapeData(DespairMemorial)
{
	shapeFile = $Despair::Path @ "res/shapes/memorial.dts";
};
datablock StaticShapeData(DespairCourtVoid)
{
	shapeFile = $Despair::Path @ "res/shapes/courtvoid.dts";
};

datablock ShapeBaseImageData(MemorialCrossImage)
{
	shapeFile = $Despair::Path @ "res/shapes/cross.dts";
	mountPoint = $headSlot;
	doColorShift = true;
	colorShiftColor = "0.65 0 0 1";
	offset = "0 0.5 0";
};

function createShape(%data, %position, %rotation, %scale, %color)
{
	if (%color $= "") %color = $defCol;
	%shape = new StaticShape()
	{
		datablock = %data;
		position = %position;
		rotation = %rotation;
		scale = %scale;
	};
	%shape.setNodeColor("ALL", %color);
	return %shape;
}

function createCourtroom()
{
	if (isObject(CourtroomGroup))
		CourtroomGroup.deleteAll();
	else
		new SimGroup(CourtroomGroup);
	%oldInstantGroup = $instantGroup;
	$instantGroup = CourtroomGroup;

	$defCol = (getRandom() * 0.6) SPC (getRandom() * 0.6) SPC (getRandom() * 0.6) SPC 1;

	$courtvoid = createShape(DespairCourtVoid,  "0 0 -300");
	$courtvoid.setNodeColor("ALL", (getRandom() * 0.3) SPC (getRandom() * 0.3) SPC (getRandom() * 0.3) SPC 1);
	//$courtvoid.playThread(0, "spin"); //VERY nauseating effect

	$stand0 = createShape(DespairStand,  "0 0 -300");
	$stand1 = createShape(DespairStand,  "0 0 -300");
	$stand2 = createShape(DespairStand,  "0 0 -300");
	$stand3 = createShape(DespairStand,  "0 0 -300");
	$stand4 = createShape(DespairStand,  "0 0 -300");
	$stand5 = createShape(DespairStand,  "0 0 -300");
	$stand6 = createShape(DespairStand,  "0 0 -300");
	$stand7 = createShape(DespairStand,  "0 0 -300");
	$stand8 = createShape(DespairStand,  "0 0 -300");
	$stand9 = createShape(DespairStand,  "0 0 -300");
	$stand10 = createShape(DespairStand, "0 0 -300");
	$stand11 = createShape(DespairStand, "0 0 -300");
	$stand12 = createShape(DespairStand, "0 0 -300");
	$stand13 = createShape(DespairStand, "0 0 -300");
	$stand14 = createShape(DespairStand, "0 0 -300");
	$stand15 = createShape(DespairStand, "0 0 -300");
	$stand16 = createShape(DespairStand, "0 0 -300");
	$stand17 = createShape(DespairStand, "0 0 -300");
	$stand18 = createShape(DespairStand, "0 0 -300");
	$stand19 = createShape(DespairStand, "0 0 -300");
	$stand20 = createShape(DespairStand, "0 0 -300");
	$stand21 = createShape(DespairStand, "0 0 -300");
	$stand22 = createShape(DespairStand, "0 0 -300");
	$stand23 = createShape(DespairStand, "0 0 -300");
	
	$memorial0 = createShape(DespairMemorial,  "0 0 -300");
	$memorial1 = createShape(DespairMemorial,  "0 0 -300");
	$memorial2 = createShape(DespairMemorial,  "0 0 -300");
	$memorial3 = createShape(DespairMemorial,  "0 0 -300");
	$memorial4 = createShape(DespairMemorial,  "0 0 -300");
	$memorial5 = createShape(DespairMemorial,  "0 0 -300");
	$memorial6 = createShape(DespairMemorial,  "0 0 -300");
	$memorial7 = createShape(DespairMemorial,  "0 0 -300");
	$memorial8 = createShape(DespairMemorial,  "0 0 -300");
	$memorial9 = createShape(DespairMemorial,  "0 0 -300");
	$memorial10 = createShape(DespairMemorial, "0 0 -300");
	$memorial11 = createShape(DespairMemorial, "0 0 -300");
	$memorial12 = createShape(DespairMemorial, "0 0 -300");
	$memorial13 = createShape(DespairMemorial, "0 0 -300");
	$memorial14 = createShape(DespairMemorial, "0 0 -300");
	$memorial15 = createShape(DespairMemorial, "0 0 -300");
	$memorial16 = createShape(DespairMemorial, "0 0 -300");
	$memorial17 = createShape(DespairMemorial, "0 0 -300");
	$memorial18 = createShape(DespairMemorial, "0 0 -300");
	$memorial19 = createShape(DespairMemorial, "0 0 -300");
	$memorial20 = createShape(DespairMemorial, "0 0 -300");
	$memorial21 = createShape(DespairMemorial, "0 0 -300");
	$memorial22 = createShape(DespairMemorial, "0 0 -300");
	$memorial23 = createShape(DespairMemorial, "0 0 -300");

	$instantGroup = %oldInstantGroup;
}
if (!isObject(MissionCleanup))
	schedule("0", "0", "createCourtroom");

function despairOnKill(%victim, %attacker, %crit)
{
	if(!isObject(%victim) || !isObject(%attacker))
		return;

	if(%victim == %attacker)
	{
		%victim.player.suicide = true;
		return 1;
	}

	if(!%victim.player.aboutToKill && !%victim.killer && !%attacker.killer)
	{
		%player = %attacker.player;
		%player.KnockOut(60);
		%player.noWeapons = true;

		%hold = %player.carryObject;

		%hold.lastTosser = %player;
		%hold.carryEnd = $Sim::Time;
		%hold.carryPlayer = 0;
		%player.carryObject = 0;
		%player.startCarrying = false;
		%player.lastNormal = "";
		%player.playThread(2, "root");
		ServerPlay3D("BodyDropSound" @ getRandom(1, 3), %hold.getPosition());
		if(%player.choking)
		{
			%hold.stopAudio(0);
			%player.choking = "";
		}

		//Clear all blood involved
		schedule(2500, 0, clearBloodBySource, %player);
		schedule(2500, 0, clearBloodBySource, %victim.player);
		%player.setBloody(0, 0, 0);
		%player.bleedTicks = 0;
		%victim.player.setBloody(0, 0, 0);
		%victim.player.bleedTicks = 0;

		messageClient(%victim, '', "<font:Impact:30>You just got RDMed!\c6 Please \c3/report [msg]\c6 the situation leading up to this.");
		messageClient(%attacker, '', "<font:Impact:30>You just RDMed!\c6 Please \c3/report [msg]\c6 the situation leading up to this.");
		%msg = "<font:Impact:30>" @ %attacker.getplayername() SPC "RDMed" SPC %victim.getPlayerName() @ "!";
		//echo("-+ " @ %msg);
		RS_Log("[RDM]" SPC %attacker.getPlayerName() SPC "(" @ getCharacterName(%attacker.character, 1) @ ") [" @ %attacker.getBLID() @ "] RDMed " @
				%victim.getPlayerName() SPC "(" @ getCharacterName(%victim.character, 1) @ ") [" @ %victim.getBLID() @ "]", "\c2");
		%count = ClientGroup.getCount();
		for (%i = 0; %i < %count; %i++)
		{
			%other = ClientGroup.getObject(%i);
			if (%other.isAdmin)
			{
				messageClient(%other, '', %msg);
				%other.play2d(DespairAdminBwoinkSound);
			}
		}
		cancel(%victim.player.critLoop);
		%victim.player.health = %player.maxhealth;
		%victim.player.KnockOut($Despair::SleepKnockout);
		return 0;
	}

	%player = %victim.player;
	if(!isObject(%player))
		%player = %victim.character.player;
	if(%player.aboutToKill || %victim.killer || %attacker.killer)
	{
		if(%player.aboutToKill)
		{
			%player.aboutToKill.health = $Despair::CritThreshold;
			%player.aboutToKill.critLoop();
			%player.aboutToKill = "";
		}

		%attacker.player.aboutToKill = %player; //Attacker can be killed
		%player.isMurdered = true; //rip they're legit

		//log stuff
		%tod = getDayCycleTime();
		%tod += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
		%tod = %tod - mFloor(%tod); //get rid of excess stuff
		%tod = getDayCycleTimeString(%tod, 1);

		if(!%crit) //only give pass to final blow/bleed out
		{
			$deathCount++;
			$lastVictim = %victim.character;

			%victim.killerHelper = true; //they gon help da killa
			%msg = "<color:FF0000>You just got murdered! You will now be able to speak exclusively with the killer. Use /spectate to interact with other dead people instead.";
			messageClient(%victim, '', "<font:impact:30>" @ %msg);

			if(%victim.killer && !%attacker.killer)
			{
				%attacker.player.aboutToKill = "";
				%attacker.killer = true;
				$pickedKiller = %attacker;
				%attacker.play2d(KillerJingleSound);
				%msg = "<color:FF0000>You murdered the killer in cold blood! Now it's your turn to get away with it...";
				messageClient(%attacker, '', "<font:impact:30>" @ %msg);
				if(%attacker.player.unconscious)
					%attacker.player.WakeUp();
				if(%attacker.player.statusEffect[$SE_sleepSlot] !$= "")
					%attacker.player.removeStatusEffect($SE_sleepSlot);
				%attacker.player.addMood(-10, "Oh, god... What have you done?");
			}
			else if ($deathCount <= 0)
			{
				%attacker.player.addMood(10, "You did it... Now you finally have a chance of escape!");
				%msg = "<color:FF0000>After comitting this crime, you will no longer be able to sprint. Comitting more murder after this is ill-advised.";
				messageClient(%attacker, '', "<font:impact:30>" @ %msg);
			}
			if ($deathCount >= $maxDeaths)
				DespairSetWeapons(0);
			//if(!isEventPending($DefaultMiniGame.subEventSchedule))
			//	$DefaultMiniGame.subEventSchedule = schedule($Despair::MissingLength*1000, 0, "despairStartInvestigation");
			RS_Log("[DMGLOG]" SPC %attacker.getPlayerName() SPC "[" @ %attacker.getBLID() @ "] murdered " @ 
					%victim.getPlayerName() SPC "[" @ %victim.getBLID() @ "]", "\c4");

			if(isObject(%player.attackImage[%player.attackCount]))
				%happening = getCharacterName(%attacker.character, 1, 1) SPC "murdered" SPC getCharacterName(%victim.character, 1, 1) SPC "with" SPC %player.attackImage[%player.attackCount].item.uiName;
			else
				%happening = getCharacterName(%attacker.character, 1, 1) SPC "murdered" SPC getCharacterName(%victim.character, 1, 1) SPC "with" SPC %player.attackType[%player.attackCount];
			$EndLog[$EndLogCount++] = "\c6[Day " @ $days @ ", " @ %tod @ "] \c0" @ %happening @ "!";

			%attacker.murders++;
			%attacker.dfSaveData();

			if(isEventPending(%attacker.player.critLoop))
			{
				cancel(%attacker.player.critLoop);
				%attacker.player.health = %attacker.player.maxhealth / 4;
				messageClient(%attacker, '', "\c5The kill has given you \c6second wind\c5!");
			}
			%victim.deaths++; //Only counts legit murders
			%victim.dfSaveData();
			%victim.AddPoints(1); //"yay you started the killfest" consolation prize for the victim so they're not TOO salty
		}
		else
		{
			RS_Log("[DMGLOG]" SPC %attacker.getPlayerName() SPC "[" @ %attacker.getBLID() @ "] critted " @ 
					%victim.getPlayerName() SPC "[" @ %victim.getBLID() @ "]", "\c4");

			if(isObject(%player.attackImage[%player.attackCount]))
				%happening = getCharacterName(%attacker.character, 1, 1) SPC "struck down" SPC getCharacterName(%victim.character, 1, 1) SPC "with" SPC %player.attackImage[%player.attackCount].item.uiName;
			else
				%happening = getCharacterName(%attacker.character, 1, 1) SPC "struck down" SPC getCharacterName(%victim.character, 1, 1) SPC "with" SPC %player.attackType[%player.attackCount];
			$EndLog[$EndLogCount++] = "\c6[Day " @ $days @ ", " @ %tod @ "] \c0" @ %happening @ "!";
		}
		return 1;
	}
}

function despairCheckInvestigation(%player, %corpse)
{
	if(!%corpse.suicide && !%corpse.checkedBy[%player])
	{
		if(isObject(%player.client))
		{
			if(!%player.client.killer && %player.client.TempPoints <= 0) //Atm only discovering the body gives you a point during investigation
				%player.client.AddPoints(1); //Body found
			%player.client.play2d(DespairBodyDiscoverySound @ (%corpse.mangled ? 5 : getRandom(1, 4)));
			%player.client.despairCorpseVignette(%corpse.mangled ? 300 : 200);
			if(!%corpse.discovered)
			{
				RS_Log(%player.client.getPlayerName() SPC "(" @ %player.client.getBLID() @ ") discovered " @ %corpse.character.name @ "'s corpse!", "\c2");
				%tod = getDayCycleTime();
				%tod += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
				%tod = %tod - mFloor(%tod); //get rid of excess stuff
				%tod = getDayCycleTimeString(%tod, 1);
				$EndLog[$EndLogCount++] = "\c6[Day " @ $days @ ", " @ %tod @ "] \c3" @ getCharacterName(%player.client.character, 1, 1) SPC "discovered" SPC %corpse.character.name @ "'s corpse!";
			}
			if ($Sim::Time - %player.lastMoodChange > 30 && %player.lastMoodText !$= "You discovered a body!")
				%player.addMood(-5, "You discovered a body!");
		}
		%corpse.checkedBy[%player] = true;
		%corpse.checked++;
		if(%corpse.checked >= 2 && !%corpse.discovered) //2 people screamed at this corpse!
		{
			despairStartInvestigation(1);
			despairMakeBodyAnnouncement("", %corpse.mangled);
			%corpse.discovered = true;
		}
		if(!%player.client.killer && %player.character.trait["Squeamish"] && !%player.squeamishFainted && !isEventPending(%player.passOutSchedule))
		{
			messageClient(%player.client, '', "\c5You're about to \c3faint\c5...!");
			%player.setSpeedScale(0.5);
			%player.passOutSchedule = %player.schedule(2000, knockOut, 30);
			%player.squeamishFainted = true; //Only one squeamish faint per round, otherwise life is pain
		}
		if(!%player.client.killer && %player.character.trait["Narcoleptic"] && !%player.squeamishFainted && !isEventPending(%player.passOutSchedule))
		{
			%player.passOutSchedule = %player.schedule(89000, knockOut, 30);
			%player.squeamishFainted = true; //Only one squeamish faint per round, otherwise life is pain
		}
	}
}

function despairMakeBodyAnnouncement(%unfound, %kira)
{
	if ($FinalBoss)
		return; //Final boss means you gotta killaroni
	serverPlay2d(AnnouncementSound);
	if (!%unfound)
		$announcements++;
	%time = getDayCycleTime();
	%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
	%time = %time - mFloor(%time); //get rid of excess stuff

	%time = getDayCycleTimeString(%time, 1);
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%cl = ClientGroup.getObject(%i);
		messageClient(%cl, '', '\c7[\c6%3\c7] \c0%2 on premises! \c5You guys have %1 minutes to investigate them before the trial starts.',
			MCeil(($investigationStart - $Sim::Time)/60),
			%unfound ? "There are UNDISCOVERED CORPSES to be found" : ($announcements > 1 ? "Another body has been discovered" : "A body has been discovered"),
			"D" @ $days @ "|" @ %time);
		if(isObject(%cl.player) && %cl.miniGame == $defaultMiniGame)
			%cl.player.addMood(-2);
	}

	%msg = "Body Announcement - " @ (%unfound ? "Undiscovered corpses" : ($announcements > 1 ? "Another body has been discovered" : "A body has been discovered"));

	RS_Log("[GAME]" @ %msg, "\c5");
	$EndLog[$EndLogCount++] = "\c6[Day " @ $days @ ", " @ %time @ "] \c2" @ %msg;

	%profile = DespairMusicInvestigationIntro @ pickField("1\t3");

	if(%unfound)
	{
		for (%i = 0; %i < $Despair::RoomCount; %i++)
		{
			%room = %i + 1;
			for(%a = 0; %a < BrickGroup_888888.NTObjectCount["_r" @ %room @ "_door"]; %a++)
			{
				%roomDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_door", %a];
				%roomDoor.lockState = false;
			}
		}
		$DefaultMiniGame.messageAll('', '\c5All doors have been unlocked!');
	}

	if($announcements > 2 || %kira) //if there's been two corpses or it's a disguise kit round
		%profile = DespairMusicInvestigationIntro2;
	if(!isObject(ServerMusic) || (ServerMusic.profile !$= %profile && ServerMusic.profile !$= %profile.loopProfile))
	{
		if(ServerMusic.profile $= DespairMusicInvestigationLoop1)
		{
			ServerPlaySong(%profile);
		}
		else
		{
			cancel($musicSchedule);
			$musicSchedule = schedule(15000, 0, ServerPlaySong, %profile);
		}
	}
}

function despairStartInvestigation(%no_announce)
{
	//%maxDeaths = mCeil(GameCharacters.getCount() / 4); //16 chars = 4 deaths, 8 chars = 2 deaths
	//if ($deathCount >= %maxDeaths)
	//	DespairSetWeapons(0);
	if ($deathCount > 0)
	{
		$investigationLength = $investigationStart $= "" ? $Despair::InvestigationLength : ($investigationStart - $Sim::Time) + $Despair::InvestigationExtraLength;
		//if($investigationStart > $Sim::Time + $investigationLength) //investigation longer than extralength
		//	return;
		if ($investigationLength > 600) //10 mins
			$investigationLength = 600;
		$investigationStart = $Sim::Time + $investigationLength;
		if (!%no_announce)
			despairMakeBodyAnnouncement(1);
		//cancel($DefaultMiniGame.subEventSchedule);
		if(!$DefaultMiniGame.noWeapons && !isEventPending($DefaultMiniGame.subEventSchedule) && $Despair::DisableWepsInvest)
		{
			if ($Despair::DisableWepsTimer > 0)
			{
				if ($pickedKiller)
					messageClient($pickedKiller, '', '<font:impact:24>\c5Warning! You will be unable to swing your weapons in \c6%1 seconds\c5!', $Despair::DisableWepsTimer);
				$DefaultMiniGame.subEventSchedule = schedule($Despair::DisableWepsTimer*1000, 0, "DespairSetWeapons", 0); //Only disable weapons 30 secs after
			}
			else
				DespairSetWeapons(0);
		}
		cancel($DefaultMiniGame.eventSchedule);
		$DefaultMiniGame.eventSchedule = schedule($investigationLength*1000, 0, "courtPlayers");
		serverPlay2d("DespairMusicInvestigationStart");
		RS_Log("[GAME] Investigation has started", "\c5");
		%tod = getDayCycleTime();
		%tod += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
		%tod = %tod - mFloor(%tod); //get rid of excess stuff
		%tod = getDayCycleTimeString(%tod, 1);

		$EndLog[$EndLogCount++] = "\c6[Day " @ $days @ ", " @ %tod @ "] \c2Investigation has started.";

		for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
		{
			%client = $DefaultMiniGame.member[%i];
			%player = %client.player;
			if(isObject(%player))
			{
				%player.setStatusEffect($SE_sleepSlot, "");
			}
		}
	}
}

function dropFoods()
{
	%charCount = GameCharacters.getCount();
	%foodCount = mCeil(%charCount * 0.6) + getRandom(-2, 2);

	while(%foodCount >= 0)
	{
		%brick = BrickGroup_888888.NTObject["_food", getRandom(0, BrickGroup_888888.NTObjectCount["_food"] - 1)];
		%brick.setItem(getRandom() > 0.1 ? "BurgerItem" : "BananaItem"); //I'm too lazy to check if the brick already has food spawned sooo....
		%foodCount--;
	}
	$DefaultMiniGame.chatMessageAll('', '\c3~~ Food has been restocked!\c6 Go eat your cheeseburgers.');
}

function despairOnMorning()
{
	RS_Log("[GAME] Good morning!", "\c5");
	
	if ($investigationStart !$= "")
		return;

	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(isObject(%player))
		{
			if(%player.statusEffect[$SE_sleepSlot] !$= "tired") //This nerd ain't exhausted, don't torture them yet.
			{
				%player.schedule(1000 * (getRandom() * 3), updateStatusEffect, $SE_sleepSlot); //Update all tiredness-related status effects
				%client.updateBottomprint();
			}
		}
	}

	if (getRandom() <= 0.4 && $Despair::Sandstorm)
	{
		//update fog
		$EnvGuiServer::VisibleDistance = 70;
		Sky.visibleDistance = $EnvGuiServer::VisibleDistance;
		$EnvGuiServer::FogDistance = 0;
		Sky.fogDistance = $EnvGuiServer::FogDistance;
		Sky.sendUpdate();
		$DefaultMiniGame.chatMessageAll('', '\c3~~ Oh, no! There is a \c6dust storm!');
	}
	else
	{
		//update fog
		$EnvGuiServer::VisibleDistance = 110;
		Sky.visibleDistance = $EnvGuiServer::VisibleDistance;
		$EnvGuiServer::FogDistance = 100;
		Sky.fogDistance = $EnvGuiServer::FogDistance;
		Sky.sendUpdate();
	}

	dropFoods();

	%count = BrickGroup_888888.NTObjectCount["_evidence"];
	if(%count <= 0)
		return;
	// prepare
	for (%i = 0; %i < %count; %i++)
		%a[%i] = %i;
	// shuffle
	while (%i--)
	{
		%j = getRandom(%i);
		%x = %a[%i - 1];
		%a[%i - 1] = %a[%j];
		%a[%j] = %x;
	}

	%evidencePapers = getRandom(0, 2);
	%tipsPapers = getRandom(3, 6);
	%trashPapers = getRandom(3, 6);
	//Spawn evidence
	for (%i = 0; %i < %count; %i++)
	{
		%brick = BrickGroup_888888.NTObject["_evidence", %a[%i]];
		%brick.setItem(PaperItem);
		%props = %brick.item.getItemProps();

		if(%evidencePapers > 0)
		{
			%props.name = "Daily News";
			%props.contents = getPaperEvidence();
			%evidencePapers--;
			continue;
		}
		else if(%tipsPapers > 0)
		{
			%props.name = "LifeHack";
			%props.contents = getPaperTips();
			%tipsPapers--;
			continue;
		}
		else if(%trashPapers > 0)
		{
			%props.name = "Paper";
			%props.contents = getPaperTrash();
			%trashPapers--;
			continue;
		}
		break;
	}

	if($days == 1 && $deathCount <= 0)
	{
		if($spawnKillerBox)
		{
			%brick = BrickGroup_888888.NTObject["_r" @ $pickedKiller.character.room @ "_closet", 0];
			if($pickedKiller.player.addTool("KillerBoxItem") != -1)
				messageClient($pickedKiller, '', '<font:Impact:24>\c5You have a box of useful items in your inventory!');
			
			else if(isObject(%brick))
			{
				%brick.setItem("KillerBoxItem");
				messageClient($pickedKiller, '', '<font:Impact:24>  \c3Your closet now contains a \c6box of useful items\c3! Pick it up and open or discard it \c0ASAP\c3!!!');
				commandToClient($pickedKiller, 'CenterPrint', "\c3Your closet now contains a \c6box of useful items\c3!", 3);
			}
		}
		else
			messageClient($pickedKiller, '', '<font:Impact:24>\c5Killer box didn\'t spawn because you rejected it.');
	}

	if($days % 4 == 2)
	{
		$DefaultMiniGame.chatMessageAll('', "\c0<font:impact:30>WARNING\c5: Next day, the body announcement will trigger if there were any deaths.");
		if($deathCount <= 0)
			messageClient($pickedKiller, '', "<font:impact:30>WARNING\c6: This is your last chance to kill. If you don't commit murder you will be \c0EXECUTED\c6.");
	}

	if($days % 4 == 3 && $investigationStart $= "") //Court 'em on the third day if there's no investigation
	{
		if($deathCount <= 0)
		{
			%ass = $pickedKiller;
			DespairPickKiller(true);
			if(isObject(%ass) && isObject(%ass.player))
				%ass.player.kill();
		}
		else
			despairStartInvestigation();
	}
}

function despairOnNoon()
{

}

function despairOnEvening()
{
    if ($investigationStart !$= "")
		return;

	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(isObject(%player))
		{
			%player.updateStatusEffect($SE_sleepSlot); //Update all tiredness-related status effects
			%client.updateBottomprint();
		}
	}
}

function despairOnLateEvening()
{
	if ($investigationStart !$= "")
		return;

	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(isObject(%player))
		{
			%player.updateStatusEffect($SE_sleepSlot);
			if(!%client.killer && %player.statusEffect[$SE_sleepSlot] $= "")
				%player.setStatusEffect($SE_sleepSlot, "sleepy");
		}
	}

	if($days == 2)
		$DefaultMiniGame.chatMessageAll('', "\c0<font:impact:30>WARNING\c5: This is going to be your last night! After this, trial period starts. Have you gathered enough evidence?");	
}

function despairKillerLottery()
{
	$Despair::Queue["Killer"] = "";
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%cl = ClientGroup.getObject(%i);
		if (!isObject(%pl = %cl.player) || %cl.miniGame != $defaultMiniGame || %pl.noWeapons || %cl.afk)
			continue;
		
		if (%cl.killerbanned)
		{
			commandToClient(%cl, 'messageBoxOK', "Killer Ballot", "You are banned from being the killer.");
			continue;
		}
		
		%cl.prompted["Killer"] = true;
		commandToClient(%cl, 'messageBoxYesNo', "Killer Ballot", "Do you wish to be included in the killer lottery for this round?", 'KillerAccept');
	}
	$DefaultMiniGame.eventSchedule = schedule(30000, 0, DespairPickKiller);
	ServerPlaySong("DespairMusicPreamble");
}

function despairOnNight()
{
	RS_Log("[GAME] Good night...", "\c5");
	if ($investigationStart !$= "")
		return;

    if (!$pickedKiller)
		despairKillerLottery();

	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(isObject(%player))
		{
			if(%client.charModeIndex !$= "") //We're in character creation mode, end our memery
			{
				cleanupCharacterCreation(%client);
				characterCreationFixPlayer(%client);
			}
			// %player.updateStatusEffect($SE_sleepSlot); //Update all tiredness-related status effects
			// %client.updateBottomprint();
		}
	}

	if($days > 0)
		return;

	if (!$DefaultMiniGame.permaDeath || $DefaultMiniGame.winRounds <= 0)
		ClearFlaggedCharacters(); //call this so joiners-leavers before night 1 are not counted for anything

	// prepare
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
		%a[%i] = %i;
	// shuffle
	while (%i--)
	{
		%j = getRandom(%i);
		%x = %a[%i - 1];
		%a[%i - 1] = %a[%j];
		%a[%j] = %x;
	}

	//rip guest list
	//%brick = BrickGroup_888888.NTObject["_guestlist", 0];
	//%brick.setItem("PaperItem");
	//%props = %brick.item.getItemProps();
	//%props.name = "Guest List";
	//%props.contents = getGuestList(0);
}

function serverCmdKillerAccept(%this)
{
	if(%this.prompted["Killer"] && !$pickedKiller)
	{
		%queue = $Despair::Queue["Killer"];
		if (strPos(%queue, %this) >= 0)
			return;
		%queue = setWord(%queue, getWordCount(%queue), %this);
		$Despair::Queue["Killer"] = %queue;
		%this.prompted["Killer"] = false;
		commandToClient(%this, 'messageBoxOK', "Killer Ballot", "You will now be included in the killer lottery.");
	}
}

function DespairPickKiller(%repick)
{
	if(!%repick)
		cancel($DefaultMiniGame.eventSchedule);
	if(!$pickedKiller || %repick)
	{
		%queue = $Despair::Queue["Killer"];
		if(isObject($pickedKiller))
        {
			$pickedKiller.killer = false;
			%queue = removeWorld(%queue, findWord(%queue, $pickedKiller));
		}
		while (getWordCount(%queue) > 0 && !isObject(%client))
		{
			%int = getRandom(0, getWordCount(%queue)-1);
			%client = getWord(%queue, %int);
			%queue = removeWord(%queue, %int);
		}
		if(isObject($forceKiller))
			%client = $forceKiller;
		$forceKiller = "";

		if(!isObject(%client))
		{
            RS_Log("[KILLER]" SPC "Nobody became killer! What the fuck!", "\c2");
			for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%other = ClientGroup.getObject(%i);
				if (%other.isAdmin)
				{
					messageClient(%other, '', "Nobody became killer! What the fuck!");
				}
			}
			return;
		}
		%client.play2d(KillerJingleSound);
		%msg = "<color:FF0000>You are plotting murder against someone! Kill them and do it in such a way that nobody finds out it\'s you!";
		messageClient(%client, '', "<font:impact:30>" @ %msg);
		$spawnKillerBox = false;
		%client.prompted["Box"] = true;
		commandToClient(%client, 'messageBoxYesNo', "MURDER TIME!", %msg @ "\n<color:000000>Do you wish for a Box of Killer Goodies to spawn?", 'KillerBoxAccept');
		%client.killer = true;
		//echo(%client.getplayername() SPC "is killa");
		RS_Log("[KILLER]" SPC %client.getPlayerName() SPC "(" @ getCharacterName(%client.character, 1, 1) @ ") [" @ %client.getBLID() @ "] became the killer!", "\c2");
		$pickedKiller = %client;

		if(%client.player.unconscious)
		{
			%client.setControlObject(%cam = %client.camera);
			%cam.setMode("CORPSE", %client.player);
			%client.player.currResting = 1;
			%client.chatMessage("\c6You are faking sleep. Press any key to get up.");
		}
		if(%client.player.statusEffect[$SE_sleepSlot] !$= "")
			%client.player.removeStatusEffect($SE_sleepSlot);
	}
}

function serverCmdKillerBoxAccept(%this)
{
	if(%this.prompted["Box"] && $days <= 1)
	{
		%queue = $Despair::Queue["Killer"];
		if (strPos(%queue, %this) < 0)
			return;
		$spawnKillerBox = true;
		%this.prompted["Box"] = false;
		commandToClient(%this, 'messageBoxOK', "Killer Box", "A killer box will now spawn in your inventory at morning.");
	}
}

function DespairSpecialChat(%client, %text)
{
	if(!$DespairTrial)
		return 0;
	if(!isObject(%player = %client.player))
		return 0;

	if($DespairTrialOpening && %client != $DespairTrialCurrSpeaker)
	{
		messageClient(%client, '', "\c0You cannot talk yet! \c5Wait for your turn.");
		return 1;
	}

	if($Sim::Time - %client.lastMemeSpeakTime < 10 && getWordCount(%text) <= 1) //No towerposting!
	{
		messageClient(%client, '', '\c5Speak in full sentences\c6!');
		return 1;
	}

	if (getWordCount(%text) == 1)
		%client.lastMemeSpeakTime = $Sim::Time;

	if($Sim::Time < %client.timeOut) //Special trial ability
	{
		messageClient(%client, '', '\c5You\'re unable to speak\c6!');
		return 1;
	}

	if(%player.objecting && $DespairTrialDiscussion !$= "") //Objection time
	{
		%player.lastScream = $Sim::Time;

		%player.objected = true;
		%player.objecting = false;
		%player.emote(AlarmProjectile);
		%player.playThread(3, "talk");
		%player.schedule(strLen(%text) * 35, "playThread", 3, "root");
		serverPlay2d("ObjectionSound");

		%shape = new Item()
		{
			datablock = DespairEmptyFloatItem;
			position = VectorAdd(%player.position, "0 0 2");
		};
		%shape.noExamine = true;
		%shape.setCollisionTimeout(%player);
		%shape.setShapeName(%text);
		%shape.setShapeNameDistance(100);
		%shape.setShapeNameColor("1 0.3 0.3");
		%shape.setVelocity("0 0 0.5");
		%shape.deleteSchedule = %shape.schedule(3000, delete);

		RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") objected!", "\c2");

		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%member = ClientGroup.getObject(%i);
			if(%member != %client && isObject(%member.player))
				%member.timeOut = $Sim::Time + 5;
			messageClient(%member, '', '\c7[%1]<color:ffff80>%2 %3<color:fffff0>,<font:Impact:20> %4', getTimeString(mFloor($Sim::Time - $DespairTrial)), getCharacterName(%client.character, 1), "objects", %text);
		}
		return 1;
	}
	return 0;
}

function courtPlayers()
{
	createCourtroom(); //Update dem colors

	SunLight.flareSize = 0;
	SunLight.sendUpdate();
	cancel($DefaultMiniGame.subEventSchedule);
	cancel($DefaultMiniGame.eventSchedule);
	cancel(DayCycle.timeSchedule);
	$EnvGuiServer::DayCycleEnabled = 0;
	DayCycle.setEnabled($EnvGuiServer::DayCycleEnabled);
	%charCount = GameCharacters.getCount();
	// prepare
	for (%i = 0; %i < %charCount; %i++)
		%a[%i] = %i;
	// shuffle
	while (%i--)
	{
		%j = getRandom(%i);
		%x = %a[%i - 1];
		%a[%i - 1] = %a[%j];
		%a[%j] = %x;
	}

	for (%i = 0; %i < 24; %i++)
	{
		$stand[%i].setTransform("0 0 -300");
		$memorial[%i].setTransform("0 0 -300");
	}

	%secs = 30;
	%radius = getMax(2, %charCount * 0.5);
	$courtvoid.setTransform("0 8 100");
	$courtvoid.setScale("20 20 20");

	for (%i = 0; %i < %charCount; %i++)
	{
		%angle = $m2pi * (%i / %charCount);
		%x = mCos(%angle) * %radius;
		%y = 8 + mSin(%angle) * %radius;
		$stand[%i].setTransform(%x SPC %y SPC "100 0 0 -1" SPC (%angle + $piOver2));

		%character = GameCharacters.getObject(%a[%i]);
		%player = %character.player;
		%client = %character.client;
		if(%client.afk && !%client.killer)
		{
			%player.delete();
		}

		%transform = $stand[%i].getSlotTransform(1);
		if(!isObject(%client) || !isObject(%player) || %player.isDead)
		{
			$memorial[%i].setTransform(%transform);
			%state = "MISSING";
			if(isObject(%player))
			{
				if(%player.isMurdered)
					%state = "MURDER";
				else if(%player.suicide)
					%state = "SUICIDE";
				else if(%player.executed) //ran outta map or something
					%state = "EXECUTED";
				else
					%state = "AFK";
			}
			%doll = new Player()
			{
				datablock = playerFrozenArmor;
				character = %character;
			};
			%doll.setTransform(%transform);
			%doll.setScale("1 0.05 1");
			%doll.mangled = %character.mangled;
			%doll.desaturate = true;
			%doll.applyAppearance(%character);
			%doll.hideNode("larm");
			%doll.hideNode("larmslim");
			%doll.hideNode("rarm");
			%doll.hideNode("rarmslim");
			%doll.hideNode("lhand");
			%doll.hideNode("rhand");
			%doll.hideNode("chest");
			%doll.hideNode("femchest");
			%doll.hideNode("pants");
			%doll.hideNode("lshoe");
			%doll.hideNode("rshoe");
			%doll.mountImage(MemorialCrossImage, 0);
			%doll.noExamine = true;
			%doll.setShapeName(getCharacterName(%character, 1) SPC "(" @ %state @ ")", "8564862");

			GameRoundCleanup.add(%doll);

			if(%doll.mangled)
			{
				$mangled = %doll;
				%secs = 40;
				%doll.setShapeName("Unknown" SPC "(" @ %state @ ")", "8564862");
			}
		}
		else
		{
			%player.WakeUp();
			if(%player.inCameraEvent)
			{
				%player.inCameraEvent = false;
			}

			$stand[%i].player = %player;
			%player.setTransform(vectorAdd(getWords(%transform, 0, 2), "0 0 0.1") SPC getWords(%transform, 3, 6));
			%player.changeDataBlock(playerFrozenArmor);
			%player.playThread(0, "standing");
			%player.setVelocity("0 0 0");
		}
	}

	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		if(isObject($mangled))
		{
			//aim the camera at the target
			%pos = vectorAdd($mangled.getHackPosition(), vectorScale($mangled.getForwardVector(), 3));
			%delta = vectorSub($mangled.getHackPosition(), %pos);
			%deltaX = getWord(%delta, 0);
			%deltaY = getWord(%delta, 1);
			%deltaZ = getWord(%delta, 2);
			%deltaXYHyp = vectorLen(%deltaX SPC %deltaY SPC 0);

			%rotZ = mAtan(%deltaX, %deltaY) * -1; 
			%rotX = mAtan(%deltaZ, %deltaXYHyp);

			%aa = eulerRadToMatrix(%rotX SPC 0 SPC %rotZ); //this function should be called eulerToAngleAxis...

			%camera = %client.camera;

			%camera.setTransform(%pos SPC %aa);
			%camera.setFlyMode();
			%camera.mode = "Observer";
			%client.setControlObject(%camera);
			%camera.setControlObject(%client.dummyCamera);
			%client.schedule(10000, playPath, TrialIntroPath);
		}
		else
			%client.playPath(TrialIntroPath);
	}

	if(isObject($mangled))
		ServerPlaySong(pickField("DespairMusicIntense" TAB "DespairMusicWonderfulIntro"));
	else
		ServerPlaySong("DespairMusicOpeningIntro");
	$DefaultMiniGame.chatMessageAll('', '\c5<font:impact:30>Everyone now has %1 seconds to prepare their opening statements! Before that, nobody can talk.', %secs);
	$DefaultMiniGame.chatMessageAll('', '\c5<font:impact:30>Suggested topics of discussion: \c6Your Alibi, Friend\'s Alibi, Crime Scene, Murder Weapon, Thoughts and Suspicions');
	if(isObject($mangled))
		$DefaultMiniGame.schedule(1000, chatMessageAll, '', "<font:impact:30>\c0A victim had their \c6identity stolen\c0. One of you shouldn't be alive . . .");
	else if($deathCount > 2) //Throw them a bone.
		$DefaultMiniGame.chatMessageAll('', '<font:impact:30>\c5Due to the number of deaths, only the killer of \c6%1\c5 will be relevant for this trial.', getCharacterName($lastVictim, 1));
	$DefaultMiniGame.eventSchedule = schedule(%secs * 1000, 0, DespairStartOpeningStatements);

	$chatDelay = 0.25;

	$forceVoteCount = 0;
	$DespairTrialCurrSpeaker = "";
	$DespairTrial = $Sim::Time;
	$DespairTrialVote = false;
	$DespairTrialOpening = true;
	$DespairTrialDiscussion = "";
	DespairSetWeapons(0);

	$EnvGuiServer::VisibleDistance = 500;
	Sky.visibleDistance = $EnvGuiServer::VisibleDistance;
	$EnvGuiServer::FogDistance = 400;
	Sky.fogDistance = $EnvGuiServer::FogDistance;
	Sky.sendUpdate();

	despairBottomPrintLoop();

	RS_Log("[GAME] Trial period has begun.", "\c5");

	$EndLog[$EndLogCount++] = "\c6[" @ getTimeString(mFloor($Sim::Time - $DespairTrial)) @ "] \c5Trial has begun.";
}

function DespairStartOpeningStatements()
{
	cancel($DefaultMiniGame.eventSchedule);
	if(!isObject($mangled))
		ServerPlaySong("DespairMusicOpeningLoop");
	$DefaultMiniGame.chatMessageAll('', "\c5Let's hear everybody out.");
	$DefaultMiniGame.eventSchedule = schedule(1000, 0, DespairCycleOpeningStatements, 0);
	RS_Log("[GAME] Opening statements!", "\c5");
}

function DespairCycleOpeningStatements(%j)
{
	if(%j > GameCharacters.getCount())
	{
		$DefaultMiniGame.eventSchedule = schedule(1000, 0, DespairStartDiscussion);
		$DefaultMiniGame.chatMessageAll('', "\c5All statements have been heard.");
		return;
	}
	%player = $stand[%j].player;
	%client = %player.client;
	if(isObject(%player))
	{
		$DespairTrialCurrSpeaker = %client;
		for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
		{
			%targ = $DefaultMiniGame.member[%i];
			%camera = %targ.camera;
			//aim the camera at the target
			%pos = vectorAdd(%player.getHackPosition(), vectorScale(%player.getForwardVector(), 3));
			%delta = vectorSub(%player.getHackPosition(), %pos);
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
			%targ.setControlObject(%camera);
			%camera.setControlObject(%targ.dummyCamera);
		}
		$DefaultMiniGame.chatMessageAll('', "\c5What will" SPC getCharacterName(%player.character, 1) SPC "say?");
		%client.chatMessage("\c5(It's your turn!)");
		$DefaultMiniGame.eventSchedule = schedule(10000, 0, DespairCycleOpeningStatements, %j+1);
	}
	else
	{
		DespairCycleOpeningStatements(%j+1);
	}
}

function DespairStartDiscussion()
{
	RS_Log("[GAME] Discussion start!", "\c5");
	cancel($DefaultMiniGame.eventSchedule);
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%client.playPath(TrialDiscussionPath);
		%client.camera.schedule(5000, setControlObject, %client.camera);
		if(isObject(%client.player))
			%client.schedule(5000, setControlObject, %client.player);
		else
			%client.schedule(5000, setControlObject, %client.camera);
	}
	$DespairTrialOpening = false;
	$DespairTrialDiscussion = $Sim::Time;
	$chatDelay = $Despair::TrialChatDelay; //less spam, please

	%time = $Despair::DiscussPeriod + ($Despair::DiscussExtraLength * ($deathCount - 1)); //extra 1.30 mins for every extra body

	if(isObject($mangled))
		%time = $Despair::MangleTimer;

	if(%time >= $Despair::DiscussPeriod * 1.5)
		ServerPlaySong("DespairMusicWorldendDominator");
	else if(%time >= $Despair::DiscussPeriod && getRandom(0, 1) == 1)
		ServerPlaySong("DespairMusicTrialDiscussion" @ (getRandom(0, 1) == 1 ? "Loop5" : "Intro4"));
	else
		ServerPlaySong("DespairMusicTrialDiscussionIntro" @ getRandom(1, 3));

	$DefaultMiniGame.chatMessageAll('', "\c5You have \c3" @ %time / 60 @ " minutes\c5 to discuss and reveal the killer.");
	$DefaultMiniGame.chatMessageAll('', "\c5After time has passed you will have to \c0eliminate the killer\c5 by \c3voting.");
	$DefaultMiniGame.chatMessageAll('', "\c0You cannot afford a mistake. \c5Choose wrong, and everyone but the killer will die.");
	$DefaultMiniGame.eventSchedule = schedule(%time * 1000, 0, DespairStartVote);
}

function DespairStartVote()
{
	RS_Log("[GAME] Vote start!", "\c5");
	cancel($DefaultMiniGame.eventSchedule);
	$DespairTrialVote = true;
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(isObject(%player))
		{
			%player.voteTarget = "0";
			%player.canReceiveVote = "1";
			%player.canCastVote = "1";
			%player.playThread(2, armReadyRight);
			DespairUpdateCastVote(%player);
		}
	}

	$chatDelay = $Despair::chatDelay; //BURNING DISCUSSION

	ServerPlaySong("DespairMusicVoteStart");
	$DefaultMiniGame.chatMessageAll('', "\c5Look at the person you think is the killer within 30 seconds. The person with the most votes \c0will die.");
	$DefaultMiniGame.eventSchedule = schedule(30000, 0, DespairEndVote);

	$EndLog[$EndLogCount++] = "\c6[" @ getTimeString(mFloor($Sim::Time - $DespairTrial)) @ "] \c5Voting start!";
}

function DespairEndVote()
{
	RS_Log("[GAME] Vote end!", "\c5");
	cancel($DefaultMiniGame.eventSchedule);
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;

		if (%player.canCastVote)
		{
			%player.voteTarget.votes++;
			%player.playThread(2, root);
			%player.correctVote = 0;

			if(isObject(%player.voteTarget))
			{
				$EndLog[$EndLogCount++] = "\c6[" @ getTimeString(mFloor($Sim::Time - $DespairTrial)) @ "]\c3" SPC getCharacterName(%client.character, 1, 1) SPC "voted for" SPC getCharacterName(%player.voteTarget.character, 1, 1);
				if(%player.voteTarget.client.killer)
					%player.correctVote = 1;
			}
			else
			{
				$EndLog[$EndLogCount++] = "\c6[" @ getTimeString(mFloor($Sim::Time - $DespairTrial)) @ "]\c3" SPC getCharacterName(%client.character, 1, 1) SPC "didn't vote!";
			}

			cancel(%player.DespairUpdateCastVote);
			%player.canCastVote = "";
			%player.voteTarget = "";
		}
	}
	$DefaultMiniGame.subEventSchedule = schedule(4000, 0, "ServerPlaySong", "DespairMusicPreamble");
	$DefaultMiniGame.chatMessageAll('', "\c5Your votes have been cast. Did you make the right choice? Or the dreadfully wrong one?");
	$DefaultMiniGame.chatMessageAll('', "\c6One minute to say your prayers...");
	$DefaultMiniGame.eventSchedule = schedule(60000, 0, DespairEndTrial);
}

function DespairEndTrial()
{
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		
		if (%player.canReceiveVote)
		{
			if (%highestVotes !$= "" && %player.votes $= %highestVotes)
				%tie = 1;
			else if (%highestVotes $= "" || %player.votes > %highestVotes)
			{
				%tie = 0;
				%highestVotes = %player.votes;
				%unfortunate = %player;
			}

			%player.canReceiveVote = "";
		}
	}

	if (%tie)
	{
		$DefaultMiniGame.chatMessageAll('', "\c5It's a tie. Killer wins.");
	}
	else if (isObject(%unfortunate))
	{
		if(%unfortunate.client.killer)
		{
			$DefaultMiniGame.chatMessageAll('', "\c5The killer has been eliminated! Innocents won.");
			%unfortunate.setVelocity(vectorAdd(vectorScale(%unfortunate.getForwardVector(), 3), "0 0 10"));
			%unfortunate.kill();
			%win = 1;
		}
		else
			$DefaultMiniGame.chatMessageAll('', '\c5Majority vote - \c6%1\c5 - is innocent. Killer wins.', %unfortunate.client.getPlayerName());
	}
	else
		$DefaultMiniGame.chatMessageAll('', "\c5Nobody voted. Killer wins.");

	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%player = %client.player;
		if(%client.killer)
			continue;
		if(!%win)
		{
			%player.setVelocity(vectorAdd(vectorScale(%player.getForwardVector(), 3), "0 0 10"));//%player.setVelocity(vectorAdd(vectorSub(%player.getPosition(), "0 8 0"), "0 0 10"));
			%player.kill();
		}
		else if(isObject(%player))
		{
			%client.AddPoints(3 + %player.correctVote*3); //Correct vote gives you 2 more points
			%client.innocentWins++;
			%client.correctVotes += %player.correctVote;
		}
	}

	if(!%win)
	{
		serverPlay2d("DespairMusicKillerWin");
		$defaultMiniGame.winRounds = 0;
		$pickedKiller.AddPoints(10);
		$pickedKiller.killerWins++;

		$shuttersOpen = "";
		//Always have at least X shutters open
		%done = 0;
		%shutters = mCeil($DefaultMiniGame.numMembers * 0.15); //20% of current players
		while(getWordCount($shuttersOpen) < $shutterCount && %done <= %shutters)
		{
			%num = getRandom(1, $shutterCount);
			if(strpos($shuttersOpen, %num) == -1)
			{
				$shuttersOpen = setWord($shuttersOpen, getWordCount($shuttersOpen), %num);
				%done++;
			}
		}
	}
	else
	{
		serverPlay2d("DespairMusicInnocentsWin");
		$defaultMiniGame.winRounds += 1;
		while(getWordCount($shuttersOpen) < $shutterCount)
		{
			%num = getRandom(1, $shutterCount);
			if(strpos($shuttersOpen, %num) == -1)
			{
				$shuttersOpen = setWord($shuttersOpen, getWordCount($shuttersOpen), %num);
				break;
			}
		}
	}

	despairEndGame();
}

function DespairUpdateCastVote(%player)
{
	cancel(%player.DespairUpdateCastVote);

	if (!%player.canCastVote)
		return;

	%a = %player.getEyePoint();
	%v = %player.getAimVector();
	%b = VectorAdd(%a, VectorScale(%v, 100));
	%mask = $TypeMasks::PlayerObjectType;
	%ray = containerRayCast(%a, %b, %mask, %player);
	%col = firstWord(%ray);

	if (%col && !%col.canReceiveVote)
		%col = "0";
	
	%player.voteTarget = %col;
	if(isObject(%col) && isObject(%col.client) && isObject(%col.character))
		commandToClient(%player.client, 'CenterPrint', "\c3You are voting against\c6" SPC getCharacterName(%col.character, 1), 1);
	else
		commandToClient(%player.client, 'ClearCenterPrint');
	%player.DespairUpdateCastVote = schedule(32, %player, "DespairUpdateCastVote", %player);
}

function DespairTrialDropTool(%pl, %slot)
{
	%cl = %pl.client;
	if (!isObject(%pl.tool[%slot]))
		return;
	%tool = %pl.tool[%slot];
	//if(%tool.isIcon)
	//	return;
	%pl.tool[%slot] = "";
	messageClient(%cl, 'MsgItemPickup', '', %slot, -1, 1);
	%value = 3.825 + %pl.itemOffset;
	if (isObject(%props = %pl.itemProps[%slot]))
		%pl.itemProps[%slot] = "";
	if (%tool.className $= "Hat")
	{
		%pl.unMountImage(2);
		%pl.applyAppearance();
	}
	if (%tool.getName() $= "CoatItem")
	{
		%pl.unMountImage(1);
		%pl.applyAppearance();
	}
	%item = new Item()
	{
		position = "0 0 0";
		datablock = %tool;
		itemProps = %props;
		minigame = %cl;
		static = true;
		rotate = true;
	};
	%value -= getWord(%box = %item.getWorldBox(), 2);
	%height = getWord(%box, 5) - getWord(%box, 2);
	%pl.itemOffset += %height + 0.2;
	%offset = setWord("0 0 0", 2, %value);
	%item.setTransform(vectorAdd(%pl.getPosition(), %offset));
	if (%tool.getName() $= "KeyItem")
		%item.setShapeName(%item.itemProps.name);
	else if (%tool.getName() $= "PaperItem")
		%item.setShapeName(%item.itemProps.name);
	else
		%item.setShapeName(%tool.uiName);
	%item.canPickUp = false;
	MissionCleanup.add(%item);
	GameRoundCleanup.add(%item);
	RS_Log(%pl.client.getPlayerName() SPC "(" @ %pl.client.getBLID() @ ") used /dropTool '" @ %tool.uiName @ "'", "\c2");
}

function DespairTrialOnAlarm(%client)
{
	if(!isObject(%pl = %client.player))
		return;
	if($DespairTrialDiscussion $= "")
		return;
	if ($Sim::Time - %pl.lastScream < 60)
		return;

	if (%pl.character.trait["Loudmouth"] || (!%pl.objected && !%pl.character.trait["Softspoken"]))
	{
		%pl.objecting = !%pl.objecting;
		messageClient(%client, '', '\c5Your next message will %1be an \c3Objection\c5.', %pl.objecting ? "" : "no longer ");
	}
}
