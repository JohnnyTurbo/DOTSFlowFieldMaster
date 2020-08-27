using Unity.Entities;
using UnityEngine;

namespace TMG.ECSFlowField
{
    public class CostFieldHelper : MonoBehaviour
    {
        public static CostFieldHelper instance;
        
        private EntityCommandBufferSystem _ecbSystem;
        private int _terrainMask;

        private void Awake()
        {
            instance = this;
            _terrainMask = LayerMask.GetMask("Impassible", "Terrain");

        }

        public byte EvaluateCost(Vector3 worldPos, float cellRadius)
        {
            byte newCost = 1;
            Vector3 cellHalfExtents = Vector3.one * cellRadius;
            Collider[] obstacles = Physics.OverlapBox(worldPos, cellHalfExtents, Quaternion.identity, _terrainMask);
            bool hasIncreasedCost = false;
            foreach (Collider col in obstacles)
            {
                if (col.gameObject.layer == 8)
                {
                    newCost = byte.MaxValue;
                    break;
                }
                else if (!hasIncreasedCost && col.gameObject.layer == 9)
                {
                    newCost = 5;
                    hasIncreasedCost = true;
                }
            }
            return newCost;
        }
    }
}