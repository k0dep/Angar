using System;
using System.Collections.Generic;
using Angar.Data;
using UnityEngine;

namespace Angar.Scripts
{
    [Serializable]
    public class DataSetItem : IPoolObjectOverrideData
    {
        [SerializeField]
        private Vector3 _position;

        [SerializeField]
        private Vector3 _rotation;

        [SerializeField]
        private Vector3 _scale;



        [SerializeField]
        private List<int> _intArray;

        [SerializeField]
        private List<float> _floatArray;

        [SerializeField]
        private List<string> _stringArray;

        [Serializable]
        public struct ArrayMappingItem
        {
            [SerializeField]
            public string Key;

            [SerializeField]
            public int Index;

            public ArrayMappingItem(string key, int index)
            {
                Key = key;
                Index = index;
            }
        }

        [SerializeField]
        private List<ArrayMappingItem> _mapping;

        [NonSerialized]
        private Dictionary<string, int> _keyMapping;


        public event Action<IPoolObjectOverrideData> EventChanged;


        public Vector3 Position
        {
            get { return _position; }
            set
            {
                if(_position == value)
                    return;

                _position = value;

                if (EventChanged != null)
                    EventChanged(this);
            }
        }

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


        public DataSetItem()
        {
            _mapping = new List<ArrayMappingItem>();
            _intArray = new List<int>();
            _floatArray = new List<float>();
            _stringArray = new List<string>();
        }


        public int GetInt(string key)
        {
            if (_keyMapping == null)
                InitMapping();

            if (!_keyMapping.ContainsKey(key))
                throw new KeyNotFoundException("key: " + key + " not found in value registry");

            if (_intArray.Count <= _keyMapping[key])
                throw new ArgumentOutOfRangeException("key value not registered in internal type array. Check calling value type");

            return _intArray[_keyMapping[key]];
        }

        public float GetFloat(string key)
        {
            if (_keyMapping == null)
                InitMapping();

            if (!_keyMapping.ContainsKey(key))
                throw new KeyNotFoundException("key: " + key + " not found in value registry");

            if (_floatArray.Count <= _keyMapping[key])
                throw new ArgumentOutOfRangeException("key value not registered in internal type array. Check calling value type");

            return _floatArray[_keyMapping[key]];
        }

        public string GetString(string key)
        {
            if (_keyMapping == null)
                InitMapping();

            if (!_keyMapping.ContainsKey(key))
                throw new KeyNotFoundException("key: " + key + " not found in value registry");

            if (_stringArray.Count <= _keyMapping[key])
                throw new ArgumentOutOfRangeException("key value not registered in internal type array. Check calling value type");

            return _stringArray[_keyMapping[key]];
        }


        public void SetInt(string key, int value)
        {
            if (_keyMapping == null)
                InitMapping();

            if (!_keyMapping.ContainsKey(key))
            {
                _keyMapping.Add(key, _intArray.Count);
                _intArray.Add(value);
                _mapping.Add(new ArrayMappingItem(key, _intArray.Count - 1));
            }

            _intArray[_keyMapping[key]] = value;

            if (EventChanged != null)
                EventChanged(this);
        }

        public void SetFloat(string key, float value)
        {
            if (_keyMapping == null)
                InitMapping();

            if (!_keyMapping.ContainsKey(key))
            {
                _keyMapping.Add(key, _floatArray.Count);
                _floatArray.Add(value);
                _mapping.Add(new ArrayMappingItem(key, _floatArray.Count - 1));
            }

            _floatArray[_keyMapping[key]] = value;

            if (EventChanged != null)
                EventChanged(this);
        }

        public void SetString(string key, string value)
        {
            if (_keyMapping == null)
                InitMapping();

            if (!_keyMapping.ContainsKey(key))
            {
                _keyMapping.Add(key, _stringArray.Count);
                _stringArray.Add(value);
                _mapping.Add(new ArrayMappingItem(key, _stringArray.Count - 1));
            }

            _stringArray[_keyMapping[key]] = value;

            if (EventChanged != null)
                EventChanged(this);
        }



        public Dictionary<string, int> GetIntPairs()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, float> GetFloatPairs()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetStringPairs()
        {
            throw new NotImplementedException();
        }



        private void InitMapping()
        {
            if(_mapping == null)
                throw new Exception("mapping is null");

            _keyMapping = new Dictionary<string, int>();

            foreach (var arrayMappingItem in _mapping)
            {
                _keyMapping[arrayMappingItem.Key] = arrayMappingItem.Index;
            }
        }
    }
}
