using System;
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
            Sample = new Windows.UI.Xaml.Controls.MediaElement();

            // Prevent autoplay of all samples when the UI loads
            Sample.AutoPlay = false;
        }

        // Cannot be in constructor because async methods cannot be called from constructors
        public async void SetSource(string sourceFile) {
            // Get file with URI
            Uri uri = new Uri(sourceFile);
            Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);

            // Get a stream from the audio file
            Windows.Storage.Streams.IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

            // Set the source of the Sample to the stream
            Sample.SetSource(stream, file.ContentType);
        }

        public string Name { get; private set; }

        public Windows.UI.Xaml.Controls.MediaElement Sample { get; private set; }
    }
}
