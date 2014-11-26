﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            
            _instance = new PlaybackEngine();
            return _instance;
        }

        private static PlaybackEngine _instance;

        // Private constructor
        private PlaybackEngine()
        {
            // Initialize members
            _parameters = new List<PlaybackParameter>();
            SystemVolumeLimit = 1.0;

            CreateDefaultParameters();
        }

        private void CreateDefaultParameters()
        {
            PlaybackParameter distanceVolume = new PlaybackParameter();
            _parameters.Add(distanceVolume);
            PlaybackParameter handTouch = new PlaybackParameter();
            _parameters.Add(handTouch);
        }

        // Interface
        public void PlaySound(Sound sound)
        {
            // Seek to beginning of sound
            sound.ResetToBeginning();

            // Calculate current volume according to parameters and system volume limit
            sound.AdjustVolume(CalculatePlaybackVolume());

            // TODO: Figure out way to play same Sound overlapping (use case: fast input)

            // Play Sound
            sound.Play();
        }

        public void PlaySoundPreview(Sound sound)
        {
            // Play Sound at full volume
            sound.ResetToBeginning();
            sound.AdjustVolume(SystemVolumeLimit);
            sound.Play();
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
            double playbackVolume = SystemVolumeLimit * 0.75;

            // Set maximum volume adjustment factor per parameter such that
            // if all paramaters were set to 50 system would play at full limit
            // and if all parameters were set to -50 volume would be half limit.
            double maxAdjustFactorPerParameter = (SystemVolumeLimit * .25) / _parameters.Count;

            FetchLatestParameterValues();

            // Adjust playbackVolume according to parameters
            foreach (PlaybackParameter param in _parameters)
            {
                playbackVolume += maxAdjustFactorPerParameter * param.Level * param.Multiplier;
            }

            return playbackVolume;
        }

        private double GetDistanceMultiplier(double distance)
        {
            return (distance - ZeroedDistance) / ZeroedDistance;
        }

        private async void FetchLatestParameterValues()
        {
            // _parameters[0] is distance
            string distance = await Client.GetInstance().HttpGetAsync("/api/kinect/distance");
            if (distance != null)
            {
                double distanceCast = Convert.ToDouble(distance);
                _parameters[0].AdjustLevel(GetDistanceMultiplier(distanceCast));
                Debug.WriteLine("Distance multiplyer: " + GetDistanceMultiplier(distanceCast));
            }

            // _parameters[1] is hand touch
            string handContact = await Client.GetInstance().HttpGetAsync("/api/kinect/contact");
            if (handContact != null)
            {
                bool handContactCast = Convert.ToBoolean((string)handContact);
                _parameters[1].AdjustLevel(handContactCast ? 0.5 : -0.5);
                Debug.WriteLine("Hand Multiplyer: " + (handContactCast ? 0.5 : -0.5));
            }
            
        }

        // Members

        private List<PlaybackParameter> _parameters;

        public double SystemVolumeLimit { get; private set; } // upper limit of playback volume

        private const double ZeroedDistance = 6.0; // Feet of 'normal separation' apart
    }
}
