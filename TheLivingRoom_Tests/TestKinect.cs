using System;
using System.Collections.Generic;
using TheLivingRoom;

namespace TheLivingRoom_Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class KinectTest
    {
        private Kinect kinect;

        [SetUp]
        public void Init()
        {
            kinect = new Kinect();
            kinect.InitKinect(); 
        }

        [TearDown]
        public void Destroy()
        {
            kinect.StopKinect();
        }

        [Test]
        public void TestSomething()
        {
            Assert.AreEqual(1, 1, "wat");
        }
    }
}
