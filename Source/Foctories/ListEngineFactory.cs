using Angar.PositionEngine;
using UnityEngine;

namespace Angar.Factory
{
    public class ListEngineFactory : ITargetableFactory
    {
        public float NearRadius { get; set; }
        public float FarRadius { get; set; }

        public ListEngineFactory(float nearRadius, float farRadius)
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
    }
}
