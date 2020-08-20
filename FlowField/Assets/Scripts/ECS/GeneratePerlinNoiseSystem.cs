using Unity.Entities;

namespace TMG.ECSFlowField
{
    public class GeneratePerlinNoiseSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((in GeneratePerlinNoiseTag noiseTag) =>
            {
                
            }).Run();
        }
    }
}