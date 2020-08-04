using NUnit.Framework;
using TMG.FlowField;
using UnityEngine;
using System.Collections.Generic;
using FluentAssertions;

namespace Tests
{
    public class FlowFieldTests
    {
        [Test]
        public void Create_Grid()
        {
            FlowField testFlowField = A.FlowField.WithCellRadius(0.5f).WithSize(10, 10);
            testFlowField.CreateGrid();
            testFlowField.grid.Length.Should().Be(100);
        }

        [Test]
        public void Check_Walkable_Cell()
        {
            FlowField testFlowField = A.FlowField.WithCellRadius(0.5f).WithSize(10, 10);
            testFlowField.CreateGrid();
            Cell testCell = testFlowField.grid[5, 5];

            Cell destinationCell = testFlowField.grid[0, 0];
            testFlowField.CreateIntegrationField(destinationCell);

            testCell.bestCost.Should().NotBe(ushort.MaxValue);
        }

        [Test]
        public void Check_Impassible_Cell()
        {
            FlowField testFlowField = A.FlowField.WithCellRadius(0.5f).WithSize(10, 10);
            testFlowField.CreateGrid();
            Cell testCell = testFlowField.grid[5, 5];
            testCell.MakeImpassible();

            Cell destinationCell = testFlowField.grid[0, 0];
            testFlowField.CreateIntegrationField(destinationCell);

            testCell.bestCost.Should().Be(ushort.MaxValue);
        }

        [Test]
        public void Get_Cell_At_Relative_Pos()
		{
            FlowField testFlowField = A.FlowField.WithCellRadius(0.5f).WithSize(10, 10);
            testFlowField.CreateGrid();

            Vector2Int originCellIndex = new Vector2Int(5, 5);

            List <Cell> neighborCells = testFlowField.GetNeighborCells(originCellIndex, GridDirection.CardinalAndIntercardinalDirections);

            List<Cell> expectedCells = new List<Cell>();

            foreach(GridDirection curDirection in GridDirection.CardinalAndIntercardinalDirections)
			{
                Vector2Int curIndex = originCellIndex + curDirection;
                expectedCells.Add(testFlowField.grid[curIndex.x, curIndex.y]);
			}
            neighborCells.Should().BeEquivalentTo(expectedCells);
        }

        [Test]
        public void Get_Cells_At_Relative_Pos_On_Corner_Cells()
        {
            FlowField testFlowField = A.FlowField.WithCellRadius(0.5f).WithSize(10, 10);
            testFlowField.CreateGrid();

            Vector2Int originCellIndex1 = new Vector2Int(0, 0);
            Vector2Int originCellIndex2 = new Vector2Int(9, 9);

            List<Cell> neighborCells1 = testFlowField.GetNeighborCells(originCellIndex1, GridDirection.CardinalAndIntercardinalDirections);
            List<Cell> neighborCells2 = testFlowField.GetNeighborCells(originCellIndex2, GridDirection.CardinalAndIntercardinalDirections);

            List<Cell> expectedCells1 = new List<Cell>();
            expectedCells1.Add(testFlowField.grid[0, 1]);
            expectedCells1.Add(testFlowField.grid[1, 1]);
            expectedCells1.Add(testFlowField.grid[1, 0]);

            List<Cell> expectedCells2 = new List<Cell>();
            expectedCells2.Add(testFlowField.grid[9, 8]);
            expectedCells2.Add(testFlowField.grid[8, 8]);
            expectedCells2.Add(testFlowField.grid[8, 9]);

            neighborCells1.Should().BeEquivalentTo(expectedCells1);
            neighborCells2.Should().BeEquivalentTo(neighborCells2);
        }

        [Test]
        public void Test_Integration_Field()
		{
            FlowField testFlowField = A.FlowField.WithCellRadius(0.5f).WithSize(10, 10);
            testFlowField.CreateGrid();
            Cell destinationCell = testFlowField.grid[0, 0];

            testFlowField.CreateIntegrationField(destinationCell);

            foreach(Cell curCell in testFlowField.grid)
			{
                ushort expectedCost = (ushort)(curCell.gridIndex.x + curCell.gridIndex.y);
                curCell.bestCost.Should().Be(expectedCost);
			}
        }

        [Test]
        public void Test_Integration_Field_With_Costs()
        {
            FlowField testFlowField = A.FlowField.WithCellRadius(0.5f).WithSize(10, 10);
            testFlowField.CreateGrid();
            Cell destinationCell = testFlowField.grid[0, 0];
            Cell testCell = testFlowField.grid[5, 5];
            testCell.IncreaseCost(10);

            testFlowField.CreateIntegrationField(destinationCell);

            foreach (Cell curCell in testFlowField.grid)
            {
                if (curCell.Equals(testCell))
                {
                    curCell.bestCost.Should().Be(20);
                    continue;
                }
                ushort expectedValue = (ushort)(curCell.gridIndex.x + curCell.gridIndex.y);
                curCell.bestCost.Should().Be(expectedValue);
            }
        }

        [Test]
        public void Test_Getting_Cell_From_World_Position()
		{
            FlowField testFlowField = A.FlowField.WithCellRadius(0.5f).WithSize(10, 10);
            testFlowField.CreateGrid();
            Vector3 worldPos = new Vector3(0.5f, 0, 0.5f);
            Cell testCell = testFlowField.GetCellFromWorldPos(worldPos);
            Cell expectedCell = testFlowField.grid[0, 0];

            testCell.Should().Be(expectedCell);
		}

        [Test]
        public void Test_Getting_Cell_From_World_Position_Off_Grid_Min()
        {
            FlowField testFlowField = A.FlowField.WithCellRadius(0.5f).WithSize(10, 10);
            testFlowField.CreateGrid();
            Vector3 worldPos = new Vector3(-5f, 0, -5f);
            Cell testCell = testFlowField.GetCellFromWorldPos(worldPos);
            Cell expectedCell = testFlowField.grid[0, 0];

            testCell.Should().Be(expectedCell);
        }

        [Test]
        public void Test_Getting_Cell_From_World_Position_Off_Grid_Max()
        {
            FlowField testFlowField = A.FlowField.WithCellRadius(0.5f).WithSize(10, 10);
            testFlowField.CreateGrid();
            Vector3 worldPos = new Vector3(100f, 0, 100f);
            Cell testCell = testFlowField.GetCellFromWorldPos(worldPos);
            Cell expectedCell = testFlowField.grid[9, 9];

            testCell.Should().Be(expectedCell);
        }
    }
}
