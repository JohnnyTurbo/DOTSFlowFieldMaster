using NUnit.Framework;
using TMG.FlowField;
using UnityEngine;
using System.Collections.Generic;
using FluentAssertions;

namespace Tests
{
    public class FlowFieldGridTests
    {
        [Test]
        public void Create_Grid()
        {
            FlowFieldGrid testGrid = A.FlowField.WithNodeRadius(0.5f).WithSize(10, 10);
            testGrid.CreateGrid();
            testGrid.grid.Length.Should().Be(100);
        }

        [Test]
        public void Check_Walkable_Node()
        {
            FlowFieldGrid testGrid = A.FlowField.WithNodeRadius(0.5f).WithSize(10, 10);
            testGrid.CreateGrid();
            Node testNode = testGrid.grid[5, 5];

            Node goalNode = testGrid.grid[0, 0];
            testGrid.CreateIntegrationField(goalNode);

            testNode.bestCost.Should().NotBe(ushort.MaxValue);
        }

        [Test]
        public void Check_Impassible_Node()
        {
            FlowFieldGrid testGrid = A.FlowField.WithNodeRadius(0.5f).WithSize(10, 10);
            testGrid.CreateGrid();
            Node testNode = testGrid.grid[5, 5];
            testNode.MakeImpassible();

            Node goalNode = testGrid.grid[0, 0];
            testGrid.CreateIntegrationField(goalNode);

            testNode.bestCost.Should().Be(ushort.MaxValue);
        }

        [Test]
        public void Get_Node_At_Relative_Pos()
		{
            FlowFieldGrid testGrid = A.FlowField.WithNodeRadius(0.5f).WithSize(10, 10);
            testGrid.CreateGrid();

            Vector2Int originVector = new Vector2Int(5, 5);

            List <Node> neighborNodes = testGrid.GetNeighborNodes(originVector, GridDirection.CardinalAndIntercardinalDirections);

            List<Node> expectedNodes = new List<Node>();

            foreach(GridDirection curDirection in GridDirection.CardinalAndIntercardinalDirections)
			{
                Vector2Int curIndex = originVector + curDirection;
                expectedNodes.Add(testGrid.grid[curIndex.x, curIndex.y]);
			}
            neighborNodes.Should().BeEquivalentTo(expectedNodes);
        }

        [Test]
        public void Get_Node_At_Relative_Pos_On_Corner_Nodes()
        {
            FlowFieldGrid testGrid = A.FlowField.WithNodeRadius(0.5f).WithSize(10, 10);
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

            neighborNodes1.Should().BeEquivalentTo(expectedNodes1);
            neighborNodes2.Should().BeEquivalentTo(neighborNodes2);
        }

        [Test]
        public void Test_Integration_Field()
		{
            FlowFieldGrid testGrid = A.FlowField.WithNodeRadius(0.5f).WithSize(10, 10);
            testGrid.CreateGrid();
            Node goalNode = testGrid.grid[0, 0];

            testGrid.CreateIntegrationField(goalNode);

            foreach(Node curNode in testGrid.grid)
			{
                ushort expectedValue = (ushort)(curNode.nodeIndex.x + curNode.nodeIndex.y);
                curNode.bestCost.Should().Be(expectedValue);
			}
        }

        [Test]
        public void Test_Integration_Field_With_Costs()
        {
            FlowFieldGrid testGrid = A.FlowField.WithNodeRadius(0.5f).WithSize(10, 10);
            testGrid.CreateGrid();
            Node goalNode = testGrid.grid[0, 0];
            Node testNode = testGrid.grid[5, 5];
            testNode.IncreaseCost(10);

            testGrid.CreateIntegrationField(goalNode);

            foreach (Node curNode in testGrid.grid)
            {
                if (curNode.Equals(testNode))
                {
                    curNode.bestCost.Should().Be(20);
                    continue;
                }
                ushort expectedValue = (ushort)(curNode.nodeIndex.x + curNode.nodeIndex.y);
                curNode.bestCost.Should().Be(expectedValue);
            }
        }

        [Test]
        public void Test_Getting_Node_From_World_Position()
		{
            FlowFieldGrid testGrid = A.FlowField.WithNodeRadius(0.5f).WithSize(10, 10);
            testGrid.CreateGrid();
            Vector3 worldPos = new Vector3(0.5f, 0, 0.5f);
            Node testNode = testGrid.GetNodeFromWorldPos(worldPos);
            Node expectedNode = testGrid.grid[0, 0];

            testNode.Should().Be(expectedNode);
		}

        [Test]
        public void Test_Getting_Node_From_World_Position_Off_Grid_Min()
        {
            FlowFieldGrid testGrid = A.FlowField.WithNodeRadius(0.5f).WithSize(10, 10);
            testGrid.CreateGrid();
            Vector3 worldPos = new Vector3(-5f, 0, -5f);
            Node testNode = testGrid.GetNodeFromWorldPos(worldPos);
            Node expectedNode = testGrid.grid[0, 0];

            testNode.Should().Be(expectedNode);
        }

        [Test]
        public void Test_Getting_Node_From_World_Position_Off_Grid_Max()
        {
            FlowFieldGrid testGrid = A.FlowField.WithNodeRadius(0.5f).WithSize(10, 10);
            testGrid.CreateGrid();
            Vector3 worldPos = new Vector3(100f, 0, 100f);
            Node testNode = testGrid.GetNodeFromWorldPos(worldPos);
            Node expectedNode = testGrid.grid[9, 9];

            testNode.Should().Be(expectedNode);
        }
    }
}
