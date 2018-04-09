function Instruments::messageBoxOK(%this, %title, %body, %client) {
  if (%client $= "") {
    MessageBoxOK(%title, %body);
  }
  else {
    commandToClient(%client, 'MessageBoxOK', %title, %body);
  }
}

function Instruments::messageBoxOKCancel(%this, %title, %body, %ok, %client) {
  if (%client $= "") {
    MessageBoxOKCancel(%title, %body, %ok);
  }
  else {
    commandToClient(%client, 'MessageBoxOKCancel', %title, %body, %ok);
  }
}

function Instruments::messageBoxYesNo(%this, %title, %body, %yes, %client) {
  if (%client $= "") {
    MessageBoxYesNo(%title, %body, %yes);
  }
  else {
    commandToClient(%client, 'MessageBoxYesNo', %title, %body, %yes);
  }
}
