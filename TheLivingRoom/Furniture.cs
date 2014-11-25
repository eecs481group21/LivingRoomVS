using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLivingRoom
{
    class Furniture
    {

        public Furniture(string name, double height, double width)
        {
            Name = name;
            Height = height;
            Width = Width;
            _layout = new Dictionary<Point, TriggerPoint>();
        }

        public bool AddTriggerPoint(double x, double y, TriggerPoint newTriggerPoint)
        {
            // Upper left position (0,0)
            if (x > Width || x < 0 || y > Height || y < 0)
            {
                return false;
            }
            else if (newTriggerPoint == null) {
                return false;
            }
            else
            {
                Point newTriggerLocation = new Point(x, y);

                // Make certain no TriggerPoint is already in this location
                if (_layout.ContainsKey(newTriggerLocation))
                {
                    return false;
                }
                else
                {
                    // Add this TriggerPoint to the layout
                    _layout.Add(newTriggerLocation, newTriggerPoint);
                    return true;
                }
            }
        }

        public void ClearTriggerPoints()
        {
            _layout.Clear();
        }

        // Members
        public string Name { get; set; }
        
        private Dictionary<Point, TriggerPoint> _layout { get; set; }

        public double Height { get; private set; }
        public double Width { get; private set; }
    }
}
