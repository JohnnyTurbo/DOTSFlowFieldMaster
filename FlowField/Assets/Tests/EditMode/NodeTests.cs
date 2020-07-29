using NUnit.Framework;
using TMG.FlowField;
using FluentAssertions;

namespace Tests
{
    public class NodeTests
    {
        [Test]
        public void Increase_Cost_By_1()
        {
            Node testNode = A.Node.IsWalkable().AtZero();

            testNode.IncreaseCost(1);

            testNode.cost.Should().Be(2);
            testNode.walkable.Should().BeTrue();
        }

        [Test]
        public void Increase_Cost_By_255()
		{
            Node testNode = A.Node.IsWalkable().AtZero();

            testNode.IncreaseCost(255);

            testNode.cost.Should().Be(byte.MaxValue);
            testNode.walkable.Should().BeFalse();
        }

        [Test]
        public void Increase_Cost_By_256()
        {
            Node testNode = A.Node.IsWalkable().AtZero();

            testNode.IncreaseCost(256);

            testNode.cost.Should().Be(byte.MaxValue);
            testNode.walkable.Should().BeFalse();
        }

        [Test]
        public void Make_Impassible()
        {
            Node testNode = A.Node.IsWalkable().AtZero();

            testNode.MakeImpassible();

            testNode.cost.Should().Be(byte.MaxValue);
            testNode.walkable.Should().BeFalse();
        }
    }
}
