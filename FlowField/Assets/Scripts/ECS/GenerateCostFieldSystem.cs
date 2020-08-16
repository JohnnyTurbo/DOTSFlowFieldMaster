using Unity.Entities;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
	public class GenerateCostFieldSystem : SystemBase
	{
		EntityCommandBufferSystem ecbSystem;

		protected override void OnCreate()
		{
			ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
		}

		protected override void OnUpdate()
		{
			/*var commandBuffer = ecbSystem.CreateCommandBuffer().AsParallelWriter();
			float3 cellHalfExtents = new float3(0.5f, 0.5f, 0.5f);

			Entities.ForEach((Entity entity, int entityInQueryIndex, ref CellData cellData, in GenerateCostFieldTag costFieldTag) =>
			{
				commandBuffer.RemoveComponent<GenerateCostFieldTag>(entityInQueryIndex, entity);
				
				

			}).ScheduleParallel();*/
		}
	}
}