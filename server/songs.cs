function SimObject::instrSetSongPattern (%this, %index, %pattern)
{
	if (!Instruments::isValidPatternIndex(%index) || strlen(%pattern) > $Instruments::Max::PatternLength)
	{
		return false;
	}

	%this.instrSongPattern[%index] = %pattern;

	return true;
}

function SimObject::instrPlaySong (%this, %song)
{
	%this.instrStopPlaying();

	%length = strlen(%song);

	if (%length <= 0 || %length > $Instruments::Max::SongLength)
	{
		return;
	}

	%this.instrSong = %song;
	%this.instrSongIndex = -1;

	%this.instrPlayNextPattern();
}

// If %delay is blank, it will be set to the default tempo.
function SimObject::instrPlayNextPattern (%this, %delay)
{
	%this.instrSongIndex++;

	if (%this.instrSongIndex == 0)
	{
		%this.onInstrumentsSongStart();
	}

	if (%this.instrSongIndex >= getWordCount(%this.instrSong))
	{
		%this.onInstrumentsSongEnd();
		return;
	}

	%patternIndex = getWord(%this.instrSong, %this.instrSongIndex);

	if (Instruments::isValidPatternIndex(%patternIndex))
	{
		%pattern = %this.instrSongPattern[%patternIndex];
	}
	else
	{
		// We don't want the song to end just because there's an invalid index, but we also don't
		// want to play a pattern with an invalid index.
		%pattern = "";
	}

	%this.instrPlayPattern(%pattern, 0, %delay);
}

// ------------------------------------------------

function SimObject::instrIsPlaying (%this)
{
	return %this.instrIsPlayingPattern || %this.instrIsPlayingSong;
}

function SimObject::instrStopPlaying (%this)
{
	cancel(%this.instrSongSchedule);
	cancel(%this.instrPatternSchedule);

	%this.instrIsPlayingSong = false;
	%this.instrIsPlayingPattern = false;
}

// ------------------------------------------------

function SimObject::onInstrumentsSongStart (%this)
{
	%this.instrIsPlayingSong = true;
}

function SimObject::onInstrumentsSongEnd (%this)
{
	%this.instrIsPlayingSong = false;
}
