using System;
using System.Collections.Generic;
using Angar.Scripts;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Angar.Data
{
    /// <summary>
    /// Реализация IDataset в виде ScriptableObject объекта unity3d. Позволяет хранить данные дейтасета в удобном виде.
    /// </summary>
    public class Dataset : ScriptableObject, IDataset
    {
        [SerializeField, HideInInspector]
        private List<DatasetItem> _DataSet = new List<DatasetItem>();

        /// <summary>
        /// Смотри <see cref="IDataset.ChangedEvent"/>
        /// </summary>
        public event Action<IDataset, int> ChangedEvent;

        /// <summary>
        /// Смотри <see cref="IDataset.AddedEvent"/>
        /// </summary>
        public event Action<IDataset, IDatasetItem> AddedEvent;

        /// <summary>
        /// Смотри <see cref="IDataset.RemovedEvent"/>
        /// </summary>
        public event Action<IDataset, int, IDatasetItem> RemovedEvent;

        public void OnEnable()
        {
            foreach (var datasetItem in _DataSet)
            {
                datasetItem.EventChanged += DatasetItemOnEventChanged;
            }
        }

        /// <summary>
        /// Смотри <see cref="IDataset"/>
        /// </summary>
        public IDatasetItem this[int index]
        {
            get { return _DataSet[index]; }
            set
            {
                if(_DataSet[index] == value)
                    return;

                _DataSet[index] = (DatasetItem)value;
                ApplyDatasetElement();
                ChangedEvent?.Invoke(this, index);
            }
        }

        /// <summary>
        /// Смотри <see cref="IDataset.Count"/>
        /// </summary>
        public int Count
        {
            get { return _DataSet.Count; }
        }

        /// <summary>
        /// Смотри <see cref="IDataset.Add"/>
        /// </summary>
        public void Add(IDatasetItem value)
        {
            _DataSet.Add((DatasetItem)value);
            value.EventChanged += DatasetItemOnEventChanged;

            AddedEvent?.Invoke(this, value);

            ApplyDatasetElement();
        }


        /// <summary>
        /// Смотри <see cref="IDataset.Remove"/>
        /// </summary>
        public void Remove(int index)
        {
            var value = _DataSet[index];
            _DataSet.RemoveAt(index);
            value.EventChanged -= DatasetItemOnEventChanged;

            RemovedEvent?.Invoke(this, index, value);

            ApplyDatasetElement();
        }

        public void ApplyDatasetElement()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        private void DatasetItemOnEventChanged(IDatasetItem o)
        {
            ChangedEvent?.Invoke(this, _DataSet.IndexOf((DatasetItem)o));
            ApplyDatasetElement();
        }

    }
}