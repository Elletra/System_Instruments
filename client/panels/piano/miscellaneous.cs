function InstrumentsClient::setDelayOrTempo(%this, %delayOrTempo) {
  InstrumentsClient.addToPhrase(%delayOrTempo, "other");
}

function InstrumentsClient::onDelayChanged(%this) {
  %delay = $Instruments::GUI::Delay;

  InstrumentsDlg_Tempo.setValue(mFloatLength(60000 / %delay, 0));
  %delay = mClamp(%delay, 0, Instruments.const["HIGHEST_DELAY"]);
  // It's clamping to 0 rather than the lowest possible delay because
  // it makes typing in the textbox infuriating

  $Instruments::GUI::Delay = %delay;
  InstrumentsDlg_Delay.setValue(%delay);
}

function InstrumentsClient::onTempoChanged(%this) {
  %tempo = $Instruments::GUI::Tempo;

  InstrumentsDlg_Delay.setValue(mFloatLength((1 / %tempo) * 60000, 0));
  %tempo = mClamp(%tempo, 0, Instruments.const["HIGHEST_TEMPO"]);

  $Instruments::GUI::Tempo = %tempo;
  InstrumentsDlg_Tempo.setValue(%tempo);
}
