using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLivingRoom
{
    class SoundPack
    {
        public SoundPack(string name)
        {
            Name = name;
            Sounds = new List<Sound>();
        }

        public bool AddSound(Sound newSound)
        {
            if (newSound != null)
            {
                // Add song to SoundPack
                Sounds.Add(newSound);
            }
            return false;
        }

        public void Clear()
        {
            Sounds.Clear();
        }

        // Members
        public string Name { get; private set; }

        public List<Sound> Sounds { get; private set; }
    }
}
