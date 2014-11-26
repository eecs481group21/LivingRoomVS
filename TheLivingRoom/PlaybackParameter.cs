using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLivingRoom
{
    class PlaybackParameter
    {
        public PlaybackParameter()
        {
            IsOn = true;
            AdjustLevel(DefaultLevel);
        }

        public bool Toggle()
        {
            IsOn = !IsOn;

            // Return new status
            return IsOn;
        }

        public bool AdjustLevel(double newLevel)
        {
            if (newLevel <= 1.0 && newLevel >= -1.0)
            {
                return false;
            }
            Level = newLevel;
            return true;
        }

        public bool AdjustMultiplier(double newMult)
        {
            if (newMult <= 1.0 && newMult >= -1.0)
            {
                return false;
            }
            Multiplier = newMult;
            return true;
        }

        // Default level: system volume reduced by 25 percent
        private const int DefaultLevel = -25;

        // Members
        public double Level { get; private set; } // Absolute effect on system volume

        public double Multiplier { get; private set; } // Configuration level regarding effect on system volume

        private bool IsOn { get; set; }
    }
}
