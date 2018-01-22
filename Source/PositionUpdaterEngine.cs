using System;
using System.Collections.Generic;
using Angar.Data;
using UnityEngine;

namespace Angar
{
    public class PositionUpdaterEngine : IPositionUpdater
    {
        public float PositionTollerance = 0.3f;

        public float FarRadius { get; set; }
        public float NearRadius { get; set; }

        public IPostionTargetSource TargetSource { get; set; }

        public IPoolDataSet DataSet
        {
            get { return _dataSetSource; }
            set
            {
                if(_dataSetSource == value) return;

                _dataSetSource = value;
                UnloadAll();

                if(_dataSetSource != null)
                    _dataSetSource.ChangedEvent += (set, i) => UnloadAll();
            }
        }

        public HashSet<int> Loaded { get; set; }

        public event Action<int> EventLoad;
        public event Action<int> EventUnload;

        private IPoolDataSet _dataSetSource;

        private Vector3 lastPosition = new Vector3(2333231, 3123156, 12331);

        public PositionUpdaterEngine(float nearRadius, float farRadius, IPostionTargetSource targetSource, IPoolDataSet dataSetSource)
        {
            Loaded = new HashSet<int>();
            FarRadius = farRadius;
            NearRadius = nearRadius;
            TargetSource = targetSource;
            DataSet = dataSetSource;
        }

        public virtual void Load(int i)
        {
            if (Loaded.Contains(i))
                return;

            Loaded.Add(i);

            if (EventLoad != null)
                EventLoad(i);

        }

        public virtual void Unload(int i)
        {

            if (!Loaded.Contains(i))
                return;



            Loaded.Remove(i);

            if (EventUnload != null)
                EventUnload(i);

        }

        public virtual void UpdateRange(float delta)
        {
            var position = TargetSource.Position;
            var distanceLast = (position - lastPosition).magnitude;
            if (distanceLast < PositionTollerance)
                return;

            lastPosition = position;

            PassDataSet(position);
        }

        protected virtual void PassDataSet(Vector3 position)
        {
            for (var i = 0; i < DataSet.Count; i++)
            {
                var distance = (position - DataSet[i].Position).magnitude;
                if (distance < NearRadius || distance > FarRadius)
                    Unload(i);
                else
                    Load(i);
            }
        }

        public void UnloadAll()
        {
            var loadedSet = new List<int>(Loaded);

            foreach (var item in loadedSet)
            {
                Unload(item);
            }
        }

        public void FireLoad(int i)
        {
            if (EventLoad != null)
                EventLoad(i);
        }

        public void FireUnload(int i)
        {
            if (EventUnload != null)
                EventUnload(i);
        }
    }
}
