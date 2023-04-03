function SimObject::instrumentsPlayNote (%this, %parsedNote)
{
	if (%parsedNote $= "" || Instruments::isDirective(%parsedNote) || Instruments::isRest(%parsedNote))
	{
		return;
	}

	%parsedNote = getField(%parsedNote, 0);
	%count = mClamp(getWordCount(%parsedNote), 0, 4);

	for (%i = 0; %i < %count; %i++)
	{
		// TODO: Modular system for instrument sounds, rather than this hardcoded method.
		%note = strreplace(getWord(%parsedNote, %i), "#", "S");

		%this.instrumentsPlaySound(%note @ "_Note_Sound");
	}

	%this.onInstrumentsPlayNote(%parsedNote);
}

function SimObject::instrumentsPlaySound (%this, %sound)
{
	// To be overridden by subclasses.
}

function SimObject::onInstrumentsPlayNote (%this, %parsedNote)
{
	// To be overridden by subclasses.
}

// All objects with a transform.
function SceneObject::instrumentsPlaySound (%this, %sound)
{
	if (Instruments::isValidSound(%sound))
	{
		serverPlay3D(%sound, %this.getPosition());
	}
}

function GameConnection::instrumentsPlaySound (%this, %sound)
{
	if (Instruments::isValidSound(%sound))
	{
		%this.play2D(%sound);
	}
}

function GameConnection::onInstrumentsPlayNote (%this, %parsedNote)
{
	// FIXME: This is just a temporary debug thing.
	%this.bottomPrint(%parsedNote, 2, true);
}
