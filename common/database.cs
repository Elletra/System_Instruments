// ------------------------------------------------
// The instrument database is an object that contains data on instruments such as note names,
// sound datablocks (server only, see "server/database.cs"), etc.
//
// This is the base super class that client- and server-specific databases inherit from. You don't
// really want to use it directly.
// ------------------------------------------------

// ------------------------------------------------

function InstrumentDatabase::create(%name)
{
	return new ScriptObject(%name)
	{
		superClass = InstrumentDatabase;
	};
}

// ------------------------------------------------

function InstrumentDatabase::onAdd(%this)
{
	%this.instrCount = 0;
}

function InstrumentDatabase::onRemove(%this)
{
	%this.clear();
}

// ------------------------------------------------

function InstrumentDatabase::addInstrument(%this, %name)
{
	if (%this.hasInstrument(%name))
	{
		return %this.getInstrument(%name);
	}

	if (!InstrumentsServer::isValidInstrumentName(%name))
	{
		error("addInstrument() - Invalid instrument name '", %name, "'");
		return 0;
	}

	%entry = %this.createEntry(%name);
	%index = %this.instrCount;

	%this.instrData[%name] = %entry;
	%this.instrName[%index] = %name;
	%this.instrIndex[%name] = %index;
	%this.instrCount++;

	return %entry;
}

function InstrumentDatabase::getInstrument(%this, %name)
{
	return %this.hasInstrument(%name) ? %this.instrData[%name] : 0;
}

function InstrumentDatabase::hasInstrument(%this, %name)
{
	return %this.instrData[%name] !$= "";
}

function InstrumentDatabase::getCount(%this)
{
	return %this.instrCount;
}

function InstrumentDatabase::clear(%this)
{
	%count = %this.getCount();

	for (%i = 0; %i < %count; %i++)
	{
		%name = %this.instrName[%i];
		%data = %this.instrData[%name];

		%this.instrData[%name] = "";
		%this.instrName[%i] = "";
		%this.instrIndex[%name] = "";

		%data.delete();
	}

	%this.instrCount = 0;
}

// To allow subclasses to override and use custom classes (see "server/database.cs").
function InstrumentDatabase::createEntry(%this, %name)
{
	return InstrumentData::create(%name);
}

// ------------------------------------------------

function InstrumentData::create(%name)
{
	return new ScriptObject()
	{
		superClass = InstrumentData;
		instrName = %name;
	};
}

// ------------------------------------------------

function InstrumentData::onAdd(%this)
{
	%this.instrNoteCount = 0;
}

function InstrumentData::onRemove(%this)
{
	// Implementation to allow other scripts to package the function.
}

// ------------------------------------------------

function InstrumentData::addNote(%this, %name)
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

function InstrumentData::hasNote(%this, %name)
{
	return %this.instrNoteIndex[%name] !$= "";
}

function InstrumentData::getNoteName(%this, %index)
{
	return %this.instrNoteName[%index];
}

function InstrumentData::getNoteIndex(%this, %name)
{
	return %this.instrNoteIndex[%name];
}

function InstrumentData::getCount(%this)
{
	return %this.instrNoteCount;
}
