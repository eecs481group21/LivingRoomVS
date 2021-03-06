﻿using System.Diagnostics;
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
        
        // Used in assigning Sounds to TriggerPoints
        TriggerPoint toBeAssignedTriggerPoint;
        Grid toBeAssignedTriggerTile;
        List<Grid> triggerTiles;

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

            // Set to-be-assigned helpers to null
            toBeAssignedTriggerPoint = null;
            toBeAssignedTriggerTile = null;

            triggerTiles = new List<Grid>();

            // Listen to global KeyDown events
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            // Get all of the sounds available
            RenderSounds();

            // Get all of the furniture available
            RenderFurniture();
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            FurnitureEngine.GetInstance().HandleTrigger(args.VirtualKey);
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
            Debug.WriteLine("LoadState Called.");

            // Check if any of the triggers are assigned and restore the tile
            foreach (Grid triggerTile in triggerTiles)
            {
                // Look up if this trigger should be set with a Sound
                String triggerID = triggerTile.Tag.ToString();
                if (e.PageState != null && e.PageState.ContainsKey(triggerID))
                {
                    // Set the TriggerPoint and change its tile Icon
                    TriggerPoint triggerPoint = FurnitureEngine.GetInstance().GetTriggerPointByID(triggerID);
                    Sound sound = FurnitureEngine.GetInstance().GetSoundByName(e.PageState[triggerID].ToString());
                    if (triggerPoint != null && sound != null)
                    {
                        // Set trigger point
                        triggerPoint.Set(sound);

                        // Set TriggerTile thumbnail to Sound icon
                        Image triggerTileThumbnail = triggerTile.Children[0] as Image;
                        string uriString = "ms-appx:///Assets/SoundPacks/Garage/Icons/" + sound.Name.ToLower() + ".png";
                        triggerTileThumbnail.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(uriString));

                        // Reset TriggerTile label to triggernumber
                        int triggerNumber = (int)triggerTile.GetValue(Grid.ColumnProperty);
                        Grid triggerTileLabelGrid = triggerTile.Children[1] as Grid;
                        TextBlock triggerTileLabel = triggerTileLabelGrid.Children[0] as TextBlock;
                        triggerTileLabel.Text = triggerNumber.ToString();

                        // Change background color of TriggerTile to normal color (white)
                        triggerTile.Background = new SolidColorBrush { Color = Color.FromArgb(255, 255, 255, 255) };
                    }
                }
            }

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
            Debug.WriteLine("SaveState Called.");

            // Save state of each trigger
            List<KeyValuePair<String, String>> stateOfTriggers = FurnitureEngine.GetInstance().GetStateOfTriggers();

            foreach (KeyValuePair<String, String> statePair in stateOfTriggers)
            {
                Debug.WriteLine("Saving mapping " + statePair.Key + " to " + statePair.Value + ".");
                e.PageState[statePair.Key] = statePair.Value;
            }
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
                  Click Event Handlers
         **************************************/

        // Handle transition to Settings page
        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
            FurnitureEngine.GetInstance().ClearTriggers();
        }

        // Play preview of Sound corresponding to this SoundTile
        private void SoundTile_PointerPressed(object sender, RoutedEventArgs e)
        {
            Grid clickedTile = (Grid)sender;
            int row = (int)clickedTile.GetValue(Grid.RowProperty);
            int col = (int)clickedTile.GetValue(Grid.ColumnProperty);

            int soundId = row * 3 + col;

            // Get Sound that corresponds with this cell
            Sound theSound = FurnitureEngine.GetInstance().GetCurrentSoundPack().Sounds[soundId];

            // Check if a TriggerPoint is in the process of being assigned.
            // If so, assign the TriggerPoint.
            if (toBeAssignedTriggerPoint != null)
            {
                // Set trigger point
                toBeAssignedTriggerPoint.Set(theSound);

                // Set TriggerTile thumbnail to Sound icon
                Image triggerTileThumbnail = toBeAssignedTriggerTile.Children[0] as Image;
                Image soundTileThumbnail = clickedTile.Children[0] as Image;
                triggerTileThumbnail.Source = soundTileThumbnail.Source;

                // Reset TriggerTile label to triggernumber
                int triggerNumber = (int)toBeAssignedTriggerTile.GetValue(Grid.ColumnProperty);
                Grid triggerTileLabelGrid = toBeAssignedTriggerTile.Children[1] as Grid;
                TextBlock triggerTileLabel = triggerTileLabelGrid.Children[0] as TextBlock;
                triggerTileLabel.Text = triggerNumber.ToString();

                // Change background color of TriggerTile to normal color (white)
                toBeAssignedTriggerTile.Background = new SolidColorBrush { Color = Color.FromArgb(255, 255, 255, 255) };
                
                // Clear to-be-assigned helpers
                toBeAssignedTriggerPoint = null;
                toBeAssignedTriggerTile = null;
            }
            // Play preview of Sound
            PlaybackEngine.GetInstance().PlaySoundPreview(theSound);
        }

        private void TriggerTile_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Grid clickedTile = (Grid)sender;

            // Trigger number equal to column number of tile
            int triggerNumber = (int)clickedTile.GetValue(Grid.ColumnProperty);

            // Furniture number equal to row number of parent Grid object
            int furnitureNumber = (int)clickedTile.Parent.GetValue(Grid.RowProperty);

            // The only use case for pressing a TriggerTile is when setting its Sound
            // Therefore, we always clear the the previous sound if applicable
            // and wait for the user to press a SoundTile

            // Check if another Trigger was pending and change it back to a "Not Set" tile
            if (toBeAssignedTriggerTile != null)
            {
                // Reset thumbnail
                Image prevTileThumbnail = toBeAssignedTriggerTile.Children[0] as Image;
                prevTileThumbnail.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/not_set2.png"));

                // Reset label to trigger number
                int prevTriggerNumber = (int)toBeAssignedTriggerTile.GetValue(Grid.ColumnProperty);
                Grid prevTileLabelGrid = toBeAssignedTriggerTile.Children[1] as Grid;
                TextBlock prevTileLabel = prevTileLabelGrid.Children[0] as TextBlock;
                prevTileLabel.Text = prevTriggerNumber.ToString();

                // Reset background color (to white)
                toBeAssignedTriggerTile.Background = new SolidColorBrush { Color = Color.FromArgb(255, 255, 255, 255) };
            }

            // Set to-be-assigned helper members
            toBeAssignedTriggerTile = clickedTile;
            toBeAssignedTriggerPoint = FurnitureEngine.GetInstance().GetFurnitureAtIndex(furnitureNumber).GetTriggerPointAtIndex(triggerNumber - 1);

            // Reset TriggerPoint, if applicable
            if (toBeAssignedTriggerPoint.IsSet())
            {
                // Unassign Sound from this Trigger
                toBeAssignedTriggerPoint.Clear();
            }

            // Set thumbnail to pending "hourglass" icon
            Image tileThumbnail = toBeAssignedTriggerTile.Children[0] as Image;
            tileThumbnail.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/hourglass.png"));

            // Set label to "Pending"
            Grid tileLabelGrid = toBeAssignedTriggerTile.Children[1] as Grid;
            TextBlock tileLabel = tileLabelGrid.Children[0] as TextBlock;
            tileLabel.Text = "Pending";

            // Set Tile background to pending assignment color (light orange)
            toBeAssignedTriggerTile.Background = new SolidColorBrush { Color = Color.FromArgb(255, 247, 231, 178) };
        }

        /*private void TriggerTile_CancelAssignment(object sender, PointerRoutedEventArgs e)
        {
            if (toBeAssignedTriggerPoint != null)
            {
                // Reset thumbnail
                Image tileThumbnail = toBeAssignedTriggerTile.Children[0] as Image;
                tileThumbnail.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/not_set.png"));

                // Reset background (white)
                toBeAssignedTriggerTile.Background = new SolidColorBrush { Color = Color.FromArgb(255, 255, 255, 255) };
            }
        }*/

        /**************************************
            Dynamic UI initialization methods
         **************************************/

        // Populate the soundGrid according to the current SoundPack
        private void RenderSounds()
        {
            List<Sound> sounds = FurnitureEngine.GetInstance().GetCurrentSoundPack().Sounds;

            // Grid holds maximum of 9 Sounds
            int numSoundButtons = Math.Min(sounds.Count, 9);

            for (int i = 0; i < numSoundButtons; ++i)
            {
                Grid soundTile = GridUtility.CreateEmptyTile();
                soundTile.Background = new SolidColorBrush { Color = Color.FromArgb(255, 204, 248, 255) };

                // Set tile image to instrument icon
                Image soundTileImage = soundTile.Children[0] as Image;
                string uriString = "ms-appx:///Assets/SoundPacks/Garage/Icons/" + sounds[i].Name.ToLower() + ".png";
                soundTileImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(uriString));

                // Set the tile label to the instrument name
                Grid tileLabelGrid = soundTile.Children[1] as Grid;
                TextBlock tileLabel = tileLabelGrid.Children[0] as TextBlock;
                tileLabel.Text = sounds[i].Name;

                // Add the tile in the appropriate row/column
                soundTile.SetValue(Grid.ColumnProperty, 0);
                soundTile.SetValue(Grid.RowProperty, i / 3);
                soundTile.SetValue(Grid.ColumnProperty, i % 3);

                // Handle PointerPressed event (similar to Click, but for Grids)
                soundTile.PointerPressed += SoundTile_PointerPressed;

                soundGrid.Children.Add(soundTile);
            }
        }

        // Populate the furnitureGrid according to FurnitureEngine
        private void RenderFurniture()
        {
            List<Furniture> furniture = FurnitureEngine.GetInstance().GetFurnitureItems();

            // At most 3 pieces of Furniture are displayed in furnitureGrid
            int numFurnitureGrids = Math.Min(furniture.Count, 3);

            // Render each piece of Furniture in a row with 4 columns as follows:
            // Col0: Furniture Layout Tile, Col1-3: Trigger Point(s)
            for (int i = 0; i < numFurnitureGrids; ++i)
            {
                Grid curFurnitureRow = GridUtility.CreateFurnitureRow(i);

                // Create Furniture layout tile in column 0
                Grid layoutTile = GridUtility.CreateFurnitureLayoutTile(furniture[i].Name);
                layoutTile.SetValue(Grid.ColumnProperty, 0);
                curFurnitureRow.Children.Add(layoutTile);

                // Add up to three Furniture TriggerPoints in curFurnitureRow columns 1-3
                for (int j = 0; j < furniture[i].NumTriggerPoints(); ++j)
                {
                    // Create tile grid in column j + 1
                    Grid triggerTile = GridUtility.CreateEmptyTile();
                    // Give extra right margin to final column
                    if (j == 2)
                    {
                        triggerTile.Margin = new Thickness(10, 10, 20, 10);
                    }

                    // Second child is labelGrid. First child of labelGrid is label.
                    Image tileTriggerImage = triggerTile.Children[0] as Image;
                    tileTriggerImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/not_set2.png"));
                    Grid tileTriggerLabelGrid = triggerTile.Children[1] as Grid;
                    TextBlock tileTriggerLabel = tileTriggerLabelGrid.Children[0] as TextBlock;
                    tileTriggerLabel.Text = (j + 1).ToString();

                    // Handle PointerPressed events from all TriggerTiles (assignment)
                    triggerTile.PointerPressed += TriggerTile_PointerPressed;

                    triggerTile.SetValue(Grid.ColumnProperty, (j + 1));
                    curFurnitureRow.Children.Add(triggerTile);

                    // Set tag of this tile to corresponding TriggerPoint's ID
                    TriggerPoint triggerPoint = FurnitureEngine.GetInstance().GetFurnitureAtIndex(i).GetTriggerPointAtIndex(j);
                    triggerTile.Tag = triggerPoint.ID;

                    triggerTiles.Add(triggerTile);
                }

                // Add curFurnitureRow to furnitureGrid
                curFurnitureRow.SetValue(Grid.RowProperty, i);
                furnitureGrid.Children.Add(curFurnitureRow);
            }
        }
    }
}
