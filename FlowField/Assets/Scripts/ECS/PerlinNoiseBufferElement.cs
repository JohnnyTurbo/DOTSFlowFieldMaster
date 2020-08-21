using Unity.Entities;

namespace TMG.ECSFlowField
{
    [InternalBufferCapacity(250)]
    public struct PerlinNoiseBufferElement : IBufferElementData
    {
        public float height;

        public static implicit operator float(PerlinNoiseBufferElement perlinNoiseBufferElement)
        {
            return perlinNoiseBufferElement.height;
        }

        public static implicit operator PerlinNoiseBufferElement(float height)
        {
            return new PerlinNoiseBufferElement {height = height};
        }
    }
}