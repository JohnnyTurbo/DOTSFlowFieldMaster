using Unity.Collections;
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

			Entities.ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<EntityBufferElement> buffer,
				in GenerateIntegrationFieldTag generateIntegrationFieldTag,
				in DestinationCellData destinationCellData, in FlowFieldData flowFieldData) =>
			{
				commandBuffer.RemoveComponent<GenerateIntegrationFieldTag>(entity);

				DynamicBuffer<Entity> entityBuffer = buffer.Reinterpret<Entity>();
				NativeArray<CellData> cellDatas = new NativeArray<CellData>(entityBuffer.Length, Allocator.TempJob);

				int2 gridSize = flowFieldData.gridSize;

				for (int i = 0; i < entityBuffer.Length; i++)
				{
					cellDatas[i] = GetComponent<CellData>(entityBuffer[i]);
				}

				int destIndex = ECSHelper.ToFlatIndex(destinationCellData.destinationIndex, gridSize.y);
				CellData destinationCell = cellDatas[destIndex];
				destinationCell.cost = 0;
				destinationCell.bestCost = 0;
				cellDatas[destIndex] = destinationCell;

				NativeQueue<int2> cellsToCheck = new NativeQueue<int2>(Allocator.TempJob);
				NativeList<int2> neighborIndices = new NativeList<int2>(Allocator.TempJob);

				cellsToCheck.Enqueue(destinationCellData.destinationIndex);
				while (cellsToCheck.Count > 0)
				{
					int2 curCellIndex = cellsToCheck.Dequeue();
					int curFlatIndex = ECSHelper.ToFlatIndex(curCellIndex, gridSize.y);
					CellData curCellData = cellDatas[curFlatIndex];
					neighborIndices.Clear();
					ECSHelper.GetNeighborIndices(curCellIndex, GridDirection.CardinalDirections, gridSize, ref neighborIndices);
					foreach (int2 neighborIndex in neighborIndices)
					{
						int flatNeighborIndex = ECSHelper.ToFlatIndex(neighborIndex, gridSize.y);
						CellData neighborCellData = cellDatas[flatNeighborIndex];
						if (neighborCellData.cost == byte.MaxValue)
						{
							continue;
						}

						if (neighborCellData.cost + curCellData.bestCost < neighborCellData.bestCost)
						{
							neighborCellData.bestCost = (ushort) (neighborCellData.cost + curCellData.bestCost);
							cellDatas[flatNeighborIndex] = neighborCellData;
							cellsToCheck.Enqueue(neighborIndex);
						}
					}
				}

				for (int i = 0; i < cellDatas.Length; i++)
				{
					CellData curCullData = cellDatas[i];
					neighborIndices.Clear();
					ECSHelper.GetNeighborIndices(curCullData.gridIndex, GridDirection.AllDirections, gridSize, ref neighborIndices);
					ushort bestCost = curCullData.bestCost;
					int2 bestDirection = int2.zero;
					foreach (int2 neighborIndex in neighborIndices)
					{
						int flatNeighborIndex = ECSHelper.ToFlatIndex(neighborIndex, gridSize.y);
						CellData neighborCellData = cellDatas[flatNeighborIndex];
						if (neighborCellData.bestCost < bestCost)
						{
							bestCost = neighborCellData.bestCost;
							bestDirection = neighborCellData.gridIndex - curCullData.gridIndex;
						}
					}
					curCullData.bestDirection = bestDirection;
					cellDatas[i] = curCullData;
				}

				GridDebug.instance.ClearList();

				for (int i = 0; i < entityBuffer.Length; i++)
				{
					commandBuffer.SetComponent(entityBuffer[i], cellDatas[i]);
					commandBuffer.AddComponent<AddToDebugTag>(entityBuffer[i]);
				}

				neighborIndices.Dispose();
				cellDatas.Dispose();
				cellsToCheck.Dispose();
			}).Run();
		}
	}
}