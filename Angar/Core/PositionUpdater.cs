using System;
using System.Collections.Generic;
using Angar.Data;
using Angar.PositionEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Angar
{
    [AddComponentMenu("Angar/Position updater")]
    [ExecuteInEditMode]
    public class PositionUpdater : MonoBehaviour, IPositionUpdater, IPositionUpdaterComponent, IPoolSystemInitializable, IPoolSystemClearable
    {
        private float Elapsed = 10000;

        [SerializeField]
        private Object _TargetSource;

        [SerializeField]
        private Object _DataSetSource;

        private IPostionTargetSource _targetSourceCache;
        private IPositionUpdaterEngine _engine;


        public IPositionUpdaterEngine Engine
        {
            get { return _engine; }

            set
            {
                _engine = value;
                if(_engine != null)
                    _engine.Initialize(DataSet, TargetSource);
            }
        }
        

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

        public IDataset DataSet
        {
            get { return _DataSetSource as IDataset; }
            set { _DataSetSource = value as Object; }
        }

        public HashSet<int> Loaded { get; set; }
        public event Action<int> EventLoad;
        public event Action<int> EventUnload;


        public float NearRadius
        {
            get { return Engine.NearRadius; }
            set { Engine.NearRadius = value; }
        }

        public float FarRadius
        {
            get { return Engine.FarRadius; }
            set { Engine.FarRadius = value; }
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

            if (!(_DataSetSource is IDataset))
                _DataSetSource = null;
        }



        public void Initialize()
        {
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

            Engine.UpdateRange(delta);
        }


        public void Update()
        {
            if(Application.isPlaying)
                UpdateRange(Time.deltaTime);
        }
    }
}