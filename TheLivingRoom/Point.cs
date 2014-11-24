using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLivingRoom
{
    class Point : System.Object
    {
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double x { get; set; }
        public double y { get; set; }

        private static double epsilon = .0001;

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            // Attempt cast to type Point
            Point p = obj as Point;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (doublesEqual(x, p.x)) && (doublesEqual(y, p.y));
        }

        public bool Equals(Point p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (doublesEqual(x, p.x)) && (doublesEqual(y, p.y));
        }

        public override int GetHashCode()
        {
            // Simple hash
            int hash = 17;
            hash *= 23 + x.GetHashCode();
            hash *= 23 + y.GetHashCode();
            return hash;
        }

        public static bool operator ==(Point a, Point b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return doublesEqual(a.x, b.x) && doublesEqual(a.y, b.y);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        private static bool doublesEqual(double a, double b) {
            return (Math.Abs(a - b) < epsilon);
        }
    }
}
