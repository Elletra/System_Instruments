// Double underscores required to prevent conflicts with the old, broken hexToRGB, _compToHex, etc. functions

function __rgbToHex( %rgb )
{
  %r = __compToHex( 255 * getWord( %rgb, 0 ) );
  %g = __compToHex( 255 * getWord( %rgb, 1 ) );
  %b = __compToHex( 255 * getWord( %rgb, 2 ) );

  return %r @ %g @ %b;
}
 
function __hexToRGB( %rgb )
{
  %r = __hexToComp( getSubStr( %rgb, 0, 2 ) ) / 255;
  %g = __hexToComp( getSubStr( %rgb, 2, 2 ) ) / 255;
  %b = __hexToComp( getSubStr( %rgb, 4, 2 ) ) / 255;

  return %r SPC %g SPC %b;
}
 
function __compToHex( %comp )
{
  %left = mFloor( %comp / 16 );
  %comp = mFloor( %comp - %left * 16 );

  %left = getSubStr( "0123456789ABCDEF", %left, 1 );
  %comp = getSubStr( "0123456789ABCDEF", %comp, 1 );

  return %left @ %comp;
}
 
function __hexToComp( %hex )
{
  %left = getSubStr( %hex, 0, 1 );
  %comp = getSubStr( %hex, 1, 2 );

  %left = striPos( "0123456789ABCDEF", %left );
  %comp = striPos( "0123456789ABCDEF", %comp );

  if ( %left < 0 || %comp < 0 )
  {
    return 0;
  }

  return %left * 16 + %comp;
}

function _rgbToDec(%color)
{
  %r = getWord(%color, 0);
  %g = getWord(%color, 1);
  %b = getWord(%color, 2);

  %a = getWord(%color, 3);

  if(%a $= "")
    %a = 255;

  %dec = mFloatLength(%r / 255, 6) SPC mFloatLength(%g / 255, 6) SPC mFloatLength(%b / 255, 6) SPC mFloatLength(%a / 255, 6);

  return %dec;
}

function _decToRGB(%color)
{
  %r = getWord(%color, 0);
  %g = getWord(%color, 1);
  %b = getWord(%color, 2);

  %a = getWord(%color, 3);

  if(%a $= "")
    %a = 1;

  %rgb = mFloatLength(%r * 255, 0) SPC mFloatLength(%g * 255, 0) SPC mFloatLength(%b * 255, 0) SPC mFloatLength(%a * 255, 0);

  return %rgb;
}
