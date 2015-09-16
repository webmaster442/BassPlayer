/*==================================================================
 Functions
 ===================================================================*/
void Display(int target, int row, byte leds)
{
  HT1632.renderTarget(target);
  switch (leds)
  {
    case 0:
      _displays[target][row] = 0x00;
      break;
    case 1:
      _displays[target][row] = 0x01;
      break;
    case 2:
      _displays[target][row] = 0x03;
      break;
    case 3:
      _displays[target][row] = 0x07;
      break;
    case 4:
      _displays[target][row] = 0x0f;
      break;
    case 5:
      _displays[target][row] = 0x1f;
      break;
    case 6:
      _displays[target][row] = 0x3f;
      break;
    case 7:
      _displays[target][row] = 0x7f;
      break;
    case 8:
      _displays[target][row] = 0xff;
      break;
  }
}

__inline void DoRender()
{
  int adc = analogRead(LEVELPOT);
  int level = map(adc, 0, 1023, 1, 16);
  HT1632.renderTarget(1);
  HT1632.setBrightness(level);
  HT1632.SetRamBuffer(_row0, 32);
  HT1632.render();
  HT1632.renderTarget(0);
  HT1632.setBrightness(level);
  HT1632.SetRamBuffer(_row1, 32);
  HT1632.render();
}
__inline float Temperature()
{
  float temp;
  int raw = analogRead(THERMISTOR);
  long resistance = PADRESISTOR * ((1024.0 / (1024-raw)) - 1);
  Serial.println(raw);
  temp = log(resistance);
  temp = 1 / (0.001129148 + (0.000234125 * temp) + (0.0000000876741 * temp * temp * temp));
  temp = temp - 273.15;  // Convert Kelvin to Celsius
  return temp;
}

__inline void DoTime()
{
  RTC.read(_tm);
  sprintf(_textbuff, "%02d:%02d:%02d", _tm.Hour, _tm.Minute, _tm.Second);
  HT1632.renderTarget(0);
  HT1632.clear();
  HT1632.drawText(_textbuff, 0, 2, FONT_5X4, FONT_5X4_END, FONT_5X4_HEIGHT);
  HT1632.render();

  char temp[] = {0};
  floatToString(temp, Temperature(), 2);
  sprintf(_textbuff, "%s °C", temp);
  HT1632.renderTarget(1);
  HT1632.clear();
  HT1632.drawText(_textbuff, 0, 1, FONT_5X4, FONT_5X4_END, FONT_5X4_HEIGHT);
  HT1632.render();
  delay(100);
}

__inline void SetLevel()
{
  int adc = analogRead(LEVELPOT);
  int level = map(adc, 0, 1023, 1, 16);
  HT1632.renderTarget(0);
  HT1632.setBrightness(level);
  HT1632.renderTarget(1);
  HT1632.setBrightness(level);
}

void SetTime()
{
  tmElements_t time;
  time.Year = 2000 + recv[1];
  time.Month = recv[2];
  time.Day = recv[3];
  time.Hour = recv[4];
  time.Minute = recv[5];
  time.Second = recv[6];
  RTC.write(time);
}


