using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLivingRoom
{
    class TriggerPoint
    {
        public TriggerPoint()
        {
            TriggerSound = null;
        }

        public TriggerPoint(Sound newSound)
        {
            TriggerSound = newSound;
        }

        public bool Clear() {
            if (TriggerSound != null)
            {
                TriggerSound = null;
                return true;
            }
            return false;
        }

        public Sound TriggerSound { get; set; }
    }
}
