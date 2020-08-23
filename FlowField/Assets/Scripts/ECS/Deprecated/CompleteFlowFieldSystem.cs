using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
    [DisableAutoCreation]
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
            
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<EntityBufferElement> buffer,
                in CompleteFlowFieldTag completeFlowFieldTag, in FlowFieldData flowFieldData) =>
            {
                commandBuffer.RemoveComponent<CompleteFlowFieldTag>(entity);
                DynamicBuffer<Entity> entityBuffer = buffer.Reinterpret<Entity>();
                NativeArray<CellData> cellDatas = new NativeArray<CellData>(entityBuffer.Length, Allocator.TempJob);

                int2 gridSize = flowFieldData.gridSize;

                for (int i = 0; i < entityBuffer.Length; i++)
                {
                    cellDatas[i] = GetComponent<CellData>(entityBuffer[i]);
                }
                
                
            }).Run();
        }
    }
}