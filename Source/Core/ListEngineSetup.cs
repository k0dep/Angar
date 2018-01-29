using UnityEngine;

namespace Angar.PositionEngine
{
    [ExecuteInEditMode]
    public class ListEngineSetup : MonoBehaviour, IPoolSystemInitializable, IPoolSystemClearable
    {
        private IPositionUpdaterEngine _engine;

        [SerializeField]
        private Component _updater;

        public float NearRadius;
        public float FarRadius;

#if UNITY_EDITOR
        public bool debug = false;
#endif

        public IPositionUpdaterComponent Updater
        {
            get { return _updater as IPositionUpdaterComponent; }
            set { _updater = value as Component; }
        }


        public void Initialize()
        {
            if (Updater == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[ListEngineSetup] Updater is null. Ignore initialization", gameObject);
#endif
                return;
            }

            _engine = new PositionUpdaterListEngine(NearRadius, FarRadius);
            Updater.Engine = _engine;
        }

        public void Clear()
        {
            _engine = null;
        }


        public void Update()
        {
            if(_engine == null)
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
        }
    }
}
