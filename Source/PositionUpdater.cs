using System;
using System.Collections.Generic;
using Angar.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Angar
{
    [AddComponentMenu("Angar/Position updater")]
    [ExecuteInEditMode]
    public class PositionUpdater : MonoBehaviour, IPositionUpdater, IPoolSystemInitializable, IPoolSystemClearable
    {
        private float Elapsed = 10000;

        [SerializeField]
        private Object _TargetSource;

        [SerializeField]
        private Object _DataSetSource;

        private IPostionTargetSource _targetSourceCache;


        protected PositionUpdaterEngineOctree Engine { get; set; }


        public float WorldStartSize;
        public float MinimumNodeSize;

        public float _NearRadius;
        public float _FarRadius;

        public int MaximumLoadPerUpdate = 30;
        public int MaximumUnLoadPerUpdate = 30;

        public float DelayTime = 1.0f;


        public IPostionTargetSource TargetSource
        {
            get
            {
                if (_targetSourceCache == null)
                    _targetSourceCache = _TargetSource as IPostionTargetSource;
                return _targetSourceCache;
            }
            set
            {
                _targetSourceCache = value;
                _TargetSource = value as Object;
            }
        }

        public IPoolDataSet DataSet
        {
            get { return _DataSetSource as IPoolDataSet; }
            set { _DataSetSource = value as Object; }
        }

        public HashSet<int> Loaded { get; set; }
        public event Action<int> EventLoad;
        public event Action<int> EventUnload;


        public float NearRadius
        {
            get { return _NearRadius; }
            set
            {
                _NearRadius = value;
                Engine.NearRadius = value;
            }
        }

        public float FarRadius
        {
            get { return _FarRadius; }
            set
            {
                _FarRadius = value;
                Engine.FarRadius = value;
            }
        }


        public void Start()
        {
#if UNITY_EDITOR
            AngarEditorSettings.FireAddedUpdater(this);
#endif

            if(Application.isPlaying)
                Initialize();
        }

        public void OnDestroy()
        {
#if UNITY_EDITOR
            AngarEditorSettings.FireRemovedUpdater(this);
#endif

            Clear();
        }

        public void OnValidate()
        {
            if (!(_TargetSource is IPostionTargetSource))
                _TargetSource = null;

            if (!(_DataSetSource is IPoolDataSet))
                _DataSetSource = null;
        }



        public void Initialize()
        {
            Engine = new PositionUpdaterEngineOctree(_NearRadius, _FarRadius, TargetSource, DataSet, WorldStartSize, MinimumNodeSize);
        }

        public void Clear()
        {
            Engine = null;
        }

        public void UpdateRange(float delta)
        {
            if (Engine == null)
                return;

            var countLoaded = 0;
            lock (Engine.LoadQueue)
            {
                while (Engine.LoadQueue.Count > 0 && countLoaded < MaximumLoadPerUpdate)
                {
                    if (EventLoad != null)
                        EventLoad(Engine.LoadQueue.Dequeue());
                    countLoaded++;
                }
            }

            countLoaded = 0;
            lock (Engine.UnloadQueue)
            {
                while (Engine.UnloadQueue.Count > 0 && countLoaded < MaximumUnLoadPerUpdate)
                {
                    if (EventUnload != null)
                        EventUnload(Engine.UnloadQueue.Dequeue());
                    countLoaded++;
                }
            }

            Elapsed += delta;

            if (Elapsed > DelayTime)
                Elapsed = 0;
            else
                return;

            ValidateLoadingRanges();
            Engine.UpdateRange(delta);
        }


        public void Update()
        {
            if(Application.isPlaying)
                UpdateRange(Time.deltaTime);
        }

        private void ValidateLoadingRanges()
        {
            if (Math.Abs(Engine.FarRadius - _FarRadius) > 0.0001f)
                FarRadius = _FarRadius;

            if (Math.Abs(Engine.NearRadius - _NearRadius) > 0.0001f)
                NearRadius = _NearRadius;
        }

        public void OnDrawGizmosSelected()
        {
            if(Engine == null)
                return;

            Engine.Octree.DrawAllBounds();
        }
    }
}