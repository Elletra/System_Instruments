function Instruments::validatePattern(%pattern)
{
	%length = strlen(%pattern);

	if (%length <= 0)
	{
		return $Instruments::Error::PatternMin;
	}

	if (%length > $Instruments::Max::PatternLength)
	{
		return $Instruments::Error::PatternMax;
	}

	if (strpos(%pattern, "\t") != -1 || strpos(%pattern, "\n") != -1 || stripMLControlChars(%pattern) !$= %pattern)
	{
		return $Instruments::Error::InvalidChars;
	}

	return $Instruments::Error::None;
}
