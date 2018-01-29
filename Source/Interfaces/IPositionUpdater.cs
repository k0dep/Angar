using System;
using System.Collections.Generic;
using Angar.Data;
using Angar.PositionEngine;

namespace Angar
{
    public interface IPositionUpdaterBase
    {
        float FarRadius { get; set; }
        float NearRadius { get; set; }

        IPostionTargetSource TargetSource { get; set; }
        IPoolDataSet DataSet { get; set; }

        HashSet<int> Loaded { get; set; }

        void UpdateRange(float delta);
    }

    public interface IPositionUpdaterComponent
    {
        IPositionUpdaterEngine Engine { get; set; }
    }

    public interface IPositionUpdater : IPositionUpdaterBase, IPositionUpdaterEvents
    {
    }

    public interface IPositionUpdaterEvents
    {
        event Action<int> EventLoad;
        event Action<int> EventUnload;
    }
}