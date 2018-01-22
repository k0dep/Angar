using System;
using System.Collections.Generic;
using Angar.Data;

namespace Angar
{
    public interface IPositionUpdater
    {
        float FarRadius { get; set; }
        float NearRadius { get; set; }

        IPostionTargetSource TargetSource { get; set; }
        IPoolDataSet DataSet { get; set; }

        HashSet<int> Loaded { get; set; }

        event Action<int> EventLoad;
        event Action<int> EventUnload;

        void UpdateRange(float delta);
    }
}