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
            SoundPacks = new List<SoundPack>();
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

        public SoundPack GetCurrentSoundPack()
        {
            if (_currentSoundPackIndex >= 0 && _currentSoundPackIndex < SoundPacks.Count) 
            {
                return SoundPacks[_currentSoundPackIndex];
            }

            return null;
        }

        public Sound GetSoundByName(String searchName)
        {
            foreach (Sound sound in SoundPacks[_currentSoundPackIndex].Sounds)
            {
                if (sound.Name.Equals(searchName))
                {
                    return sound;                  
                }
            }
            return null;
        }

        public TriggerPoint GetTriggerPointByID(String searchID)
        {
            foreach (TriggerPoint triggerPoint in _triggers.Values) {
                if (triggerPoint.ID.Equals(searchID))
                {
                    return triggerPoint;
                }
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
            if (newIndex < SoundPacks.Count)
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

            // Create couch with two TriggerPoints
            CreateCouch();
        }

        private void CreateDefaultSoundPack()
        {
            // Create Sounds associated with Default SoundPack
            Sound bells = new Sound("Bells");
            bells.InitWithSource("ms-appx:///Assets/SoundPacks/Garage/Audio/bells.mp3");
            Sound drums = new Sound("Drums");
            drums.InitWithSource("ms-appx:///Assets/SoundPacks/Garage/Audio/drums.mp3");
            Sound guitar = new Sound("Guitar");
            guitar.InitWithSource("ms-appx:///Assets/SoundPacks/Garage/Audio/guitar.mp3");
            Sound harmonica = new Sound("Harmonica");
            harmonica.InitWithSource("ms-appx:///Assets/SoundPacks/Garage/Audio/harmonica.mp3");
            Sound strings = new Sound("Strings");
            strings.InitWithSource("ms-appx:///Assets/SoundPacks/Garage/Audio/strings.mp3");
            Sound oboe = new Sound("Oboe");
            oboe.InitWithSource("ms-appx:///Assets/SoundPacks/Garage/Audio/oboe.mp3");
            Sound piano = new Sound("Piano");
            piano.InitWithSource("ms-appx:///Assets/SoundPacks/Garage/Audio/piano.mp3");
            Sound sax = new Sound("Saxophone");
            sax.InitWithSource("ms-appx:///Assets/SoundPacks/Garage/Audio/saxophone.mp3");
            Sound horns = new Sound("Horns");
            horns.InitWithSource("ms-appx:///Assets/SoundPacks/Garage/Audio/horns.mp3");

            // Add to Default
            SoundPack defaultPack = new SoundPack("Garage");
            defaultPack.AddSound(bells);
            defaultPack.AddSound(drums);
            defaultPack.AddSound(guitar);
            defaultPack.AddSound(harmonica);
            defaultPack.AddSound(strings);
            defaultPack.AddSound(oboe);
            defaultPack.AddSound(piano);
            defaultPack.AddSound(sax);
            defaultPack.AddSound(horns);

            // Add to list of SoundPacks
            SoundPacks.Add(defaultPack);
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

        private void CreateCouch()
        {
            // Create a couch with two TriggerPoints
            Furniture couch = new Furniture("Couch");
            TriggerPoint couchSeatLeft = new TriggerPoint();
            TriggerPoint couchSeatRight = new TriggerPoint();
            couch.AddTriggerPoint(couchSeatLeft);
            couch.AddTriggerPoint(couchSeatRight);
            _furniture.Add(couch);
            
            // Add triggers
            AddTrigger(Windows.System.VirtualKey.B, couchSeatLeft);
            AddTrigger(Windows.System.VirtualKey.C, couchSeatRight);
        }

        private void AddTrigger(Windows.System.VirtualKey key, TriggerPoint triggerPoint)
        {
            if (triggerPoint != null)
            {
                _triggers.Add(key, triggerPoint);
            }
        }

        public void ClearTriggers()
        {
            foreach (KeyValuePair<Windows.System.VirtualKey, TriggerPoint> trigger in _triggers)
            {
                trigger.Value.Clear();
            }
        }

        public List<KeyValuePair<String, String>> GetStateOfTriggers()
        {
            List<KeyValuePair<String, String>> state = new List<KeyValuePair<string, string>>();

            foreach (TriggerPoint trigger in _triggers.Values)
            {
                if (trigger.TriggerSound != null)
                {
                    KeyValuePair<String, String> curState = new KeyValuePair<string, string>(trigger.ID, trigger.TriggerSound.Name);
                    state.Add(curState);
                }
            }

            return state;
        }

        // Members
        private List<Furniture> _furniture;

        private Dictionary<Windows.System.VirtualKey, TriggerPoint> _triggers;

        public List<SoundPack> SoundPacks { get; private set; }

        private int _currentSoundPackIndex;
    }
}
