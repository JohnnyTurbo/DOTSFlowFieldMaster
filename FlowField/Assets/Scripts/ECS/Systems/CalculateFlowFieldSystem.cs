using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
	public class CalculateFlowFieldSystem : SystemBase
	{
		private EntityCommandBufferSystem _ecbSystem;

		protected override void OnCreate()
		{
			_ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
		}
		
		protected override void OnUpdate()
		{
			var commandBuffer = _ecbSystem.CreateCommandBuffer();

			Entities.ForEach((Entity entity, ref DynamicBuffer<EntityBufferElement> buffer, 
				in CalculateFlowFieldTag calculateFlowFieldTag, in DestinationCellData destinationCellData, 
				in FlowFieldData flowFieldData) =>
			{
				commandBuffer.RemoveComponent<CalculateFlowFieldTag>(entity);

				DynamicBuffer<Entity> entityBuffer = buffer.Reinterpret<Entity>();
				NativeArray<CellData> cellDataContainer = new NativeArray<CellData>(entityBuffer.Length, Allocator.TempJob);

				int2 gridSize = flowFieldData.gridSize;

				for (int i = 0; i < entityBuffer.Length; i++)
				{
					cellDataContainer[i] = GetComponent<CellData>(entityBuffer[i]);
				}

				int flatDestinationIndex = FlowFieldHelper.ToFlatIndex(destinationCellData.destinationIndex, gridSize.y);
				CellData destinationCell = cellDataContainer[flatDestinationIndex];
				destinationCell.cost = 0;
				destinationCell.bestCost = 0;
				cellDataContainer[flatDestinationIndex] = destinationCell;

				NativeQueue<int2> indicesToCheck = new NativeQueue<int2>(Allocator.TempJob);
				NativeList<int2> neighborIndices = new NativeList<int2>(Allocator.TempJob);

				indicesToCheck.Enqueue(destinationCellData.destinationIndex);
				
				// Integration Field
				while (indicesToCheck.Count > 0)
				{
					int2 cellIndex = indicesToCheck.Dequeue();
					int cellFlatIndex = FlowFieldHelper.ToFlatIndex(cellIndex, gridSize.y);
					CellData curCellData = cellDataContainer[cellFlatIndex];
					neighborIndices.Clear();
					FlowFieldHelper.GetNeighborIndices(cellIndex, GridDirection.CardinalDirections, gridSize, ref neighborIndices);
					foreach (int2 neighborIndex in neighborIndices)
					{
						int flatNeighborIndex = FlowFieldHelper.ToFlatIndex(neighborIndex, gridSize.y);
						CellData neighborCellData = cellDataContainer[flatNeighborIndex];
						if (neighborCellData.cost == byte.MaxValue)
						{
							continue;
						}

						if (neighborCellData.cost + curCellData.bestCost < neighborCellData.bestCost)
						{
							neighborCellData.bestCost = (ushort) (neighborCellData.cost + curCellData.bestCost);
							cellDataContainer[flatNeighborIndex] = neighborCellData;
							indicesToCheck.Enqueue(neighborIndex);
						}
					}
				}

				// Flow Field
				for (int i = 0; i < cellDataContainer.Length; i++)
				{
					CellData curCullData = cellDataContainer[i];
					neighborIndices.Clear();
					FlowFieldHelper.GetNeighborIndices(curCullData.gridIndex, GridDirection.AllDirections, gridSize, ref neighborIndices);
					ushort bestCost = curCullData.bestCost;
					int2 bestDirection = int2.zero;
					foreach (int2 neighborIndex in neighborIndices)
					{
						int flatNeighborIndex = FlowFieldHelper.ToFlatIndex(neighborIndex, gridSize.y);
						CellData neighborCellData = cellDataContainer[flatNeighborIndex];
						if (neighborCellData.bestCost < bestCost)
						{
							bestCost = neighborCellData.bestCost;
							bestDirection = neighborCellData.gridIndex - curCullData.gridIndex;
						}
					}
					curCullData.bestDirection = bestDirection;
					cellDataContainer[i] = curCullData;
				}

				GridDebug.instance.ClearList();

				for (int i = 0; i < entityBuffer.Length; i++)
				{
					commandBuffer.SetComponent(entityBuffer[i], cellDataContainer[i]);
					commandBuffer.AddComponent<AddToDebugTag>(entityBuffer[i]);
				}

				neighborIndices.Dispose();
				cellDataContainer.Dispose();
				indicesToCheck.Dispose();
				commandBuffer.AddComponent<CompleteFlowFieldTag>(entity);
			}).Run();
		}
	}
}