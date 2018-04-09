if (!isObject(PianoKeyProfile)) {
  new GuiControlProfile(PianoKeyProfile) {
    fillColor = "255 255 255 255";
    fillColorHL = "255 255 255 255";
    fillColorNA = "255 255 255 255";
    justify = "bottom";
    textOffset = "0 24";
  };
}

if (!isObject(PianoSharpKeyProfile)) {
  new GuiControlProfile(PianoSharpKeyProfile) {
    fillColor = "0 0 0 255";
    fillColorHL = "0 0 0 255";
    fillColorNA = "0 0 0 255";
    justify = "bottom";
    textOffset = "0 24";
  };
}

if (!isObject(InstrumentsBoundKeyProfile)) {
  new GuiControlProfile(InstrumentsBoundKeyProfile : GuiButtonProfile) {
    fontType = "Arial Bold";
  };
}
