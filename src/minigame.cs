if (!isObject(GameRoundCleanup))
	new SimSet(GameRoundCleanup);

if (!isObject(GameCharacters))
	new SimSet(GameCharacters);

function ClearFlaggedCharacters()
{
	for(%i=0; %i < GameCharacters.getCount(); %i++)
	{
		%char = GameCharacters.getObject(%i);
		if(isObject(%char.client) && !%char.isDead)
			continue;

		%char.delete();
	}
}

function createPlayer(%client)
{
	if(%client.spectating)
	{
		messageClient(%client, '', '\c5You didn\'t spawn because you are spectating.');
		return;
	}
	cleanupCharacterCreation(%client);
	if (isObject(%client.character))
		%nopersist = %client.character.client.noPersistance;

	if ($defaultMiniGame.permaDeath)
		%nopersist = false;

    //Create character if none exists
    %gender = getRandomGender();
	if(!isObject(%character = %client.character) || %character.isDead || %nopersist)
	{
		if ($defaultMiniGame.permaDeath && $defaultMiniGame.winRounds > 0)
		{
            messageClient(%client, '', '\c5You are dead - you cannot respawn during permadeath.');
            return;
		}
		if(isObject(%character))
			%character.delete();
		%character = new ScriptObject()
		{
			class = "Character";

			gender = %gender;
			name = getRandomName(%gender);
			appearance = getRandomAppearance(%gender);

			client = %client;
			clientName = %client.getPlayerName();
		};
		GameCharacters.add(%character);
		//pick traits
		if($Despair::Traits::Enabled)
			GenerateTraits(%character, %client);

		%client.character = %character;
	}
	else
		%survivalPerk = true;

	if(getField(%character.appearance, 3) $= "")
		%character.appearance = setField(%character.appearance, 3, getRandomHairName(%character.gender));

	%room = %character.room;
	if(%room $= "")
	{
		if(!$freeCount)
		{
			messageClient(%client, '', '\c5All rooms are occupied - you will have to live with someone else (or be a bum)');
			%roomSpawn = BrickGroup_888888.NTObject["_bumSpawn", getRandom(0, BrickGroup_888888.NTObjectCount["_bumSpawn"] - 1)];
		}
		else
		{
			%freeIndex = getRandom(1, $freeCount);
			%room = $freeRoom[%freeIndex];
			$roomOwner[%room] = %character;
			$freeRoom[%freeIndex] = $freeRoom[$freeCount];
			$freeCount--;

			%roomDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_door", 0];
			%roomSpawn = BrickGroup_888888.NTObject["_r" @ %room @ "_spawn", 0];
			%roomCloset = BrickGroup_888888.NTObject["_r" @ %room @ "_closet", 0];
		}
	}
	else
	{
		%roomDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_door", 0];
		%roomSpawn = BrickGroup_888888.NTObject["_r" @ %room @ "_spawn", 0];
		%roomCloset = BrickGroup_888888.NTObject["_r" @ %room @ "_closet", 0];
	}

	RS_Log("[NewRound]" SPC %client.getPlayerName() SPC "(" @ %client.getBLID() @ ")'s character is '" @ %character.name @ "'", "\c1");

	%character.room = %room;
	//Assign character to client
	%client.killer = false;
	%client.spawnPlayer();
	%player = %client.player;
	%player.character = %character; //post-death reference to character

	if(%character.trait["Frail"])
	{
		%player.maxhealth = 90;
		%player.health = 90;
	}

	%character.player = %player;
	%player.setDatablock(PlayerDespairArmor);
	%player.room = %room;
	%player.setShapeNameDistance(0);
	%player.setShapeNameColor("1 1 1");
	%player.setTransform(%roomSpawn.getSpawnPoint());
	%client.updateAFKCheck();

	centerPrint(%client, "<bitmap:" @ $Despair::Path @ "res/logo><br><br><br><br><br><br><br><font:impact:30>\c6CHAPTER " @ ($defaultMiniGame.winRounds+1), 6);
	commandToClient(%client,'PlayGui_CreateToolHud',PlayerDespairArmor.maxTools);
	commandToClient(%client, 'SetVignette', true, $EnvGuiServer::VignetteMultiply SPC $EnvGuiServer::VignetteColor);

	//Hat icon for hats
	%data = noHatIcon.getID();
	%player.hatSlot = %player.getDataBlock().maxTools - 1;
	%player.tool[%player.hatSlot] = %data;
	messageClient(%client, 'MsgItemPickup', '', %player.hatSlot, %data, true);

	//Weapon icon for weapons
	%data = noWeaponIcon.getID();
	%player.weaponSlot = 0;
	%player.tool[%player.weaponSlot] = %data;
	messageClient(%client, 'MsgItemPickup', '', %player.weaponSlot, %data, true);

	if(%room !$= "")
	{
		%props = KeyItem.newItemProps(%player, 0);
		%props.name = "Room #" @ $roomNum[%room] @ " Key";
		%props.id = %roomDoor.lockId;

		%player.addTool(KeyItem, %props);

		%roomDoor.doorMaxHits = 4;
		%roomDoor.lockpickDifficulty = 5;
		%roomDoor.setColor(57); //Bring back color to used doors
	}

	if(%character.trait["Hatter"])
	{
		%choices = "HatBlindItem HatCatItem HatChefItem HatCowboyItem HatDuckItem HatFancyItem HatFedoraItem HatFoxItem HatGangsterItem HatMountyItem HatPartyhatItem HatRHoodItem HatStrawItem HatSunglassesItem HatTophatItem HatWizardItem";
		%pick = getWord(%choices, getRandom(0, getWordCount(%choices)-1));
		if(isObject(%roomCloset))
			%roomCloset.spawnItem("0 0 1", %pick);
		else
			%pick.onPickup("", %player);
	}

	if(%character.trait["Gang Member"])
	{
		if(isObject(%roomCloset))
			%roomCloset.spawnItem("0 0 1", "LockpickItem");
		else
			%player.addTool(LockpickItem);
		HatGangsterRedItem.onPickup("", %player);
	}
	if(%character.trait["Repairman"])
	{
		if(isObject(%roomCloset))
			%roomCloset.spawnItem("0 0 1", "RepairkitItem");
		else
			%player.addTool(RepairkitItem);
	}

	if(%character.trait["Chekhov's Gunman"])
	{
		%revolverSlot = %player.setTool(%player.weaponSlot, "RevolverItem", $gunmanProps);
		if (!isObject($gunmanProps) || $gunmanProps.character != %character)
		{
			if(isObject($gunmanProps))
				$gunmanProps.delete();

			$gunmanProps = %player.getItemProps(%revolverSlot);
			$gunmanProps.character = %character;
			$gunmanProps.noDeleteAlways = true;
		}
	}

	if(%survivalPerk)
	{
		messageClient(%client, '', '\c5Since you survived last round, you will be \c6%1\c5 once more and receive a survival bonus!', %character.name);
		%type = getRandom(1,3);
		switch(%type)
		{
			case 1:
				for(%i = 0; %i < getFieldCount(%character.traitList); %i++)
				{
					%trait = getField(%character.traitList, %i);
					%done = false;
					for(%j = 0; %j < getFieldCount($Despair::Traits::Negative); %j++)
					{
						if(%trait $= getField($Despair::Traits::Negative, %j)) //negative trait
						{
							%character.traitList = removeField(%character.traitList, %i);
							%character.trait[%trait] = false;
							messageClient(%client, '', '\c5Your \c0%1\c5 trait is now gone!', %trait);
							%done = true;
							break;
						}
						if(%done)
							break;
					}
					if(%done)
						break;
				}
			case 2:
				if(isObject(%roomCloset))
				{
					messageClient(%client, '', '\c5Your closet now contains a \c3useful item!');
					%choices = "LockpickItem FlashlightItem RadioItem CleanSprayItem BananaItem";
					%pick = getWord(%choices, %index = getRandom(0, getWordCount(%choices)-1));
					%roomCloset.spawnItem("0 0 1", %pick);
				}
			case 3:
				if(isObject(%roomCloset))
				{
					messageClient(%client, '', '\c5Your closet now contains a \c3weapon!');
					%choices = "KnifeItem BatItem UmbrellaItem";
					%pick = getWord(%choices, %index = getRandom(0, getWordCount(%choices)-1));
					%roomCloset.spawnItem("0 0 1", %pick);
				}
		}
		if($gunmanChar == %character && isObject($gunmanProps))
		{
			$gunmanProps.ammo = getMin($gunmanProps.ammo + 1, $gunmanProps.maxAmmo);
			messageClient(%client, '', '\c5You got an \c3additional bullet\c5 for your gun!');
		}
	}
	return %player;
}

