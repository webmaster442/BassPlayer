using System;
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
        /// <summary>
        /// Preset name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Preset Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Output Extension
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Preset command line
        /// </summary>
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
            this.Add(new Preset
            {
                Name = "FLAC",
                Description = "Convert Audio to Free Losless Audio Codec (FLAC) format",
                CommandLine = "ffmpeg -i {input} -vn -acodec flac -aq {slider text=\"compression:\" min=\"1\" max=\"8\" step=\"1\" val=\"3\"} {output}", 
                Extension = "flac"
            });
            this.Add(new Preset
            {
                Name = "MP3 CBR",
                Description = "Mp3 format with Constant bitrate",
                CommandLine = "ffmpeg -i {input} -acodec mp3 -b:a {slider text=\"Bitrate\" min=\"8\" stops=\"8;16;32;40;48;56;64;80;96;112;128;160;192;224;256;320\" val=\"192\"} {output}",
                Extension = "mp3"
            });
            this.Add(new Preset
            {
                Name = "MP3 VBR",
                Description = "Mp3 format with Variable bitrate",
                CommandLine = "ffmpeg -i {input} -acodec mp3 -q:a {slider text=\"Quality\" min=\"1\" max=\"9\" val=\"7\" step=\"1\"} {output}",
                Extension = "mp3"
            });
        }
    }
}
