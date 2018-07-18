function InstrumentsBindset::onAdd(%this) {
  %this.bindCount = 0;
  %this.currIndex = 0;
}

function InstrumentsBindset::addBind(%this, %key, %phrase) {
  if (!Instruments.isKeyAllowed(%key)) {
    return;
  }

  if (!_strEmpty(%this.bindIndex[%key])) {
    %index = %this.bindIndex[%key];

    if (_strEmpty(%this._bind[%index])) {
      %this.bindCount++;
    }

    %this._bind[%index] = %key TAB %phrase;
    return;
  }

  if (%this.currIndex >= Instruments.const["MAX_BINDS"]) {
    return;
  }

  %index = %this.currIndex;

  %this._bind[%index] = %key TAB %phrase;
  %this.bindIndex[%key] = %index;
  %this.currIndex++;
  %this.bindCount++;
}

function InstrumentsBindset::removeBind(%this, %key) {
  %index = %this.bindIndex[%key];

  if (_strEmpty(%this._bind[%index])) {
    return;
  }

  %this._bind[%index] = "";
  %this.bindCount--;

  if (%this.bindCount < 0) {
    %this.bindCount = 0;
  }
}

function InstrumentsBindset::clearBinds(%this) {
  %count = %this.currIndex;

  if (%count <= 0) {
    return;
  }

  for (%i = 0; %i < %count; %i++) {
    %key = getField(%this._bind[%i], 0);
    %this.removeBind(%key);
  }

  %this.bindCount = 0;
}

function InstrumentsBindset::getBindKey(%this, %index) {
  return %this._bind[%index];
}

function InstrumentsBindset::getBindIndex(%this, %key) {
  return %this.bindIndex[%key];
}

function InstrumentsBindset::getBoundPhrase(%this, %key) {
  %index = %this.bindIndex[%key];
  return getField(%this._bind[%index], 1);
}
