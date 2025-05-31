//Base
$Despair::DayLength = 600; //10 minutes for a full cycle
$Despair::InvestigationLength = 300; //5 mins
$Despair::InvestigationExtraLength = 90; //+1:30 mins for every new body
$Despair::CritThreshold = -80; //How much negative health can the player take before dying

$Despair::DisableWepsInvest = true; //Disable weapons when investgation starts?
$Despair::DisableWepsTimer = 30; //How many seconds until the weps are disabled.

$Despair::MinShutters = 2; //How many shutters should be open from the start.
//Trial
$Despair::TrialChatDelay = 1; //Chat delay in seconds to prevent spamming
$Despair::ChatDelay = 0.75; //Chat delay in seconds to prevent spamming
$Despair::DiscussPeriod = 420; //7 mins
$Despair::DiscussExtraLength = 90; //+1:30 mins for every body in trial
$Despair::MangleTimer = 900; //15 minutes if a mangled body turns up for discusison phase
$Despair::CanForceVote = 60; //1 min has to pass before you can forcevote
$Despair::CanForceTrial = 90; //1 min 30 seconds have to pass before you can force trial
// $Despair::MissingLength = 120; //2 mins until body announcement is made automatically
//Sleep
$Despair::SleepOverjoyed = 75;
$Despair::SleepHappy = 80;
$Despair::SleepSad = 130;
$Despair::SleepDepressed = 150;
$Despair::SleepDefault = 90;
$Despair::SleepKnockout = 30;

