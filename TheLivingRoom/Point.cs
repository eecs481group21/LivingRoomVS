using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLivingRoom
{
    class Point : Object
    {
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        private const double Epsilon = .0001;

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            // Attempt cast to type Point
            Point p = obj as Point;
            if ((Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (DoublesEqual(X, p.X)) && (DoublesEqual(Y, p.Y));
        }

        public bool Equals(Point p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (DoublesEqual(X, p.X)) && (DoublesEqual(Y, p.Y));
        }

        public override int GetHashCode()
        {
            // Simple hash
            int hash = 17;
            hash *= 23 + X.GetHashCode();
            hash *= 23 + Y.GetHashCode();
            return hash;
        }

        public static bool operator ==(Point a, Point b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return DoublesEqual(a.X, b.X) && DoublesEqual(a.Y, b.Y);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        private static bool DoublesEqual(double a, double b) {
            return (Math.Abs(a - b) < Epsilon);
        }
    }
}
