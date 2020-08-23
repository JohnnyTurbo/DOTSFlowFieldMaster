using Unity.Entities;

namespace TMG.ECSFlowField
{
	[InternalBufferCapacity(250)]
	public struct GridCellBufferElement : IBufferElementData
	{
		public CellData cell;

		public static implicit operator CellData (GridCellBufferElement gridCellBufferElement)
		{
			return gridCellBufferElement.cell;
		}

		public static implicit operator GridCellBufferElement(CellData cellData)
		{
			return new GridCellBufferElement { cell = cellData };
		}
	}
}