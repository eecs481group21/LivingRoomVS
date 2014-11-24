using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace TheLivingRoom
{
    class PlaybackEngine
    {
        // Get instance of Singleton
        public static PlaybackEngine GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                _instance = new PlaybackEngine();
                return _instance;
            }
        }

        private static PlaybackEngine _instance { get; set; }

        // Private constructor
        private PlaybackEngine()
        {
            // Initialize members
            _parameters = new List<PlaybackParameter>();
            _systemVolumeLimit = 0.1;

            CreateDefaultParameters();
        }

        private void CreateDefaultParameters()
        {
            PlaybackParameter distanceVolume = new PlaybackParameter();
            _parameters.Add(distanceVolume);
        }

        // Interface
        public void PlaySound(Sound sound)
        {
            // Seek to beginning of sound
            sound.Sample.Position = new TimeSpan(0, 0, 0);
            sound.Sample.AutoPlay = true;

            // Calculate current volume according to parameters and system volume limit
            sound.Sample.Volume = CalculatePlaybackVolume();

            // Play sample
            if (sound.Sample.CurrentState == MediaElementState.Playing)
            {
                // TODO: Allow for either same Sound to be triggered overlapping or restart immediately.
                sound.Sample.Position = new TimeSpan(0, 0, 0);
            }

            sound.Sample.Play();
        }

        public void PlaySoundPreview(Sound sound)
        {
            // TODO: actually implement
            PlaySound(sound);
        }

        public void StopPlayback()
        {
            // TODO: implement
        }

        public bool AdjustParameter(PlaybackParameter parameter, int newValue)
        {
            return parameter.AdjustLevel(newValue);
        }

        public bool ToggleParameter(PlaybackParameter parameter)
        {
            return parameter.Toggle();
        }

        public bool SetVolumeLimit(double newVolume)
        {
            if (newVolume > 0.0 && newVolume < 1.0)
            {
                _systemVolumeLimit = newVolume;
                return true;
            }
            return false;
        }

        private double CalculatePlaybackVolume()
        {
            if (_systemVolumeLimit == 0.0)
            {
                return _systemVolumeLimit;
            }

            // Assume default, no parameter volume level is half max volume
            double playbackVolume = _systemVolumeLimit / 2;

            // Set maximum volume adjustment factor per parameter such that
            // if all paramaters were set to 50 system would play at full limit
            // and if all parameters were set to -50 volume would be zero.
            double maxAdjustFactorPerParameter = playbackVolume / _parameters.Count;

            // Adjust playbackVolume according to parameters
            foreach (PlaybackParameter param in _parameters)
            {
                // Parameter adjustment ratio is its level relative to max level of 50
                double paramAdjustmentRatio = param.Level / 50.0;

                // Parameter adjustment factor is the ratio multiplied by the max factor per parameter
                // Note that this also accounts for negative adjustments properly
                double paramAdjustmentFactor = paramAdjustmentRatio * maxAdjustFactorPerParameter;

                // Adjust playback volume according to the level of this parameter
                playbackVolume += paramAdjustmentFactor;
            }
            
            return playbackVolume;
        }

        // Members
        private List<PlaybackParameter> _parameters { get; set; }

        private double _systemVolumeLimit { get; set; } // upper limit of playback volume
    }
}
