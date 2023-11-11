// Generally, if you want to call this function, you'll want to leave %index and %delay blank.
function SimObject::instrPlayPattern(%this, %pattern, %index, %delay)
{
	cancel(%this.instrSongSchedule);
	cancel(%this.instrPatternSchedule);

	if (%index $= "")
	{
		%index = 0;
	}

	if (%index == 0)
	{
		%this.onInstrumentsPatternStart();
	}

	%noteCount = getWordCount(%pattern);

	if (%noteCount <= 0 || %index >= %noteCount)
	{
		%this.onInstrumentsPatternEnd(%delay);
		return;
	}

	if (%delay $= "")
	{
		%delay = Instruments::getDelayFromTempo($Instruments::Default::Tempo);
	}
	else
	{
		%delay = mClamp(%delay, $Instruments::Min::Delay, $Instruments::Max::Delay);
	}

	%note = Instruments::parseNote(getWord(%pattern, %index));

	if (Instruments::isDirective(%note))
	{
		%directive = getField(%note, 0);
		%value = getField(%note, 1);

		if (%directive $= "$T")
		{
			%delay = Instruments::getDelayFromTempo(%value);
		}
	}
	else
	{
		%this.instrPlayNote(%note);
	}

	%this.instrPatternSchedule = %this.schedule(
		Instruments::getNoteDelay(%note, %delay),
		"instrPlayPattern",
		%pattern,
		%index + 1,
		%delay
	);
}

// ------------------------------------------------

function SimObject::onInstrumentsPatternStart(%this)
{
	%this.instrIsPlayingPattern = true;
}

function SimObject::onInstrumentsPatternEnd(%this, %delay)
{
	%this.instrIsPlayingPattern = false;

	if (%this.instrIsPlayingSong)
	{
		%this.instrPlayNextPattern(%delay);
	}
}
