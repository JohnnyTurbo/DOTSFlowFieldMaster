using Unity.Entities;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
	public struct FlowFieldData : IComponentData
	{ 
		public int2 gridSize;
		public float cellRadius;
		public float noiseScale;
	}
}
