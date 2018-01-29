using UnityEngine;

namespace Angar.PositionEngine
{
    [ExecuteInEditMode]
    public class OctreeEngineSetup : MonoBehaviour, IPoolSystemInitializable, IPoolSystemClearable
    {
        private IPositionUpdaterEngine _engine;

        [SerializeField]
        private Component _updater;

        public float NearRadius;
        public float FarRadius;
        public float WorldStartSize = 70;
        public float WorldNodeSize = 70;

        public IPositionUpdaterComponent Updater
        {
            get { return _updater as IPositionUpdaterComponent; }
            set { _updater = value as Component; }
        }


        public void Initialize()
        {
            if (Updater == null)
                return;

            _engine = new PositionUpdaterEngineOctree(NearRadius, FarRadius, WorldStartSize, WorldNodeSize);
            Updater.Engine = _engine;
        }

        public void Clear()
        {
            _engine = null;
        }


        public void Update()
        {
            if (_engine == null)
                return;

            _engine.NearRadius = NearRadius;
            _engine.FarRadius = FarRadius;
        }

        public void OnValidate()
        {
            if (!(_updater is IPositionUpdaterComponent))
                _updater = null;

            if (NearRadius < 0)
                NearRadius = 0;

            if (FarRadius < 0 && FarRadius < NearRadius)
                FarRadius = NearRadius;

            if (WorldStartSize < 10)
                WorldStartSize = 10;

            if (WorldNodeSize < 1)
                WorldStartSize = 1;
        }
    }
}

