using Unity.Entities;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
    [GenerateAuthoringComponent]
    public struct FlowFieldControllerData : IComponentData
    {
        public int2 gridSize;
        public float cellRadius;
    }
}