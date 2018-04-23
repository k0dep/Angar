using System;

namespace Angar.Data
{
    /// <summary>
    /// Интерфейс для дейтасета пула
    /// </summary>
    public interface IDataset
    {
        /// <summary>
        /// Событие, вызывающееся при изменении коллекции дейтасета или изменении елемента пренадлежащего коллекции
        /// </summary>
        event Action<IDataset, int> ChangedEvent;

        /// <summary>
        /// Вызывается при добавлении нового елемента в коллекцию дейтасета
        /// </summary>
        event Action<IDataset, IDatasetItem> AddedEvent;

        /// <summary>
        /// Вызывается при удалении елемента из коллекции дейтасета 
        /// </summary>
        event Action<IDataset, int, IDatasetItem> RemovedEvent;

        /// <summary>
        /// Индексатор позволяющий получить объект коллекции дейтасета по его индексу в датасете
        /// </summary>
        /// <param name="index">индекс объекта дейтасета</param>
        /// <returns>объект дейтасета</returns>
        IDatasetItem this[int index] { get; set; }

        /// <summary>
        /// Колличество елементов в коллекции дейтасета
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Добавляет новый объкт в дейтасет
        /// </summary>
        /// <param name="value">добавляемый объект</param>
        void Add(IDatasetItem value);

        /// <summary>
        /// Удаляет существующий объект из коллекции дейтасета по индексу
        /// </summary>
        /// <param name="index">индекс удаляемого объекта</param>
        void Remove(int index);
    }
}