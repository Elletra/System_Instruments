function InstrumentsNamespace::onAdd(%this) {
  %this.addOnCount = 0;
  %this.instrumentCount = 0;
}

function InstrumentsNamespace::get(%this, %field) {
  return %this._[%field];
}

function InstrumentsNamespace::set(%this, %field, %value) {
  %this._[%field] = %value;
}

// Because I'm a lazy bastard
function InstrumentsNamespace::_(%this, %instrument) {
  return %this.get("Instrument_" @ %instrument);
}

// Just for consistency's sake
function InstrumentsNamespace::getInstrument(%this, %instrument) {
  return %this.get("Instrument_" @ %instrument);
}

function InstrumentsNamespace::getInstrumentFromIndex(%this, %index) {
  %instrument = %this.name(%index);
  return %this.getInstrument(%instrument);
}

function InstrumentsNamespace::index(%this, %instrument) {
  return %this.get("instrumentIndex_" @ %instrument);
}

function InstrumentsNamespace::name(%this, %index) {
  return %this.get("instrumentName_" @ %index);
}

function InstrumentsNamespace::isValidInstrumentIndex(%this, %index) {
  return isObject(%this.getInstrumentFromIndex(%index));
}

function InstrumentsNamespace::newAddOn(%this, %addon, %author) {
  if (%this.get("addOnIndex_" @ %addon) !$= "") {
    return;
  }

  %this.set("addOn_" @ %this.addOnCount, %addon);
  %this.set("addOnIndex_" @ %addon, %this.addOnCount);
  %this.set("addOnAuthor_" @ %addon, %author);

  %this.addOnCount++;
}

function InstrumentsNamespace::clearAddOn(%this, %addon) {
  if (%this.get("addOnIndex_" @ %addon) $= "") {
    return;
  }

  %index = %this.addOnIndex(%addon);

  %this.set("addOn_" @ %index, "");
  %this.set("addOnIndex_" @ %addon, "");
  %this.set("addOnAuthor_" @ %addon, "");
}

function InstrumentsNamespace::clearAllAddOns(%this) {
  %count = %this.addOnCount;

  for (%i = 0; %i < %count; %i++) {
    %addon = %this.addOn(%i);
    %this.clearAddOn(%addon);
  }

  %this.addOnCount = 0;
}

function InstrumentsNamespace::addOn(%this, %index) {
  return %this.get("addOn_" @ %index);
}

function InstrumentsNamespace::addOnIndex(%this, %addon) {
  return %this.get("addOnIndex_" @ %addon);
}

function InstrumentsNamespace::addOnAuthor(%this, %addon) {
  return %this.get("addOnAuthor_" @ %addon);
}

function InstrumentsNamespace::addInstrument(%this, %instrument) {
  %obj = %this._(%instrument);

  if (!isObject(%obj) || %obj.class !$= "InstrumentData") {
    %obj = new ScriptObject() {
      class = "InstrumentData";
      instrument = %instrument;
      length = 0;
    };

    %this.set("Instrument_" @ %instrument, %obj);
    %this.set("instrumentIndex_" @ %instrument, %this.instrumentCount);
    %this.set("instrumentName_" @ %this.instrumentCount, %instrument);

    %this.instrumentCount++;
  }

  return %obj;
}

function InstrumentsNamespace::clearInstruments(%this) {
  %count = %this.instrumentCount;

  for (%i = 0; %i < %count; %i++) {
    %name = %this.name(%i);
    %obj = %this._(%name);

    %this.set("Instrument_" @ %name, "");
    %this.set("instrumentIndex_" @ %name, "");
    %this.set("instrumentName_" @ %i, "");

    if (isObject(%obj)) { 
      %obj.delete();
    }
  }

  %this.clearAllAddOns();
  %this.instrumentCount = 0;
}

function InstrumentsNamespace::addNote(%this, %instrument, %note) {
  %obj = %this._(%instrument);

  if (!isObject(%obj)) {
    %obj = %this.addInstrument(%instrument);
  }

  %soundCount = %obj.length;

  %obj.set("sound_" @ %soundCount, %note);
  %obj.set("soundIndex_" @ %note, %soundCount);
  %obj.length++;
}
