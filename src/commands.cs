//GENERAL
function serverCmdKeepChar(%this)
{
	%this.noPersistance = !%this.noPersistance;
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /keepchar '" @ (!%this.noPersistance ? "yes" : "no") @ "'", "\c2");
	messageClient(%this, '', '\c5You will \c6%1\c5 keep your character between rounds if you survive.', !%this.noPersistance ? "now" : "no longer");
}
function serverCmdTraits(%this)
{
	if(!isObject(%character = %this.character))
		return;
	messageClient(%this, '', '\c5Your traits are\c6:');
	for(%i = 0; %i < getFieldCount(%character.traitList); %i++)
	{
		%trait = getField(%character.traitList, %i);
		%desc = $Despair::Traits::Description[%trait];
		messageClient(%this, '', ' \c5%1 - \c6%2', %trait, %desc);
	}
}

function serverCmdMood(%this)
{
	if(!isObject(%obj = %this.player))
		return;
	messageClient(%this, '', '\c5You are %1%2\c5.', getMoodColor(getMoodName(%obj.mood)), getMoodName(%obj.mood));
}

function serverCmdCustomChar(%this, %do)
{
	if (!isObject(%this.character) || !isObject(%this.player) || $days > 0)
	{
		messageClient(%this, '', '\c5You must be alive and use this command before roles have been decided.');
		return;
	}
    if ($DefaultMiniGame.permaDeath && $DefaultMiniGame.winRounds > 0)
	{
		messageClient(%this, '', '\c5You cannot change your character during permadeath mode.');
		return;
	}
	if (%do)
	{
		if (!%this.SpendPoints(15))
		{
			messageClient(%this, '', '\c5Not enough points.');
			return;
		}
        enterCharacterCreation(%this);
		return;
	}
	%message = "\c2Are you sure you want to customize your character?\nIt costs 15 points!\nYou will need to finish your character before night hits!\nYou can also use /customname <firstname> <lastname> which costs 10 points.";
	commandToClient(%this, 'messageBoxYesNo', "CustomChar", %message, 'CustomCharAccept');
}
function serverCmdCustomCharAccept(%this)
{
    serverCmdCustomChar(%this, true);
}

function serverCmdCC(%this) //shorthand for "customchar"
{
	serverCmdCustomChar(%this);
}

function serverCmdCustomName(%this, %firstname, %lastname)
{
	if (!isObject(%this.character) || !isObject(%this.player) || $days > 0)
	{
		messageClient(%this, '', '\c5You must be alive and use this command before roles have been decided.');
		return;
	}
    if ($DefaultMiniGame.permaDeath && $DefaultMiniGame.winRounds > 0)
	{
		messageClient(%this, '', '\c5You cannot change your character during permadeath mode.');
		return;
	}
	if (%firstname $= "" || %lastname $= "")
	{
		messageClient(%this, '', '\c5Invalid argument! Correct usage: /customname <firstname> <lastname>.');
		return;
	}
	if (strlen(%firstname) < 2 || strlen(%lastname) < 2)
	{
		messageClient(%this, '', '\c5You must use more than a single letter for your name!');
		return;
	}
	if (!%this.SpendPoints(10))
	{
		messageClient(%this, '', '\c5Not enough points! You need \c310 points\c5 to use this command.');
		return;
	}
	//Make 'em correct
	%firstname = strupr(getSubStr(%firstname, 0, 1)) @ strlwr(getSubStr(%firstname, 1, strlen(%lastname)));
	%lastname = strupr(getSubStr(%lastname, 0, 1)) @ strlwr(getSubStr(%lastname, 1, strlen(%lastname)));
	%this.character.name = %firstname SPC %lastname;
	messageClient(%this, '', '\c5You spent \c310 points\c5 and set your new name to %1.', %this.character.name);
}

function serverCmdStats(%this, %target)
{
	if(%target $= "" || !%this.isAdmin)
		%target = %this;
	else
		%target = findclientbyname(%target);
	if(!isObject(%target))
	{
		messageClient(%this, '', '\c5Invalid target!');
		return;
	}
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /stats '" @ %target.getPlayerName() SPC "(" @ %target.getBLID() @ ")'", "\c2");
	messageClient(%this, '', '\c5Here are the stats for %1.', %target.getPlayerName());
	
	messageClient(%this, '', '\c2++\c5Points\c6: %1', %target.points);
	messageClient(%this, '', '\c2++\c5Points Total\c6: %1', %target.pointstotal);
	messageClient(%this, '', '\c2++\c5Killer Wins\c6: %1', %target.killerWins);
	messageClient(%this, '', '\c2++\c5Murders\c6: %1', %target.murders);
	messageClient(%this, '', '\c2++\c5Innocent Wins\c6: %1', %target.innocentWins);
	messageClient(%this, '', '\c2++\c5Correct Votes\c6: %1', %target.correctVotes);
	messageClient(%this, '', '\c2++\c5Deaths\c6: %1', %target.deaths);
}

