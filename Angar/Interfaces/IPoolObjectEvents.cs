using Angar.Data;

namespace Angar
{
    /// <summary>
    /// Интерфейс для определения методов вызывающихся в моменты установки или удаления объекта из мира
    /// </summary>
    public interface IPoolObjectEvents
    {
        /// <summary>
        /// Вызывается при удалении объекта из мира
        /// </summary>
        void PoolDeinitialize();

        /// <summary>
        /// Вызывается при добавлении объекта в мир
        /// </summary>
        /// <param name="datasetObject">индекс обхекта в коллекции дейтасета</param>
        /// <param name="dataset">дейтасет которому принадлежит обхект</param>
        /// <param name="datasetData">елемент дейтасета</param>
        void PoolInitialize(int datasetObject, IDataset dataset, IDatasetItem datasetData);
    }
}
