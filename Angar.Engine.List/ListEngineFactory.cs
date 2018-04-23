using UnityEngine;

namespace Angar.Factory
{
    public class ListEngineFactory : IEngineSetupFactory
    {
        public float NearRadius { get; set; }
        public float FarRadius { get; set; }

        [ConstructorName("List engine")]
        public ListEngineFactory([Parameter("Near distance", 0.0f)] float nearRadius, [Parameter("Far distance", 100.0f)] float farRadius)
        {
            NearRadius = nearRadius;
            FarRadius = farRadius;
        }

        public void Create(GameObject target)
        {
            var listEngine = target.AddComponent<ListEngineSetup>();
            listEngine.NearRadius = NearRadius;
            listEngine.FarRadius = FarRadius;
        }

        public void Create(GameObject target, IPositionUpdaterComponent updater)
        {
            var listEngine = target.AddComponent<ListEngineSetup>();
            listEngine.NearRadius = NearRadius;
            listEngine.FarRadius = FarRadius;
            listEngine.Updater = updater;
        }
    }
}
