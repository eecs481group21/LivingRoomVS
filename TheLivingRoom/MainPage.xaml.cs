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
                  Click Event Handlers
         **************************************/

        // Handle transition to Settings page
        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        // Play preview of Sound corresponding to this SoundTile
        private void SoundTile_PointerPressed(object sender, RoutedEventArgs e)
        {
            Grid clickedTile = (Grid)sender;
            int row = (int)clickedTile.GetValue(Grid.RowProperty);
            int col = (int)clickedTile.GetValue(Grid.ColumnProperty);

            int soundId = row * 3 + col;

            // Get Sound that corresponds with this cell
            Sound theSound = FurnitureEngine.GetInstance().GetSoundPack().Sounds[soundId];

            // Play preview of Sound
            PlaybackEngine.GetInstance().PlaySoundPreview(theSound);           
        }

        private void TriggerTile_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Grid clickedTile = (Grid)sender;
            int row = (int)clickedTile.GetValue(Grid.RowProperty);

            int triggerNumber = row;
        }

        // Prepare this furniture trigger point for assignment.
        // Remove old assignment if applicable.
        private void FurnitureButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            int row = (int)clickedButton.GetValue(Grid.RowProperty);

            
        }

        /**************************************
            Dynamic UI initialization methods
         **************************************/

        // Populate the soundGrid according to the current SoundPack
        private void RenderSounds()
        {
            List<Sound> sounds = FurnitureEngine.GetInstance().GetSoundPack().Sounds;

            // Grid holds maximum of 9 Sounds
            int numSoundButtons = Math.Min(sounds.Count, 9);

            for (int i = 0; i < numSoundButtons; ++i)
            {
                Grid soundTile = CreateEmptyTile();
                soundTile.Background = new SolidColorBrush { Color = Color.FromArgb(255, 204, 248, 255) };

                // Set tile image to instrument icon
                Image soundTileImage = soundTile.Children[0] as Image;
                string uriString = "ms-appx:///Assets/SoundPacks/Default/Icons/" + sounds[i].Name.ToLower() + ".png";
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

            // At most 3 pieces of furniture are represented in furnitureGrid
            int numFurnitureGrids = Math.Min(furniture.Count, 3);

            // Render each piece of furniture as a Grid with 4 columns
            for (int i = 0; i < numFurnitureGrids; ++i)
            {
                Grid curFurnitureGrid = new Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                ColumnDefinition c1 = new ColumnDefinition();
                ColumnDefinition c2 = new ColumnDefinition();
                ColumnDefinition c3 = new ColumnDefinition();
                ColumnDefinition c4 = new ColumnDefinition();
                // Set equal widths
                c1.Width = new GridLength(1, GridUnitType.Star);
                c2.Width = new GridLength(1, GridUnitType.Star);
                c3.Width = new GridLength(1, GridUnitType.Star);
                c4.Width = new GridLength(1, GridUnitType.Star);
                curFurnitureGrid.ColumnDefinitions.Add(c1);
                curFurnitureGrid.ColumnDefinitions.Add(c2);
                curFurnitureGrid.ColumnDefinitions.Add(c3);
                curFurnitureGrid.ColumnDefinitions.Add(c4);

                
                // Create layout image grid in column 0
                Grid imageTile = CreateEmptyTile();

                // First child is image, second is labelGrid. First child of labelGrid is label.
                Image tileImage = imageTile.Children[0] as Image;
                tileImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/chair.png"));
                Grid tileLabelGrid = imageTile.Children[1] as Grid;
                TextBlock tileLabel = tileLabelGrid.Children[0] as TextBlock;
                tileLabel.Text = "Chair";

                imageTile.SetValue(Grid.ColumnProperty, 0);
                curFurnitureGrid.Children.Add(imageTile);
                
                // Add up to three Furniture TriggerPoints in curFurnitureGrid columns 1-3
                for (int j = 0; j < furniture[i].NumTriggerPoints(); ++j)
                {
                    // Create tile grid in column j + 1
                    Grid triggerTile = CreateEmptyTile();

                    // Second child is labelGrid. First child of labelGrid is label.
                    Image tileTriggerImage = triggerTile.Children[0] as Image;
                    tileTriggerImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/not_set.png"));
                    Grid tileTriggerLabelGrid = triggerTile.Children[1] as Grid;
                    TextBlock tileTriggerLabel = tileTriggerLabelGrid.Children[0] as TextBlock;
                    tileTriggerLabel.Text = "Trigger " + (j + 1);

                    // Handle PointerPressed events from all Trigger tiles
                    triggerTile.PointerPressed += TriggerTile_PointerPressed;

                    triggerTile.SetValue(Grid.ColumnProperty, (j + 1));
                    curFurnitureGrid.Children.Add(triggerTile);
                }
                              
                // Add curFurnitureGrid to furnitureGrid
                curFurnitureGrid.SetValue(Grid.RowProperty, i);
                furnitureGrid.Children.Add(curFurnitureGrid);
            }
        }

        private static Grid CreateEmptyTile()
        {
            Grid tileGrid = new Grid();
            RowDefinition r1 = new RowDefinition(); // Image Row
            RowDefinition r2 = new RowDefinition(); // Label Row
            r1.Height = new GridLength(3, GridUnitType.Star);
            r2.Height = new GridLength(1, GridUnitType.Star);
            tileGrid.RowDefinitions.Add(r1);
            tileGrid.RowDefinitions.Add(r2);

            // Stretch Grid to fill parent
            tileGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            tileGrid.VerticalAlignment = VerticalAlignment.Stretch;
            tileGrid.Margin = new Thickness(10, 10, 10, 10);
            tileGrid.Background = new SolidColorBrush { Color = Color.FromArgb(255, 255, 255, 255) };
            tileGrid.IsTapEnabled = true;

            // Add image to 0th row of tile
            Image image = new Image
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            }; // No source initially
            image.SetValue(Grid.RowProperty, 0);
            tileGrid.Children.Add(image);

            // Put label in a StackPanel (to change background color)
            Grid labelGrid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new SolidColorBrush {Color = Color.FromArgb(255, 64, 64, 64)}
            };

            // Create TextBlock and add to labelPanel
            TextBlock label = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush {Color = Color.FromArgb(255, 255, 255, 255)},
                FontSize = 36.0
            };
            labelGrid.Children.Add(label);

            // Add labelPanel to 1st row of tile
            labelGrid.SetValue(Grid.RowProperty, 1);
            tileGrid.Children.Add(labelGrid);

            return tileGrid;
        }
    }
}
