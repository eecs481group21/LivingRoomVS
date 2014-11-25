using System.Diagnostics;
using Windows.UI;
using TheLivingRoom.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace TheLivingRoom
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            // Listen to global KeyDown events
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            // Get all of the sounds available
            RenderSounds();

            // Get all of the furniture available
            RenderFurniture();
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            // TODO: Handle all input by calling HandleInput in FurnitureEngine

            // Proof of concept
            if (args.VirtualKey == Windows.System.VirtualKey.A)
            {
                // Get piano sound
                Sound pianoSound = FurnitureEngine.GetInstance().GetSoundPack().Sounds[7];

                // Play piano sound with playback engine
                PlaybackEngine.GetInstance().PlaySound(pianoSound);
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button) sender;
            int row = (int)clickedButton.GetValue(Grid.RowProperty);
            int col = (int)clickedButton.GetValue(Grid.ColumnProperty);

            int soundId = row * 3 + col;

            // Get piano sound
            Sound theSound = FurnitureEngine.GetInstance().GetSoundPack().Sounds[soundId];

            // Play piano sound with playback engine
            PlaybackEngine.GetInstance().PlaySound(theSound);           
        }

        // Populate the sound grid according to the current chosen SoundPack
        private void RenderSounds()
        {
            List<Sound> sounds = FurnitureEngine.GetInstance().GetSoundPack().Sounds;

            for (int i = 0; i < sounds.Count; ++i)
            {
                var soundButton = new Button
                {
                    Content = sounds[i].Name,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };

                soundButton.Click += Button_Click;
                soundButton.Background = new SolidColorBrush { Color = Color.FromArgb(179, 114, 207, 60) };
                soundButton.SetValue(Grid.RowProperty, i / 3);
                soundButton.SetValue(Grid.ColumnProperty, i % 3);

                soundGrid.Children.Add(soundButton);

            }
        }

        private void RenderFurniture()
        {
            List<Furniture> furniture = FurnitureEngine.GetInstance().GetFurnitureItems();

            for (int i = 0; i < furniture.Count; ++i)
            {
                var furnitureButton = new Button
                {
                    Content = furniture[i].Name,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };

                // furnitureButton.Click += Button_Click;
                furnitureButton.Background = new SolidColorBrush { Color = Color.FromArgb(179, 60, 114, 207) };
                furnitureButton.SetValue(Grid.RowProperty, i);

                furnitureGrid.Children.Add(furnitureButton);
            }
        }
    }
}
