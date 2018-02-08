using Angar.PositionEngine;
using UnityEngine;

namespace Angar.Factory
{
    public class OctreeEngineFactory : IEngineSetupFactory
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

    public interface IEngineSetupFactory
    {
        void Create(GameObject target, IPositionUpdaterComponent updater);
    }
}
