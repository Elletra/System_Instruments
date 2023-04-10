function FileObject::instrWriteHeader (%this)
{
	%this.writeLine("BLM " @ %this.instrFileVersion
		SPC %this.instrFileNotationVersion
		SPC %this.instrFileType
		SPC %this.instrFileInstrument
	);

	%this.writeLine(%this.instrFileCredits);
	%this.writeLine(%this.instrFileUploader);
}

function FileObject::instrWritePattern (%this)
{
	%this.instrWriteHeader();
	%this.writeLine(%this.instrFilePattern);
}

function FileObject::instrWriteSong (%this)
{
	%this.instrWriteHeader();
	%this.writeLine(%this.instrFileSong);

	for (%i = 0; %i < $Instruments::Max::SongPatterns; %i++)
	{
		%this.writeLine(%this.instrFileSongPattern[%i]);
	}
}
