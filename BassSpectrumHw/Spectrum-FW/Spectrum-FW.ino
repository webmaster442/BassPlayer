/*
  Arduino Spectrum Analyzer v2
 Created by: webmaster442
 */
#include <HT1632.h>
#include <font_5x4.h>
#include <images.h>
#include <Wire.h>
#include <Time.h>
#include <DS1307RTC.h>
#include <math.h>

/*==================================================================
 Global Variables
 ===================================================================*/
byte _value = 0;
byte _leds = 0;
byte _low = 0;
byte _high = 0;
byte _row0[32] = {
  0
};
byte _row1[32] = {
  0
};
byte *_displays[] = {
  _row0, _row1
};
char _textbuff[32];
tmElements_t _tm;

#define THERMISTOR 0
#define PADRESISTOR 10000.0
#define LEVELPOT 1

#define SPECTRUM 0x02
#define SPECTRUMINVERSE 0x85
#define LEVELS 0x49
#define TIME 0x57

void setup()
{
  //12 - lower panel CS
  //13 - high panel cs
  //10 - WR
  //9 - Data
  HT1632.begin(13, 12, 10, 9);
  Serial.begin(115200);
  SetLevel();
}

char recv[34] = {0};

void loop()
{
  Serial.readBytesUntil(255, recv, 34);
  byte command = (byte)recv[0];
  if (command == SPECTRUM || command == SPECTRUMINVERSE)
  {
    for (int j = 1; j < 34; ++j)
    {
      _leds = map((byte)recv[j], 0, 255, 0, 16);
      if (_leds > 7)
      {
        _low = 8;
        _high = _leds - 8;
      }
      else
      {
        _low = _leds;
        _high = 0;
      }
      Display(0, j - 1, _low);
      Display(1, j - 1, _high);
    }
    if (command == SPECTRUMINVERSE)
    {
      for (int i=0; i<32; i++)
      {
        _row0[i] = ~_row0[i];
        _row1[i] = ~_row1[i];
      }
    }
    DoRender();
    recv[0] = 0;
  }
  else if (command == LEVELS)
  {
    _low = map((byte)recv[1], 0, 254, 0, 32);
    _high = map((byte)recv[2], 0, 254, 0, 32);
    for (int i = 0; i < 32; i++)
    {
      if (i < _low) _row0[i] = 0x18;
      else _row0[i] = 0;
      if (i < _high) _row1[i] = 0x18;
      else _row1[i] = 0;
    }
    DoRender();
    recv[0] = 0;
  }
  else if (command == TIME)
  {
    SetTime();
    recv[0] = 0;
  }
  else
  {
    DoTime();
    SetLevel();
  }
}
