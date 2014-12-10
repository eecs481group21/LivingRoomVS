using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace TheLivingRoom_Kinect
{
    public class Kinect
    {

        private KinectSensor _sensor;
        private MotionTracker _motionTracker;
        private static Kinect _instance;

        public int FramePixelDataLength
        {
            get { return _sensor.DepthStream.FramePixelDataLength; }
            set { throw new NotImplementedException(); }
        }

        public KinectSensor Sensor
        {
            get { return _sensor; }
            set { throw new NotImplementedException(); }
        }

        public EventHandler<AllFramesReadyEventArgs> AllFramesReady
        {
            get { return null; }
            set { _sensor.AllFramesReady += value; }
        }

        public static Kinect GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Kinect();
                if (!_instance.InitKinect())
                {
                    // No Kinect found
                    return null;
                }
            }

            return _instance;
        }

        // Prevent construction outside of GetInstance()
        private Kinect()
        {
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

            StartKinect();
            return true;
        }

        public void StartKinect() 
        {
            _sensor.Start();
            // Get color image data from the Kinect
            _sensor.ColorStream.Enable();
            // Get depth data from the Kinect
            _sensor.DepthStream.Range = DepthRange.Default;
            _sensor.DepthStream.Enable();
            // Enable Skeletal tracking
            _sensor.SkeletonStream.Enable();
            _sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            // True if using near mode
            _sensor.SkeletonStream.EnableTrackingInNearRange = false;

            _motionTracker = new MotionTracker();
        }

        /*****************************************************************
        * Function: StopKinect
        * Description: Halts the Kinect from recieving input.
        * Parameters: None
        ******************************************************************/
        public void StopKinect()
        {
            // Only Stop the Kinect if we're using one
            if (_sensor != null && _sensor.IsRunning)
            {
                _sensor.Stop();
            }
        }

        /*****************************************************************
        * Function: GetDistance
        * Description: Use the MotionTracker class to process the 
        *              distance between points
        * Parameters: 
        *      DepthImagePoint a, b: DepthImageFrames recieved from the 
        *                            Kinect. The internal data should 
        *                            not be manipulated in any way.
        ******************************************************************/
        public double GetDistance(DepthImagePoint a, DepthImagePoint b)
        {
            return _motionTracker.GetDistance(a, b);
        }

        /*****************************************************************
         * Function: LogDist
         * Description: Use the Motion Tracker class to log the data
         * Parameters: 
         *      double deltaDist: Distance between two points
        *****************************************************************/
        public void LogDist(double deltaDist)
        {
            _motionTracker.LogDist(deltaDist);
        }

        /*****************************************************************
         * Function: LogContact
         * Description: Use the Motion Tracker class to log the data
         * Parameters: 
         *      DepthImagePoint a, b: DepthImageFrames recieved from the 
        *                             Kinect. The internal data should 
        *                             not be manipulated in any way.
        *****************************************************************/
        public void LogContact(DepthImagePoint a, DepthImagePoint b)
        {
            _motionTracker.LogContact(a, b);
        }

        public double GetDistChangeRatio()
        {
            return _motionTracker.GradeInterpersonalMovement();
        }

        public bool GetPastHandContact()
        {
            if (_motionTracker == null)
            {
                return false;
            }
            return _motionTracker.GradeHandContact();
        }

        public double GetLastDistance()
        {
            if (_motionTracker == null)
            {
                return 0;
            }
            return _motionTracker.GetLastDistance();
        }

        internal class MotionTracker
        {
            private readonly FixedLengthQueue<double> _pastDistances;
            private readonly FixedLengthQueue<bool> _pastContact;
            private const int Fps = 30; // Frames per second
            private const double CaptureWindow = 0.5; // In seconds
            private const double AvgHumanFps = 7; // Average human feet per second
            private const double TouchTolerance = 0.6; // In feet            

            public MotionTracker()
            {
                _pastDistances = new FixedLengthQueue<double>((int)(Fps * CaptureWindow));
                _pastContact = new FixedLengthQueue<bool>((int)(Fps * CaptureWindow));
            }

            /*****************************************************************
             * Function: GetDistance
             * Description: Determines the distance between 2 DepthImageFrame
             *              points in inches by manipulating pixel coordinates and depth 
             * data. 
             * Parameters: 
             *      DepthImagePoint a, b: DepthImageFrames recieved from the 
             *                            Kinect. The internal data should 
             *                            not be manipulated in any way.
            *****************************************************************/
            public double GetDistance(DepthImagePoint a, DepthImagePoint b)
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
             *              the 640 pixel width limit it takes to represent an inch.
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
             *              the 480 pixel height limit it takes to represent an inch.
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
             *              the point is.
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

            /*****************************************************************
             * Function: LogDist
             * Description: Log a value of the current deltaDistance in the 
             *              list of the previous Fps*CaptureWindow distances
             * Parameters: 
             *      double deltaDist: Distance between two points
            *****************************************************************/
            public void LogDist(double deltaDist)
            {
                _pastDistances.Enqueue(deltaDist);

                // Debug.Assert(_pastDistances.Capacity <= (Fps * CaptureWindow));
                if (_pastDistances.Count > (Fps * CaptureWindow))
                {
                    Console.Error.WriteLine("MotionTracker::LogDist - _pastDistances Count: " + _pastDistances.Count);
                }
            }

            /*****************************************************************
             * Function: LogContact
             * Description: Log a value of the current contact in the 
             *              list of the previous Fps*CaptureWindow instances
             * Parameters: 
             *      DepthImagePoint a, b: DepthImageFrames recieved from the 
             *                            Kinect. The internal data should 
             *                            not be manipulated in any way. These 
             *                            values represent the hands of users.
            *****************************************************************/
            public void LogContact(DepthImagePoint a, DepthImagePoint b)
            {
                bool contact =  GetDistance(a, b)/12.0 < TouchTolerance;
                _pastContact.Enqueue(contact);
            }

            /*****************************************************************
             * Function: GradeInterpersonalMovement
             * Description: Will return a double -1.0 to 1.0 that will 
             *              correspond to the rate of changing distance 
             *              between users. -1 meaning moving farther apart, 
             *              and 1 being moving closer together.
             * Parameters: 
             *      DepthImagePoint a, b: None
            *****************************************************************/
            public double GradeInterpersonalMovement()
            {
                // Queue is not full yet
                if (_pastDistances.Count != _pastDistances.Size)
                {
                    return 0.0;
                }

                double distanceMoved = _pastDistances.Last() - _pastDistances.First();

                // Faster than expected fps
                if (distanceMoved > AvgHumanFps)
                {
                    return 1.0;
                }
                if (distanceMoved < -1 * AvgHumanFps)
                {
                    return -1.0;
                }
                return distanceMoved / AvgHumanFps;
            }

            /*****************************************************************
             * Function: GradeHandContact
             * Description: Will return a bool that will correspond to if the
             *              users have made hand contact in the last captureWindow 
             * Parameters: 
             *      DepthImagePoint a, b: DepthImageFrames recieved from the 
             *                            Kinect. The internal data should 
             *                            not be manipulated in any way.
            *****************************************************************/
            public bool GradeHandContact()
            {
                return _pastContact.Contains(true);
            }

            public double GetLastDistance()
            {
                return _pastDistances.LastOrDefault();
            }
        }
    }
}
