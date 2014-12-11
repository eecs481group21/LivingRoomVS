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
            setID();
        }

        public TriggerPoint(Sound newSound)
        {
            TriggerSound = newSound;
            setID();
        }

        public void Set(Sound newSound) {
            TriggerSound = newSound;
        }

        public bool IsSet()
        {
            return (TriggerSound != null);
        }

        public bool Clear() {
            if (TriggerSound == null) return false;
            TriggerSound = null;
            return true;
        }

        private void setID()
        {
            ID = GetHashCode().ToString();
        }

        public Sound TriggerSound { get; set; }

        public String ID { get; private set; }
    }
}
