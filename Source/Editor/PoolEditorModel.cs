#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Angar.Data;
using UnityEditor;
using UnityEngine;

namespace Angar
{
    public class PoolEditorModel
    {
        public IList<EditorPoolInstance> UpdatersState { get; set; }

        public bool Editing
        {
            get { return AngarEditorSettings.EditMode; }
            set { AngarEditorSettings.EditMode = value; }
        }


        private List<PoolObjectEditModeUtility> _selectedPoolItems;


        public PoolEditorModel()
        {
            UpdatersState = new List<EditorPoolInstance>();
            Editing = false;
        }




        public void EnterEditMode()
        {
            var pools = Object.FindObjectsOfType<Component>()
                .Select(t => t as IPool<GameObject>)
                .Where(t => t != null)
                .ToArray();

            foreach (var pool in pools)
            {
                pool.OnGenerate -= PoolOnOnGenerate;
                pool.OnGenerate += PoolOnOnGenerate;
            }

            Editing = true;

            var initializables = Object.FindObjectsOfType<Component>()
                .Select(t => t as IPoolSystemInitializable)
                .Where(t => t != null)
                .Where(t =>
                {
                    var updater = t as IPositionUpdater;
                    return updater != null && UpdatersState.Any(u => u.Updater == updater);
                })
                .ToArray();

            foreach (var poolSystemInitializable in initializables)
            {
                poolSystemInitializable.Initialize();
            }

            initializables = Object.FindObjectsOfType<Component>()
                .Select(t => t as IPoolSystemInitializable)
                .Where(t => t != null)
                .Where(t => !(t is IPositionUpdater))
                .ToArray();

            foreach (var poolSystemInitializable in initializables)
            {
                poolSystemInitializable.Initialize();
            }
        }

        public void ExitEditMode()
        {
            var clearables = Object.FindObjectsOfType<Component>()
                .Select(t => t as IPoolSystemClearable).Where(t => t != null).ToArray();

            foreach (var poolSystemClearable in clearables)
            {
                poolSystemClearable.Clear();
            }

            Editing = false;
        }

        public void Update(float delta)
        {
            if(!Editing)
                return;

            foreach (var updater in UpdatersState)
            {
                if (updater.Active)
                    updater.Updater.UpdateRange(delta);
            }

        }

        public void RefreshUpdaters()
        {
            var updaters = Object.FindObjectsOfType<Component>()
                .Select(t => t as IPositionUpdater)
                .Where(t => t != null)
                .ToList();

            foreach (var positionUpdater in updaters)
            {
                AddUpdater(positionUpdater);
            }

            foreach (var updater in UpdatersState)
            {
                if (!updaters.Contains(updater.Updater))
                    RemoveUpdater(updater.Updater);
            }
        }

        public void SetUpdaterActivity(IPositionUpdater updater, bool value)
        {
            if(Editing)
                return;

            if (UpdatersState.All(t => t.Updater != updater))
                return;

            UpdatersState.First(t => t.Updater == updater).Active = value;
        }

        public void AddUpdater(IPositionUpdater updater)
        {
            if (UpdatersState.Any(t => t.Updater == updater))
                return;

            UpdatersState.Add(new EditorPoolInstance(updater, false));
        }

        public void RemoveUpdater(IPositionUpdater updater)
        {
            if (UpdatersState.All(t => t.Updater != updater))
                return;

            UpdatersState.Remove(UpdatersState.First(t => t.Updater == updater));
        }

        public void GetItemsFromSelected()
        {
            _selectedPoolItems = Selection.gameObjects
                .Select(t => t.GetComponent<PoolObjectEditModeUtility>())
                .Where(t => t != null)
                .ToList();
        }

        public void RemoveFindedObjects()
        {
            GetItemsFromSelected();

            var forDelete = _selectedPoolItems.Select(t => new {dataset = t.Dataset, index = t.Index});

            ExitEditMode();

            foreach (var item in forDelete)
            {
                item.dataset.Remove(item.index);
            }

            EnterEditMode();
        }



        private void PoolOnOnGenerate(object o, GameObject gameObject)
        {
            gameObject.AddComponent<PoolObjectEditModeUtility>();
        }
    }


    public class EditorPoolInstance
    {
        public IPositionUpdater Updater { get; set; }
        public bool Active { get; set; }

        public EditorPoolInstance(IPositionUpdater updater, bool active)
        {
            Updater = updater;
            Active = active;
        }
    }
}

#endif