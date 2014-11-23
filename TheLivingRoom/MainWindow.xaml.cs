using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace TheLivingRoom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Kinect _kinect;
        private DepthImagePixel[] _depthImagePixels;
        private Shape _lastHead = null;
        private Shape _lastHand;
        private Shape _lastLine;
        private Shape _lastHand2;

        public MainWindow()
        {
            InitializeComponent();
            _kinect = new Kinect();
        }

        /*****************************************************************
         * Function: CreateBitmap
         * Description: Creates a bitmap image from the frame captures by 
         * the Kinect. Doing this in a loop will produce the video feed 
         * on screen.
         * Parameters: 
         *      ColorImageFrame frame: frame captured by the Kinect
        *****************************************************************/
        private BitmapSource CreateBitmap(ColorImageFrame frame)
        {
            var pixelData = new byte[frame.PixelDataLength];
            frame.CopyPixelDataTo(pixelData);

            var stride = frame.Width * frame.BytesPerPixel;
            var bitmap = BitmapSource.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData,
                stride);
            return bitmap;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (btnStart.Content.ToString() == "Start")
            {
                if (_kinect.InitKinect())
                {
                    btnStart.Content = "Stop";
                    textBlockError.Text = "";
                    // Function to be called when the Kinect is ready to work
                    _kinect.AllFramesReady += _sensor_AllFramesReady;
                }
                else
                {
                    textBlockError.Text = "Error: No Kinect device connected. Try again.";
                }
            }
            else
            {
                _kinect.StopKinect();
                btnStart.Content = "Start";
            }
        }

        // Function to run when the Kinect is reading data
        void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // Set to size of total length of pixel data buffer for each ImageFrame in ImageStream
            _depthImagePixels = new DepthImagePixel[_kinect.FramePixelDataLength];

            // Get depth data for the frame
            using (var frame = e.OpenDepthImageFrame())
            {
                if (frame == null)
                {
                    return;
                }
                frame.CopyDepthImagePixelDataTo(_depthImagePixels);
            }

            // Get color info from the frame
            using (var frame = e.OpenColorImageFrame())
            {
                if (frame == null)
                {
                    return;
                }
                var bitmap = CreateBitmap(frame);
                // Set the canvas to display the image
                canvasFeed.Background = new ImageBrush(bitmap);
            }

            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                {
                    return;
                }
                var skeletons = new Skeleton[frame.SkeletonArrayLength];
                frame.CopySkeletonDataTo(skeletons);
                var skeleton1 = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
                if (skeleton1 == null)
                {
                    return;
                }
                // Skeletal coordinates
                //var headPosition1 = skeleton1.Joints[JointType.Head].Position;
                var rightHandPosition1 = skeleton1.Joints[JointType.HandRight].Position;
                var leftHandPosition1 = skeleton1.Joints[JointType.HandLeft].Position;
                var mapper = new CoordinateMapper(_kinect.Sensor);

                //var colorPointHead1 = mapper.MapSkeletonPointToColorPoint(headPosition1,
                //ColorImageFormat.RgbResolution640x480Fps30);
                var colorPointHand1 = mapper.MapSkeletonPointToColorPoint(rightHandPosition1,
                   ColorImageFormat.RgbResolution640x480Fps30);
                var colorPointHand2 = mapper.MapSkeletonPointToColorPoint(leftHandPosition1,
                   ColorImageFormat.RgbResolution640x480Fps30);

                //var depthPointHead1 = mapper.MapSkeletonPointToDepthPoint(headPosition1,
                //DepthImageFormat.Resolution640x480Fps30);
                var depthPointHand1 = mapper.MapSkeletonPointToDepthPoint(rightHandPosition1,
                    DepthImageFormat.Resolution640x480Fps30);
                var depthPointHand2 = mapper.MapSkeletonPointToDepthPoint(leftHandPosition1,
                    DepthImageFormat.Resolution640x480Fps30);

                textBlockDistInches.Text = Math.Round((_kinect.GetDistance(depthPointHand1, depthPointHand2) / 12), 4).ToString();

                //canvasFeed.Children.Remove(lastHead);
                canvasFeed.Children.Remove(_lastHand);
                canvasFeed.Children.Remove(_lastHand2);
                canvasFeed.Children.Remove(_lastLine);


                //Line headToHandLine = DrawLine(colorPointHead1, colorPointHand1);
                Line headToHandLine = DrawLine(colorPointHand2, colorPointHand1);
                _lastLine = headToHandLine;
                //Shape circle = CreateCircle(colorPointHead1);
                Shape circle = CreateCircle(colorPointHand2);
                //lastHead = circle;
                _lastHand2 = circle;
                Shape circle2 = CreateCircle(colorPointHand1);
                _lastHand = circle2;
                canvasFeed.Children.Add(circle);
                canvasFeed.Children.Add(circle2);
                canvasFeed.Children.Add(headToHandLine);
            }
        }

        private Line DrawLine(ColorImagePoint a, ColorImagePoint b)
        {
            var line = new Line
            {
                X1 = a.X,
                Y1 = a.Y,
                X2 = b.X,
                Y2 = b.Y,
                Stroke = Brushes.Aqua,
                Fill = Brushes.Aqua,
                StrokeThickness = 2
            };
            return line;
        }

        private Shape CreateCircle(ColorImagePoint colorPoint)
        {
            var circle = new Ellipse
            {
                Fill = Brushes.Chartreuse,
                Height = 20,
                Width = 20,
                Stroke = Brushes.Chartreuse,
                StrokeThickness = 2
            };
            Canvas.SetLeft(circle, colorPoint.X);
            Canvas.SetTop(circle, colorPoint.Y);
            return circle;
        }

    }
}