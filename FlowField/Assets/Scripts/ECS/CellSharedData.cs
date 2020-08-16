using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace TMG.ECSFlowField
{
	public struct CellSharedData : ISharedComponentData
	{
		public float cellRadius;
		public float cellDiameter;
		public float3 halfExtents;
		public CollisionFilter costFieldFilter;
	}
}