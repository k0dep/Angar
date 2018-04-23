
using System;
using Angar.Data;

namespace Angar
{
    /// <summary>
    /// Прокси для выбора дейтасета
    /// </summary>
    public interface IDatasetProxy : IDataset
    {
        IDataset Origin { get; set; }

        /// <summary>
        /// Вызывается при изменении ссыдлки на исходный дейтасет
        /// </summary>
        event Action<IDatasetProxy, IDataset> OriginChangedEvent;
    }
}
