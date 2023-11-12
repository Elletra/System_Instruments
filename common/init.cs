// ------------------------------------------------
// !!! Constants -- DO NOT change these !!!
// ------------------------------------------------

$Instruments::Version = "2.0.0";
$Instruments::NotationVersion = 4;

// There was no `FileVersion` variable in the 1.x.x versions of instruments, but this add-on does
// use a different file format from the 1.x.x versions, so we started at 2 for the 2.0.0 release.
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
// Besides, the human ear can't hear faster than 33 milliseconds anyway ;)
$Instruments::Max::Tempo = 225;

// The notation format only supports whole notes to eighth notes due to technical limitations.
$Instruments::Min::NoteDivision = 1;
$Instruments::Max::NoteDivision = 8;

// Tempo and delay are inversely related, so we swap them here.
$Instruments::Min::Delay = (60000 / $Instruments::Max::Tempo) / $Instruments::Max::NoteDivision;
$Instruments::Max::Delay = (60000 / $Instruments::Min::Tempo) * $Instruments::Min::NoteDivision;

// This is the standard tempo for most music programs.
$Instruments::Default::Tempo = 120;

// This just feels like a good limit.
$Instruments::Max::SongPatterns = 50;

// The reason for these is that serverCmd arguments have a length limit of 255 characters.
$Instruments::Max::PatternLength = 255;
$Instruments::Max::SongLength = 255;
$Instruments::Max::CreditsLength = 255;
$Instruments::Max::UploaderLength = 255;

// ------------------------------------------------

$Instruments::ErrorCodeCount = -1;

// No error.
$Instruments::Error::None = $Instruments::ErrorCodeCount++;
// Object does not exist.
$Instruments::Error::ObjectDoesNotExist = $Instruments::ErrorCodeCount++;
// Value contains invalid characters.
$Instruments::Error::InvalidChars = $Instruments::ErrorCodeCount++;
// Pattern is too short.
$Instruments::Error::PatternMin = $Instruments::ErrorCodeCount++;
// Pattern is too long.
$Instruments::Error::PatternMax = $Instruments::ErrorCodeCount++;
// Song is too short.
$Instruments::Error::SongMin = $Instruments::ErrorCodeCount++;
// Song is too long.
$Instruments::Error::SongMax = $Instruments::ErrorCodeCount++;
// Not enough song patterns in a song (0).
$Instruments::Error::SongPatternMin = $Instruments::ErrorCodeCount++;
// Too many song patterns in a song.
$Instruments::Error::SongPatternMax = $Instruments::ErrorCodeCount++;
// Failed to open file for read/write.
$Instruments::Error::FileOpen = $Instruments::ErrorCodeCount++;
// Invalid file name.
$Instruments::Error::FileName = $Instruments::ErrorCodeCount++;
// The file did not start with "BLM".
$Instruments::Error::FileSignature = $Instruments::ErrorCodeCount++;
// Invalid file type (pattern, song, bindset, etc.).
$Instruments::Error::FileType = $Instruments::ErrorCodeCount++;
// Unsupported file version.
$Instruments::Error::FileVersion = $Instruments::ErrorCodeCount++;
// Unsupported notation version.
$Instruments::Error::NotationVersion = $Instruments::ErrorCodeCount++;

// ------------------------------------------------

$Instruments::FileType::Pattern = 0;
$Instruments::FileType::Song = 1;

// ------------------------------------------------

// A hack to fix inheritance because it completely breaks unless the super class relationship is
// established first. (See here: https://forum.blockland.us/index.php?topic=290879.0)
//
// This happens because TorqueScript is a horrible language!
function Instruments::fixInheritanceHack()
{
	new ScriptObject(InstrumentsServer)
	{
		superClass = InstrumentDatabase;
		class = InstrumentServerDatabase;
	}.delete();

	new ScriptObject()
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
