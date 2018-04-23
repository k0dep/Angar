using System;
using System.Collections.Generic;
using UnityEngine;

namespace Angar.Data
{
    /// <summary>
    /// Интерфейс для елемента из коллекции дейтасета
    /// который описывает модель объекта мира
    /// </summary>
    public interface IDatasetItem
    {
        /// <summary>
        /// Позиция объекта
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Вращение объекта
        /// </summary>
        Vector3 Rotation { get; set; }

        /// <summary>
        /// Масштаб объекта
        /// </summary>
        Vector3 Scale { get; set; }

        /// <summary>
        /// Получает строку значения по ключю из хранилища текущего объекта
        /// </summary>
        /// <param name="key">ключ для выборки</param>
        /// <returns>строковое значение</returns>
        string Get(string key);

        /// <summary>
        /// Устанавливает значение для ключа, либо заменяет прошлое значение на новое, если строка с таким ключем уже существовала
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="value">новое значение</param>
        void Set(string key, string value);

        /// <summary>
        /// Получает все строки ключ-значение из хранилища для текущего объекта
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> GetData();

        /// <summary>
        /// Создает новое хранилище из перечиаления ключей-значение
        /// </summary>
        /// <param name="newData">новые строки ключ-значение</param>
        void SetData(IEnumerable<KeyValuePair<string, string>> newData);

        /// <summary>
        /// Событие, вызываемае при изменении данных текущего объекта дейтасета
        /// </summary>
        event Action<IDatasetItem> EventChanged;
    }
}