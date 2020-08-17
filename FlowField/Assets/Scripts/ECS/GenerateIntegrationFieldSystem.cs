using Unity.Entities;

namespace TMG.ECSFlowField
{
	public class GenerateIntegrationFieldSystem : SystemBase
	{
		EntityCommandBufferSystem ecbSystem;

		protected override void OnCreate()
		{
			ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
		}

		protected override void OnUpdate()
		{
			var commandBuffer = ecbSystem.CreateCommandBuffer();

			Entities.ForEach((Entity entity, int entityInQueryIndex, in GenerateIntegrationFieldTag generateIntegrationFieldTag) =>
			{
				commandBuffer.RemoveComponent<GenerateIntegrationFieldTag>(entity);
				UnityEngine.Debug.Log("Integration");
			}).WithoutBurst().Run();
		}
	}
}