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

        public bool AdjustLevel(int newLevel)
        {
            if (newLevel < -50 || newLevel > 50)
            {
                return false;
            }
            Level = newLevel;
            return true;
        }

        // Default level: system volume reduced by 25 percent
        private const int DefaultLevel = -25;

        // Members
        public int Level { get; private set; }

        private bool IsOn { get; set; }
    }
}
