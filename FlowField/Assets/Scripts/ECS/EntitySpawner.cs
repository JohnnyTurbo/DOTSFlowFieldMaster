using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace TMG.ECSFlowField
{
    public class EntitySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private int numUnitsPerSpawn;
        [SerializeField] private float2 _maxSpawnPos;
        [SerializeField] private float moveSpeed;
        
        private Entity _entityPrefab;
        private EntityManager _entityManager;
        private List<Entity> unitsInGame;
        int colMask;
        private BlobAssetStore blobAssetStore;
        private void Awake()
        {
            
            blobAssetStore = new BlobAssetStore();
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
            _entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(unitPrefab, settings);
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            unitsInGame = new List<Entity>();
            colMask = LayerMask.GetMask("Impassible", "Units");
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                for (int i = 0; i < numUnitsPerSpawn; i++)
                {
                    var newUnit = _entityManager.Instantiate(_entityPrefab);
                    unitsInGame.Add(newUnit);
                    float3 newPosition;
                    do
                    {
                        newPosition = new float3(Random.Range(0f, _maxSpawnPos.x), 0, Random.Range(0, _maxSpawnPos.y));
                        _entityManager.SetComponentData(newUnit, new Translation {Value = newPosition});
                    } while (Physics.OverlapSphere(newPosition, 0.25f, colMask).Length > 0);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                foreach (Entity entity in unitsInGame)
                {
                    _entityManager.DestroyEntity(entity);
                }
                unitsInGame.Clear();
            }
        }

        private void OnDestroy()
        {
            blobAssetStore.Dispose();        
        }
    }
}