function roomPlayers()
{
	$freeCount = 0;
	for (%i = 0; %i < $Despair::RoomCount; %i++)
	{
		%room = %i + 1;
		for(%a = 0; %a < BrickGroup_888888.NTObjectCount["_r" @ %room @ "_door"]; %a++)
		{
			%roomDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_door", %a];
			%roomDoor.lockId = "R"@%room;
			%roomDoor.lockState = true;
			%roomDoor.doorMaxHits = "";
			%roomDoor.lockpickDifficulty = "";
			%roomDoor.setColor(50); //Gray out unused doors
		}
		%roomBathDoor = BrickGroup_888888.NTObject["_r" @ %room @ "_bathdoor", 0];
		%roomSpawn = BrickGroup_888888.NTObject["_r" @ %room @ "_spawn", 0];
		%roomCloset = BrickGroup_888888.NTObject["_r" @ %room @ "_closet", 0];
		%roomCloset.setItem("");
		if(isObject($roomOwner[%room])) //$roomOwner[%room] is a character. If that character is deleted, rip.
		{
			continue;
		}

		$freeRoom[$freeCount++] = %room;
		//talk("Room " @ $roomNum[%room] @ " was free!");
	}

	%count = $DefaultMiniGame.numMembers;
	for (%i = 0; %i < %count; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%client.killerHelper = false;

		if (%client.player)
			%client.player.delete();

		%player = createPlayer(%client);

		if(isObject(%player))
		{
			// %ln = getWord(%player.character.name, 1);
			// if(%sibling[%ln] !$= "")
			// {
			// 	messageClient(%client, '', '\c5You have a \c6sibling\c5 this round! Their name is %1 and they live in room %2.', %sibling[%ln].character.name, $roomNum[%sibling[%ln].character.room]);
			// 	messageClient(%sibling[%ln].client, '', '\c5You have a \c6sibling\c5 this round! Their name is %1 and they live in room %2.', %player.character.name, $roomNum[%player.character.room]);
			// }
			// %sibling[%ln] = %player;
			%client.playPath(IntroPath);
			%client.schedule(6000, setControlObject, %player);
			%client.camera.schedule(6000, setControlObject, %client.camera);
		}
		else
		{
			%client.camera.setMode("Observer");
			%client.setControlObject(%client.camera);
			%client.camera.setControlObject(%client.camera);
		}
	}
}

