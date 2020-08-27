using Unity.Entities;

namespace TMG.ECSFlowField
{
    public class CompleteFlowFieldSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _ecbSystem.CreateCommandBuffer();
            
            Entities.ForEach((Entity entity, in CompleteFlowFieldTag completeFlowFieldTag, in FlowFieldData flowFieldData) =>
            {
                commandBuffer.RemoveComponent<CompleteFlowFieldTag>(entity);
                EntityMovementSystem.instance.SetMovementValues();
            }).Run();
        }
    }
}