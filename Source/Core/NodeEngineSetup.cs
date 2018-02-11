using Angar.PositionEngine;
using UnityEngine;

namespace Angar.Source.Core
{
    [ExecuteInEditMode]
    public class NodeEngineSetup : MonoBehaviour, IPoolSystemInitializable, IPoolSystemClearable
    {
        private IPositionUpdaterEngine _engine;

        [SerializeField]
        private Component _updater;

        public float NearRadius;
        public float FarRadius;
        public float NodeSize;


        public IPositionUpdaterComponent Updater
        {
            get { return _updater as IPositionUpdaterComponent; }
            set { _updater = value as Component; }
        }


        public void Initialize()
        {
            if (Updater == null)
            {
                return;
            }

            _engine = new PositionUpdaterNodeEngine(NearRadius, FarRadius, NodeSize);
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

            if (NodeSize <= 0)
                NodeSize = 1.0f;
        }
    }

}