function SimObject::instrGetInstrument (%this)
{
	return %this.instrInstrument;
}

function SimObject::instrSetInstrument (%this, %instrument)
{
	if (InstrumentsServer.hasInstrument(%instrument))
	{
		// Not a big fan of `instrInstrument`, but we do want to keep names and prefixes consistent...
		%this.instrInstrument = %instrument;
	}
}
