using System;
using System.Collections.Generic;
using UnityEngine;

namespace Angar.Data
{
    public interface IPoolObjectOverrideData
    {
        Vector3 Position { get; set; }
        Vector3 Rotation { get; set; }
        Vector3 Scale { get; set; }

        int GetInt(string key);
        float GetFloat(string key);
        string GetString(string key);


        void SetInt(string key, int value);
        void SetFloat(string key, float value);
        void SetString(string key, string value);

        Dictionary<string, int> GetIntPairs();
        Dictionary<string, float> GetFloatPairs();
        Dictionary<string, string> GetStringPairs();

        event Action<IPoolObjectOverrideData> EventChanged;
    }
}