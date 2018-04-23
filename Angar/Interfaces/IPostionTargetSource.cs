using UnityEngine;

namespace Angar
{
    /// <summary>
    /// Источник позиции
    /// </summary>
    public interface IPostionTargetSource
    {
        /// <summary>
        /// Должна возвращать позицию захватываемого обхекта
        /// </summary>
        Vector3 Position { get; }
    }
}