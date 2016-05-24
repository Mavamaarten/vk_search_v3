using System;
using System.Threading;
using System.Timers;
using System.Web.UI;
using System.Windows;
using Un4seen.Bass;
using Timer = System.Timers.Timer;

namespace vk_search_v3.Playback
{
    class Mp3Player
    {
        private int streamHandle;
        private SYNCPROC playbackStoppedProc;
        private readonly Timer tmrUpdatePosition;

        public event EventHandler OnPlaybackEnded;
        public event EventHandler<Exception> OnException;
        public event EventHandler<Tuple<long, long>> OnPlaybackPositionUpdated;

        public Mp3Player()
        {
            tmrUpdatePosition = new Timer(1000);
            tmrUpdatePosition.Elapsed += TmrUpdatePosition_Elapsed;

            if (!Bass.BASS_Init(-1, 44100, 0, IntPtr.Zero))
            {
                OnException?.Invoke(this, new Exception("Failed to initialize audio device."));
            }
        }

        private void TmrUpdatePosition_Elapsed(object sender, ElapsedEventArgs e)
        {
            long len = Bass.BASS_ChannelGetLength(streamHandle);
            long pos = Bass.BASS_ChannelGetPosition(streamHandle);
            double totaltime = Bass.BASS_ChannelBytes2Seconds(streamHandle, len);
            long elapsedtime = (long) Bass.BASS_ChannelBytes2Seconds(streamHandle, pos);
            long remainingtime = (long) (totaltime - elapsedtime);

            OnPlaybackPositionUpdated?.Invoke(this, new Tuple<long, long>(elapsedtime, remainingtime));
        }

        public void PlayUrl(string url)
        {
            new Thread(() =>
            {
                Bass.BASS_ChannelStop(streamHandle);
                playbackStoppedProc = OnPlaybackStopped;
                streamHandle = Bass.BASS_StreamCreateURL(url, (int)BASSFlag.BASS_DEFAULT, (int)BASSFlag.BASS_DEFAULT, null, IntPtr.Zero);

                if (streamHandle == 0)
                {
                    OnException?.Invoke(this, new Exception(Bass.BASS_ErrorGetCode().ToString()));
                    return;
                }

                Bass.BASS_ChannelSetSync(streamHandle, BASSSync.BASS_SYNC_END, 1, playbackStoppedProc, IntPtr.Zero);
                Bass.BASS_ChannelPlay(streamHandle, false);
                tmrUpdatePosition.Start();
            }).Start();
        }

        public void Pause()
        {
            if (Bass.BASS_ChannelIsActive(streamHandle) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                Bass.BASS_ChannelPause(streamHandle);
                tmrUpdatePosition.Stop();
            }
        }

        public void Play()
        {
            if (Bass.BASS_ChannelIsActive(streamHandle) == BASSActive.BASS_ACTIVE_PAUSED)
            {
                Bass.BASS_ChannelPlay(streamHandle, false);
                tmrUpdatePosition.Start();
            }
        }

        private void OnPlaybackStopped(int handle, int channel, int data, IntPtr user)
        {
            OnPlaybackEnded?.Invoke(this, null);
            tmrUpdatePosition.Stop();
        }
    }
}
