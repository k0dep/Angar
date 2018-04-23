using UnityEngine;

namespace Angar.Factory
{
    public class OctreeEngineFactory : IEngineSetupFactory
    {
        public float NearRadius { get; set; }
        public float FarRadius { get; set; }
        public float MinimumNodeSize { get; set; }
        public float StartWorldSize { get; set; }

        [ConstructorName("Octree-based enging")]
        public OctreeEngineFactory([Parameter("Near distance", 0.0f)] float nearRadius,
            [Parameter("Far distance", 100.0f)] float farRadius,
            [Parameter("Minimum node size", 70.0f)] float minimumNodeSize,
            [Parameter("Initial world size (optional)", 100.0f)] float startWorldSize)
        {
            NearRadius = nearRadius;
            FarRadius = farRadius;
            MinimumNodeSize = minimumNodeSize;
            StartWorldSize = startWorldSize;
        }


        public void Create(GameObject target, IPositionUpdaterComponent updater)
        {
            var engine = target.AddComponent<OctreeEngineSetup>();
            engine.NearRadius = NearRadius;
            engine.FarRadius = FarRadius;
            engine.WorldNodeSize = MinimumNodeSize;
            engine.WorldStartSize = StartWorldSize;
            engine.Updater = updater;
        }
    }
}
