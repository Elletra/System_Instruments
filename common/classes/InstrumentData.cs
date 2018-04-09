function InstrumentData::note(%this, %index) {
  return %this._["sound_" @ %index];
}

function InstrumentData::index(%this, %note) {
  return %this._["soundIndex_" @ %note];
}

function InstrumentData::get(%this, %field) {
  return %this._[%field];
}

function InstrumentData::set(%this, %field, %value) {
  %this._[%field] = %value;
}