function despairEndGame()
{
	if (isEventPending($DefaultMiniGame.restartSchedule))
		return;
	cancel($musicSchedule);
	cancel($DefaultMiniGame.subEventSchedule);
	cancel($DefaultMiniGame.restartSchedule);
	cancel($DefaultMiniGame.eventSchedule);
	if($DefaultMiniGame.numMembers <= 1)
	{
		cancel(DayCycle.timeSchedule);
		return;
	}

	$DefaultMiniGame.chatMessageAll('', '\c5What actually happened:');
	for(%i = 0; %i <= $EndLogCount; %i++)
	{
		$DefaultMiniGame.chatMessageAll('', $EndLog[%i]);
	}

	$DefaultMiniGame.chatMessageAll('', '\c6%1\c5 was the killer!', $pickedKiller.character.name);
	UpdatePeopleScore();
	//$pickedKiller.character.isDead = true;
	$DefaultMiniGame.restartSchedule = $DefaultMiniGame.schedule(20000, reset, 0);
}

function despairResetShutters()
{
	//Reset shutters
	$shutterCount = 0;
	while(isObject(BrickGroup_888888.NTObject["_shutter" @ $shutterCount + 1, 0]))
	{
		$shutterCount++;
		for(%i = 0; %i < BrickGroup_888888.NTObjectCount["_shutter" @ $shutterCount]; %i++)
		{
			%shutter = BrickGroup_888888.NTObject["_shutter" @ $shutterCount, %i];
			if(strpos($shuttersOpen, $shutterCount) == -1)
			{
				%shutter.disappear(0);
			}
			else
			{
				%shutter.disappear(-1);
			}
		}
	}

	if(getWordCount($shuttersOpen) > 0)
    {
        talk("There are now " @ getWordCount($shuttersOpen) @  " open shutters!");
		for(%i=1; %i <= getWordCount($shuttersOpen); %i++)
            talk("\c3" @ $shutterNum[getWord($shuttersOpen, %i-1)] @ "\c6 is open for this round!");
	}
	else
        talk("There are no open shutters.");
}

