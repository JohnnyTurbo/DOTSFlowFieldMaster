using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace TMG.ECSFlowField
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EntityMovementSystem : SystemBase
    {
        public static EntityMovementSystem instance;
        
        private EntityQuery _flowFieldQuery;
        private Entity _flowFieldEntity;
        private FlowFieldData _flowFieldData;
        private DestinationCellData _destinationCellData;
        private DynamicBuffer<EntityBufferElement> _entityBuffer;
        private DynamicBuffer<Entity> _gridEntities;
        private static NativeArray<CellData> _cellDataContainer;
        
        protected override void OnCreate()
        {
            instance = this;
        }

        public void SetMovementValues()
        {
            _flowFieldQuery = GetEntityQuery(typeof(FlowFieldData));
            _flowFieldEntity = _flowFieldQuery.GetSingletonEntity();
            _flowFieldData = EntityManager.GetComponentData<FlowFieldData>(_flowFieldEntity);
            _destinationCellData = EntityManager.GetComponentData<DestinationCellData>(_flowFieldEntity);
            _entityBuffer = EntityManager.GetBuffer<EntityBufferElement>(_flowFieldEntity);
            _gridEntities = _entityBuffer.Reinterpret<Entity>();
            
            if (_cellDataContainer.IsCreated)
            {
                _cellDataContainer.Dispose();
            }
            _cellDataContainer = new NativeArray<CellData>(_gridEntities.Length, Allocator.Persistent);
            
            for (int i = 0; i < _entityBuffer.Length; i++)
            {
                _cellDataContainer[i] = GetComponent<CellData>(_entityBuffer[i]);
            }
            
            Entities.ForEach((ref EntityMovementData entityMovementData) =>
            {
                entityMovementData.destinationReached = false;
            }).Run();
        }

        protected override void OnUpdate()
        {
            if (_flowFieldEntity.Equals(Entity.Null)) {return;}
            float deltaTime = Time.DeltaTime;
            FlowFieldData flowFieldData = _flowFieldData;
            int2 destinationCell = _destinationCellData.destinationIndex;
            JobHandle jobHandle = new JobHandle();
            jobHandle = Entities.ForEach((ref PhysicsVelocity physVelocity, ref EntityMovementData entityMovementData, 
                ref Translation translation) =>
            {
                int2 curCellIndex = FlowFieldHelper.GetCellIndexFromWorldPos(translation.Value, flowFieldData.gridSize,
                    flowFieldData.cellRadius * 2);
                
                if (curCellIndex.Equals(destinationCell))
                {
                    entityMovementData.destinationReached = true;
                }
                
                int flatCurCellIndex = FlowFieldHelper.ToFlatIndex(curCellIndex, flowFieldData.gridSize.y);
                float2 moveDirection = _cellDataContainer[flatCurCellIndex].bestDirection;
                float finalMoveSpeed = (entityMovementData.destinationReached ? entityMovementData.destinationMoveSpeed : entityMovementData.moveSpeed) * deltaTime;
                
                physVelocity.Linear.xz = moveDirection * finalMoveSpeed;
                translation.Value.y = 0f;

            }).ScheduleParallel(jobHandle);
            jobHandle.Complete();
        }
        
        protected override void OnDestroy()
        {
            _cellDataContainer.Dispose();
        }
    }
}