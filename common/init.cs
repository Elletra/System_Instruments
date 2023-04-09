// ------------------------------------------------
// !!! Constants -- DO NOT change these !!!
// ------------------------------------------------

$Instruments::Version = "2.0.0";
$Instruments::NotationVersion = 4;
$Instruments::FileVersion = 2;

// I generally don't like arbitrary limitations, so I set this to the lowest possible value.
$Instruments::Min::Tempo = 1;

// The reason for this is technical limitations. 60000 / 225 is a 266.6666... millisecond delay,
// but we also have to account for the fact that this can be divided into eighth notes.
// 
// This makes the delay 33.3333... which is almost the same as the tick rate of the engine (32ms).
//
// If we were to go lower than 33.3333... things would probably get weird, so I think it's best to
// just get it as close as possible to the tick rate without going under.
//
// Plus, the human ear can't hear faster than 33 milliseconds anyway ;)
$Instruments::Max::Tempo = 225;

// This is the standard tempo for most music programs.
$Instruments::Default::Tempo = 120;

// This just feels like a good limit.
$Instruments::Max::SongPatterns = 50;

// serverCmd arguments have a length limit of 255 characters.
$Instruments::Max::PatternLength = 255;
$Instruments::Max::SongLength = 255;

// ------------------------------------------------

// A hack to fix inheritance because it completely breaks unless the super class relationship is
// established first. (See here: https://forum.blockland.us/index.php?topic=290879.0)
//
// This happens because TorqueScript is a horrible language!
function Instruments::fixInheritanceHack ()
{
	new ScriptGroup (InstrumentsServer)
	{
		superClass = InstrumentDatabase;
		class = InstrumentServerDatabase;
	}.delete();

	new ScriptObject ()
	{
		superClass = InstrumentData;
		class = InstrumentServerData;
	}.delete();

	$Instruments::InheritanceFixed = true;
}

if (!$Instruments::InheritanceFixed)
{
	Instruments::fixInheritanceHack();
}
