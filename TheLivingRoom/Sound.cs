﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLivingRoom
{
    class Sound
    {
        public Sound(string name) {
            Name = name;
            _sample = new Windows.UI.Xaml.Controls.MediaElement();
            _stream = null;

            // Prevent autoplay of all samples when the UI loads
            _sample.AutoPlay = false;
        }

        // Cannot be in constructor because async methods cannot be called from constructors
        public async void InitWithSource(string sourceFile) {
            // Get file with URI
            Uri uri = new Uri(sourceFile);
            Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);

            // Configure stream-related members
            _stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            _mimeType = file.ContentType;
        }

        // Play Sound
        public void Play()
        {
            _sample.Play();
        }

        // Adjust volume of Sound.  Must reset source because volume does not change
        // reliably after the source is set.
        public void AdjustVolume(double newVol)
        {
            // Check for valid volume
            if (newVol >= 0.0 && newVol <= 1.0)
            {
                // 1. Reset the MediaElement (no alternative to clear source)
                // 2. Adjust the volume
                // 3. Set the source.
                _sample = new Windows.UI.Xaml.Controls.MediaElement {Volume = newVol};
                _sample.SetSource(_stream, _mimeType);
            }
        }

        // Seek to beginning of Sound
        public void ResetToBeginning()
        {
            _sample.Position = new TimeSpan(0, 0, 0);
            _sample.AutoPlay = true;
        }

        // Members
        public string Name { get; private set; }

        private Windows.UI.Xaml.Controls.MediaElement _sample;

        // Private stream-related members
        private Windows.Storage.Streams.IRandomAccessStream _stream;
        private string _mimeType;
    }
}
