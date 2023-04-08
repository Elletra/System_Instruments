function InstrumentsServer::create ()
{
	while (isObject(InstrumentsServer))
	{
		InstrumentsServer.delete();
	}

	return InstrumentServerDatabase::create(InstrumentsServer);
}

// ------------------------------------------------

function InstrumentsServer::init (%this)
{
	%this.loadInstruments();
}

// ------------------------------------------------

function InstrumentsServer::loadInstruments (%this)
{
	echo("\n--------- Loading Instruments ---------");

	%list = %this.buildAddOnList();
	%count = getRecordCount(%list);

	for (%i = 0; %i < %count; %i++)
	{
		%this.loadInstrument(getRecord(%list, %i));
	}

	//* Absolutely insane hack to not fuck up the add-on loading loop. (Torque is a good engine.) *//

	%pattern = "Add-Ons/*/server.cs";

	for (%file = findFirstFile(%pattern); %file !$= ""; %file = findNextFile(%pattern))
	{
		if (%pattern $= "Add-Ons/System_Instruments/server.cs")
		{
			break;
		}
	}

	%count = %this.getCount();

	echo("\n\c4Loaded ", %count, " instrument", %count != 1 ? "s" : "", " successfully.");
}

// Since Torque can't handle nested file finding loops properly, we build a list of add-ons instead.
function InstrumentsServer::buildAddOnList (%this)
{
	%list = "";
	%pattern = "Add-Ons/Instrument_*/server.cs";

	for (%file = findFirstFile(%pattern); %file !$= ""; %file = findNextFile(%pattern))
	{
		%addOn = getSubStr(%file, 8, strlen(%file));
		%addOn = getSubStr(%addOn, 0, strpos(%addOn, "/"));

		if (InstrumentsServer::isAddOnEnabled(%addOn))
		{
			if (%list $= "")
			{
				%list = %addOn;
			}
			else
			{
				%list = %list NL %addOn;
			}
		}
	}

	return %list;
}

function InstrumentsServer::loadInstrument (%this, %addOn)
{
	echo("Loading instrument: ", %addOn);

	%this.loadSoundFiles(%addOn);
	%this.loadCredits(%addOn);

	%scriptFile = "Add-Ons/" @ %addOn @ "/instrument.cs";

	if (isFile(%scriptFile))
	{
		exec(%scriptFile);
	}
}

function InstrumentsServer::loadSoundFiles (%this, %addOn)
{
	%pattern = "Add-Ons/" @ %addOn @ "/sounds/*.wav";

	for (%file = findFirstFile(%pattern); %file !$= ""; %file = findNextFile(%pattern))
	{
		%this.loadSoundFile(%file);
	}
}

function InstrumentsServer::loadSoundFile (%this, %file)
{
	%fileBase = fileBase(%file);

	// Instrument names are retrieved from the file name itself, rather than the add-on name.
	// This is to support sound packs.
	%instrument = getSubStr(%fileBase, 0, strpos(%fileBase, "_"));

	if (!InstrumentsServer::isValidInstrumentName(%instrument))
	{
		warn("'", %instrument, "' is not a valid instrument name, skipping...");
		return;
	}

	%note = getSubStr(%fileBase, strlen(%instrument) + 1, strlen(%fileBase));

	if (!InstrumentsServer::isValidNoteName(%note))
	{
		warn("'", %note, "' is not a valid note name, skipping...");
		return;
	}

	%sound = InstrumentsServer::createNoteSound(%instrument, %note, %file);

	if (isObject(%sound))
	{
		%data = %this.addInstrument(%instrument);

		if (isObject(%data))
		{
			%data.addNote(%note, %sound);
		}
	}
}

function InstrumentsServer::loadCredits (%this, %addOn)
{
	// TODO: Implement
	error("ERROR: InstrumentsServer::loadCredits() not implemented!");
}

// ------------------------------------------------

function InstrumentsServer::isAddOnEnabled (%addOn)
{
	// `$AddOnLoaded` is for the game mode system.
	return $AddOn__[%addOn] == 1 || $AddOnLoaded__[%addOn] == 1;
}

function InstrumentsServer::getSoundFileName (%instrument, %note)
{
	return "Instrument_" @ %instrument @ "_" @ strreplace(%note, "#", "Sharp") @ "_Sound";
}

function InstrumentsServer::createNoteSound (%instrument, %note, %file)
{
	%sound = InstrumentsServer::getSoundFileName(%instrument, %note);

	if (isObject(%sound))
	{
		error("Object or datablock with name '", %sound, "' already exists!");
		return 0;
	}

	//* Datablock names must be constants, so we have to do some weird fuckery... *//

	while (isObject(__TEMP_NOTE_NAME__))
	{
		__TEMP_NOTE_NAME__.delete();
	}

	datablock AudioProfile (__TEMP_NOTE_NAME__)
	{
		description = "AudioClosest3D";
		fileName = %file;
		preload = true;
	};

	if (!isObject(__TEMP_NOTE_NAME__))
	{
		error("Failed to create datablock for '", %file, "'");
		return 0;
	}

	__TEMP_NOTE_NAME__.setName(%sound);

	return %sound;
}
