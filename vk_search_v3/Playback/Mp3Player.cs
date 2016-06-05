using System;
using System.Threading;
using System.Timers;
using Un4seen.Bass;
using Timer = System.Timers.Timer;

namespace vk_search_v3.Playback
{
    public class Mp3Player
    {
        private int streamHandle;
        private bool playbackStarting;
        private readonly Timer tmrUpdatePosition;

        public event EventHandler OnPlaybackEnded;
        public event EventHandler<Exception> OnException;
        public event EventHandler<Tuple<long, long>> OnPlaybackPositionUpdated;
        public event EventHandler<PlaybackStates> OnPlaybackStateChanged;

        public enum PlaybackStates
        {
            STOPPED,
            PLAYING,
            PAUSED,
            STALLED
        }

        public PlaybackStates PlaybackState { get; set; }

        public Mp3Player()
        {
            PlaybackState = PlaybackStates.STOPPED;

            tmrUpdatePosition = new Timer(1000);
            tmrUpdatePosition.Elapsed += TmrUpdatePosition_Elapsed;

            if (!Bass.BASS_Init(-1, 44100, 0, IntPtr.Zero))
            {
                OnException?.Invoke(this, new Exception("Failed to initialize audio device."));
            }
        }

        private void TmrUpdatePosition_Elapsed(object sender, ElapsedEventArgs e)
        {
            var channelLength = Bass.BASS_ChannelGetLength(streamHandle);
            var channelPosition = Bass.BASS_ChannelGetPosition(streamHandle);
            var channelLengthSeconds = Bass.BASS_ChannelBytes2Seconds(streamHandle, channelLength);
            var elapsedtimeSeconds = (long) Bass.BASS_ChannelBytes2Seconds(streamHandle, channelPosition);
            var remainingtime = (long) (channelLengthSeconds - elapsedtimeSeconds);

            OnPlaybackPositionUpdated?.Invoke(this, new Tuple<long, long>(elapsedtimeSeconds, remainingtime));
        }

        public void PlayUrl(string url)
        {
            if (playbackStarting) return;

            playbackStarting = true;
            Bass.BASS_ChannelStop(streamHandle);

            new Thread(() =>
            {
                streamHandle = Bass.BASS_StreamCreateURL(url, (int)BASSFlag.BASS_DEFAULT, (int)BASSFlag.BASS_DEFAULT, null, IntPtr.Zero);

                if (streamHandle == 0)
                {
                    OnException?.Invoke(this, new Exception(Bass.BASS_ErrorGetCode().ToString()));
                    return;
                }

                Bass.BASS_ChannelSetSync(streamHandle, BASSSync.BASS_SYNC_END, 1, OnPlaybackStopped, IntPtr.Zero);
                Bass.BASS_ChannelSetSync(streamHandle, BASSSync.BASS_SYNC_STALL, 1, OnPlaybackStalled, IntPtr.Zero);
                Bass.BASS_ChannelPlay(streamHandle, false);
                tmrUpdatePosition.Start();

                playbackStarting = false;
            }).Start();
        }

        private void OnPlaybackStalled(int handle, int channel, int data, IntPtr user)
        {
            PlaybackState = PlaybackStates.STALLED;
            OnPlaybackStateChanged?.Invoke(this, PlaybackStates.STALLED);
        }

        public void Pause()
        {
            if (Bass.BASS_ChannelIsActive(streamHandle) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                Bass.BASS_ChannelPause(streamHandle);
                tmrUpdatePosition.Stop();
                PlaybackState = PlaybackStates.PAUSED;
                OnPlaybackStateChanged?.Invoke(this, PlaybackStates.PAUSED);
            }
        }

        public void Play()
        {
            if (Bass.BASS_ChannelIsActive(streamHandle) == BASSActive.BASS_ACTIVE_PAUSED)
            {
                Bass.BASS_ChannelPlay(streamHandle, false);
                tmrUpdatePosition.Start();
                PlaybackState = PlaybackStates.PLAYING;
                OnPlaybackStateChanged?.Invoke(this, PlaybackStates.PLAYING);
            }
        }

        private void OnPlaybackStopped(int handle, int channel, int data, IntPtr user)
        {
            OnPlaybackEnded?.Invoke(this, null);
            tmrUpdatePosition.Stop();
            PlaybackState = PlaybackStates.STOPPED;
            OnPlaybackStateChanged?.Invoke(this, PlaybackStates.STOPPED);
        }

        public void PlayPause()
        {
            switch (Bass.BASS_ChannelIsActive(streamHandle))
            {
                case BASSActive.BASS_ACTIVE_STOPPED:
                case BASSActive.BASS_ACTIVE_PAUSED:
                case BASSActive.BASS_ACTIVE_STALLED:
                    if (streamHandle != 0)
                    {
                        Bass.BASS_ChannelPlay(streamHandle, false);
                        PlaybackState = PlaybackStates.PLAYING;
                        OnPlaybackStateChanged?.Invoke(this, PlaybackStates.PLAYING);
                    }
                    break;

                case BASSActive.BASS_ACTIVE_PLAYING:
                    Bass.BASS_ChannelPause(streamHandle);
                    PlaybackState = PlaybackStates.PAUSED;
                    OnPlaybackStateChanged?.Invoke(this, PlaybackStates.PAUSED);
                    break;
            }
        }
    }
}
