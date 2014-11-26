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
    public sealed partial class SettingsPage : Page
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


        public SettingsPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            RenderSoundPacks();
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
            // Set slider value to current system volume limit
            volumeSlider.Value = PlaybackEngine.GetInstance().SystemVolumeLimit;
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

        /**************************************
            Dynamic UI initialization methods
         **************************************/

        private void RenderSoundPacks()
        {
            List<SoundPack> soundPacks = FurnitureEngine.GetInstance().SoundPacks;

            // A maximum of 3 packs are displayed
            int numPacks = Math.Min(3, soundPacks.Count);

            for (int i = 0; i < numPacks; ++i)
            {
                // Create a Tile for this SoundPack
                Grid soundpackTile = GridUtility.CreateEmptyTile();

                // Set the SoundPack icon
                Image soundpackImage = soundpackTile.Children[0] as Image;
                string uriString = "ms-appx:///Assets/SoundPacks/" + soundPacks[i].Name + "/" + soundPacks[i].Name + ".png";
                soundpackImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(uriString));

                // Set the Tile tile
                Grid soundpackTileLabelGrid = soundpackTile.Children[1] as Grid;
                TextBlock soundpackLabel = soundpackTileLabelGrid.Children[0] as TextBlock;
                soundpackLabel.Text = soundPacks[i].Name;

                // Add tile to proper row in soundpackGrid
                soundpackTile.SetValue(Grid.RowProperty, i);
                soundpackGrid.Children.Add(soundpackTile);
            }
        }
        private void volumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (volumeSlider != null)
            {
                // Change system volume limit in PlaybackEngine
                PlaybackEngine.GetInstance().SetVolumeLimit(volumeSlider.Value);
            }
        }
    }
}
