using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLivingRoom
{
    class FurnitureEngine
    {
        // Get instance of Singleton
        public static FurnitureEngine GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            
            _instance = new FurnitureEngine();
            return _instance;
        }

        private static FurnitureEngine _instance;

        // Private constructor
        private FurnitureEngine()
        {
            _furniture = new List<Furniture>();
            _triggers = new Dictionary<Windows.System.VirtualKey, TriggerPoint>();
            _soundPacks = new List<SoundPack>();
            _currentSoundPackIndex = -1;

            InitBetaDemo();
        }

        public void HandleTrigger(Windows.System.VirtualKey keyEvent)
        {
            // See if keyEvent matches any triggers
            foreach (KeyValuePair<Windows.System.VirtualKey, TriggerPoint> pair in _triggers)
            {
                // If event matches the trigger and the trigger has been assigned a Sound
                if (pair.Key == keyEvent && pair.Value.IsSet())
                {
                    // Play the sound according to playback parameters
                    PlaybackEngine.GetInstance().PlaySound(pair.Value.TriggerSound);
                    break;
                }
            }
        }

        public SoundPack GetSoundPack()
        {
            if (_currentSoundPackIndex >= 0 && _currentSoundPackIndex < _soundPacks.Count) 
            {
                return _soundPacks[_currentSoundPackIndex];
            }

            return null;
        }

        public List<Furniture> GetFurnitureItems()
        {
            return _furniture.Count > 0 ? _furniture : null;
        }

        public Furniture GetFurnitureAtIndex(int index)
        {
            if (index < _furniture.Count)
            {
                return _furniture[index];
            }
            return null;
        }

        public bool ChangeSoundPack(int newIndex)
        {
            if (newIndex < _soundPacks.Count)
            {
                _currentSoundPackIndex = newIndex;
                return true;
            }
            return false;
        }

        private void InitBetaDemo()
        {
            // Create Default SoundPack
            CreateDefaultSoundPack();
            _currentSoundPackIndex = 0;

            // Create chair with one TriggerPoint
            CreateChair();
        }

        private void CreateDefaultSoundPack()
        {
            // Create Sounds associated with Default SoundPack
            Sound piano = new Sound("Piano");
            piano.InitWithSource("ms-appx:///Assets/SoundPacks/Default/Audio/piano.mp3");
            Sound violin = new Sound("Violin");
            violin.InitWithSource("ms-appx:///Assets/SoundPacks/Default/Audio/violin.mp3");
            Sound trumpet = new Sound("Trumpet");
            trumpet.InitWithSource("ms-appx:///Assets/SoundPacks/Default/Audio/trumpet.mp3");
            Sound guitar = new Sound("Guitar");
            guitar.InitWithSource("ms-appx:///Assets/SoundPacks/Default/Audio/guitar.mp3");
            Sound drums = new Sound("Drums");
            drums.InitWithSource("ms-appx:///Assets/SoundPacks/Default/Audio/drums.mp3");
            Sound cello = new Sound("Cello");
            cello.InitWithSource("ms-appx:///Assets/SoundPacks/Default/Audio/cello.mp3");
            Sound xylophone = new Sound("Xylophone");
            xylophone.InitWithSource("ms-appx:///Assets/SoundPacks/Default/Audio/xylophone.mp3");
            Sound sax = new Sound("Saxophone");
            sax.InitWithSource("ms-appx:///Assets/SoundPacks/Default/Audio/sax.mp3");
            Sound flute = new Sound("Flute");
            flute.InitWithSource("ms-appx:///Assets/SoundPacks/Default/Audio/flute.mp3");

            // Add to Default
            SoundPack defaultPack = new SoundPack("Default");
            defaultPack.AddSound(piano);
            defaultPack.AddSound(violin);
            defaultPack.AddSound(trumpet);
            defaultPack.AddSound(guitar);
            defaultPack.AddSound(drums);
            defaultPack.AddSound(cello);
            defaultPack.AddSound(xylophone);
            defaultPack.AddSound(sax);
            defaultPack.AddSound(flute);

            // Add to list of SoundPacks
            _soundPacks.Add(defaultPack);
        }

        private void CreateChair()
        {
            // Create a chair with one TriggerPoint
            Furniture chair = new Furniture("Chair");
            TriggerPoint chairSeat = new TriggerPoint();
            chair.AddTriggerPoint(chairSeat);
            _furniture.Add(chair);
      
            // TriggerPoint Key must be hard-coded b/c corresponding key
            // is sent by Arduino application which is independent of this app
            AddTrigger(Windows.System.VirtualKey.A, chairSeat);
        }

        private void AddTrigger(Windows.System.VirtualKey key, TriggerPoint triggerPoint)
        {
            if (triggerPoint != null)
            {
                _triggers.Add(key, triggerPoint);
            }
        }

        // Members
        private readonly List<Furniture> _furniture;

        private readonly Dictionary<Windows.System.VirtualKey, TriggerPoint> _triggers;

        private readonly List<SoundPack> _soundPacks;

        private int _currentSoundPackIndex;
    }
}
