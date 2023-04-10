exec("./read.cs");
exec("./write.cs");

// ------------------------------------------------

// ------------------------------------------------
// Functions shared by read and write functions.
// ------------------------------------------------

function FileObject::instrFileSetHeader (%this, %fileVer, %notVer, %type, %instrument, %credits, %uploader)
{
	%this.instrFileVersion = %fileVer;
	%this.instrFileNotationVersion = %notVer;
	%this.instrFileType = %type;
	%this.instrFileInstrument = %instrument;
	%this.instrFileCredits = %credits;
	%this.instrFileUploader = %uploader;
}

function FileObject::instrFileSetPattern (%this, %pattern)
{
	%this.instrFilePattern = getSubStr(%pattern, 0, $Instruments::Max::PatternLength);
}

function FileObject::instrFileSetSong (%this, %song)
{
	%this.instrFileSong = getSubStr(%song, 0, $Instruments::Max::SongLength);
}

function FileObject::instrFileSetSongPattern (%this, %index, %pattern)
{
	if (Instruments::isValidPatternIndex(%index))
	{
		%this.instrFileSongPattern[%index] = getSubStr(%pattern, 0, $Instruments::Max::PatternLength);
	}
}

// ------------------------------------------------

function InstrumentsFileIO::getFilePath (%fileName, %type, %isServer)
{
	%folder = %isServer ? "server" : "client";
	%typeString = InstrumentsFileIO::getTypeString(%type);

	return "config/" @ %folder @ "/instruments/" @ %typeString @ "s/" @ %fileName @ ".blm";
}

function InstrumentsFileIO::getTypeString (%type)
{
	switch$ (%type)
	{
		case $Instruments::FileType::Pattern:
			return "pattern";

		case $Instruments::FileType::Song:
			return "song";

		default:
			return "invalid";
	}
}

// ------------------------------------------------

function InstrumentsFileIO::isValidType (%type)
{
	return %type $= $Instruments::FileType::Pattern || %type $= $Instruments::FileType::Song;
}

// This function is more what this add-on supports for file names, not necessarily Windows-specific
// limitations, though those are also accounted for.
function InstrumentsFileIO::isValidFileName (%fileName)
{
	if (strlen(%filename) <= 0)
	{
		return false;
	}

	if (strlen(%filename) >= 255)
	{
		return false;
	}

	%chars = "\\ / : ; * ? \" < > | \r \n \x7F \xA0";
	%count = getWordCount(%chars);

	for (%i = 0; %i < %count; %i++)
	{
		if (strpos(%fileName, getWord(%chars, %i)) != -1)
		{
			return false;
		}
	}

	return stripMLControlChars(%fileName) $= %fileName;
}
