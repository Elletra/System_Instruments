// Extension: .blm (*B*lock*l*and *M*usic file)
// Format:
//   BLM <file version> <notation version> <type: 0 for pattern, 1 for song>
//   <credits>
//   <uploader name>TAB<uploader bl_id>
//   <pattern if pattern, song if song>
//   <if song, pattern 0>
//   <if song, pattern 1>
//   <if song, pattern ...>
//   <if song, pattern N>

// ------------------------------------------------

//* TODO: Add support for reading/converting old notation versions. *//

function FileObject::instrFileRead(%this, %expectedFileType)
{
	%data = %this.readLine();
	%signature = getWord(%data, 0);

	if (%signature !$= "BLM")
	{
		return $Instruments::Error::FileSignature TAB %signature;
	}

	%fileVersion = getWord(%data, 1);

	// File versions earlier than 2 aren't currently supported, nor are versions newer than ours.
	if (%fileVersion < 2 || %fileVersion > $Instruments::FileVersion)
	{
		return $Instruments::Error::FileVersion TAB %fileVersion;
	}

	%notationVersion = getWord(%data, 2);

	// Notation versions earlier than 4 aren't currently supported, nor are versions newer than ours.
	if (%notationVersion < 4 || %notationVersion > $Instruments::NotationVersion)
	{
		return $Instruments::Error::NotationVersion TAB %notationVersion;
	}

	%type = getWord(%data, 3);

	if (%type !$= %expectedFileType || !InstrumentsFileIO::isValidType(%type))
	{
		return $Instruments::Error::FileType TAB %type;
	}

	%credits = %this.readLine();
	%uploader = %this.readLine();

	%this.instrFileSetHeader(%fileVersion, %notationVersion, %type, %credits, %uploader);

	switch (%type)
	{
		case $Instruments::FileType::Pattern:
			%this.instrFileSetPattern(%this.readLine());

		case $Instruments::FileType::Song:
			%this.instrFileSetSong(%this.readLine());

			// Blank out any existing song patterns.
			for (%i = 0; %i < $Instruments::Max::SongPatterns; %i++)
			{
				%this.instrFileSetSongPattern(%i, "");
			}

			for (%i = 0; %i < $Instruments::Max::SongPatterns && !%this.isEOF(); %i++)
			{
				%this.instrFileSetSongPattern(%i, %this.readLine());
			}

		default:
			return $Instruments::Error::FileType TAB %expectedFileType;
	}

	return $Instruments::Error::None;
}
