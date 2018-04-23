namespace Angar
{
    /// <summary>
    /// Определяет метод который вызывается в момент когда нужно очистить состояние
    /// </summary>
    public interface IPoolSystemClearable
    {
        /// <summary>
        /// Вызывается в момент когда нужно очистить состояние
        /// </summary>
        void Clear();
    }
}