function despairPrepareGame()
{
	cancel($DefaultMiniGame.restartSchedule);
	GameRoundCleanup.deleteAll();
	clearDecals();
	if (!$DefaultMiniGame.permaDeath || $DefaultMiniGame.winRounds <= 0)
		ClearFlaggedCharacters();
	// Close *all* doors
	%count = BrickGroup_888888.getCount();

	for (%i = 0; %i < %count; %i++)
	{
		%brick = BrickGroup_888888.getObject(%i);

		%data = %brick.getDataBlock();
		%name = %brick.getName();
		if (%data.isDoor)
		{
			%brick.lockState = false;
			%brick.doorHits = 0;
			%brick.broken = false;
			%brick.setDataBlock(%brick.isCCW ? %data.closedCCW : %data.closedCW);
			%brick.onDoorClose();
			%brick.doorMaxHits = 4;
            %brick.lockpickDifficulty = 5; //6 seconds
		}
		//Consistent item spawns
		if(isObject(%brick.itemData))
			%brick.setItem(%brick.itemData);
		if(isObject(%brick.item))
			%brick.itemData = %brick.item.getDataBlock();
	}

	//Hats!
	%choices[1] = "HatBlindItem HatCatItem HatChefItem HatCowboyItem HatDuckItem HatFancyItem HatFedoraItem HatFoxItem HatGangsterItem HatMountyItem HatPartyhatItem HatRHoodItem HatStrawItem HatSunglassesItem HatTophatItem HatWizardItem";
	%choices[2] = "HatClownItem HatDisguiseItem HatDogeItem HatRichardItem HatSkimaskItem HatGasmaskItem HatMummyItem HatNinjaItem"; //Items that disguise you
	for (%i = 0; %i < BrickGroup_888888.NTObjectCount["_hatSpawn"]; %i++)
	{
		%brick = BrickGroup_888888.NTObject["_hatSpawn", %i];
		%type = 1;
		if(getWordCount(%choices[2]) && getRandom() < 0.15) //less chance for it to be disguise
			%type = 2;
		%pick = getWord(%choices[%type], %index = getRandom(0, getWordCount(%choices[%type])-1));
		if(isObject(%pick))
			%brick.setItem(%pick);
		%choices[%type] = removeWord(%choices[%type], %index); //Only one of a kind
	}

	//Random items!
	%choices = "RazorItem RepairkitItem RepairkitItem PenItem PenItem FlashlightItem FlashlightItem RadioItem RadioItem RadioItem";
	for (%i = 0; %i < BrickGroup_888888.NTObjectCount["_randomItem"]; %i++)
	{
		%brick = BrickGroup_888888.NTObject["_randomItem", %i];
		%pick = getWord(%choices, %index = getRandom(0, getWordCount(%choices)-1));
		if(isObject(%pick))
			%brick.setItem(%pick);
		%choices = removeWord(%choices, %index); //Only one of a kind
	}

	//Reset papers
	for (%i = 0; %i < BrickGroup_888888.NTObjectCount["_evidence"]; %i++)
	{
		%brick = BrickGroup_888888.NTObject["_evidence", %i];
		%brick.setItem("");
	}

	//Reset Foods
	for (%i = 0; %i < BrickGroup_888888.NTObjectCount["_food"]; %i++)
	{
		%brick = BrickGroup_888888.NTObject["_food", %i];
		%brick.setItem("");
	}

	//Reset Guest list
	%brick = BrickGroup_888888.NTObject["_guestlist", 0];
	%brick.setItem("");

	for (%i = 0; %i < 24; %i++)
	{
		$stand[%i].setTransform("0 0 -300");
		$memorial[%i].setTransform("0 0 -300");
	}

	despairResetShutters();

	$courtVoid.setTransform("0 0 -300");
	$courtvoid.setScale("1 1 1");

	$chatDelay = $Despair::chatDelay;

	$DespairTrial = "";
	$DespairTrialVote = false;
	$announcements = 0;
	$investigationStart = "";
	$pickedKiller = "";
	$lastVictim = "";
	$days = 0;
	$deathCount = 0;
	$maxDeaths = 99;//getMax(1, $aliveCount);
	$forceVoteCount = 0;

	//EoR report reset
	$EndLogCount = -1;

	if($EnvGuiServer::DayCycleEnabled <= 0)
	{
		$EnvGuiServer::DayCycleFile = "Add-Ons/DayCycle_DespairFever/fever.daycycle";
		loadDayCycle($EnvGuiServer::DayCycleFile);
		$EnvGuiServer::DayCycleEnabled = 1;
		DayCycle.setEnabled($EnvGuiServer::DayCycleEnabled);
	}
	$EnvGuiServer::DayLength = $Despair::DayLength;
	DayCycle.setDayLength($EnvGuiServer::DayLength);
	setDayCycleTime(0.4); //Starts at evening
	if(!isEventPending(DayCycle.timeSchedule))
		DayCycle.timeSchedule();

	//update sunflare size
	SunLight.flareSize = $EnvGuiServer::SunFlareSize;
	SunLight.sendUpdate();

	//update fog
	$EnvGuiServer::VisibleDistance = 130;
	Sky.visibleDistance = $EnvGuiServer::VisibleDistance;
	$EnvGuiServer::FogDistance = 100;
	Sky.fogDistance = $EnvGuiServer::FogDistance;
	Sky.sendUpdate();

    if ($defaultMiniGame.winRounds < 0)
		$defaultMiniGame.winRounds = 0;

	roomPlayers();

	DespairSetWeapons(1);
	serverStopSong();
	serverPlay2d("DespairMusicGameStart");
}

