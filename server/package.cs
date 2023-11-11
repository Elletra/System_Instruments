package System_Instruments__server
{
	function createMission()
	{
		Parent::createMission();

		if (isObject(InstrumentsServer))
		{
			// Add it to the MissionCleanup so it's automatically deleted when the server closes.
			MissionCleanup.add(InstrumentsServer);
		}
	}

	function onServerDestroyed()
	{
		// Delete it manually if it didn't get automatically deleted for some reason.
		if (isObject(InstrumentsServer))
		{
			InstrumentsServer.delete();
		}

		Parent::onServerDestroyed();
	}
};
activatePackage(System_Instruments__server);
