using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace TheLivingRoom
{
    // Static functions used to painlessly generate Grids commonly used in the application
    class GridUtility
    {
        public static Grid CreateEmptyTile()
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
            tileGrid.IsDoubleTapEnabled = true;

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
                Background = new SolidColorBrush { Color = Color.FromArgb(255, 64, 64, 64) }
            };

            // Create TextBlock and add to labelPanel
            TextBlock label = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush { Color = Color.FromArgb(255, 255, 255, 255) },
                FontSize = 36.0
            };
            labelGrid.Children.Add(label);

            // Add labelPanel to 1st row of tile
            labelGrid.SetValue(Grid.RowProperty, 1);
            tileGrid.Children.Add(labelGrid);

            return tileGrid;
        }

        public static Grid CreateFurnitureLayoutTile(string name)
        {
            Grid furnitureLayoutTile = CreateEmptyTile();

            // Make background transparent
            furnitureLayoutTile.Background.Opacity = 0.0;

            // Set layout image
            Image layoutImage = furnitureLayoutTile.Children[0] as Image;
            layoutImage.Stretch = Stretch.Uniform;
            string layoutImageUriString = "ms-appx:///Assets/" + name.ToLower() + ".png";
            layoutImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(layoutImageUriString));

            // Make name label background transparent and change font color (grey) and weight (bold)
            Grid furnitureNameLabelGrid = furnitureLayoutTile.Children[1] as Grid;
            furnitureNameLabelGrid.Background.Opacity = 0.0;
            TextBlock nameLabel = furnitureNameLabelGrid.Children[0] as TextBlock;
            nameLabel.Foreground = new SolidColorBrush { Color = Color.FromArgb(255, 77, 77, 77) };
            nameLabel.FontSize = 48.0;
            nameLabel.Text = name;

            return furnitureLayoutTile;
        }

        public static Grid CreateFurnitureRow(int i)
        {
            Grid furnitureRow = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Give proper margins and background colors
            const int fullMargin = 20;
            const int halfMargin = 10;
            if (i == 0)
            {
                furnitureRow.Margin = new Thickness(fullMargin, fullMargin, fullMargin, halfMargin);
                // Lavender
                furnitureRow.Background = new SolidColorBrush { Color = Color.FromArgb(255, 227, 156, 255) };
            }
            else if (i == 1)
            {
                furnitureRow.Margin = new Thickness(fullMargin, halfMargin, fullMargin, halfMargin);
                // Turquoise
                furnitureRow.Background = new SolidColorBrush { Color = Color.FromArgb(255, 122, 255, 222) };
            }
            else
            {
                furnitureRow.Margin = new Thickness(fullMargin, halfMargin, fullMargin, fullMargin);
                // Pale Yellow
                furnitureRow.Background = new SolidColorBrush { Color = Color.FromArgb(255, 242, 255, 122) };
            }

            // Set up columns and spacing
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            ColumnDefinition c3 = new ColumnDefinition();
            ColumnDefinition c4 = new ColumnDefinition();
            c1.Width = new GridLength(1, GridUnitType.Star);
            c2.Width = new GridLength(1, GridUnitType.Star);
            c3.Width = new GridLength(1, GridUnitType.Star);
            c4.Width = new GridLength(1, GridUnitType.Star);
            furnitureRow.ColumnDefinitions.Add(c1);
            furnitureRow.ColumnDefinitions.Add(c2);
            furnitureRow.ColumnDefinitions.Add(c3);
            furnitureRow.ColumnDefinitions.Add(c4);

            return furnitureRow;
        }
    }
}
