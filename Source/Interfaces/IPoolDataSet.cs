using System;

namespace Angar.Data
{
    public interface IPoolDataSet
    {

        event Action<IPoolDataSet, int> ChangedEvent;
        event Action<IPoolDataSet, IPoolObjectOverrideData> AddedEvent;
        event Action<IPoolDataSet, int, IPoolObjectOverrideData> RemovedEvent;

        IPoolObjectOverrideData this[int index] { get; set; }
        int Count { get; }

        void Add(IPoolObjectOverrideData value);
        void Remove(int index);

    }
}