using System;
using System.Collections.Generic;
using Angar.Data;
using UnityEngine;

namespace Angar.Scripts
{
    /// <summary>
    /// Простая реализация IDatasetItem
    /// </summary>
    [Serializable]
    public class DatasetItem : IDatasetItem
    {
        [SerializeField]
        private Vector3 _position;

        [SerializeField]
        private Vector3 _rotation;

        [SerializeField]
        private Vector3 _scale;

        [SerializeField]
        private List<KeyValuePair<string, string>> _kvStorage;

        private Dictionary<string, string> _kvQuickTable;




        /// <summary>
        /// Смотри <see cref="IDatasetItem.EventChanged"/>
        /// </summary>
        public event Action<IDatasetItem> EventChanged;

        /// <summary>
        /// Смотри <see cref="IDatasetItem.Position"/>
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                if (_position == value)
                    return;

                _position = value;

                if (EventChanged != null)
                    EventChanged(this);
            }
        }

        /// <summary>
        /// Смотри <see cref="IDatasetItem.Rotation"/>
        /// </summary>
        public Vector3 Rotation
        {
            get { return _rotation; }
            set
            {
                if (_rotation == value)
                    return;

                _rotation = value;

                if (EventChanged != null)
                    EventChanged(this);
            }
        }

        /// <summary>
        /// Смотри <see cref="IDatasetItem.Scale"/>
        /// </summary>
        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                if (_scale == value)
                    return;

                _scale = value;

                if (EventChanged != null)
                    EventChanged(this);
            }
        }


        public DatasetItem()
        {
            _kvStorage = new List<KeyValuePair<string, string>>();
            _kvQuickTable = new Dictionary<string, string>();
        }


        /// <summary>
        /// Смотри <see cref="IDatasetItem.EventChanged"/>
        /// </summary>
        public string Get(string key)
        {
            return _kvQuickTable[key];
        }

        /// <summary>
        /// Смотри <see cref="IDatasetItem.EventChanged"/>
        /// </summary>
        public void Set(string key, string value)
        {
            _kvQuickTable[key] = value;

            EventChanged?.Invoke(this);
        }

        /// <summary>
        /// Смотри <see cref="IDatasetItem.EventChanged"/>
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> GetData()
        {
            return _kvStorage;
        }

        /// <summary>
        /// Смотри <see cref="IDatasetItem.EventChanged"/>
        /// </summary>
        public void SetData(IEnumerable<KeyValuePair<string, string>> newData)
        {
            _kvQuickTable = new Dictionary<string, string>();
            foreach (var keyValuePair in newData)
            {
                _kvQuickTable[keyValuePair.Key] = keyValuePair.Value;
            }
        }

    }
}
