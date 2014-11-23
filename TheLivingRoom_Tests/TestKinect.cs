using System;
using System.Collections.Generic;
using TheLivingRoom;

namespace TheLivingRoom_Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class KinectTest
    {
        private Kinect _kinect;

        [SetUp]
        public void Init()
        {
            _kinect = new Kinect();
            _kinect.InitKinect(); 
        }

        [TearDown]
        public void Destroy()
        {
            _kinect.StopKinect();
        }

        [Test]
        public void TestLogDist()
        {
            double[] distances =
            {
                1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 1.2, 3.4, 5.6, 6.7, 0.1, 2.3
            };

            // Fill up the array minus 1
            foreach (double dist in distances)
            {
                _kinect.LogDist(dist);
                Assert.AreEqual(0.0, _kinect.GetDistChangeRatio());
            }

            _kinect.LogDist(4.5);
            Assert.AreEqual(3.5/7.0, _kinect.GetDistChangeRatio());

            _kinect.LogDist(4.5);
            Assert.AreEqual(2.5 / 7.0, _kinect.GetDistChangeRatio());

            _kinect.LogDist(4.5);
            Assert.AreEqual(1.5 / 7.0, _kinect.GetDistChangeRatio());

            _kinect.LogDist(45);
            Assert.AreEqual(1.0, _kinect.GetDistChangeRatio());

            _kinect.LogDist(-45);
            Assert.AreEqual(-1.0, _kinect.GetDistChangeRatio());
        }
    }
}
