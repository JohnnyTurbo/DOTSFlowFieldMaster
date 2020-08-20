using Unity.Entities;
using Unity.Mathematics;

namespace TMG.ECSFlowField
{
    public struct PerlinNoiseData : IComponentData
    {
        public float2 perlinTextureSize;
        public bool randomizeNoiseOffset;
        public float2 perlinOffset;
        public float noiseScale;
        public int2 perlinGridStepSize;
    }
}