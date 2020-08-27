using Unity.Entities;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
	public struct CellData : IComponentData
	{
		public float3 worldPos;
		public int2 gridIndex;
		public byte cost;
		public ushort bestCost;
		public int2 bestDirection;
	}
}
