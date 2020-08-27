using Unity.Entities;

namespace TMG.ECSFlowField
{
    public struct EntityMovementData : IComponentData
    {
        public float moveSpeed;
        public float destinationMoveSpeed;
        public bool destinationReached;
    }
}