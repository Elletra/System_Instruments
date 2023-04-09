exec("./init.cs");

exec("./notes.cs");
exec("./patterns.cs");
exec("./songs.cs");
exec("./database.cs");
exec("./misc.cs");

exec("./package.cs");

if (!isObject(InstrumentsServer))
{
	InstrumentsServer::create();
	InstrumentsServer.init();
}
