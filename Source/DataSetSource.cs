using System;
using Angar.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Angar
{
    [AddComponentMenu("Angar/DataSet source proxy")]
    public class DataSetSource : MonoBehaviour, IPoolDataSetProxy
    {
        [SerializeField]
        private Object _Origin;

        private IPoolDataSet _originCache;

        public IPoolDataSet Origin
        {
            get
            {
                if (_originCache == null)
                    _originCache = _Origin as IPoolDataSet;
                return _originCache;
            }
            set
            {
                if(Origin == value)
                    return;

                if (Origin != null)
                    DesubscribeProxyEvents();

                var oldDataset = Origin;

                _originCache = value;
                if ((value as Object) != null)
                    _Origin = (Object) value;

                if(Origin != null)
                    SubscribeProxyEvents();

                if (OriginChangedEvent != null)
                    OriginChangedEvent(this, oldDataset);
            }
        }

        public event Action<IPoolDataSetProxy, IPoolDataSet> OriginChangedEvent;
        public event Action<IPoolDataSet, int> ChangedEvent;
        public event Action<IPoolDataSet, IPoolObjectOverrideData> AddedEvent;
        public event Action<IPoolDataSet, int, IPoolObjectOverrideData> RemovedEvent;


        public void OnValidate()
        {
            if (!(_Origin is IPoolDataSet))
                _Origin = null;
        }


        public IPoolObjectOverrideData this[int index]
        {
            get
            {
                CheckOrigin();
                return Origin[index];
            }
            set
            {
                CheckOrigin();
                Origin[index] = value;
            }
        }

        public int Count
        {
            get
            {
                CheckOrigin();
                return Origin.Count;
            }
        }

        public void Add(IPoolObjectOverrideData value)
        {
            CheckOrigin();

            Origin.Add(value);
        }

        public void Remove(int index)
        {
            CheckOrigin();

            Origin.Remove(index);
        }

        private void SubscribeProxyEvents()
        {
            Origin.AddedEvent += OriginOnAddedEvent;
            Origin.RemovedEvent += OriginOnRemovedEvent;
            Origin.ChangedEvent += OriginOnChangedEvent;
        }

        private void DesubscribeProxyEvents()
        {
            Origin.AddedEvent -= OriginOnAddedEvent;
            Origin.RemovedEvent -= OriginOnRemovedEvent;
            Origin.ChangedEvent -= OriginOnChangedEvent;
        }

        private void OriginOnAddedEvent(IPoolDataSet poolDataSet, IPoolObjectOverrideData poolObjectOverrideData)
        {
            if (AddedEvent != null)
                AddedEvent(poolDataSet, poolObjectOverrideData);
        }

        private void OriginOnChangedEvent(IPoolDataSet poolDataSet, int i)
        {
            if (ChangedEvent != null)
                ChangedEvent(poolDataSet, i);
        }

        private void OriginOnRemovedEvent(IPoolDataSet poolDataSet, int i, IPoolObjectOverrideData arg3)
        {
            if (RemovedEvent != null)
                RemovedEvent(poolDataSet, i, arg3);
        }

        private void CheckOrigin()
        {
            if (Origin == null)
                throw new NullReferenceException("dataset proxy not refer to origin dataset. Select datset.");
        }

    }
}
