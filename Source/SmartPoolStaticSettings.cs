#if UNITY_EDITOR

using System;

namespace Angar
{
    public class AngarEditorSettings
    {
        public static bool EditMode
        {
            get;
            set;
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