function DespairSetWeapons(%tog)
{
	$DefaultMiniGame.noWeapons = !%tog;
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%member = $DefaultMiniGame.member[%i];
		%player = %member.player;
		if (!isObject(%player))
			continue;
		if (%member.killer && !%tog)
			messageClient(%member, '', "<font:impact:24>\c5You will be unable to swing your weapons anymore.");
		//if (%player.getMountedImage(0) && %player.getMountedImage(0).item.className $= "DespairWeapon")
		//	%player.unMountImage(0);
	}
}

function DespairSetPermadeath(%tog)
{
	$DefaultMiniGame.permaDeath = %tog;
	if ($defaultMiniGame.permaDeath)
	{
		$DefaultMiniGame.chatMessageAll('', '\c0<font:impact:30>Permadeath mode has been enabled\c5!');
		$DefaultMiniGame.chatMessageAll('', '<font:impact:30>\c5From this point on, your death will mean you will only be able to spectate until innocents lose or there\'s only two people remaining!');
	}
	else
	{
		$DefaultMiniGame.chatMessageAll('', '<font:impact:30>\c5Permadeath mode has been disabled.');
	}
}

function fxDayCycle::timeSchedule(%this, %lastStage)
{
	cancel(%this.timeSchedule);

	if($DespairTrial)
		return;

	if(isEventPending($bottomPrintSchedule))
		cancel($bottomPrintSchedule);

	%time = getDayCycleTime();

	if(%time > 0.75)
		%stage = "NIGHT";
	else if(%time > 0.6666) //10 PM
		%stage = "LATE EVENING";
	else if(%time > 0.4)
		%stage = "EVENING";
	else if(%time > 0.25)
		%stage = "NOON";
	else
		%stage = "MORNING";

	if(%stage !$= %lastStage)
		despairCycleStage(%stage);

	if(isObject($DefaultMiniGame))
	{
		for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
		{
			%member = $DefaultMiniGame.member[%i];
			%member.updateBottomprint();
		}
	}

	%sched = getMax(50, (%this.DayLength * 60) / 86400); //insanely weird and complicated thingy to make schedule happen every time a "second" actually passes
	%this.timeSchedule = %this.schedule(%sched, timeSchedule, %stage);
}

