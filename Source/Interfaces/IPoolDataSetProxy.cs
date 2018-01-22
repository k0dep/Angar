
using System;
using Angar.Data;

namespace Angar
{
    public interface IPoolDataSetProxy : IPoolDataSet
    {
        IPoolDataSet Origin { get; set; }

        /// <summary>
        /// Rise when dataset reference has changed, 2 parameter - old dataset
        /// </summary>
        event Action<IPoolDataSetProxy, IPoolDataSet> OriginChangedEvent;
    }
}
