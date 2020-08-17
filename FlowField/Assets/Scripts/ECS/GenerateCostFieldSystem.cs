﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine.Rendering;

namespace TMG.ECSFlowField
{
	public class GenerateCostFieldSystem : SystemBase
	{
		EntityCommandBufferSystem ecbSystem;
		BuildPhysicsWorld buildPhysicsWorld;
		//CollisionWorld collisionWorld;
		static readonly byte impassibleTag = 1 << 0;
		static readonly byte roughTerrainTag = 1 << 1;

		protected override void OnCreate()
		{
			ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
			buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		}

		protected override void OnStartRunning()
		{
			//collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
		}

		protected override void OnUpdate()
		{
			var commandBuffer = ecbSystem.CreateCommandBuffer();//.AsParallelWriter();
			var collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;

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

				UnityEngine.Debug.Log($"impasstag {impassibleTag}, roughTag{roughTerrainTag}");

				NativeList<int> hitIndecies = new NativeList<int>(Allocator.TempJob);

				bool haveHit = collisionWorld.OverlapAabb(input, ref hitIndecies);
				if (haveHit)
				{
					//UnityEngine.Debug.Log($"Cell Pos: {cellData.gridIndex.ToString()} hit {hitIndecies.Length} things");
					//foreach(int curIndex in hitIndecies)
					bool hasIncreasedCost = false;
					for (int i = 0; i < hitIndecies.Length; i++)
					{
						RigidBody rb = collisionWorld.Bodies[i];
						UnityEngine.Debug.Log($"Collided w/{rb.Entity.Index}");
						if (rb.CustomTags == impassibleTag)
						{
							UnityEngine.Debug.Log("IMPASSIBLE");
							//cellData.cost = byte.MaxValue;
							IncreaseCellCost(ref cellData, byte.MaxValue);
						}
						else if (!hasIncreasedCost && rb.CustomTags.Equals(roughTerrainTag))
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
				hitIndecies.Dispose();
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