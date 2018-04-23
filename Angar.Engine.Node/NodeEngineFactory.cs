using UnityEngine;

namespace Angar.Factory
{
    public class NodeEngineFactory : IEngineSetupFactory
    {
        public float NearRadius { get; set; }
        public float FarRadius { get; set; }
        public float NodeSize { get; set; }
        public bool UseMultithreading { get; set; }

        [ConstructorName("Node-based engine")]
        public NodeEngineFactory([Parameter("Near distance", 0.0f)] float nearRadius,
            [Parameter("Far distance", 100.0f)] float farRadius,
            [Parameter("Node size", 30.0f)] float nodeSize,
            [Parameter("Use multithreading", true)] bool useMultithreading)
        {
            NearRadius = nearRadius;
            FarRadius = farRadius;
            NodeSize = nodeSize;
            UseMultithreading = useMultithreading;
        }

        public void Create(GameObject target, IPositionUpdaterComponent updater)
        {
            var engine = target.AddComponent<NodeEngineSetup>();
            engine.NearRadius = NearRadius;
            engine.FarRadius = FarRadius;
            engine.NodeSize = NodeSize;
            engine.UseMultithreading = UseMultithreading;
            engine.Updater = updater;
        }
    }
}
