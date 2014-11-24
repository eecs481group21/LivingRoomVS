using System;
using System.Collections.Generic;
using System.Linq;
using TheLivingRoom_Kinect;

namespace TheLivingRoom_Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class TestFixedLengthQueue
    {
        private FixedLengthQueue<double> _flQueue;
        private const int QueueSize = 10;

        [SetUp]
        public void Init()
        {
            _flQueue = new FixedLengthQueue<double>(QueueSize);
        }

        [Test]
        public void TestEnqueuePastSize()
        {
            // Overfill the queue
            for (int i = 0; i < QueueSize * 2; i++)
            {
                _flQueue.Enqueue(i);
            }

            // No expansion
            Assert.AreEqual(_flQueue.Count, QueueSize);

            // Make sure only the second half is stored
            for (int i = QueueSize; i < QueueSize * 2; i++)
            {
                Assert.AreEqual(_flQueue.Dequeue(), i);
            }
        }

        [Test]
        public void TestEnqueueBeforeSize()
        {
            for (int i = 0; i < (QueueSize / 2); i++)
            {
                _flQueue.Enqueue(i);
            }

            Assert.AreEqual(_flQueue.Count, QueueSize / 2);
            Assert.AreEqual(_flQueue.Size, QueueSize);

            for (int i = 0; i < (QueueSize / 2); i++)
            {
                Assert.AreEqual(_flQueue.Dequeue(), i);
            }
        }

        [Test]
        public void TestFirstLast()
        {
            // Fill the queue
            for (int i = 0; i < QueueSize; i++)
            {
                _flQueue.Enqueue(i);
            }

            Assert.AreEqual(_flQueue.First(), 0);
            Assert.AreEqual(_flQueue.Last(), QueueSize - 1);

            _flQueue.Enqueue(-21);

            Assert.AreEqual(_flQueue.First(), 1);
            Assert.AreEqual(_flQueue.Last(), -21);
        }
    }
}
