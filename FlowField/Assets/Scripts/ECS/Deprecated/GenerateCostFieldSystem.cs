using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace TMG.ECSFlowField
{
	[DisableAutoCreation]
	public class GenerateCostFieldSystem : SystemBase
	{
		private EntityCommandBufferSystem _ecbSystem;
		private BuildPhysicsWorld _buildPhysicsWorld;
		//CollisionWorld collisionWorld;
		private const byte _impassibleTag = 1 << 0;
		private const byte _roughTerrainTag = 1 << 1;

		protected override void OnCreate()
		{
			_ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
			_buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		}

		protected override void OnUpdate()
		{
			var commandBuffer = _ecbSystem.CreateCommandBuffer();//.AsParallelWriter();
			var collisionWorld = _buildPhysicsWorld.PhysicsWorld.CollisionWorld;

			Entities.ForEach((Entity entity, int entityInQueryIndex, ref CellData cellData, in GenerateCostFieldTag costFieldTag) =>
			{
				//commandBuffer.RemoveComponent<GenerateCostFieldTag>(entityInQueryIndex, entity);
				CellSharedData cellSharedData = cellData.cellBlobData.Value;
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

				UnityEngine.Debug.Log($"impasstag {_impassibleTag}, roughTag{_roughTerrainTag}");

				NativeList<int> hitIndices = new NativeList<int>(Allocator.TempJob);

				bool haveHit = collisionWorld.OverlapAabb(input, ref hitIndices);
				if (haveHit)
				{
					//UnityEngine.Debug.Log($"Cell Pos: {cellData.gridIndex.ToString()} hit {hitIndecies.Length} things");
					
					bool hasIncreasedCost = false;
					//for (int i = 0; i < hitIndecies.Length; i++)
					foreach(int curIndex in hitIndices)
					{
						RigidBody rb = collisionWorld.Bodies[curIndex];
						UnityEngine.Debug.Log($"Collided w/{rb.Entity.Index}");
						if (rb.CustomTags == _impassibleTag)
						{
							UnityEngine.Debug.Log("IMPASSIBLE");
							//cellData.cost = byte.MaxValue;
							IncreaseCellCost(ref cellData, byte.MaxValue);
						}
						else if (!hasIncreasedCost && rb.CustomTags.Equals(_roughTerrainTag))
						{
							// Code smell: check for out of range
							// TODO: flexible values to increase cost
							//cellData.cost += 3;
							IncreaseCellCost(ref cellData, 3);
							hasIncreasedCost = true;
						}
					}
				}
				//UnityEngine.Debug.Log($"Cost: {cellData.cost}");
				hitIndices.Dispose();
				commandBuffer.AddComponent<AddToDebugTag>(entity);
			}).Run();

			Entities.ForEach((Entity entity, int entityInQueryIndex, in FlowFieldData flowFieldData, in GenerateCostFieldTag generateCostFieldTag) =>
			{
				commandBuffer.RemoveComponent<GenerateCostFieldTag>(entity);
				commandBuffer.AddComponent<GenerateIntegrationFieldTag>(entity);
			}).Run();
		}

		private static void IncreaseCellCost(ref CellData cellData, int amnt)
		{
			byte cost = cellData.cost;
			if (cost == byte.MaxValue) { return; }
			if (amnt + cost >= 255) { cost = byte.MaxValue; }
			else { cost += (byte)amnt; }
			cellData.cost = cost;
		}
	}
}