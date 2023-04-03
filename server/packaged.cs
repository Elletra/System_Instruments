package System_Instruments__server
{
	function createMission ()
	{
		Parent::createMission();

		//if (isObject(InstrumentsServer))
		//{
		//	InstrumentsServer.delete();
		//}

		//new ScriptObject(InstrumentsServer);
	}

	function destroyServer ()
	{
		//InstrumentsServer.delete();
		Parent::destroyServer();
	}
};
activatePackage(System_Instruments__server);
