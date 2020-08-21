using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.ECSFlowField
{
    [DisableAutoCreation]
    public class GenerateCostFieldFromPerlinNoiseSystem : SystemBase
    {
        private EntityQuery _perlinNoiseDataQuery;

        private EntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var commandBuffer = _ecbSystem.CreateCommandBuffer();
            _perlinNoiseDataQuery = GetEntityQuery(typeof(FlowFieldData));
            Entity perlinNoiseEntity = _perlinNoiseDataQuery.GetSingletonEntity();
            //PerlinNoiseData perlinNoiseData = EntityManager.GetComponentData<PerlinNoiseData>(perlinNoiseEntity);
            DynamicBuffer<PerlinNoiseBufferElement> perlinNoiseBufferElements = EntityManager.GetBuffer<PerlinNoiseBufferElement>(perlinNoiseEntity);
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref CellData cellData, in GenerateCostFieldTag costFieldTag) =>
            {
                int2 gridIndex = cellData.gridIndex;
                int flatIndex = ((gridIndex.x + 1) + (gridIndex.y + 1)) - 1;
                float height = perlinNoiseBufferElements[flatIndex].height;
                float scaledHeight = height * 255f;
                int testInt = (int) math.round(scaledHeight);
                byte testByte = (byte) math.clamp(testInt,1, 255);
                cellData.cost = testByte;
                commandBuffer.AddComponent<AddToDebugTag>(entity);
                commandBuffer.RemoveComponent<GenerateCostFieldTag>(entity);

            }).Run();
        }
    }
}