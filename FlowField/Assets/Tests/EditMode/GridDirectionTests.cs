using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMG.FlowField;
using UnityEngine;
using FluentAssertions;

namespace Tests
{
    public class GridDirectionTests
    {
        [Test]
        public void GridDirectionNone()
        {
            //Assert.AreEqual(new Vector2Int(0, 0), GridDirection.None.Vector);
            GridDirection.None.Vector.Should().Be(Vector2Int.zero);
        }

        [Test]
        public void GridDirectionNorth()
        {
            //Assert.AreEqual(new Vector2Int(0, 1), GridDirection.North.Vector);
            GridDirection.North.Vector.Should().Be(Vector2Int.up);
        }

        [Test]
        public void GridDirectionNorthEast()
        {
            //Assert.AreEqual(new Vector2Int(1, 1), GridDirection.NorthEast.Vector);
            GridDirection.NorthEast.Vector.Should().Be(new Vector2Int(1, 1));
        }

        [Test]
        public void GridDirectionEast()
        {
            //Assert.AreEqual(new Vector2Int(1, 0), GridDirection.East.Vector);
            GridDirection.East.Vector.Should().Be(Vector2Int.right);
        }

        [Test]
        public void GridDirectionSouthEast()
        {
            //Assert.AreEqual(new Vector2Int(1, -1), GridDirection.SouthEast.Vector);
            GridDirection.SouthEast.Vector.Should().Be(new Vector2Int(1, -1));
        }

        [Test]
        public void GridDirectionSouth()
        {
            //Assert.AreEqual(new Vector2Int(0, -1), GridDirection.South.Vector);
            GridDirection.South.Vector.Should().Be(Vector2Int.down);
        }

        [Test]
        public void GridDirectionSouthWest()
        {
            //Assert.AreEqual(new Vector2Int(-1, -1), GridDirection.SouthWest.Vector);
            GridDirection.SouthWest.Vector.Should().Be(new Vector2Int(-1, -1));
        }

        [Test]
        public void GridDirectionWest()
        {
            //Assert.AreEqual(new Vector2Int(-1, 0), GridDirection.West.Vector);
            GridDirection.West.Vector.Should().Be(Vector2Int.left);
        }

        [Test]
        public void GridDirectionNorthWest()
        {
            //Assert.AreEqual(new Vector2Int(-1, 1), GridDirection.NorthWest.Vector);
            GridDirection.NorthWest.Vector.Should().Be(new Vector2Int(-1, 1));
        }
    }
}
