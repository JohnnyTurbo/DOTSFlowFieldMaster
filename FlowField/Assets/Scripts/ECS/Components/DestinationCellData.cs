using Unity.Entities;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
    public struct DestinationCellData : IComponentData
    {
        public int2 destinationIndex;
    }
}