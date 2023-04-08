function SimObject::instrPlayNote (%this, %parsedNote)
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

function SimObject::instrPlaySound (%this, %sound)
{
	// To be overridden by subclasses.
}

// ------------------------------------------------

function SimObject::onInstrumentsPlayNote (%this, %parsedNote)
{
	// To be overridden by subclasses.
}

// ------------------------------------------------

// All objects with a transform.
function SceneObject::instrPlaySound (%this, %sound)
{
	if (InstrumentsServer::isValidSound(%sound))
	{
		serverPlay3D(%sound, %this.getPosition());
	}
}

// ------------------------------------------------

function GameConnection::instrPlaySound (%this, %sound)
{
	if (InstrumentsServer::isValidSound(%sound))
	{
		%this.play2D(%sound);
	}
}

// ------------------------------------------------

function GameConnection::onInstrumentsPlayNote (%this, %parsedNote)
{
	// FIXME: This is just a temporary debug thing.
	%this.bottomPrint(%parsedNote, 2, true);
}

// ------------------------------------------------

// Whether what we want to play is even a valid sound datablock.
function InstrumentsServer::isValidSound (%sound)
{
	return isObject(%sound) && %sound.getClassName() $= "AudioProfile";
}
