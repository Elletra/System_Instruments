// Extension: .blm
// Format:
// BLM <file version> <notation version> <type: 0 for pattern, 1 for song> <recommended instrument (NOT mandatory instrument)>
// <credits>
// <uploader name>TAB<uploader bl_id>
// <pattern if pattern, song if song>
// <if song, pattern 0>
// <if song, pattern 1>
// <if song, pattern ...>
// <if song, pattern N>


// TODO: Add validation for patterns, songs, file names, types, etc.

// ------------------------------------------------

// ------------------------------------------------
// !!! Constants -- DO NOT change these !!!
// ------------------------------------------------

$Instruments::FileType::Invalid = -1;
$Instruments::FileType::Pattern = 0;
$Instruments::FileType::Song = 1;

$Instruments::FileMode::Read = 0;
$Instruments::FileMode::Write = 1;

// ------------------------------------------------

function InstrumentsFileIO::start (%fileName, %mode, %type, %isServer)
{
	if (!InstrumentsFileIO::isValidFileName(%fileName))
	{
		error("Invalid file name '", %fileName, "'");
		return 0;
	}

	if (!InstrumentsFileIO::isValidMode(%mode))
	{
		error("Invalid file mode '", %type, "'");
		return 0;
	}

	if (!InstrumentsFileIO::isValidType(%type))
	{
		error("Invalid file type '", %type, "'");
		return 0;
	}

	%file = new FileObject ()
	{
		instrFileType = %type;
		instrIsServerFile = %isServer;
	};

	%path = InstrumentsFileIO::getFilePath(%fileName, %type, %isServer);
	%success = false;

	if (%mode $= $Instruments::FileMode::Read)
	{
		%success = %file.openForRead(%path);
	}
	else if (%mode $= $Instruments::FileMode::Write)
	{
		%success = %file.openForWrite(%path);
	}

	if (!%success)
	{
		%file.delete();
		%file = 0;
	}

	return %file;
}

function InstrumentsFileIO::end (%file)
{
	if (isObject(%file))
	{
		%file.close();
		%file.delete();
	}
}

// ------------------------------------------------

function InstrumentsFileIO::startWrite (%fileName, %type, %isServer)
{
	return InstrumentsFileIO::start(%fileName, $Instruments::FileMode::Write, %type, %isServer);
}

function InstrumentsFileIO::writeHeader (%file, %instrument, %credits, %uploader)
{
	if (!isObject(%file))
	{
		return false;
	}

	%file.writeLine("BLM " @ $Instruments::FileVersion
		SPC $Instruments::NotationVersion
		SPC %file.instrFileType
		SPC %instrument
	);

	%file.writeLine(%credits);
	%file.writeLine(%uploader);

	return true;
}

function InstrumentsFileIO::writePattern (%file, %pattern)
{
	if (!isObject(%file))
	{
		return false;
	}

	%file.writeLine(%pattern);

	return true;
}

function InstrumentsFileIO::writeSong (%file, %song, %patterns)
{
	if (!isObject(%file))
	{
		return false;
	}

	%file.writeLine(%song);

	%count = getRecordCount(%patterns);

	for (%i = 0; %i < %count; %i++)
	{
		InstrumentsFileIO::writePattern(%file, getRecord(%patterns, %i));
	}

	return true;
}

function InstrumentsFileIO::writePatternFile (%fileName, %pattern, %isServer, %instrument, %credits, %uploader)
{
	%file = InstrumentsFileIO::startWrite(%fileName, $Instruments::FileType::Pattern, %isServer);

	if (!isObject(%file))
	{
		return false;
	}

	InstrumentsFileIO::writeHeader(%file, %instrument, %credits, %uploader);
	InstrumentsFileIO::writePattern(%file, %pattern);
	InstrumentsFileIO::end(%file);

	return true;
}

function InstrumentsFileIO::writeSongFile (%fileName, %song, %patterns, %isServer, %instrument, %credits, %uploader)
{
	%file = InstrumentsFileIO::startWrite(%fileName, $Instruments::FileType::Song, %isServer);

	if (!isObject(%file))
	{
		return false;
	}

	InstrumentsFileIO::writeHeader(%file, %instrument, %credits, %uploader);
	InstrumentsFileIO::writeSong(%file, %song, %patterns);
	InstrumentsFileIO::end(%file);

	return true;
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
	switch (%type)
	{
		case $Instruments::FileType::Pattern:
			return "pattern";

		case $Instruments::FileType::Song:
			return "song";

		default:
			return "invalid";
	}
}

function InstrumentsFileIO::isValidMode (%mode)
{
	return %mode $= $Instruments::FileMode::Read || %mode $= $Instruments::FileMode::Write;
}

function InstrumentsFileIO::isValidType (%type)
{
	return %type $= $Instruments::FileType::Pattern || %type $= $Instruments::FileType::Song;
}

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
