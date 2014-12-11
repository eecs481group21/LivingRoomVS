using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
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
            
            _instance = new PlaybackEngine();
            return _instance;
        }

        private static PlaybackEngine _instance;

        // Private constructor
        private PlaybackEngine()
        {
            // Initialize members
            Parameters = new List<PlaybackParameter>();
            SystemVolumeLimit = 1.0;

            CreateDefaultParameters();
        }

        private void CreateDefaultParameters()
        {
            PlaybackParameter distanceVolume = new PlaybackParameter("Distance");
            Parameters.Add(distanceVolume);
            PlaybackParameter handTouch = new PlaybackParameter("Touching Hands");
            Parameters.Add(handTouch);
        }

        // Interface
        public void PlaySound(Sound sound)
        {
            // Seek to beginning of sound
            sound.ResetToBeginning();

            // Calculate current volume according to parameters and system volume limit
            double vol = 1.0;

            Debug.WriteLine("Adjusted volume level: " + vol);

            sound.AdjustVolume(vol);

            // Play Sound
            sound.Play();
        }

        public void PlaySoundPreview(Sound sound)
        {
            // Play Sound at full volume
            sound.ResetToBeginning();
            sound.AdjustVolume(SystemVolumeLimit);
            sound.PlayPreview();
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
            if (newVolume >= 0.0 && newVolume <= 1.0)
            {
                SystemVolumeLimit = newVolume;
                return true;
            }
            return false;
        }

        private double CalculatePlaybackVolume()
        {
            if (SystemVolumeLimit == 0.0)
            {
                return SystemVolumeLimit;
            }

            // Assume default, no parameter volume level is 3/ volume limit
            double playbackVolume = SystemVolumeLimit * 0.5;

            // Set maximum volume adjustment factor per parameter such that
            // if all paramaters were set to 50 system would play at full limit
            // and if all parameters were set to -50 volume would be half limit.
            double maxAdjustFactorPerParameter = (SystemVolumeLimit * .5) / NumberParametersOn();
            
            FetchLatestParameterValues();

            // Adjust playbackVolume according to parameters
            foreach (PlaybackParameter param in Parameters)
            {
                if (param.IsOn)
                {
                    playbackVolume += maxAdjustFactorPerParameter * param.Level * param.Multiplier;
                    Debug.WriteLine("Adding: " + maxAdjustFactorPerParameter + " * " + param.Level + " * " + param.Multiplier);
                }
            }

            return playbackVolume;
        }

        private int NumberParametersOn()
        {
            int count = 0;
            foreach (PlaybackParameter param in Parameters) {
                if (param.IsOn) {
                    ++count;
                }
            }
            return count;
        }

        private double GetDistanceMultiplier(double distance)
        {
            return (ZeroedDistance - distance) / ZeroedDistance;
        }

        private void FetchLatestParameterValues()
        {
            // _parameters[0] is distance
            string distance = Client.GetInstance().HttpGetAsync("/api/kinect/distance");
            if (distance != null)
            {
                double distanceCast = Convert.ToDouble(distance);
                Parameters[0].AdjustLevel(GetDistanceMultiplier(distanceCast));
                Debug.WriteLine("Distance multiplyer: " + GetDistanceMultiplier(distanceCast));
            }

            // _parameters[1] is hand touch
            string handContact = Client.GetInstance().HttpGetAsync("/api/kinect/contact");
            if (handContact != null)
            {
                bool handContactCast = Convert.ToBoolean((string)handContact);
                Parameters[1].AdjustLevel(handContactCast ? 0.5 : -0.5);
                Debug.WriteLine("Hand Multiplyer: " + (handContactCast ? 0.5 : -0.5));
            }
            
        }

        // Members

        public List<PlaybackParameter> Parameters { get; private set; }

        public double SystemVolumeLimit { get; private set; } // upper limit of playback volume

        private const double ZeroedDistance = 2.0; // Feet of 'normal separation' apart
    }
}
