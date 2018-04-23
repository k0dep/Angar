using UnityEngine;

namespace Angar.Factory
{
    /// <summary>
    /// Интерфейс для фабрики EngineSetup-ов
    /// </summary>
    public interface IEngineSetupFactory
    {
        /// <summary>
        /// Создает EngineSetup в объекте
        /// </summary>
        /// <param name="target">Цель для создания</param>
        /// <param name="updater">Цель для внедрения движка</param>
        void Create(GameObject target, IPositionUpdaterComponent updater);
    }
}