using NUnit.Framework;
using UnityEngine;
using TMG.FlowField;

namespace Tests
{
    public class NodeTests
    {
        [Test]
        public void Increase_Cost_By_1()
        {
            Node testNode = new Node(true, Vector3.zero, Vector2Int.zero);

            testNode.IncreaseCost(1);

            Assert.AreEqual(2, testNode.cost);
            Assert.IsTrue(testNode.walkable);
        }

        [Test]
        public void Increase_Cost_By_255()
		{
            Node testNode = new Node(true, Vector3.zero, Vector2Int.zero);

            testNode.IncreaseCost(255);

            Assert.AreEqual(255, testNode.cost);
            Assert.IsFalse(testNode.walkable);
        }

        [Test]
        public void Increase_Cost_By_256()
        {
            Node testNode = new Node(true, Vector3.zero, Vector2Int.zero);

            testNode.IncreaseCost(256);

            Assert.AreEqual(255, testNode.cost);
            Assert.IsFalse(testNode.walkable);
        }

        [Test]
        public void Make_Impassible()
        {
            Node testNode = new Node(true, Vector3.zero, Vector2Int.zero);

            testNode.MakeImpassible();

            Assert.AreEqual(255, testNode.cost);
            Assert.IsFalse(testNode.walkable);
        }
    }
}
