using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
    public static class ECSHelper
    {
        public static void GetNeighborIndices(int2 originIndex, List<GridDirection> directions, int2 gridSize, ref NativeList<int2> results)
        {
            foreach (int2 curDirection in directions)
            {
                int2 neighborIndex = GetIndexAtRelativePosition(originIndex, curDirection, gridSize);
				
                if (neighborIndex.x >= 0)
                {
                    results.Add(neighborIndex);
                }
            }
        }

        public static int2 GetIndexAtRelativePosition(int2 originPos, int2 relativePos, int2 gridSize)
        {
			
            int2 finalPos = originPos + relativePos;
            if (finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
            {
                return new int2(-1, -1);
            }
            else
            {
                return finalPos;
            }
        }

        public static int ToFlatIndex(int2 index2D, int height)
        {
            return height * index2D.x + index2D.y;
        }

        public static int2 GetCellIndexFromWorldPos(float3 worldPos, int2 gridSize, float cellDiameter)
        {
            float percentX = worldPos.x / (gridSize.x * cellDiameter);
            float percentY = worldPos.z / (gridSize.y * cellDiameter);

            percentX = math.clamp(percentX, 0f, 1f);
            percentY = math.clamp(percentY, 0f, 1f);
            
            int2 cellIndex = new int2
            {
                x = math.clamp((int) math.floor((gridSize.x) * percentX), 0, gridSize.x - 1),
                y = math.clamp((int) math.floor((gridSize.y) * percentY), 0, gridSize.y - 1)
            };

            return cellIndex;
        }
    }
}