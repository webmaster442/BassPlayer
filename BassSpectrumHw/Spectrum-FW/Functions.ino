/*==================================================================
 Functions
 ===================================================================*/
__inline void WelcomeAnimation()
{
  strcpy(_textbuff,"Audio Spectrum");
  int j=0;
  int wd = HT1632.getTextWidth(_textbuff, FONT_8X4_END, FONT_8X4_HEIGHT);
  
  for (int i=0; i<((strlen(_textbuff)-1)*7)+3; i++)
  {
    HT1632.renderTarget(0);
    HT1632.clear();
    HT1632.drawText(_textbuff, OUT_SIZE - j, 0, FONT_8X4, FONT_8X4_END, FONT_8X4_HEIGHT);
    HT1632.render();
    j = (i+1)%(wd + OUT_SIZE);
    
    HT1632.renderTarget(1);
    HT1632.clear();
    if(i% 8 == 0)
    {
      HT1632.drawImage(IMG_SPEAKER_A, IMG_SPEAKER_WIDTH,  IMG_SPEAKER_HEIGHT, 0, 0);
      HT1632.drawImage(IMG_MUSICNOTE, IMG_MUSICNOTE_WIDTH,  IMG_MUSICNOTE_HEIGHT, 8, 0);
      HT1632.drawImage(IMG_MUSIC, IMG_MUSIC_WIDTH,  IMG_MUSIC_HEIGHT, 13, 1);
      HT1632.drawImage(IMG_MUSICNOTE, IMG_MUSICNOTE_WIDTH,  IMG_MUSICNOTE_HEIGHT, 23, 0);
      HT1632.drawImage(IMG_MUSICNOTE, IMG_MUSICNOTE_WIDTH,  IMG_MUSICNOTE_HEIGHT, 28, 1);
    } 
    else
    {
      HT1632.drawImage(IMG_SPEAKER_B, IMG_SPEAKER_WIDTH,  IMG_SPEAKER_HEIGHT, 0, 0);
      HT1632.drawImage(IMG_MUSICNOTE, IMG_MUSICNOTE_WIDTH,  IMG_MUSICNOTE_HEIGHT, 8, 1);
      HT1632.drawImage(IMG_MUSIC, IMG_MUSIC_WIDTH,  IMG_MUSIC_HEIGHT, 13, 0);
      HT1632.drawImage(IMG_MUSICNOTE, IMG_MUSICNOTE_WIDTH,  IMG_MUSICNOTE_HEIGHT, 23, 1);
      HT1632.drawImage(IMG_MUSICNOTE, IMG_MUSICNOTE_WIDTH,  IMG_MUSICNOTE_HEIGHT, 28, 0);
    }
    HT1632.render();
    delay(15);
  }
  HT1632.renderTarget(1);
  HT1632.clear();
  HT1632.render();
}

__inline void Display(int target, int row, byte leds)
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
  HT1632.renderTarget(1);
  HT1632.SetRamBuffer(_row0, 31);
  //for (int i=0; i<32; i++) HT1632.setRam(i, displays[0][i]);
  HT1632.render();
  HT1632.renderTarget(0);
  HT1632.SetRamBuffer(_row1, 31);
  //for (int i=0; i<32; i++) HT1632.setRam(i, displays[1][i]);
  HT1632.render();
}
__inline float Temperature()
{
  float temp;
  int raw = analogRead(THERMISTOR);
  long resistance = PADRESISTOR * ((1024.0 / raw) - 1);
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
  HT1632.drawText(_textbuff, 0, 0, FONT_8X4, FONT_8X4_END, FONT_8X4_HEIGHT);
  HT1632.render();
  
  char temp[] = {0};
  floatToString(temp, Temperature(), 2);
  sprintf(_textbuff, " %s C", temp);
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

__inline void FlushBuffer()
{
  while (Serial.available() > 0) Serial.read();
}


