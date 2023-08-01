// Make sure it's within the range and is actually an integer.
function Instruments::isValidPatternIndex (%index)
{
	return mClamp(%index, 0, $Instruments::Max::SongPatterns - 1) $= %index;
}

function Instruments::validateSong (%song, %patterns)
{
	%length = strlen(%song);

	if (%length <= 0)
	{
		return $Instruments::Error::SongMin;
	}

	if (%length > $Instruments::Max::SongLength)
	{
		return $Instruments::Error::SongMax;
	}

	%count = getRecordCount(%patterns);

	if (%count <= 0)
	{
		return $Instruments::Error::SongPatternMin;
	}

	if (%count > $Instruments::Max::SongPatterns)
	{
		return $Instruments::Error::SongPatternMax;
	}

	for (%i = 0; %i < %count; %i++)
	{
		%pattern = getRecord(%patterns, %i);

		if (%pattern !$= "")
		{
			%error = Instruments::validatePattern(%pattern);

			if (%error !$= $Instruments::Error::None)
			{
				return %error;
			}
		}
	}

	return $Instruments::Error::None;
}