function serverCmdFakeSpeed(%this, %thing)
{
	if(!isObject(%player = %this.player))
		return;
	if(!%this.killer)
		return;
	switch$(%thing)
	{
		case "tired":
			%player.setSpeedScale(0.9);
			messageClient(%this, '', '\c5You will now have the same walkspeed as \c6Tired\c5. \c3Dash\c5 to cancel.');
		case "exhausted":
			%player.setSpeedScale(0.6);
			messageClient(%this, '', '\c5You will now have the same walkspeed as \c6Exhausted\c5. \c3Dash\c5 to cancel.');
		case "default":
			%player.updateSpeedScale(1);
			messageClient(%this, '', '\c5Your walkspeed is now normal.');
		default:
			messageClient(%this, '', '\c5Usage: \c3/fakeSpeed tired, exhausted \c5OR\c3 default\c5 for normal speed.');
	}
}

function serverCmdForceVote(%client)
{
	if ($DespairTrial && ($DespairTrialOpening || $DespairTrialVote))
	{
		messageClient(%client, '', '\c5You can only force vote at discussion phase!');
		return;
	}

	if (!$DespairTrial && !$investigationStart)
	{
		messageClient(%client, '', '\c5You can only force vote during investigation or trial!');
		return;
	}

	if (%client.miniGame == $defaultMiniGame)
	{
		if (!%client.isAdmin && (!isObject(%client.player) || !isObject(%client.character)))
			return;
		if ($DespairTrial)
			%currTime = $Sim::Time - $DespairTrialDiscussion;
		else if($investigationStart)
			%currTime = $Sim::Time - ($investigationStart - $investigationLength);
		if (!%client.isAdmin && %currTime < ($DespairTrial ? $Despair::CanForceVote : $Despair::CanForceTrial))
		{
			messageClient(%client, '', '\c5You can only vote %1 minutes after %2 had started!', mCeil(($DespairTrial ? $Despair::CanForceVote : $Despair::CanForceTrial) / 60), $DespairTrial ? "trial" : "investigation");
			return;
		}
		for (%i = 0; %i < $defaultMiniGame.numMembers; %i++)
		{
			%member = $defaultMiniGame.member[%i];
			%player = %member.player;
			if (!isObject(%player))
				continue;
			%alivePlayers++;
		}
		for (%i = 1; %i <= $forceVoteCount; %i++)
		{
			%member = $forceVotes[%i];

			if (!isObject(%member) || !isObject(%member.character))
				continue;
			if (%member == %client)
			{
				messageClient(%client, '', '\c5You already voted!');
				return;
			}
			%validVotes++;
		}
		$forceVotes[$forceVoteCount++] = %client;
		%validVotes++;
		if (%validVotes >= (MFloor(%alivePlayers * 0.8))) // if at least 90% of alive players voted
		{
			$defaultMiniGame.messageAll('', '\c3%1 has voted to start the %2 early!\c6 There are enough votes to force the %2.',
				getCharacterName(%client.character, 1), $DespairTrial ? "vote" : "trial");
			%start = true;
		}
		else
		{
			$defaultMiniGame.messageAll('', '\c3%1 has voted to start the %2 early!\c6 Do /forcevote to concur. %3 votes left.',
				getCharacterName(%client.character, 1), $DespairTrial ? "vote" : "trial", MFloor(%alivePlayers * 0.8) - %validVotes);
		}
	}
	else if (%client.isAdmin) //"Admin" forcevote only works outside minigame
		%start = true;
	if (%start)
	{
		if ($DespairTrial)
			DespairStartVote();
		else
			CourtPlayers();
	}
}

