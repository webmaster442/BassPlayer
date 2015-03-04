﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFConverter
{
    [Serializable]
    public class Preset
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Extension { get; set; }
        public string CommandLine { get; set; }
    }

    internal interface IPresetControl
    {
        string InputPattern { get; set; }
        string GeneratePattern();
        void SetupFromTokens(Dictionary<string, string> Tokens);
    }

    internal class PresetManager: ObservableCollection<Preset>
    {
        public PresetManager() : base()
        {
            this.Add(new Preset
            {
                Name = "Wav",
                Description = "Converts input file(s) to wav, as it is.\r\nNo additional processing is involved",
                CommandLine = "ffmpeg.exe -i {input} {output} {extension}",
                Extension = "wav"
            });
            this.Add(new Preset 
            {
                Name = "Wav 16 bit, 2ch, 44100Khz",
                Description = "Converts input file(s) to CD Audio compatible wave\r\n 1 sec =  172.26 KiB",
                CommandLine = "ffmpeg.exe -i {input} -ac 2 -ar 44100 -sample_fmt s16 {output}",
                Extension = "wav"
            });
            this.Add(new Preset
            {
                Name = "Extract Audio track from video",
                Description = "Extracts Audio track from video to wav file  to CD Audio compatible wave",
                CommandLine = "ffmpeg -i {input} -vn -ac 2 -ar 44100 -sample_fmt s16 {output}",
                Extension = "wav"
            });
            this.Add(new Preset
            {
                Name = "Extract Audio track from MP4",
                Description = "Extracts Audio track from mp4 video to an m4a file.",
                CommandLine = "ffmpeg -i {input} -vn -acodec copy {output}",
                Extension = "m4a"
            });
        }
    }
}