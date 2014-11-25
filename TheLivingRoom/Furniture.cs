using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLivingRoom
{
    class Furniture
    {

        public Furniture(string name)
        {
            Name = name;

            _triggerPoints = new List<TriggerPoint>();
        }

        public bool AddTriggerPoint(TriggerPoint newTriggerPoint)
        {
            // Each Furniture can have at most 3 trigger points
            if (_triggerPoints.Count < 3)
            {
                _triggerPoints.Add(newTriggerPoint);
                return true;
            }
            
            return false;
        }

        public TriggerPoint GetTriggerPointAtIndex(int index)
        {
            if (index < _triggerPoints.Count && index >= 0)
            {
                return _triggerPoints[index];
            }
            return null;
        }

        public int NumTriggerPoints()
        {
            return _triggerPoints.Count;
        }

        public void ClearTriggerPoints()
        {
            _triggerPoints.Clear();
        }

        // Members
        public string Name { get; set; }
        
        private readonly List<TriggerPoint> _triggerPoints;
    }
}
