//TODO: Replace $despairTrial and $investigationStart with $gameState and $gameStateStart instead
//TODO: /sign for writing
//TODO: Disable non-killer-induced death during investigation
//TODO: Breaking Doors alert, /report and admin /pm
//TODO: fix corpses eating your items
//TODO: fix servercmddroptool not checking for walls
//TODO: something very important i forgot :(

registerLoadingScreen("https://i.imgur.com/KLJ5q1v.png", "png", "FeverLoadScreen");
$Despair::Path = filePath(expandFileName("./description.txt")) @ "/";
function r(%p)
{
	//setModPaths(getModPaths());
	if (%p !$= "")
		exec("./" @ %p @ ".cs");
	else
		exec("./server.cs");
}

exec("./config.cs");

//lib
exec("./lib/daycycles.cs");
exec("./lib/decals.cs");
exec("./lib/hitregion.cs");
exec("./lib/itemfuncs.cs");
exec("./lib/itemprops.cs");
exec("./lib/los.cs");
exec("./lib/misc.cs");
exec("./lib/noObservers.cs");
exec("./lib/pathCamera.cs");
exec("./lib/scope.cs");
exec("./lib/timedfiring.cs");
exec("./lib/timedraycast.cs");
//src
exec("./src/savedata.cs");
exec("./src/sounds.cs"); //Important to be first - contains audio descriptions
exec("./src/logging.cs"); //Logging
exec("./src/blood.cs");
exec("./src/character.cs");
exec("./src/chat.cs");
exec("./src/commands.cs");
exec("./src/corpse.cs");
exec("./src/display.cs");
exec("./src/door.cs");
exec("./src/examine.cs");
exec("./src/fibers.cs");
exec("./src/footsteps.cs");
exec("./src/hats.cs");
exec("./src/health.cs");
exec("./src/help.cs");
exec("./src/minigame.cs");
exec("./src/mood.cs");
exec("./src/music.cs");
exec("./src/namelist.cs");
exec("./src/player.cs");
exec("./src/queuechooser.cs");
exec("./src/sleep.cs");
exec("./src/statuseffects.cs");
exec("./src/traits.cs");
exec("./src/trial.cs");
exec("./src/vocal.cs");
exec("./src/weapons.cs");
exec("./src/write.cs");
exec("./src/inventory.cs"); //Package activation order matters!
exec("./src/events.cs"); //exec events last 'cuz it takes some variables from statuseffects.cs
exec("./src/charactercreator.cs"); //Exec after minigame because this one overwrites serverCmdLight
//items
exec("./src/items/banana.cs");
exec("./src/items/bloodpack.cs");
exec("./src/items/box.cs");
exec("./src/items/burger.cs");
exec("./src/items/cleanspray.cs");
exec("./src/items/coat.cs");
exec("./src/items/disguise.cs");
exec("./src/items/flashbang.cs");
exec("./src/items/flashlight.cs");
exec("./src/items/key.cs");
exec("./src/items/lockpick.cs");
exec("./src/items/mop.cs");
exec("./src/items/paper.cs");
exec("./src/items/pen.cs");
exec("./src/items/radio.cs");
exec("./src/items/razor.cs");
exec("./src/items/repairkit.cs");
//weapons
exec("./src/weapons/axe.cs");
exec("./src/weapons/bat.cs");
exec("./src/weapons/katana.cs");
exec("./src/weapons/knife.cs");
exec("./src/weapons/scalpel.cs"); //Reskinned knife
exec("./src/weapons/leadpipe.cs");
exec("./src/weapons/machete.cs");
exec("./src/weapons/revolver.cs");
exec("./src/weapons/wrench.cs");
exec("./src/weapons/sledgehammer.cs");
exec("./src/weapons/taser.cs");
exec("./src/weapons/shovel.cs");
exec("./src/weapons/umbrella.cs");
exec("./src/weapons/handhammer.cs");
exec("./src/weapons/crowbar.cs");

if (!isObject(IntroPath))
	exec("./src/campaths/intro.cs");

if (!isObject(TrialIntroPath))
	exec("./src/campaths/TrialIntro.cs");

if (!isObject(TrialDiscussionPath))
	exec("./src/campaths/TrialDiscussion.cs");


addExtraResource($Despair::Path @ "res/logo.png");

package Despair_ObjectFix
{
	function SimSet::add(%set,%p0,%p1,%p2,%p3,%p4,%p5,%p6,%p7,%p8,%p9,%p10,%p11,%p12,%p13)
	{
		
		%added = 0;
		%total = 0;
		while(%p[%total] !$= "")
		{
			if(!%set.isMember(%p[%total]))
			{
				%p[%added] = %p[%total];
				%added++;
			}
			%total++;
		}
		switch(%added)
		{
			case 0:
				return;
			case 1:
				Parent::add(%set,%p0);
			case 2:
				Parent::add(%set,%p0,%p1);
			case 3:
				Parent::add(%set,%p0,%p1,%p2);
			case 4:
				Parent::add(%set,%p0,%p1,%p2,%p3);
			case 5:
				Parent::add(%set,%p0,%p1,%p2,%p3,%p4);
			case 6:
				Parent::add(%set,%p0,%p1,%p2,%p3,%p4,%p5);
			case 7:
				Parent::add(%set,%p0,%p1,%p2,%p3,%p4,%p5,%p6);
			case 8:
				Parent::add(%set,%p0,%p1,%p2,%p3,%p4,%p5,%p6,%p7);
			case 9:
				Parent::add(%set,%p0,%p1,%p2,%p3,%p4,%p5,%p6,%p7,%p8);
			case 10:
				Parent::add(%set,%p0,%p1,%p2,%p3,%p4,%p5,%p6,%p7,%p8,%p9);
			case 11:
				Parent::add(%set,%p0,%p1,%p2,%p3,%p4,%p5,%p6,%p7,%p8,%p9,%p10);
			case 12:
				Parent::add(%set,%p0,%p1,%p2,%p3,%p4,%p5,%p6,%p7,%p8,%p9,%p10,%p11);
			case 13:
				Parent::add(%set,%p0,%p1,%p2,%p3,%p4,%p5,%p6,%p7,%p8,%p9,%p10,%p11,%p12);
			case 14:
				Parent::add(%set,%p0,%p1,%p2,%p3,%p4,%p5,%p6,%p7,%p8,%p9,%p10,%p11,%p12,%p13);
			default:
				Parent::add(%set,%p0,%p1,%p2,%p3,%p4,%p5,%p6,%p7,%p8,%p9,%p10,%p11,%p12,%p13);
		}
	}
};
deactivatepackage(Despair_ObjectFix);
activatepackage(Despair_ObjectFix);
