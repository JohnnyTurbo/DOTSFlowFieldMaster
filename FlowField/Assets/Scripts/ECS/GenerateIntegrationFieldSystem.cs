using Unity.Entities;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
	public class GenerateIntegrationFieldSystem : SystemBase
	{
		private EntityCommandBufferSystem _ecbSystem;

		protected override void OnCreate()
		{
			_ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
		}

		protected override void OnUpdate()
		{
			var commandBuffer = _ecbSystem.CreateCommandBuffer();

			Entities.ForEach((Entity entity, int entityInQueryIndex, in GenerateIntegrationFieldTag generateIntegrationFieldTag, in DestinationCellData destinationCellData) =>
			{
				commandBuffer.RemoveComponent<GenerateIntegrationFieldTag>(entity);
				UnityEngine.Debug.Log($"Integration. {destinationCellData.destinationIndex.ToString()}");
				
			}).WithoutBurst().Run();
		}

		private static int ToFlatIndex(int2 index2D, int width)
		{
			return width * index2D.x + index2D.y;
		}

		private static int2 To2dIndex(int index, int width)
		{
			int x = index % width;
			int y = index / width;
			return new int2(x, y);
		}
	}
}