function despairCycleStage(%stage)
{
	talk("It is now \c3" @ %stage);
	if(%stage $= "NIGHT")
	{
		$days++;
		despairOnNight();
	}

	if(%stage $= "EVENING")
	{
		despairOnEvening();
	}

	if(%stage $= "LATE EVENING")
	{
		despairOnLateEvening();
	}

	if(%stage $= "NOON")
	{
		despairOnNoon();
	}

	if(%stage $= "MORNING")
	{
		%high = -1;
		%choice[%high++] = "Did you know we have a <a:discord.gg/9gDaarJ>Discord</a>\c6 channel? Join the discussion!";
		%choice[%high++] = "\c3/help\c6 contains a lot of useful gameplay information and tips. Check it out!";
		%choice[%high++] = "Did you know you can create a custom character with /cc command";
		%choice[%high++] = "Did you know we have a <a:discord.gg/9gDaarJ>Discord</a>\c6 channel? Join the discussion!";
		%choice[%high++] = "Murder weapon and blood contain a lot more information that you may initially think. Don't write them off!";
		%choice[%high++] = "Once investigation period starts, weapons are disabled, but non-standard murders may still happen!";
		%choice[%high++] = "You can spamclick people to shove them!";	
		%choice[%high++] = "\c3Investigatives\c6 are incredibly helpful! They can tell time of death, number and type of cuts, choking victims and all the like!";
		%choice[%high++] = "If you survive and win as an innocent, you will keep your character AND your room number!";
		%choice[%high++] = "Cleaning up the crime scene is impossible once investigation starts.";
		%choice[%high++] = "You can loot bodies by pressing \c3Light Key\c6 and clicking an item! Plant stuff using \c3Ctrl+W\c6!";
		%choice[%high++] = "\c3Hold-click\c6 and \c3move your mouse\c6 to start carrying a body! Only killers can carry corpses, though.";
		%choice[%high++] = "At least two people must scream or examine a body to start the investigation.";
		%choice[%high++] = "You can choke people by carrying a body and \c3Holding Rightclick\c6! It will take 6 seconds, however.";
		%choice[%high++] = "Instead of focusing on a single person the entire time, you should check as many people as possible for being the killer.";
		%choice[%high++] = "You have no idea how important alibis are! If you pay attention and remember who went where, you might figure something out!";
		%choice[%high++] = "Fibers can be dropped when you swing your weapon, you get hit or you interact with a body! They also drop if you sleep.";
		%choice[%high++] = "Fibers take on the color of your hair, your clothes and your shoes. Coats and hair-hiding masks obscure fibers.";
		%choice[%high++] = "You can fake dying messages if you get your hands bloody and /write!";
		%choice[%high++] = "Discussing alibis during investigation is a TERRIBLE idea! Use the time you are given to find \c3physical evidence\6!";
		%choice[%high++] = "Alibi = a person's movements and actions druing a specified period, usually the approximate Time of Death.";
		%choice[%high++] = "Did you know Killer can fake speed with /fakespeed tired or exhausted command.";
		%choice[%high++] = "We also have a <a:https://despair-fever.fandom.com/wiki/Despair_Fever_Wiki>Wiki</a>\c6!";

		$DefaultMiniGame.chatMessageAll('', '\c5~~[Day \c3%1\c5]\c6 Good morning, everyone! %2', $days, %choice[getRandom(%high)]);
		despairOnMorning();
	}
	$currTimeStage = %stage;
}

