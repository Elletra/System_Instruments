package System_Instruments__common
{
	// ------------------------------------------------
	// ScriptGroup method overrides
	// ------------------------------------------------

	// Clear name mapping before clearing database.
	function InstrumentDatabase::clear (%this)
	{
		%count = %this.getCount();

		for (%i = 0; %i < %count; %i++)
		{
			%this.instrData[%this.getObject(%i).instrName] = "";
		}

		Parent::clear();
	}

	// Clear name mapping before deleting everything.
	function InstrumentDatabase::deleteAll (%this)
	{
		%count = %this.getCount();

		for (%i = 0; %i < %count; %i++)
		{
			%this.instrData[%this.getObject(%i).instrName] = "";
		}

		Parent::deleteAll(%this);
	}

	// We override the `remove()` method for one main reason: It is a variadic function, and TorqueScript
	// doesn't support variadic functions.
	//
	// Sure, you could do the %arg[%i] thing, but you're still going to have to hardcode a set number of
	// arguments that will always be smaller than what the base function supports.
	//
	// Normally this wouldn't be a problem, but we would need to remove the `instrData` field associated
	// with whatever instrument is being removed, so we would have to package this function and screw up
	// the variadiac nature of it.
	//
	// Also there's not really a good reason to be removing individual instruments from databases anyway,
	// so it's best to just disable the functionality altogether. I don't like doing this, but it's
	// necessary.
	function InstrumentDatabase::remove ()
	{
		error("remove() - Cannot remove specific instruments from the database");
	}
};
activatePackage(System_Instruments__common);