function serverCmdSpectate(%this)
{
	if (%this.killerHelper)
	{
		%this.killerHelper = false;
		messageClient(%this, '', '\c5You are no longer eligible for helping the killer. Dead chat enabled.');
		return;
	}
	if ($despairTrial !$= "")
	{
		messageClient(%this, '', '\c5You cannot spectate - trial is in progress!');
		return;
	}
	if ($investigationStart !$= "")
	{
        messageClient(%this, '', '\c5You cannot spectate - the investigation is in progress!');
        return;
	}
	%this.spectating = !%this.spectating;
	messageClient(%this, '', '\c5You are \c6%1\c5 spectating.', %this.spectating ? "now" : "no longer");
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /keepchar '" @ (!%this.noPersistance ? "yes" : "no") @ "'", "\c2");
	if(isObject(%this.player) && %this.spectating)
	{
		%this.character.isDead = true;
		%this.camera.setMode("Observer");
		%this.setControlObject(%this.camera);
		%this.camera.setControlObject(%this.camera);
		%this.player.delete(); //Should be safe to do
	}
	else
	{
		if(!$pickedKiller && !$DefaultMiniGame.permaDeath)
			createPlayer(%this);
	}
}

//ADMIN ONLY
function serverCmdWhoIs(%client, %a, %b)
{
	if (!%client.isAdmin)
		return;

	%search = trim(%a SPC %b);
	RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") used /whois '" @ %search @ "'", "\c2");
	%charCount = GameCharacters.getCount();
	for (%i = 0; %i < %charCount; %i++)
	{
		%character = GameCharacters.getObject(%i);

		if (%search $= "" || striPos(%character.clientName, %search) != -1 || striPos(%character.name, %search) != -1)
		{
			messageClient(%client, '', '\c3%1 \c6is\c3 %2\c6, room \c3%3', %character.clientName, %character.name, $roomNum[%character.room]);
		}
	}
}

function serverCmdPermadeath(%this)
{
	if (!%this.isAdmin)
		return;
    DespairSetPermadeath(!$DefaultMiniGame.permaDeath);
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /permadeath '" @ ($DefaultMiniGame.permaDeath ? "yes" : "no") @ "'", "\c2");
}

function serverCmdKill(%this, %target)
{
	if(!%this.isAdmin)
		return;
	%target = findclientbyname(%target);
	if(!isObject(%target))
	{
		messageClient(%this, '', '\c5Invalid target!');
		return;
	}
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /kill '" @ %target.getPlayerName() SPC "(" @ %target.getBLID() @ ")'", "\c2");
	messageClient(%this, '', '\c5You have force-killed %1.', %target.getPlayerName());
	messageClient(%target, '', '\c5You have been force-killed.');
	if(isObject(%target.player))
	{
		for(%i=0;%i<%target.player.getDataBlock().maxTools;%i++)
		{
			%target.player.dropTool(%i);
		}
		%target.character.isDead = true;
		%target.camera.setMode("Observer");
		%target.setControlObject(%target.camera);
		%target.camera.setControlObject(%target.camera);
		%target.player.delete(); //Should be safe to do
		$defaultMiniGame.checkLastManStanding();
	}
}

function serverCmdSetName(%client, %target, %name)
{
	if(!%this.isAdmin)
		return;
	%target = findclientbyname(%target);
	if(!isObject(%target))
	{
		messageClient(%this, '', '\c5Invalid target!');
		return;
	}
	if(%name $= "")
	{
        messageClient(%this, '', '\c5Invalid argument - intended rename must not be empty!');
        return;
    }
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /setname '" @ %target.getPlayerName() SPC "(" @ %target.getBLID() @ ") '" @ %name @ "'", "\c2");
	messageClient(%this, '', '\c5You have changed %1\'s name to %2.', %target.getPlayerName(), %name);
	messageClient(%target, '', '\c5Your name has been changed to %1.', %name);
	%target.character.name = %name;
}

function serverCmdKillerBan(%this, %target)
{
	if(!%this.isAdmin)
		return;
	%target = findclientbyname(%target);
	if(!isObject(%target))
	{
		messageClient(%this, '', '\c5Invalid target!');
		return;
	}
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /killerban '" @ %target.getPlayerName() SPC "(" @ %target.getBLID() @ ")'", "\c2");
	messageClient(%this, '', '\c5You have killer-banned %1.', %target.getPlayerName());
	messageClient(%target, '', '\c5You have been banned from being a killer.');
	
	%target.killerbanned = true;
	%target.dfSaveData();
}

