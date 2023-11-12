function FileObject::instrFileWrite(%this)
{
	%fileType = %this.instrFileType;

	%this.writeLine("BLM " @ %this.instrFileVersion SPC %this.instrFileNotationVersion SPC %fileType);
	%this.writeLine(%this.instrFileCredits);
	%this.writeLine(%this.instrFileUploader);

	switch (%fileType)
	{
		case $Instruments::FileType::Pattern:
			%this.writeLine(%this.instrFilePattern);

		case $Instruments::FileType::Song:
			%this.writeLine(%this.instrFileSong);

			//* We don't need to write a bunch of blank lines if we don't have to, so let's just get the
			//  index of the final actual song pattern. *//

			%max = 0;

			for (%i = 0; %i < $Instruments::Max::SongPatterns; %i++)
			{
				if (%this.instrFileSongPattern[%i] !$= "")
				{
					%max = %i;
				}
			}

			for (%i = 0; %i <= %max; %i++)
			{
				%this.writeLine(%this.instrFileSongPattern[%i]);
			}

		default:
			error("Invalid .blm file type '", %fileType, "'");
	}
}

// ------------------------------------------------

function InstrumentsFileIO::savePattern(%fileName, %isServer, %pattern, %credits, %uploader)
{
	if (!InstrumentsFileIO::isValidFileName(%fileName))
	{
		return $Instruments::Error::FileName;
	}

	%error = Instruments::validatePattern(%pattern);

	if (%error !$= $Instruments::Error::None)
	{
		return %error;
	}

	%file = new FileObject();
	%filePath = InstrumentsFileIO::getFilePath(%fileName, $Instruments::FileType::Pattern, %isServer);

	if (!%file.openForWrite(%filePath))
	{
		%file.delete();

		return $Instruments::Error::FileOpen;
	}

	%file.instrFileSetHeader(
		$Instruments::FileVersion,
		$Instruments::NotationVersion,
		$Instruments::FileType::Pattern,
		%credits,
		%uploader
	);

	%file.instrFileSetPattern(%pattern);
	%file.instrFileWrite();

	%file.close();
	%file.delete();

	return $Instruments::Error::None;
}

function InstrumentsFileIO::saveSong(%fileName, %isServer, %song, %patterns, %credits, %uploader)
{
	if (!InstrumentsFileIO::isValidFileName(%fileName))
	{
		return $Instruments::Error::FileName;
	}

	%error = Instruments::validateSong(%song, %patterns);

	if (%error !$= $Instruments::Error::None)
	{
		return %error;
	}

	%file = new FileObject();
	%filePath = InstrumentsFileIO::getFilePath(%fileName, $Instruments::FileType::Song, %isServer);

	if (!%file.openForWrite(%filePath))
	{
		%file.delete();

		return $Instruments::Error::FileOpen;
	}

	%file.instrFileSetHeader(
		$Instruments::FileVersion,
		$Instruments::NotationVersion,
		$Instruments::FileType::Song,
		%credits,
		%uploader
	);

	%file.instrFileSetSong(%song);

	for (%i = 0; %i < $Instruments::Max::SongPatterns; %i++)
	{
		%file.instrFileSetSongPattern(%i, getRecord(%patterns, %i));
	}

	%file.instrFileWrite();

	%file.close();
	%file.delete();

	return $Instruments::Error::None;
}
