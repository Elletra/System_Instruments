// FIXME: This will be removed for the release version.
if (!isObject(C4_Note_Sound))
{
	exec("./sounds/datablocks.cs");
}

// exec("./init.cs");

exec("./notes.cs");
exec("./patterns.cs");
exec("./songs.cs");

exec("./packaged.cs");