function GameConnection::updateAFKCheck(%this, %previous)
{
	cancel(%this.updateAFKCheck);

	if (!isObject(%player = %this.player))
		return;

	if($despairTrial)
		return;

	%transform = %player.getTransform();

	if (!%player.unconscious && %transform $= %previous && $Sim::Time - %this.lastSpeakTime >= 60)
	{
		%delay = 1000;
		if (!%this.afk)
		{
			%this.afk = true;
			RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") is afk!", "\c2");
			messageClient(%this, '', '\c2<font:Impact:20>Warning\c6: You are considered AFK. If you don\'t come back until trial you will be considered dead.');
			for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%member = ClientGroup.getObject(%i);
				if(%member.isAdmin)
				{
					messageClient(%member, '', '\c2--[\c5%1 is afk.', %this.getPlayerName());
				}
			}
		}
	}
	else
	{
		%delay = 60000;
		if(%this.afk)
		{
			%this.afk = false;
			RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") is no longer afk!", "\c2");
			messageClient(%this.client, '', '\c2<font:Impact:20>Notice\c6: You are no longer considered AFK.');
			for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%member = ClientGroup.getObject(%i);
				if(%member.isAdmin)
				{
					messageClient(%member, '', '\c2--[\c5%1 is no longer afk.', %this.getPlayerName());
				}
			}
		}
	}

	%this.updateAFKCheck = %this.schedule(%delay, "updateAFKCheck", %transform);
}

