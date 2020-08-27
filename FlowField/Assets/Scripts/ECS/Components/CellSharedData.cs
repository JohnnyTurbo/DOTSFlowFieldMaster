using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace TMG.ECSFlowField
{
	public struct CellSharedData : IComponentData
	{
		public float cellRadius;
		public float cellDiameter;
		public float3 halfExtents;
		public CollisionFilter costFieldFilter;
	}
}