function serverCmdKillerUnban(%this, %target)
{
	if(!%this.isAdmin)
		return;
	%target = findclientbyname(%target);
	if(!isObject(%target))
	{
		messageClient(%this, '', '\c5Invalid target!');
		return;
	}
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /killerunban '" @ %target.getPlayerName() SPC "(" @ %target.getBLID() @ ")'", "\c2");
	messageClient(%this, '', '\c5You have killer-unbanned %1.', %target.getPlayerName());
	messageClient(%target, '', '\c5You have been unbanned from being a killer.');
	
	%target.killerbanned = false;
	%target.dfSaveData();
}

function serverCmdForceKiller(%this, %target)
{
	if(!%this.isSuperAdmin)
		return;
	%target = findclientbyname(%target);
	if(!isObject(%target))
	{
		messageClient(%this, '', '\c5Invalid target!');
		return;
	}
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /forcekiller '" @ %target.getPlayerName() SPC "(" @ %target.getBLID() @ ")'", "\c2");
	messageClient(%this, '', '\c5You have forced %1 to become the killer.', %target.getPlayerName());
	$forceKiller = %target;
}

function serverCmdShowRoles(%this)
{
	if(!%this.isAdmin)
		return;
	%this.showRoles = !%this.showRoles;
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /showroles '" @ (%this.showRoles ? "yes" : "no") @ "'", "\c2");
	messageClient(%this, '', '\c5You will \c6%1\c5 see the roles when spectating.', %this.showRoles ? "now" : "no longer");
	%this.updateBottomprint();
}

function serverCmdPM(%this, %target, %m1, %m2, %m3, %m4, %m5, %m6, %m7, %m8, %m9, %m10, %m11, %m12, %m13, %m14, %m15, %m16, %m17, %m18, %m19, %m20, %m20, %m22, %m23, %m24, %m25, %m26, %m27, %m28, %m29, %m30, %m31, %m32)
{
	if (!%this.isAdmin)
	{
		serverCmdReport(%this, %target, %m1, %m2, %m3, %m4, %m5, %m6, %m7, %m8, %m9, %m10, %m11, %m12, %m13, %m14, %m15, %m16, %m17, %m18, %m19, %m20, %m20, %m22, %m23, %m24, %m25, %m26, %m27, %m28, %m29, %m30, %m31, %m32);
		return;
	}
	%text = %m1;
	for (%i=2; %i<=32; %i++)
		%text = %text SPC %m[%i];
	%text = trim(stripMLControlChars(%text));
	if (%text $= "")
		return;
	if (isObject(%target = findClientByName(%target)))
	{
		RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /pm '" @ %target.getPlayerName() SPC "(" @ %target.getBLID() @ ")" SPC %text @ "'", "\c2");
		messageClient(%target, '', '\c4Admin PM from \c5%1\c6: %2 \c4(/pm to respond)',%this.getPlayerName(), %text);
		%target.play2d(DespairAdminBwoinkSound);
		%msg = "\c4PM from \c5"@ %this.getPlayerName() @"\c6 to \c3"@ %target.getPlayerName() @"\c6: "@%text;
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%other = ClientGroup.getObject(%i);
			if (%other.isAdmin)
			{
				messageClient(%other, '', %msg);
			}
		}
	}
	else
	{
		messageClient(%this, '', '\c5Player not found');
	}
}
function serverCmdReport(%this, %m1, %m2, %m3, %m4, %m5, %m6, %m7, %m8, %m9, %m10, %m11, %m12, %m13, %m14, %m15, %m16, %m17, %m18, %m19, %m20, %m20, %m22, %m23, %m24, %m25, %m26, %m27, %m28, %m29, %m30, %m31, %m32)
{
	if (getSimTime() - %this.lastReport <= 10000)
	{
		messageClient(%this, '', '\c0You have to wait \c3%1\c0 seconds until you can use this again.', mCeil((%this.lastReport - getSimTime()) / 1000 ) + 10);
		return;
	}
	%text = %m1;
	for (%i=2; %i<=32; %i++)
		%text = %text SPC %m[%i];
	%text = trim(stripMLControlChars(%text));
	if (%text $= "")
		return;

	messageClient(%this, '', '\c0Your report\c6: %1', %text);
	%this.lastReport = getSimTime();

	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /report '" @ %text @ "'", "\c2");
	%msg = "\c0REPORT from \c3"@%this.getPlayerName()@"\c6:" SPC %text;
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%other = ClientGroup.getObject(%i);
		if (%other.isAdmin)
		{
			messageClient(%other, '', %msg);
			%other.play2d(DespairAdminBwoinkSound);
		}
	}
}

