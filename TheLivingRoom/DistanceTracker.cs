using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace TheLivingRoom
{
    static class DistanceTracker
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
}
