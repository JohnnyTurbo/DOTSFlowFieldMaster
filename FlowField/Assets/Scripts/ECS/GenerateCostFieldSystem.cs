using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace TMG.ECSFlowField
{
	public class GenerateCostFieldSystem : SystemBase
	{
		EntityCommandBufferSystem ecbSystem;
		BuildPhysicsWorld buildPhysicsWorld;
		CollisionWorld collisionWorld;

		protected override void OnCreate()
		{
			ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
			buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		}

		protected override void OnStartRunning()
		{
			collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
		}

		protected override void OnUpdate()
		{
			var commandBuffer = ecbSystem.CreateCommandBuffer();//.AsParallelWriter();

			Entities.ForEach((Entity entity, int entityInQueryIndex, ref CellData cellData, in CellSharedData cellSharedData, in GenerateCostFieldTag costFieldTag) =>
			{
				//commandBuffer.RemoveComponent<GenerateCostFieldTag>(entityInQueryIndex, entity);
				commandBuffer.RemoveComponent<GenerateCostFieldTag>(entity);
				float3 aabbMin = cellData.worldPos - cellSharedData.halfExtents;
				float3 aabbMax = cellData.worldPos + cellSharedData.halfExtents;

				Aabb aabb = new Aabb
				{
					Min = aabbMin,
					Max = aabbMax
				};

				//UnityEngine.Debug.Log($"World Pos: {cellData.worldPos.ToString()}\nCenter: {aabb.Center.ToString()}\nFaxtenz: {aabb.Extents}");

				OverlapAabbInput input = new OverlapAabbInput()
				{
					Aabb = aabb,
					Filter = cellSharedData.costFieldFilter
				};

				NativeList<int> hitIndecies = new NativeList<int>(Allocator.TempJob);

				bool haveHit = collisionWorld.OverlapAabb(input, ref hitIndecies);
				if (haveHit)
				{
					UnityEngine.Debug.Log($"Hitta");
				}
				hitIndecies.Dispose();
			}).WithoutBurst().Run();
		}
	}
}