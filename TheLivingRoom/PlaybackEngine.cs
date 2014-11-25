﻿using System;
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
            
            _instance = new PlaybackEngine();
            return _instance;
        }

        private static PlaybackEngine _instance;

        // Private constructor
        private PlaybackEngine()
        {
            // Initialize members
            _parameters = new List<PlaybackParameter>();
            _systemVolumeLimit = 0.7;

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
            sound.AdjustVolume(_systemVolumeLimit);
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
            playbackVolume += _parameters
                // Parameter adjustment factor is the ratio multiplied by the max factor per parameter
                .Select(param => param.Level/50.0)
                // Note that this also accounts for negative adjustments properly
                .Select(paramAdjustmentRatio => paramAdjustmentRatio * maxAdjustFactorPerParameter)
                // Adjust playback volume according to the level of this parameter
                .Sum();

            return playbackVolume;
        }

        // Members

        private List<PlaybackParameter> _parameters;

        private double _systemVolumeLimit; // upper limit of playback volume
    }
}
