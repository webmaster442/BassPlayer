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
int _counter = 0;
byte _row0[32] = {
  0};
byte _row1[32] = {
  0};
byte *_displays[] = { 
  _row0, _row1 };
char _textbuff[32];
tmElements_t _tm;
int _len = 0;
int _freeze = 0;

#define THERMISTOR 0
#define PADRESISTOR 10000.0
#define LEVELPOT 1

void setup()
{
  //12 - lower panel CS
  //13 - high panel cs
  //10 - WR
  //9 - Data
  HT1632.begin(12, 13, 10, 9);
  Serial.begin(115200);
  SetLevel();
  WelcomeAnimation();
}

void loop()
{
  _len = Serial.available();
  if (_len >= 32)
  {
    _value = Serial.read();
    _freeze = 0;
    _leds = map(_value, 0, 255, 0, 16);
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
    Display(0, _counter, _low);
    Display(1, _counter, _high);
    _counter++;
    if (_counter > 31)
    {
      _counter = 0;
      DoRender();
    }
  }
  else if (_len < 1)
  {
    DoTime();
    SetLevel();
  }
  else
  {
    ++_freeze;
    if (_freeze > 2000)
    {
      _freeze = 0;
      FlushBuffer();
      HT1632.renderTarget(0);
      HT1632.clear();
      HT1632.render();
      HT1632.renderTarget(1);
      HT1632.clear();
      HT1632.render();
    }
    delay(1);
  }
}
