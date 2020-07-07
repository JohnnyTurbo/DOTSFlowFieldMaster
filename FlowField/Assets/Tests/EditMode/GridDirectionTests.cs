using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMG.FlowField;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GridDirectionTests
    {
        [Test]
        public void GridDirectionNone()
        {
            Assert.AreEqual(new Vector2Int(0, 0), GridDirection.None.Vector);
        }

        [Test]
        public void GridDirectionNorth()
        {
            Assert.AreEqual(new Vector2Int(0, 1), GridDirection.North.Vector);
        }

        [Test]
        public void GridDirectionNorthEast()
        {
            Assert.AreEqual(new Vector2Int(1, 1), GridDirection.NorthEast.Vector);
        }

        [Test]
        public void GridDirectionEast()
        {
            Assert.AreEqual(new Vector2Int(1, 0), GridDirection.East.Vector);
        }

        [Test]
        public void GridDirectionSouthEast()
        {
            Assert.AreEqual(new Vector2Int(1, -1), GridDirection.SouthEast.Vector);
        }

        [Test]
        public void GridDirectionSouth()
        {
            Assert.AreEqual(new Vector2Int(0, -1), GridDirection.South.Vector);
        }

        [Test]
        public void GridDirectionSouthWest()
        {
            Assert.AreEqual(new Vector2Int(-1, -1), GridDirection.SouthWest.Vector);
        }

        [Test]
        public void GridDirectionWest()
        {
            Assert.AreEqual(new Vector2Int(-1, 0), GridDirection.West.Vector);
        }

        [Test]
        public void GridDirectionNorthWest()
        {
            Assert.AreEqual(new Vector2Int(-1, 1), GridDirection.NorthWest.Vector);
        }
    }
}
