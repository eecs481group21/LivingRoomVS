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
            AdjustMultiplier(DefaultMultiplier);
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
                Level = newLevel;
                return true;
            }

            if (newLevel < -1.00)
            {
                Level = -1.0;
                return true;
            }
            
            return false;            
        }

        public bool AdjustMultiplier(double newMult)
        {
            if (newMult <= 1.0 && newMult >= 0.0)
            {
                Multiplier = newMult;
                return true;
            }

            return false;
        }

        // Default level: system volume reduced by 25 percent
        private const double DefaultLevel = -0.25;
        private const double DefaultMultiplier = 1.0;

        // Members
        // Absolute effect on system volume [-1.0, 1.0]
        public double Level { get; private set; }

        // Configuration level regarding effect on system volume [0.0, 1.0]
        public double Multiplier { get; private set; } 

        private bool IsOn { get; set; }
    }
}
