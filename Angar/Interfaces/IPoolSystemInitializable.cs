namespace Angar
{
    /// <summary>
    /// Интерфейс определяющий метод который вызывается в момент когда нужно проинициализировать состояние
    /// </summary>
    public interface IPoolSystemInitializable
    {
        /// <summary>
        /// Вызывается в момент когда нужно проинициализировать состояние
        /// </summary>
        void Initialize();
    }
}
