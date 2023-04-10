// Extension: .blm (*B*lock*l*and *M*usic file)
// Format:
//   BLM <file version> <notation version> <type: 0 for pattern, 1 for song> <recommended instrument>
//   <credits>
//   <uploader name>TAB<uploader bl_id>
//   <pattern if pattern, song if song>
//   <if song, pattern 0>
//   <if song, pattern 1>
//   <if song, pattern ...>
//   <if song, pattern N>

// ------------------------------------------------

//* TODO: Add support for reading/converting old notation versions. *//

function FileObject::instrReadHeader (%this, %expectedFileType)
{
	%data = %this.readLine();
	%signature = getWord(%data, 0);

	if (%signature !$= "BLM")
	{
		return $Instruments::Error::FileSignature TAB %signature;
	}

	%fileVersion = getWord(%data, 1);

	// File versions earlier than 2 aren't currently supported, nor are versions newer than this.
	if (%fileVersion < 2 || %fileVersion > $Instruments::FileVersion)
	{
		return $Instruments::Error::FileVersion TAB %fileVersion;
	}

	%notationVersion = getWord(%data, 2);

	// Notation versions earlier than 4 aren't currently supported, nor are versions newer than this.
	if (%notationVersion < 4 || %notationVersion > $Instruments::NotationVersion)
	{
		return $Instruments::Error::NotationVersion TAB %notationVersion;
	}

	%type = getWord(%data, 3);

	if (%type !$= %expectedFileType || !InstrumentsFileIO::isValidType(%type))
	{
		return $Instruments::Error::FileType TAB %type;
	}

	%instrument = getWord(%data, 4);
	%credits = %this.readLine();
	%uploader = %this.readLine();

	%this.instrFileSetHeader(%fileVersion, %notationVersion, %type, %instrument, %credits, %uploader);

	return $Instruments::Error::None;
}

function FileObject::instrReadPattern (%this)
{
	%result = %this.instrReadHeader($Instruments::FileType::Pattern);

	if (getField(%result, 0) !$= $Instruments::Error::None)
	{
		return %result;
	}

	%this.instrFileSetPattern(%this.readLine());

	return $Instruments::Error::None;
}

function FileObject::instrReadSong (%this)
{
	%result = %this.instrReadHeader($Instruments::FileType::Song);

	if (getField(%result, 0) !$= $Instruments::Error::None)
	{
		return %result;
	}

	%this.instrFileSetSong(%this.readLine());

	for (%i = 0; %i < $Instruments::Max::SongPatterns; %i++)
	{
		%this.instrFileSetSongPattern(%i, %this.readLine());
	}

	return $Instruments::Error::None;
}
