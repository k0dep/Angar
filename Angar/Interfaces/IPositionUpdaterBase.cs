using System.Collections.Generic;
using Angar.Data;

namespace Angar
{
    /// <summary>
    /// Интерфейс определяющий сущность обновляющего позиции
    /// </summary>
    public interface IPositionUpdaterBase
    {
        /// <summary>
        /// Дальний радиус отсечения объектов
        /// </summary>
        float FarRadius { get; set; }

        /// <summary>
        /// Радиус до которого объекты должны отсекатся
        /// </summary>
        float NearRadius { get; set; }

        /// <summary>
        /// Источник позиции
        /// </summary>
        IPostionTargetSource TargetSource { get; set; }

        /// <summary>
        /// Используемый дейтасет
        /// </summary>
        IDataset DataSet { get; set; }

        /// <summary>
        /// Множество индексов видимых объектов
        /// </summary>
        HashSet<int> Loaded { get; set; }

        /// <summary>
        /// Обновление выдимых обхектов
        /// </summary>
        /// <param name="delta">разница времени с прошлого обновления</param>
        void UpdateRange(float delta);
    }
}