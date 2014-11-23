using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace TheLivingRoom
{
    class Kinect
    {

        private KinectSensor _sensor;


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

        /*****************************************************************
         * Function: InitKinect
         * Description: Initializes the Kinect to begin recieving input.
         * This function should only be called once on program startup or
         * after the kinect has been stopped.
         * Parameters: None
        ******************************************************************/
        public bool InitKinect()
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
        * Description: Use the DistanceTracker class to process the 
        *              distance between points
        * Parameters: 
        *      DepthImagePoint a, b: DepthImageFrames recieved from the 
        *                            Kinect. The internal data should 
        *                            not be manipulated in any way.
        ******************************************************************/
        public double GetDistance(DepthImagePoint a, DepthImagePoint b)
        {
            return DistanceTracker.GetDistance(a, b);
        }


        private class DistanceTracker
        {
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
            public static double GetDistance(DepthImagePoint a, DepthImagePoint b)
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

        // It tracks distances, but also listens for particular gestures
        private class GestureTracker : DistanceTracker
        {
            /*bool IsMakingEyeContact(IFTModel face1, IFTModel , face2 )
            {
            
            }*/

            /*****************************************************************
             * Function: IsMakingHandContact
             * Description: Given two points in space (assumed to be points 
             *              corresponding to users' hands, we will determine 
             *              whether or not the users are touching hands
             * Parameters: 
             *      DepthImagePoint a, b: DepthImageFrames recieved from the 
             *                            Kinect. The internal data should 
             *                            not be manipulated in any way.
            *****************************************************************/
            bool IsMakingHandContact(DepthImagePoint a, DepthImagePoint b)
            {
                // Just a stub for now
                return false;
            }

            /*****************************************************************
             * Function: GradeInterpersonalMovement
             * Description: Will return a double -1.0 to 1.0 that will 
             *              correspond to the rate of changing distance 
             *              between users. -1 meaning moving farther apart, 
             *              and 1 being moving closer together.
             * Parameters: 
             *      DepthImagePoint a, b: DepthImageFrames recieved from the 
             *                            Kinect. The internal data should 
             *                            not be manipulated in any way.
            *****************************************************************/
            double GradeInterpersonalMovement(DepthImagePoint a, DepthImagePoint b)
            {
                return 0.0;
            }

        }
    }
}
