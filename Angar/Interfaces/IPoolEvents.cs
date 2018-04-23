using System;

namespace Angar
{
    /// <summary>
    /// Интерфейс содержаций события происходящие в ходе работы пула
    /// </summary>
    /// <typeparam name="TType">Тип объекта-прототипа</typeparam>
    public interface IPoolEvents<TType>
    {
        /// <summary>
        /// Происходит при генерации нового объекта
        /// </summary>
        event Action<object, TType> EventGenerate;

        /// <summary>
        /// Происходит при попытке получения объекта из пула
        /// </summary>
        event Action<object, TType> EventPop;
        
        /// <summary>
        /// Происходит при возвращении объекта
        /// </summary>
        event Action<object, TType> EventPush;
    }
}
