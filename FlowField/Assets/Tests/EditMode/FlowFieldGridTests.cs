using NUnit.Framework;
using TMG.FlowField;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

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

        [Test]
        public void Get_Node_At_Relative_Pos()
		{
            FlowFieldGrid testGrid = new FlowFieldGrid(0.5f, new Vector2Int(10, 10));
            testGrid.CreateGrid();

            Vector2Int originVector = new Vector2Int(5, 5);

            List <Node> neighborNodes = testGrid.GetNeighborNodes(originVector, GridDirection.CardinalAndIntercardinalDirections);

            List<Node> expectedNodes = new List<Node>();

            foreach(GridDirection curDirection in GridDirection.CardinalAndIntercardinalDirections)
			{
                Vector2Int curIndex = originVector + curDirection;
                expectedNodes.Add(testGrid.grid[curIndex.x, curIndex.y]);
			}

            Assert.AreEqual(expectedNodes, neighborNodes);
        }

        [Test]
        public void Get_Node_At_Relative_Pos_On_Corner_Nodes()
        {
            FlowFieldGrid testGrid = new FlowFieldGrid(0.5f, new Vector2Int(10, 10));
            testGrid.CreateGrid();

            Vector2Int originVector1 = new Vector2Int(0, 0);
            Vector2Int originVector2 = new Vector2Int(9, 9);

            List<Node> neighborNodes1 = testGrid.GetNeighborNodes(originVector1, GridDirection.CardinalAndIntercardinalDirections);
            List<Node> neighborNodes2 = testGrid.GetNeighborNodes(originVector2, GridDirection.CardinalAndIntercardinalDirections);

            List<Node> expectedNodes1 = new List<Node>();
            expectedNodes1.Add(testGrid.grid[0, 1]);
            expectedNodes1.Add(testGrid.grid[1, 1]);
            expectedNodes1.Add(testGrid.grid[1, 0]);

            List<Node> expectedNodes2 = new List<Node>();
            expectedNodes2.Add(testGrid.grid[9, 8]);
            expectedNodes2.Add(testGrid.grid[8, 8]);
            expectedNodes2.Add(testGrid.grid[8, 9]);

            Assert.AreEqual(expectedNodes1, neighborNodes1);
            Assert.AreEqual(expectedNodes2, neighborNodes2);
        }

        [Test]
        public void Test_Integration_Field()
		{
            FlowFieldGrid testGrid = new FlowFieldGrid(0.5f, new Vector2Int(10, 10));
            testGrid.CreateGrid();
            Node goalNode = testGrid.grid[0, 0];

            testGrid.CreateIntegrationField(goalNode);

            foreach(Node curNode in testGrid.grid)
			{
                Assert.AreEqual(curNode.nodeIndex.x + curNode.nodeIndex.y, curNode.bestCost);
			}
        }

        [Test]
        public void Test_Integration_Field_With_Costs()
        {
            FlowFieldGrid testGrid = new FlowFieldGrid(0.5f, new Vector2Int(10, 10));
            testGrid.CreateGrid();
            Node goalNode = testGrid.grid[0, 0];
            Node testNode = testGrid.grid[5, 5];
            testNode.IncreaseCost(10);

            testGrid.CreateIntegrationField(goalNode);

            foreach (Node curNode in testGrid.grid)
            {
                if (curNode.Equals(testNode))
                {
                    Assert.AreEqual(20, curNode.bestCost);
                    continue;
                }
                Assert.AreEqual(curNode.nodeIndex.x + curNode.nodeIndex.y, curNode.bestCost);
            }
        }
    }
}
