using Unity.Entities;

namespace TMG.ECSFlowField
{
	public class AddToDebugSystem : SystemBase
	{
		EntityCommandBufferSystem ecbSystem;

		protected override void OnCreate()
		{
			ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
		}

		protected override void OnUpdate()
		{
			var commandBuffer = ecbSystem.CreateCommandBuffer();

			Entities.ForEach((Entity entity, int entityInQueryIndex, in CellData cellData, in AddToDebugTag addToDebugTag) =>
			{
				GridDebug.instance.gridCellData.Add(cellData);
				commandBuffer.RemoveComponent<AddToDebugTag>(entity);
				
			}).WithoutBurst().Run();
		}
	}
}