// ------------------------------------------------
// Instrument database classes for server-specific functionality.
// ------------------------------------------------

// ------------------------------------------------

function InstrumentServerDatabase::create (%name)
{
	return new ScriptObject (%name)
	{
		superClass = InstrumentDatabase;
		class = InstrumentServerDatabase;
	};
}

// ------------------------------------------------

function InstrumentServerDatabase::onAdd (%this)
{
	Parent::onAdd(%this);
}

function InstrumentServerDatabase::onRemove (%this)
{
	Parent::onRemove(%this);
}

// ------------------------------------------------

function InstrumentServerDatabase::getNoteSound (%this, %instrument, %note)
{
	if (!%this.hasInstrument(%instrument))
	{
		return 0;
	}

	%data = %this.getInstrument(%instrument);

	return %data.hasNote(%note) ? %data.getNoteSound(%note) : 0;
}

function InstrumentServerDatabase::createEntry (%this, %instrumentName)
{
	return InstrumentServerData::create(%instrumentName);
}

// ------------------------------------------------

function InstrumentServerData::create (%instrumentName)
{
	return new ScriptObject ()
	{
		superClass = InstrumentData;
		class = InstrumentServerData;
		instrName = %instrumentName;
	};
}

// ------------------------------------------------

function InstrumentServerData::onAdd (%this)
{
	Parent::onAdd(%this);
}

function InstrumentServerData::onRemove (%this)
{
	Parent::onRemove(%this);
}

// ------------------------------------------------

function InstrumentServerData::addNote (%this, %name, %datablock)
{
	%added = Parent::addNote(%this, %name);

	if (%added)
	{
		%this.instrNoteSound[%name] = %datablock;
	}

	return %added;
}

function InstrumentServerData::getNoteSound (%this, %noteName)
{
	return %this.instrNoteSound[%noteName];
}
