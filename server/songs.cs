function SimObject::instrumentsSetSongPattern (%this, %index, %pattern)
{
	if (!Instruments::isValidPatternIndex(%index) || strlen(%pattern) > $Instruments::Max::PatternLength)
	{
		return false;
	}

	%this.instrSongPattern[%index] = %pattern;

	return true;
}

function SimObject::instrumentsPlaySong (%this, %song)
{
	%this.stopPlayingInstrument();

	%length = strlen(%song);

	if (%length <= 0 || %length > $Instruments::Max::SongLength)
	{
		return;
	}

	%this.instrSong = %song;
	%this.instrSongIndex = -1;

	%this.instrumentsPlayNextPattern();
}

// If %delay is blank, it will be set to the default tempo.
function SimObject::instrumentsPlayNextPattern (%this, %delay)
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
		// want to play it.
		%pattern = "";
	}

	%this.instrumentsPlayPattern(%pattern, 0, %delay);
}

function SimObject::onInstrumentsSongStart (%this)
{
	%this.instrIsPlayingSong = true;
}

function SimObject::onInstrumentsSongEnd (%this)
{
	%this.instrIsPlayingSong = false;
}

function SimObject::isPlayingInstrument (%this)
{
	return %this.instrIsPlayingPattern || %this.instrIsPlayingSong;
}

function SimObject::isPlayingSong (%this)
{
	return %this.instrIsPlayingSong;
}

function SimObject::isPlayingPattern (%this)
{
	return %this.instrIsPlayingPattern;
}

function SimObject::stopPlayingInstrument (%this)
{
	cancel(%this.instrSongSchedule);
	cancel(%this.instrPatternSchedule);

	%this.instrIsPlayingSong = false;
	%this.instrIsPlayingPattern = false;
}
