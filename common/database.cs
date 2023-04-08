// ------------------------------------------------
// The instrument database is an object that contains data on instruments such as note names,
// sound datablocks (server only, see "server/database.cs"), etc.
//
// This is the base super class that client- and server-specific databases will inherit from.
// ------------------------------------------------

// ------------------------------------------------

function InstrumentDatabase::create (%name)
{
	return new ScriptGroup (%name)
	{
		superClass = InstrumentDatabase;
	};
}

// ------------------------------------------------

function InstrumentDatabase::onAdd (%this)
{
	// Implementation to allow other scripts to package it.
}

function InstrumentDatabase::onRemove (%this)
{
	%this.deleteAll();
}

// ------------------------------------------------

function InstrumentDatabase::addInstrument (%this, %name)
{
	if (%this.hasInstrument(%name))
	{
		return %this.getInstrument(%name);
	}

	if (!InstrumentsServer::isValidInstrumentName(%name))
	{
		error("addInstrument() - Invalid instrument name: ", %name);
		return 0;
	}

	%entry = %this.createEntry(%name);

	// Map name to instrument data for quicker lookups.
	%this.instrData[%name] = %entry;

	%this.add(%entry);

	return %entry;
}

function InstrumentDatabase::getInstrument (%this, %name)
{
	if (!%this.hasInstrument(%name))
	{
		return 0;
	}

	return %this.instrData[%name];
}

function InstrumentDatabase::hasInstrument (%this, %name)
{
	return %this.instrData[%name] !$= "";
}

// To allow subclasses to override and use custom classes (see "server/database.cs").
function InstrumentDatabase::createEntry (%this, %name)
{
	return InstrumentData::create(%name);
}

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

// We don't want anyone removing instruments from databases, so we override the `remove()` method
// inherited from `ScriptGroup`.
//
// I really don't like doing this, but it's necessary.
function InstrumentDatabase::remove ()
{
	error("InstrumentDatabase::remove() - Cannot remove specific instruments from database");
}

// ------------------------------------------------

function InstrumentData::create (%name)
{
	return new ScriptObject ()
	{
		superClass = InstrumentData;
		instrName = %name;
	};
}

// ------------------------------------------------

function InstrumentData::onAdd (%this)
{
	%this.instrNoteCount = 0;
}

function InstrumentData::onRemove (%this)
{
	// Implementation to allow other scripts to package it.
}

// ------------------------------------------------

function InstrumentData::addNote (%this, %name)
{
	if (%this.hasNote(%name))
	{
		return true;
	}

	if (!InstrumentsServer::isValidNoteName(%name))
	{
		return false;
	}

	%index = %this.instrNoteCount;

	%this.instrNoteName[%index] = %name;
	%this.instrNoteIndex[%name] = %index;
	%this.instrNoteCount++;

	return true;
}

function InstrumentData::hasNote (%this, %name)
{
	return %this.instrNoteIndex[%name] !$= "";
}

function InstrumentData::getNoteName (%this, %index)
{
	return %this.instrNoteName[%index];
}

function InstrumentData::getNoteIndex (%this, %name)
{
	return %this.instrNoteIndex[%name];
}

function InstrumentData::getCount (%this)
{
	// Coerce to integer.
	return mFloatLength(%this.instrNoteCount, 0);
}
