function FileObject::instrFileWrite (%this)
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
			error("Invalid file type '", %fileType, "'");
	}
}
