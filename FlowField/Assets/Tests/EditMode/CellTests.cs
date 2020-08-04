using NUnit.Framework;
using TMG.FlowField;
using FluentAssertions;

namespace Tests
{
    public class CellTests
    {
        [Test]
        public void Increase_Cost_By_1()
        {
            Cell testCell = A.Cell.IsWalkable().AtZero();

            testCell.IncreaseCost(1);

            testCell.cost.Should().Be(2);
            testCell.isWalkable.Should().BeTrue();
        }

        [Test]
        public void Increase_Cost_By_255()
		{
            Cell testCell = A.Cell.IsWalkable().AtZero();

            testCell.IncreaseCost(255);

            testCell.cost.Should().Be(byte.MaxValue);
            testCell.isWalkable.Should().BeFalse();
        }

        [Test]
        public void Increase_Cost_By_256()
        {
            Cell testCell = A.Cell.IsWalkable().AtZero();

            testCell.IncreaseCost(256);

            testCell.cost.Should().Be(byte.MaxValue);
            testCell.isWalkable.Should().BeFalse();
        }

        [Test]
        public void Make_Impassible()
        {
            Cell testCell = A.Cell.IsWalkable().AtZero();

            testCell.MakeImpassible();

            testCell.cost.Should().Be(byte.MaxValue);
            testCell.isWalkable.Should().BeFalse();
        }
    }
}
