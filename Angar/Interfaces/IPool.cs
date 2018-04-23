namespace Angar
{
    /// <summary>
    /// Интерфейс описывающий пулл объектов
    /// </summary>
    /// <typeparam name="TType">Тип объекта-прототипа</typeparam>
    public interface IPool<TType> : IPoolEvents<TType>
    {
        /// <summary>
        /// максимальное кол-во создаваемых пулом объектов
        /// </summary>
        int MaxSize { get; set; }

        /// <summary>
        /// Стартовое кол-во объектов
        /// </summary>
        int MinSize { get; set; }

        /// <summary>
        /// Текущий размер пула
        /// </summary>
        int CurrentSize { get; }

        /// <summary>
        /// Объект-прототип
        /// </summary>
        TType Prototype { get; set; }

        /// <summary>
        /// Метод для получения инстанса из пула
        /// </summary>
        /// <returns></returns>
        TType Pop();

        /// <summary>
        /// Возвращение вобратно в пул использованного объекта
        /// </summary>
        /// <param name="obj">Возвращаемый объект</param>
        void Push(TType obj);
    }
}