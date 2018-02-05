using Angar.PositionEngine;
using UnityEngine;

namespace Angar.Factory
{
    public class OctreeEngineFactory : ITargetableFactory
    {
        public float NearRadius { get; set; }
        public float FarRadius { get; set; }
        public float MinimumNodeSize { get; set; }
        public float StartWorldSize { get; set; }

        public OctreeEngineFactory(float nearRadius, float farRadius, float minimumNodeSize, float startWorldSize)
        {
            NearRadius = nearRadius;
            FarRadius = farRadius;
            MinimumNodeSize = minimumNodeSize;
            StartWorldSize = startWorldSize;
        }


        public void Create(GameObject target)
        {
            var engine = target.AddComponent<OctreeEngineSetup>();
            engine.NearRadius = NearRadius;
            engine.FarRadius = FarRadius;
            engine.WorldNodeSize = MinimumNodeSize;
            engine.WorldStartSize = StartWorldSize;
        }
    }
}
