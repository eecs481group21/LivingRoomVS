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
        private KinectSensor _sensor;
        private DepthImagePixel[] _depthImagePixels;
        private Shape _lastHead = null;
        private Shape _lastHand;
        private Shape _lastLine;
        private Shape _lastHand2;

        public MainWindow()
        {
            InitializeComponent();
        }

        /*****************************************************************
         * Function: InitKinect
         * Description: Initializes the Kinect to begin recieving input.
         * This function should only be called once on program startup or
         * after the kinect has been stopped.
         * Parameters: None
        ******************************************************************/
        private bool InitKinect()
        {
            // Make sure a Kinect is connected before assigning it
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];
            }
            else
            {
                return false;
            }

            _sensor.Start();
            // Get color image data from the Kinect
            _sensor.ColorStream.Enable();
            // Get depth data from the Kinect
            _sensor.DepthStream.Range = DepthRange.Default;
            _sensor.DepthStream.Enable();
            // Function to be called when the Kinect is ready to work
            _sensor.AllFramesReady += _sensor_AllFramesReady;
            // Enable Skeletal tracking
            _sensor.SkeletonStream.Enable();
            _sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            // True if using near mode
            _sensor.SkeletonStream.EnableTrackingInNearRange = false;

            return true;
        }

        /*****************************************************************
        * Function: StopKinect
        * Description: Halts the Kinect from recieving input.
        * Parameters: None
       ******************************************************************/
        private void StopKinect()
        {
            // Only Stop the Kinect if we're using one
            if (_sensor != null && _sensor.IsRunning)
            {
                _sensor.Stop();
            }
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
                if (InitKinect())
                {
                    btnStart.Content = "Stop";
                    textBlockError.Text = "";
                }
                else
                {
                    textBlockError.Text = "Error: No Kinect device connected. Try again.";
                }
            }
            else
            {
                StopKinect();
                btnStart.Content = "Start";
            }
        }

        void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // Set to size of total length of pixel data buffer for each ImageFrame in ImageStream
            _depthImagePixels = new DepthImagePixel[_sensor.DepthStream.FramePixelDataLength];

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
                var mapper = new CoordinateMapper(_sensor);

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

                textBlockDistInches.Text = Math.Round((GetDistance(depthPointHand1, depthPointHand2) / 12), 4).ToString();

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

        /*****************************************************************
         * Function: GetDistance
         * Description: Determines the distance between 2 DepthImageFrame
         * points in inches by manipulating pixel coordinates and depth 
         * data. 
         * Parameters: 
         *      DepthImagePoint a, b: DepthImageFrames recieved from the 
         *                            Kinect. The internal data should 
         *                            not be manipulated in any way.
        *****************************************************************/
        private static double GetDistance(DepthImagePoint a, DepthImagePoint b)
        {
            // Used for frame of reference to determine actual distance
            int closerDepth = Math.Min(a.Depth, b.Depth);
            // Pixels on the 640x480 sensor per foot horizontally
            double pxPerInchHor = DepthToPxPerInchHor(closerDepth);
            // Pixels on the 640x480 sensor per foot vertically
            double pxPerInchVert = DepthToPxPerInchVert(closerDepth);

            // Horizontal distance between points in inches
            double dxInches = Math.Abs(a.X - b.X) / pxPerInchHor;
            // Vertical distance between points in inches
            double dyInches = Math.Abs(a.Y - b.Y) / pxPerInchVert;
            // Depth difference between points in inches
            double dzInches = Math.Abs(DepthToInches(a.Depth) - DepthToInches(b.Depth));

            // Pythagoream theorem to get the approximate distance in inches
            return Math.Sqrt(Math.Pow(dxInches, 2) + Math.Pow(dyInches, 2) + Math.Pow(dzInches, 2));
        }

        /*****************************************************************
         * Function: DepthToPxPerInchHor
         * Description: Takes a depth and determines how many pixels of
         * the 640 pixel width limit it takes to represent an inch.
         * Parameters: 
         *      int depth: depth in units as returned by .Depth on a 
         *                 DepthImageFrame object
        *****************************************************************/
        private static double DepthToPxPerInchHor(int depth)
        {
            // These constants were calculated by graphing the number of horizontal 
            // pixels used to represent 1ft at various ranges from 840KU to 2800KU 
            // which matched the curve y = 120372x^-0.926 with and R^2 value of 
            // 0.99848. KU is used to represent the units that the Kinect returns 
            // on depth data
            const double c = 120372;
            const double e = -0.926;

            // return as inches for less decimals
            return ((Math.Pow(depth, e)) * c) / 12;
        }

        /*****************************************************************
         * Function: DepthToPxPerInchVert
         * Description: Takes a depth and determines how many pixels of
         * the 480 pixel height limit it takes to represent an inch.
         * Parameters: 
         *      int depth: depth in units as returned by .Depth on a 
         *                 DepthImageFrame object
        *****************************************************************/
        private static double DepthToPxPerInchVert(int depth)
        {
            // These constants were calculated by graphing the number of vertical 
            // pixels used to represent 1ft at various ranges from 840KU to 2800KU 
            // which matched the curve y = 37935x^-0.777 with and R^2 value of 
            // 0.98111. KU is used to represent the units that the Kinect returns 
            // on depth data
            const double c = 37935;
            const double e = -0.777;

            // return as inches for less decimals
            return ((Math.Pow(depth, e)) * c) / 12;
        }

        /*****************************************************************
         * Function: DepthToInches
         * Description: Takes a depth and determines how many inches away
         * the point is.
         * Parameters: 
         *      int depth: depth in units as returned by .Depth on a 
         *                 DepthImageFrame object
        *****************************************************************/
        private static double DepthToInches(int depth)
        {
            // These constants were calculated by graphing the relationship between the 
            // depth reported by the Kinect to actual depth in inches. Distances from the
            // minimum recognized distance of about 3 feet to 11 feet. The relationship
            // is linear at matches the line 0.0442x - 4.8759 with an R^2 value of 0.9992
            const double m = 0.0442;
            const double b = -4.8759;

            return m * depth + b;
        }
    }
}