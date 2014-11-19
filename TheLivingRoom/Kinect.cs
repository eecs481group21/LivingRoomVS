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
            get { throw new NotImplementedException(); }
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


    }
}