package DespairFever
{
	function Player::removeBody(%player)
	{
		%player.delete();
	}

	function MiniGameSO::addMember($DefaultMiniGame, %client)
	{
		Parent::addMember($DefaultMiniGame, %client);

		if (!$DefaultMiniGame.owner && $DefaultMiniGame.numMembers == 2)
			despairPrepareGame();

		messageClient(%client, '', '\c5--> \c4Please read \c3/rules\c4 and \c3/help\c4!', %this.getPlayerName());
		commandToClient(%client, 'messageBoxOK', "Welcome!", "Please read /rules and /help!");
		if(!$pickedKiller && (!$defaultMiniGame.permaDeath || $defaultMiniGame.winRounds <= 0))
			createPlayer(%client);
		else
		{
			if ($defaultMiniGame.permaDeath)
				messageClient(%client, '', '\c5You didn\'t spawn because permadeath mode is enabled - you can only spawn in the first round.');
			else if ($pickedKiller)
				messageClient(%client, '', '\c5You didn\'t spawn because the round is already in progress.');
		}
	}

	function MiniGameSO::removeMember($DefaultMiniGame, %client)
	{
		if(isObject(%pl = %client.player))
		{
			if(%pl.health <= 0)
			{
				%pl.health = $Despair::CritThreshold;
				%pl.critLoop();
			}
			else
			{
				for(%i=0;%i<%pl.getDataBlock().maxTools;%i++)
				{
					%pl.dropTool(%i);
				}
			}
		}
		if(isObject(%client.character))
		{
			%client.character.isDead = true;
		}
		if (isObject(%client))
			%client.dfSaveData();
		if(!%client.isAdmin && $pickedKiller == %client && $deathCount <= 0)
			DespairPickKiller(true);
		Parent::removeMember($DefaultMiniGame, %client);
	}

	function MiniGameSO::Reset(%this, %client)
	{
		if (%this.owner != 0)
			return Parent::reset(%this, %client);

		// Play nice with the default rate limiting.
		if (getSimTime() - %this.lastResetTime < 5000)
			return;

		cancel($musicSchedule);
		cancel($DefaultMiniGame.subEventSchedule);
		cancel($DefaultMiniGame.restartSchedule);
		cancel($DefaultMiniGame.eventSchedule);
		if($DefaultMiniGame.numMembers < 1)
			cancel(DayCycle.timeSchedule);

		Parent::reset(%this, %client);

		for(%a = 0; %a < ClientGroup.getCount(); %a++)
		{
			%client = ClientGroup.getObject(%a);
			if(%client.isAdmin)
				%admins++;
		}
		if(!%admins && $Pref::Server::Password $= "a" && $DefaultMiniGame.winRounds <= 0) //despair
		{
			for(%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%subClient = ClientGroup.getObject(%i);
				%subClient.schedule(0, "delete", "The server has been closed.");
			}
			return;
		}

		if($DefaultMiniGame.numMembers >= 1)
			despairPrepareGame();
	}

	function MiniGameSO::checkLastManStanding($DefaultMiniGame)
	{
		if ($DefaultMiniGame.owner)
			return Parent::checkLastManStanding($DefaultMiniGame);
		if (isEventPending($DefaultMiniGame.restartSchedule))
			return;
		if($despairTrial && $DespairTrialVote)
			return;

		$aliveCount = 0;
		%killerAlive = 0;
		%otherAlive = 0;

		for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
		{
			%client = $DefaultMiniGame.member[%i];
			%player = %client.player;

			if (!%player)
				continue;

			if (%client.killer)
				%killerAlive = 1;
			else
				%otherAlive = 1;

			$aliveCount++;
			%last = %client;
		}

		//$maxDeaths = 99;//getMax(1, $aliveCount - 4);

		//if($deathCount >= $maxDeaths)
		//	DespairSetWeapons(0);

		if(!%otherAlive)
		{
			talk("Everybody is dead dave");
			$defaultMiniGame.winRounds = 0; //Reset the winrounds counter for permadeath
			despairEndGame();
		}
		else if(!%killerAlive && $pickedKiller)
		{
			talk("Killer is dead, rip");
			despairEndGame();
		}
		else if($aliveCount == 1)
		{
			talk("I AM THE ONE AND ONLY");
			$defaultMiniGame.winRounds = 0; //Reset the winrounds counter for permadeath
			despairEndGame();
		}
		else if(%killerAlive && $aliveCount == 2)
		{
			if(!$FinalBoss)
			{
				$DefaultMiniGame.chatMessageAll('', 'ONLY TWO PEOPLE REMAIN\c6. If the killer doesn\'t kill the last person alive, he will die! \c3You have 5 minutes.');
				$FinalBoss = true;
				cancel($DefaultMiniGame.eventSchedule);
				$DefaultMiniGame.eventSchedule = schedule(300*1000, $pickedKiller.player, "kill");
				ServerPlaySong(pickField("DespairMusicIntense" TAB "DespairMusicWonderfulIntro"));
				DespairSetWeapons(1);
			}
		}
		else
 			$FinalBoss = false;

		return 0;
	}

	function MiniGameSO::pickSpawnPoint($DefaultMiniGame, %client)
	{
		if ($DefaultMiniGame.owner || $DefaultMiniGame.numMembers < 2)
			return Parent::pickSpawnPoint($DefaultMiniGame, %client);
	}

	function serverCmdLight(%client)
	{
		if (%client.miniGame != $DefaultMiniGame)
			Parent::serverCmdLight(%client);

		if(isObject(%client.player))
			%client.player.onLight();
	}

	function serverCmdSuicide(%client)
	{
		if (%client.miniGame != $DefaultMiniGame)
			return Parent::serverCmdSuicide(%client);

		if(isObject(%player = %client.player) && %player.health <= 0)
		{
			%player.health = $Despair::CritThreshold;
			%player.critLoop();
			messageClient(%client, '', "\c5You succumb to your wounds...");
		}
	}

	function Item::schedulePop(%this)
	{
		GameRoundCleanup.add(%this);
	}

	function ItemData::onAdd(%this, %obj)
	{
		Parent::onAdd(%this, %obj);
		if (%this.canPickUp !$= "")
			%obj.canPickUp = %this.canPickUp;
	}

	function GameConnection::onClientEnterGame(%this)
	{
		Parent::onClientEnterGame(%this);

		if(%this.isAdmin && !%this.hasRPA)
		{
			messageClient(%this, '', '\c4Uh-oh, you\'re missing \c6Client_RoleplayAdmin \c4and will not be able to see logs in your console.');
			messageClient(%this, '', '<a:www.dropbox.com/s/zplta1zrwrft3ru/Client_RoleplayAdmin.zip?dl=1>DOWNLOAD THE ADMIN CLIENT HERE</a>');
		}
		else if(%this.isAdmin && %this.RPAVersion < $RSAdmin::Version)
		{
			messageClient(%this, '', '\c4Uh-oh, your \c6Client_RoleplayAdmin \c4is out of date.');
			messageClient(%this, '', '<a:www.dropbox.com/s/zplta1zrwrft3ru/Client_RoleplayAdmin.zip?dl=1>DOWNLOAD THE ADMIN CLIENT HERE</a>');
		}

		%this.schedule(16, "dfLoadData");
	}

	function GameConnection::SetScore(%this, %num)
	{
		%num = %this.points;
		parent::SetScore(%this, %num);
	}
};
activatePackage("DespairFever");
