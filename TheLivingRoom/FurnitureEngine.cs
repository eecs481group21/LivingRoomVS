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
            else
            {
                _instance = new FurnitureEngine();
                return _instance;
            }
        }

        private static FurnitureEngine _instance { get; set; }

        // Private constructor
        private FurnitureEngine()
        {
            _furniture = new List<Furniture>();
            _triggers = new Dictionary<Windows.System.VirtualKey, TriggerPoint>();
            _soundPacks = new List<SoundPack>();
            _currentSoundPackIndex = -1;

            InitBetaDemo();
        }

        public void HandleTrigger(char input)
        {
            // This should be called upon some event in the application
            // Handle playback with PlayBackEngine
        }

        public void AddTrigger(Windows.System.VirtualKey key, TriggerPoint triggerPoint)
        {
            if (triggerPoint != null)
            {
                _triggers.Add(key, triggerPoint);
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
            piano.SetSource("ms-appx:///Assets/SoundPacks/Default/Audio/piano.mp3");
            Sound violin = new Sound("Violin");
            violin.SetSource("ms-appx:///Assets/SoundPacks/Default/Audio/violin.mp3");
            Sound trumpet = new Sound("Trumpet");
            trumpet.SetSource("ms-appx:///Assets/SoundPacks/Default/Audio/trumpet.mp3");
            Sound guitar = new Sound("Guitar");
            guitar.SetSource("ms-appx:///Assets/SoundPacks/Default/Audio/guitar.mp3");
            Sound drums = new Sound("Drums");
            drums.SetSource("ms-appx:///Assets/SoundPacks/Default/Audio/drums.mp3");
            Sound cello = new Sound("Cello");
            cello.SetSource("ms-appx:///Assets/SoundPacks/Default/Audio/cello.mp3");
            Sound xylophone = new Sound("Xylophone");
            xylophone.SetSource("ms-appx:///Assets/SoundPacks/Default/Audio/xylophone.mp3");
            Sound sax = new Sound("Saxophone");
            sax.SetSource("ms-appx:///Assets/SoundPacks/Default/Audio/sax.mp3");
            Sound flute = new Sound("Flute");
            flute.SetSource("ms-appx:///Assets/SoundPacks/Default/Audio/flute.mp3");

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
            Furniture chair = new Furniture("Chair", 5.0, 5.0);
            TriggerPoint chairSeat = new TriggerPoint();
            chair.AddTriggerPoint(2.5, 2.5, chairSeat);
            _furniture.Add(chair);            
        }

        // Members
        private List<Furniture> _furniture;

        private Dictionary<Windows.System.VirtualKey, TriggerPoint> _triggers;

        private List<SoundPack> _soundPacks;

        private int _currentSoundPackIndex;
    }
}
