using Angar.PositionEngine;

namespace Angar
{
    /// <summary>
    /// Интерйейс определюющий компонент который сожержит движок обновляющий видимые объекты
    /// </summary>
    public interface IPositionUpdaterComponent
    {
        /// <summary>
        /// Используемый движок для обновления
        /// </summary>
        IPositionUpdaterEngine Engine { get; set; }
    }
}