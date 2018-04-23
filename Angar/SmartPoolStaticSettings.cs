#if UNITY_EDITOR

using System;
using UnityEditor;

namespace Angar
{
    public class AngarEditorSettings
    {
        public static bool EditMode
        {
            get
            {
                if (!EditorPrefs.HasKey("Angar.EditMode"))
                    EditorPrefs.SetBool("Angar.EditMode", false);

                return EditorPrefs.GetBool("Angar.EditMode");
            }
            set
            {
                EditorPrefs.SetBool("Angar.EditMode", value);
            }
        }

        public static event Action<IPositionUpdater> AddedUpdaterEvent;
        public static event Action<IPositionUpdater> RemovedUpdaterEvent;

        public static void FireAddedUpdater(IPositionUpdater updater)
        {
            if (AddedUpdaterEvent != null)
                AddedUpdaterEvent(updater);
        }

        public static void FireRemovedUpdater(IPositionUpdater updater)
        {
            if (RemovedUpdaterEvent != null)
                RemovedUpdaterEvent(updater);
        }
    }
}

#endif