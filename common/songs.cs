// Make sure it's within the range and is actually an integer.
function Instruments::isValidPatternIndex (%index)
{
	return mClamp(%index, 0, $Instruments::Max::SongPatterns - 1) $= %index;
}