//Mood
$Despair::Mood::Enabled = true;
//Traits
$Despair::Traits::Enabled = true;
//Change map to school,mansion,motel,syndrome,campdespair,resort or hood depending on the build.
$map = "school";
//MOTEL MAP PREFS:
if($map $= "motel")
{
	//Allow sandstorms
	$Despair::Sandstorm = true;

	$mapCenter = "-87.7265 -20.5779 0.20202";
	$boundaries = 170;
	$Despair::RoomCount = 22;
	//male block
	$roomNum[1] = "16";
	$roomNum[2] = "17";
	$roomNum[3] = "18";
	$roomNum[4] = "26";
	$roomNum[5] = "27";
	$roomNum[6] = "28";

	//female block
	$roomNum[7] = "11";
	$roomNum[8] = "12";
	$roomNum[9] = "13";
	$roomNum[10] = "14";
	$roomNum[11] = "15";
	$roomNum[12] = "21";
	$roomNum[13] = "22";
	$roomNum[14] = "23";
	$roomNum[15] = "24";
	$roomNum[16] = "25";

	//extra rooms
	$roomNum[17] = "A";
	$roomNum[18] = "B";
	$roomNum[19] = "C";
	$roomNum[20] = "D";
	$roomNum[21] = "E";
	$roomNum[22] = "F";
}
//MANSION MAP PREFS:
if($map $= "mansion")
{
	//Allow sandstorms
	$Despair::Sandstorm = false;

	$Despair::RoomCount = 14;
	//underground
	$roomNum[1] = "B1";
	$roomNum[2] = "B2";
	$roomNum[3] = "B3";
	$roomNum[4] = "B4";
	$roomNum[5] = "B5";
	$roomNum[6] = "B6";
	$roomNum[7] = "B7";

	//aboveground
	$roomNum[8] = "Shack";
	$roomNum[9] = "F2";
	$roomNum[10] = "F3";
	$roomNum[11] = "F4";
	$roomNum[12] = "F5";
	$roomNum[13] = "F6";
	$roomNum[14] = "F7";
}
//SCHOOL MAP PREFS:
if($map $= "school")
{
	//Allow sandstorms
	$Despair::Sandstorm = false;

	$Despair::RoomCount = 22;

	$roomNum[1] = "11 Condo";
	$roomNum[2] = "12 Condo";
	$roomNum[3] = "13 Condo";
	$roomNum[4] = "14 Condo";
	$roomNum[5] = "15 Condo";
	$roomNum[6] = "16 Condo";
	$roomNum[7] = "21 Condo";
	$roomNum[8] = "22 Condo";
	$roomNum[9] = "23 Condo";
	$roomNum[10] = "24 Condo";
	$roomNum[11] = "25 Condo";

	$roomNum[12] = "11 Xing";
	$roomNum[13] = "12 Xing";
	$roomNum[14] = "13 Xing";
	$roomNum[15] = "14 Xing";
	$roomNum[16] = "15 Xing";
	$roomNum[17] = "16 Xing";
	$roomNum[18] = "21 Xing";
	$roomNum[19] = "22 Xing";
	$roomNum[20] = "23 Xing";
	$roomNum[21] = "24 Xing";
	$roomNum[22] = "25 Xing";

	$shutterNum[1] = "Gym";
	$shutterNum[2] = "Botany";
	$shutterNum[3] = "Woodworking";
	$shutterNum[4] = "Library";
	$shutterNum[5] = "Cafeteria";
	$shutterNum[6] = "Infirmary";
	$shutterNum[7] = "Tailoring";
}
//SYNDROME MAP PREFS:
if($map $= "syndrome")
{
	//Allow sandstorms
	$Despair::Sandstorm = false;

	$Despair::RoomCount = 18;

	$roomNum[1] = "1";
	$roomNum[2] = "2";
	$roomNum[3] = "3";
	$roomNum[4] = "4";
	$roomNum[5] = "5";
	$roomNum[6] = "6";
	$roomNum[7] = "7";
	$roomNum[8] = "8";
	$roomNum[9] = "9";
	$roomNum[10] = "10";
	$roomNum[11] = "11";
	$roomNum[12] = "12";
	$roomNum[13] = "13";
	$roomNum[14] = "14";
	$roomNum[15] = "15";
	$roomNum[16] = "16";
	$roomNum[17] = "17";
	$roomNum[18] = "18 Janitor";
}
//CAMP DESPAIR MAP PREFS:
if($map $= "campdespair")
{
	//Allow sandstorms
	$Despair::Sandstorm = true;

	$Despair::RoomCount = 12;
	//A House
	$roomNum[1] = "A1";
	$roomNum[2] = "A2";
	$roomNum[3] = "A3";
	$roomNum[4] = "A4";
	$roomNum[5] = "A5";
	//B House
	$roomNum[6] = "B1";
	$roomNum[7] = "B2";
	$roomNum[8] = "B3";
	$roomNum[9] = "B4";
	$roomNum[10] = "B5";
	//Janitor and Landlord House
	$roomNum[11] = "Janitor";
	$roomNum[12] = "House";
}
//RESORT MAP PREFS:
if($map $= "resort")
{
	//Allow sandstorms
	$Despair::Sandstorm = true;

	$Despair::RoomCount = 17;
	//Top Floor
	$roomNum[1] = "19";
	$roomNum[2] = "17";
	$roomNum[3] = "15";
	$roomNum[4] = "13";
	$roomNum[5] = "11";
	$roomNum[6] = "10";
	$roomNum[7] = "12";
	$roomNum[8] = "14";
	$roomNum[9] = "16";
	$roomNum[10] = "18";
	//Basement Keys
	$roomNum[11] = "B1";
	$roomNum[12] = "B2";
	$roomNum[13] = "B3";
	$roomNum[14] = "B4";
	$roomNum[15] = "B5";
	$roomNum[16] = "B6";
	//Shack Key
	$roomNum[17] = "Shack";


}
//HOOD MAP PREFS:
if($map $= "hood")
{
	//Allow sandstorms
	$Despair::Sandstorm = true;

	$Despair::RoomCount = 13;
	//Houses
	$roomNum[1] = "1 Beige House";
	$roomNum[2] = "2 Dark Green House";
	$roomNum[3] = "3 Violet House";
	$roomNum[4] = "4 Yellow House";
	$roomNum[5] = "5 Orange House";
	$roomNum[6] = "6 Blue House";
	$roomNum[7] = "7 Green House";
	$roomNum[8] = "8 Red House";
	$roomNum[9] = "9 Pink House";
	$roomNum[10] = "10 Lime House";
	$roomNum[11] = "11 White House";
	//Shack and campervan
	$roomNum[12] = "Shack";
	$roomNum[13] = "Camper Van";
}
