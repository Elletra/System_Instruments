function SimObject::instrGetInstrument(%this)
{
	return %this.instrInstrument;
}

function SimObject::instrSetInstrument(%this, %instrument)
{
	if (InstrumentsServer.hasInstrument(%instrument))
	{
		%this.instrInstrument = %instrument;
	}
}

// ------------------------------------------------

// Common checks for server commands like serverCmdInstr_playNote, serverCmdInstr_playPattern, etc.
function SimObject::instrServerCmdPlayCheck(%this)
{
	return InstrumentsServer.hasInstrument(%this.instrInstrument)
		&& ($Sim::Time - %this.instrLastPlayTime) * 1000 >= $Instruments::Min::Delay;
}

// ------------------------------------------------
// Server commands
// ------------------------------------------------

// For previews only! Players must use physical instruments.
function serverCmdInstr_setInstrument(%client, %instrument)
{
	%client.instrSetInstrument(%instrument);
}
