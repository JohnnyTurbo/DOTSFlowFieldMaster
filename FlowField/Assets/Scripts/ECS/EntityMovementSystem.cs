using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace TMG.ECSFlowField
{
    public class EntityMovementSystem : SystemBase
    {
        public static EntityMovementSystem instance;
        
        private Entity _flowField;
        private EntityQuery _flowFieldQuery;
        private Entity _flowFieldEntity;
        private FlowFieldData _flowFieldData;
        private DestinationCellData _destinationCellData;
        private DynamicBuffer<EntityBufferElement> _entityBuffer;
        private DynamicBuffer<Entity> _gridEntities;
        private EntityCommandBufferSystem _ecbSystem;
        private static NativeArray<CellData> _cellDatas;
        
        protected override void OnCreate()
        {
            instance = this;
            _ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        }

        public void SetSingleton()
        {
            _flowFieldQuery = GetEntityQuery(typeof(FlowFieldData));
            _flowFieldEntity = _flowFieldQuery.GetSingletonEntity();
            _flowFieldData = EntityManager.GetComponentData<FlowFieldData>(_flowFieldEntity);
            _destinationCellData = EntityManager.GetComponentData<DestinationCellData>(_flowFieldEntity);
            _entityBuffer = EntityManager.GetBuffer<EntityBufferElement>(_flowFieldEntity);
            _gridEntities = _entityBuffer.Reinterpret<Entity>();
            //_cellDatas.Dispose();
            _cellDatas = new NativeArray<CellData>(_gridEntities.Length, Allocator.Persistent);
            for (int i = 0; i < _entityBuffer.Length; i++)
            {
                _cellDatas[i] = GetComponent<CellData>(_entityBuffer[i]);
            }
        }

        protected override void OnDestroy()
        {
            _cellDatas.Dispose();
        }

        protected override void OnUpdate()
        {
            if (_flowFieldEntity.Equals(Entity.Null)) {return;}
            var commandBuffer = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
            float deltaTime = Time.DeltaTime;
            FlowFieldData flowFieldData = _flowFieldData;
            int2 destinationCell = _destinationCellData.destinationIndex;
            //NativeArray<CellData> cellDatas = new NativeArray<CellData>(_cellDatas.Length, Allocator.TempJob);
            //cellDatas = _cellDatas;
            JobHandle jobHandle = new JobHandle();
            jobHandle = Entities.ForEach((Entity entity, int entityInQueryIndex, ref PhysicsVelocity physVelocity, 
                ref EntityMovementData entityMovementData, ref Translation translation) =>
            {
                //PhysicsVelocity newPhysicsVelocity = physVelocity;
                //Translation newTranslation = translation;
                int2 curCellIndex = ECSHelper.GetCellIndexFromWorldPos(translation.Value, flowFieldData.gridSize,
                    flowFieldData.cellRadius * 2);
                if (curCellIndex.Equals(destinationCell))
                {
                    entityMovementData.destinationReached = true;
                }
                int flatCurCellIndex = ECSHelper.ToFlatIndex(curCellIndex, flowFieldData.gridSize.y);
                float2 moveDirection = _cellDatas[flatCurCellIndex].bestDirection;
                float finalMoveSpeed = (entityMovementData.destinationReached ? 1f : entityMovementData.moveSpeed) * deltaTime;
                
                physVelocity.Linear = new float3
                {
                    x = moveDirection.x * finalMoveSpeed,
                    y = 0,
                    z = moveDirection.y * finalMoveSpeed
                };
                translation.Value.y = 0f;
                //commandBuffer.SetComponent(entityInQueryIndex, entity, newPhysicsVelocity);
                //commandBuffer.SetComponent(entityInQueryIndex, entity, newTranslation);
                //physVelocity.Linear.xz = moveDirection * finalMoveSpeed;

            })./*Run();*/ScheduleParallel(jobHandle);
            jobHandle.Complete();
            //cellDatas.Dispose();
        }
    }
}