using System;
using Angar.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Angar
{
    [AddComponentMenu("Angar/DataSet source proxy")]
    public class DatasetSource : MonoBehaviour, IDatasetProxy
    {
        [SerializeField]
        private Object _Origin;

        private IDataset _originCache;

        public IDataset Origin
        {
            get => _originCache ?? (_originCache = _Origin as IDataset);
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

                OriginChangedEvent?.Invoke(this, oldDataset);
            }
        }

        public event Action<IDatasetProxy, IDataset> OriginChangedEvent;
        public event Action<IDataset, int> ChangedEvent;
        public event Action<IDataset, IDatasetItem> AddedEvent;
        public event Action<IDataset, int, IDatasetItem> RemovedEvent;


        public void OnValidate()
        {
            if (!(_Origin is IDataset))
                _Origin = null;
        }


        public IDatasetItem this[int index]
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

        public void Add(IDatasetItem value)
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

        private void OriginOnAddedEvent(IDataset dataset, IDatasetItem datasetItem)
        {
            AddedEvent?.Invoke(dataset, datasetItem);
        }

        private void OriginOnChangedEvent(IDataset dataset, int i)
        {
            ChangedEvent?.Invoke(dataset, i);
        }

        private void OriginOnRemovedEvent(IDataset dataset, int i, IDatasetItem arg3)
        {
            RemovedEvent?.Invoke(dataset, i, arg3);
        }

        private void CheckOrigin()
        {
            if (Origin == null)
                throw new NullReferenceException("dataset proxy not refer to origin dataset. Select datset.");
        }

    }
}
