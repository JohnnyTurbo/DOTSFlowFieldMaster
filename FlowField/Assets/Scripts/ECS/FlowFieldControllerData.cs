using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace TMG.ECSFlowField
{
    [GenerateAuthoringComponent]
    public struct FlowFieldControllerData : IComponentData
    {
        public int2 gridSize;
        public float cellRadius;
        public CollisionFilter colFilt;
        public float noiseScale;
    }
}