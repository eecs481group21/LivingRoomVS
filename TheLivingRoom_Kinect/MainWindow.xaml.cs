using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
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

namespace TheLivingRoom_Kinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Shape _lastHand;
        private Shape _lastLine;
        private Shape _lastHand2;
        private const bool SingleUserDebug = false;

        public MainWindow()
        {
            InitializeComponent();
            Kinect.GetInstance();
            Thread t = new Thread(Server.CreateHttpServer);
            t.Start();
        }

        /*****************************************************************
         * Function: CreateBitmap
         * Description: Creates a bitmap image from the frame captures by 
         * the Kinect. Doing this in a loop will produce the video feed 
         * on screen.
         * Parameters: 
         *      ColorImageFrame frame: frame captured by the Kinect
        *****************************************************************/
        private static BitmapSource CreateBitmap(ColorImageFrame frame)
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
                if (Kinect.GetInstance() != null)
                {
                    Kinect.GetInstance().StartKinect();
                    btnStart.Content = "Stop";
                    textBlockError.Text = "";
                    // Function to be called when the Kinect is ready to work
                    Kinect.GetInstance().AllFramesReady += _sensor_AllFramesReady;
                }
                else
                {
                    textBlockError.Text = "Error: No Kinect device connected. Try again.";
                }
            }
            else
            {
                Kinect.GetInstance().StopKinect();
                btnStart.Content = "Start";
            }
        }

        // Utility function to convert skeleton data to colorpoints for vizualization
        static ColorImagePoint CreateColorImagePoint(CoordinateMapper mapper, SkeletonPoint skeletonPoint)
        {
            return mapper.MapSkeletonPointToColorPoint(skeletonPoint, ColorImageFormat.RgbResolution640x480Fps30);
        }

        // Utility function to convert skeleton data to colorpoints for vizualization
        static DepthImagePoint CreateDepthImagePoint(CoordinateMapper mapper, SkeletonPoint skeletonPoint)
        {
            return mapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution640x480Fps30);   
        }

        // Function to run when the Kinect is reading data. We'll move this into 
        // the Kinect class once we transition to the Music player
        void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {

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

                int firstId;
                
                var skeleton1 = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
                if (skeleton1 == null)
                {
                    return;
                }
                else if (!SingleUserDebug)
                {
                    firstId = skeleton1.TrackingId;
                }

                Skeleton skeleton2 = null;
                if (!SingleUserDebug)
                {
                    skeleton2 = skeletons.FirstOrDefault(s => s.TrackingId != firstId && s.TrackingState == SkeletonTrackingState.Tracked);
                    if (skeleton2 == null)
                    {
                        return;
                    }
                }

                // Skeletal coordinates
                var skeleton1Position = (SingleUserDebug) ? skeleton1.Joints[JointType.HandLeft].Position :
                                                            skeleton1.Joints[JointType.Spine].Position;
                var skeleton2Position = (SingleUserDebug) ? skeleton1.Joints[JointType.HandRight].Position : 
                                                            skeleton2.Joints[JointType.Spine].Position;

                var skeleton1LeftHandPosition = skeleton1.Joints[JointType.HandLeft].Position;
                var skeleton1RightHandPosition = skeleton1.Joints[JointType.HandRight].Position;
                // If debugging, only worry about one user's hands
                var skeleton2LeftHandPosition = (SingleUserDebug) ? skeleton1.Joints[JointType.HandLeft].Position : 
                                                                    skeleton2.Joints[JointType.HandLeft].Position;
                var skeleton2RightHandPosition = (SingleUserDebug) ? skeleton1.Joints[JointType.HandRight].Position :
                                                                     skeleton2.Joints[JointType.HandRight].Position;

                var mapper = new CoordinateMapper(Kinect.GetInstance().Sensor);

                // Used for visualizing the bodies in the frame
                var colorPointSkeleton1 = CreateColorImagePoint(mapper, skeleton1Position);
                var colorPointSkeleton2 = CreateColorImagePoint(mapper, skeleton2Position);

                // Data abour where the bodies in the frame are in space
                var depthPointSkeleton1 = CreateDepthImagePoint(mapper, skeleton1Position);
                var depthPointSkeleton2 = CreateDepthImagePoint(mapper, skeleton2Position);

                double deltaDist = Kinect.GetInstance().GetDistance(depthPointSkeleton1, depthPointSkeleton2) / 12.0;
                Kinect.GetInstance().LogDist(deltaDist);

                // Data about where the bodies in frames' hands are in space
                var depthPointSkeleton1LeftHand  = CreateDepthImagePoint(mapper, skeleton1LeftHandPosition);
                var depthPointSkeleton1RightHand = CreateDepthImagePoint(mapper, skeleton1RightHandPosition);
                var depthPointSkeleton2LeftHand  = CreateDepthImagePoint(mapper, skeleton2LeftHandPosition);
                var depthPointSkeleton2RightHand = CreateDepthImagePoint(mapper, skeleton2RightHandPosition);

                // Log if hand contact was made by users in this frame
                if (SingleUserDebug)
                {
                    Kinect.GetInstance().LogContact(depthPointSkeleton1LeftHand, depthPointSkeleton1RightHand);    
                }
                else
                {
                    Kinect.GetInstance().LogContact(depthPointSkeleton1LeftHand, depthPointSkeleton1RightHand,
                                                    depthPointSkeleton2LeftHand, depthPointSkeleton2RightHand);    
                }
                
                
                // Update distance between users on view
                textBlockDistInches.Text = Math.Round(deltaDist, 4).ToString(CultureInfo.InvariantCulture);

                // Manipulate the image stream to show new points

                canvasFeed.Children.Remove(_lastHand);
                canvasFeed.Children.Remove(_lastHand2);
                canvasFeed.Children.Remove(_lastLine);

                Line headToHandLine = DrawLine(colorPointSkeleton2, colorPointSkeleton1);
                _lastLine = headToHandLine;
                Shape circle = CreateCircle(colorPointSkeleton2);
                _lastHand2 = circle;
                Shape circle2 = CreateCircle(colorPointSkeleton1);
                _lastHand = circle2;
                canvasFeed.Children.Add(circle);
                canvasFeed.Children.Add(circle2);
                canvasFeed.Children.Add(headToHandLine);
            }
        }

        private static Line DrawLine(ColorImagePoint a, ColorImagePoint b)
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

        private static Shape CreateCircle(ColorImagePoint colorPoint)
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