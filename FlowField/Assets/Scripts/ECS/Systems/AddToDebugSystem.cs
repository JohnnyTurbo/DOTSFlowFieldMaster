using Unity.Entities;

namespace TMG.ECSFlowField
{
	public class AddToDebugSystem : SystemBase
	{
		private EntityCommandBufferSystem _ecbSystem;

		protected override void OnCreate()
		{
			_ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
		}

		protected override void OnUpdate()
		{
			EntityCommandBuffer commandBuffer = _ecbSystem.CreateCommandBuffer();

			Entities.ForEach((Entity entity, in CellData cellData, in AddToDebugTag addToDebugTag) =>
			{
				GridDebug.instance.AddToList(cellData);
				commandBuffer.RemoveComponent<AddToDebugTag>(entity);
			}).Run();
		}
	}
}