using NUnit.Framework;
using TMG.FlowField;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class FlowFieldGridTests
    {
        [Test]
        public void Create_Grid()
        {
            FlowFieldGrid testGrid = new FlowFieldGrid(0.5f, new Vector2Int(10, 10));
            testGrid.CreateGrid();
            Assert.AreEqual(100, testGrid.grid.Length);
        }

        [Test]
        public void Check_Walkable_Node()
		{
            FlowFieldGrid testGrid = new FlowFieldGrid(0.5f, new Vector2Int(10, 10));
            testGrid.CreateGrid();
            Node testNode = testGrid.grid[5, 5];

            Node goalNode = testGrid.grid[0, 0];
            testGrid.CreateIntegrationField(goalNode);

            Assert.AreNotEqual(ushort.MaxValue, testNode.bestCost);
        }

        [Test]
        public void Check_Impassible_Node()
		{
            FlowFieldGrid testGrid = new FlowFieldGrid(0.5f, new Vector2Int(10, 10));
            testGrid.CreateGrid();
            Node testNode = testGrid.grid[5, 5];
            testNode.MakeImpassible();

            Node goalNode = testGrid.grid[0, 0];
            testGrid.CreateIntegrationField(goalNode);

            Assert.AreEqual(ushort.MaxValue, testNode.bestCost);
        }
    }
}
