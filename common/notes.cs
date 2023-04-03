// Splits up a note into words and fields.
function Instruments::parseNote (%note)
{
	%note = stripMLControlChars(stripChars(%note, " \t\n"));

	if (Instruments::isDirective(%note))
	{
		// Separate directive into key-value pair.
		return strreplace(%note, "=", "\t");
	}

	// Separate rests from notes and separate chords into words.
	return strreplace(strreplace(%note, "/", "\t"), "+", " ");
}

// Whether a note is an actual note or a directive (e.g. $T=120).
function Instruments::isDirective (%note)
{
	return strpos(%note, "$") == 0;
}

// Whether a note is an actual note or a rest.
function Instruments::isRest (%note)
{
	return strpos(%note, "|") == 0;
}

// Converts tempo to a millisecond delay.
function Instruments::getDelayFromTempo (%tempo)
{
	return 60000 / mClamp(%tempo, $Instruments::Min::Tempo, $Instruments::Max::Tempo);
}

// Get delay for things like half notes, quarter notes, etc.
function Instruments::getNoteDelay (%parsedNote, %baseDelay)
{
	if (Instruments::isDirective(%parsedNote))
	{
		return 1;
	}

	if (getFieldCount(%parsedNote) <= 1)
	{
		return %baseDelay;
	}

	// The notation format only allows whole notes to eighth notes due to technical limitations.
	return %baseDelay / mClamp(getField(%parsedNote, 1), 1, 8);
}

// Whether what we want to play is even a valid sound datablock.
function Instruments::isValidSound (%sound)
{
	return isObject(%sound) && %sound.getClassName() $= "AudioProfile";
}
