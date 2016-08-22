using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocals.InternalClasses
{
    public static class Utils
    {
        public static void PlaySound (string soundPath)
        {
            if (soundPath.IndexOf(".wav") == soundPath.Length - 4)
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                player.SoundLocation = soundPath;
                player.Play();
            }
            else if (soundPath.IndexOf(".mp3") == soundPath.Length - 4)
            {
                WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();

                wplayer.URL = soundPath;
                wplayer.controls.play();
            }
        }
    }
}
