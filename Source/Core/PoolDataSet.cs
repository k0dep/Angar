using System;
using System.Collections.Generic;
using Angar.Scripts;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Angar.Data
{
    public class PoolDataSet : ScriptableObject, IPoolDataSet
    {
        [SerializeField, HideInInspector]
        private List<DataSetItem> _DataSet = new List<DataSetItem>();

        public event Action<IPoolDataSet, int> ChangedEvent;
        public event Action<IPoolDataSet, IPoolObjectOverrideData> AddedEvent;
        public event Action<IPoolDataSet, int, IPoolObjectOverrideData> RemovedEvent;

        public void OnEnable()
        {
            foreach (var datasetItem in _DataSet)
            {
                datasetItem.EventChanged += DatasetItemOnEventChanged;
            }
        }

        public IPoolObjectOverrideData this[int index]
        {
            get { return _DataSet[index]; }
            set
            {
                if(_DataSet[index] == value)
                    return;

                _DataSet[index] = (DataSetItem)value;
                ApplyDatasetElement();
                if (ChangedEvent != null)
                    ChangedEvent(this, index);
            }
        }

        public int Count
        {
            get { return _DataSet.Count; }
        }

        public void Add(IPoolObjectOverrideData value)
        {
            _DataSet.Add((DataSetItem)value);
            value.EventChanged += DatasetItemOnEventChanged;

            if (AddedEvent != null)
                AddedEvent(this, value);

            ApplyDatasetElement();
        }

        public void Remove(int index)
        {
            var value = _DataSet[index];
            _DataSet.RemoveAt(index);
            value.EventChanged -= DatasetItemOnEventChanged;

            if (RemovedEvent != null)
                RemovedEvent(this, index, value);

            ApplyDatasetElement();
        }

        public void ApplyDatasetElement()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        private void DatasetItemOnEventChanged(IPoolObjectOverrideData o)
        {
            if (ChangedEvent != null)
                ChangedEvent(this, _DataSet.IndexOf((DataSetItem)o));
            ApplyDatasetElement();
        }

    }
}