using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TMG.ECSFlowField
{
    [DisableAutoCreation]
    public class GeneratePerlinNoiseSystem : SystemBase
    {
        //private Texture2D _perlinTexture;
        private EntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        }
        protected override void OnUpdate()
        {
            var commandBuffer = _ecbSystem.CreateCommandBuffer();
            Entities.ForEach((Entity entity, int entityInQueryIndex, ref FlowFieldData flowFieldData, in GeneratePerlinNoiseTag noiseTag) =>
            {
                DynamicBuffer<PerlinNoiseBufferElement> buffer = commandBuffer.AddBuffer<PerlinNoiseBufferElement>(entity);
                DynamicBuffer<float> floatBuffer = buffer.Reinterpret<float>();

                /*float2 perlinOffset = new float2(Random.Range(0, 99999), Random.Range(0, 99999));*/
                float2 perlinOffset = new float2(0, 0);
                int2 perlinTextureSize = new int2(40, 22);
                //_perlinTexture = new Texture2D(perlinTextureSize.x, perlinTextureSize.y);
                //noiseData.heightMap = new float[perlinTextureSize.x,perlinTextureSize.y];
                float minSample = 1;
                float maxSample = 0;
                float noiseScale = flowFieldData.noiseScale;
                
                for (int x = 0; x < perlinTextureSize.x; x++)
                {
                    for (int y = 0; y < perlinTextureSize.y; y++)
                    {
                        float xCoord = (float) x / perlinTextureSize.x * noiseScale + perlinOffset.x;
                        float yCoord = (float) y / perlinTextureSize.y * noiseScale + perlinOffset.y;

                        float sample = Mathf.PerlinNoise(x, y);
                        floatBuffer.Add(sample);

                        minSample = math.min(sample, minSample);
                        maxSample = math.max(sample, maxSample);

                        //noiseData.heightMap[x, y] = sample;

                        //Color perlinColor = new Color(sample, sample, sample);

                        //_perlinTexture.SetPixel(x, y, perlinColor);
                    }
                }
                Debug.Log($"Min: {minSample}\nMax: {maxSample}");
                commandBuffer.RemoveComponent<GeneratePerlinNoiseTag>(entity);

            }).Run();
        }
    }
}