function updateAdminCount()
{
	cancel($adminCountSchedule);
	if(!$Server::Dedicated)
		return;
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%client = ClientGroup.getObject(%a);
		if(%client.isAdmin)
			%admins++;
	}
	if($Pref::Server::Password $= "" && !%admins)
	{
		$Pref::Server::Name = strReplace($Pref::Server::Name, "!", "") SPC "!";
		$Pref::Server::Password = "";
	}
	else if($Pref::Server::Password $= "a" && %admins)
	{
		$Pref::Server::Password = "";
	}
	webcom_postServer();
}

function serverCmdAnnounce(%this, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13, %a14, %a15, %a16, %a17, %a18, %a19, %a20, %a20, %a22, %a23, %a24)
{
	if(!%this.isAdmin)
		return;

	%text = %a1;
	for (%i=2; %i<=24; %i++)
		%text = %text SPC %a[%i];
	%text = trim(stripMLControlChars(%text));
	if (%text $= "")
		return;
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /announce '" @ %text @ "'", "\c2");
	messageAll('MsgAdminForce', '<font:consolas:24><bitmap:base/client/ui/ci/star> \c5%1\c6: %2', %this.getPlayerName(), %text);
}

function serverCmdLockServer(%this)
{
	if(!%this.isAdmin)
		return;
	$Pref::Server::Password = "a";
	messageAll('MsgAdminForce', '<bitmap:base/client/ui/ci/star> \c3%1\c0 has \c3locked \c0the server.', %this.getPlayerName());
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /lockserver", "\c2");
}

function serverCmdUnLockServer(%this)
{
	if(!%this.isAdmin)
		return;
	$Pref::Server::Password = "";
	messageAll('MsgAdminForce', '<bitmap:base/client/ui/ci/star> \c3%1\c0 has \c3unlocked \c0the server.', %this.getPlayerName());
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /unlockserver", "\c2");
}

function serverCmdReset(%this, %do)
{
	if (!%this.isAdmin) return;
	if (%do)
	{
		$defaultMiniGame.reset(0);
		return;
	}
	%message = "\c2Are you sure you want to reset the minigame?";
	commandToClient(%this, 'messageBoxYesNo', "Reset", %message, 'resetAccept');
}
function serverCmdResetAccept(%this)
{
	serverCmdReset(%this, true);
}


function ServerCmdPlaySong(%this, %profile)
{
	if (!%this.isAdmin) return;
	serverPLaySong(%profile);
	messageAll('', '\c6%1\c2 has played song \c3%2\c2.', %this.getPlayerName(), %proifle);
}

function serverCmdForgive(%this, %target) 
 {
	if (!%this.isAdmin)
	{
        return; 
    }
	if ($despairTrial !$= "")
    {
        messageClient(%this, '', '\c5A trial is in progress!'); 
        return; 
    }
	if ($investigationStart !$= "")
	{
		messageClient(%this, '', '\c5investigation is in progress!');
		return;
	}
	%target = findclientbyname(%target); 
	if (!isObject(%target)) 
    {
		messageClient(%this, '', '\c5Invalid target!'); 
		return; 
    }
        
    RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") used /forgive '" @ %target.getPlayerName() SPC "(" @ %target.getBLID() @ ")'", "\c2"); 
    messageClient(%this, '', '\c5You have forgiven %1.', %target.getPlayerName()); 
    messageClient(%target, '', '\c5You were forgiven, and all RDM penalties have been removed.'); 
    if (isObject(%target.player)) 
    {
		%target.player.noWeapons = false; 
    }
 }
 
package DespairAdmins
{
	function GameConnection::onClientLeaveGame(%this)
	{
		if(%this.isAdmin)
			$adminCountSchedule = schedule(2000, 0, "updateAdminCount");
		Parent::onClientLeaveGame(%this);
	}

	function GameConnection::autoAdminCheck(%this)
	{
		%parent = Parent::autoAdminCheck(%this);
		if(%this.isAdmin)
			updateAdminCount();
		else if($Pref::Server::Password $= "a")
		{
			%this.schedule(0, "delete", "You are not an Admin.");
			return;
		}
		messageClient(%this, '', '\c4Hey %1, welcome to \c5Despair Fever\c4. Please read \c3/rules\c4 and \c3/help\c4!', %this.getPlayerName());
		messageClient(%this, '', '\c4Please download \c6Music and Sounds\c4 for the full experience!');

		// Admin client handshake.
		commandToClient(%this, 'RPA_Handshake');

		return %parent;
	}
};
activatePackage("DespairAdmins");
