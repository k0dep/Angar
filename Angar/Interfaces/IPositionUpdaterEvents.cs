using System;

namespace Angar
{
    /// <summary>
    /// События для действий сушности обновляющей видимости объектов
    /// </summary>
    public interface IPositionUpdaterEvents
    {
        /// <summary>
        /// Вызывается при загрузке обкекта, передает индекс объекта
        /// </summary>
        event Action<int> EventLoad;

        /// <summary>
        /// Вызывается при выгрузке обкекта, передает индекс объекта
        /// </summary>
        event Action<int> EventUnload;
    }
}