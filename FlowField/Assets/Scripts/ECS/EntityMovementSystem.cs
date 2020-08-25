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
        private DynamicBuffer<EntityBufferElement> _entityBuffer;
        private DynamicBuffer<Entity> _gridEntities;
        private static NativeArray<CellData> _cellDatas;
        
        protected override void OnCreate()
        {
            instance = this;
        }

        public void SetSingleton()
        {
            _flowFieldQuery = GetEntityQuery(typeof(FlowFieldData));
            _flowFieldEntity = _flowFieldQuery.GetSingletonEntity();
            _flowFieldData = EntityManager.GetComponentData<FlowFieldData>(_flowFieldEntity);
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

            FlowFieldData flowFieldData = _flowFieldData;
            //NativeArray<CellData> cellDatas = new NativeArray<CellData>(_cellDatas.Length, Allocator.TempJob);
            //cellDatas = _cellDatas;
            JobHandle jobHandle = new JobHandle();
            /*jobHandle = */Entities.ForEach((ref PhysicsVelocity physVelocity, in Translation translation,
                in EntityMovementTag movementTag) =>
            {
                int2 curCellIndex = ECSHelper.GetCellIndexFromWorldPos(translation.Value, flowFieldData.gridSize,
                    flowFieldData.cellRadius * 2);
                
                int flatCurCellIndex = ECSHelper.ToFlatIndex(curCellIndex, flowFieldData.gridSize.y);
                int2 moveDirection = _cellDatas[flatCurCellIndex].bestDirection;
                physVelocity.Linear.xz = moveDirection;
            }).WithoutBurst().Run();//ScheduleParallel(jobHandle);
            //jobHandle.Complete();
            //cellDatas.Dispose();
        }
    }
}