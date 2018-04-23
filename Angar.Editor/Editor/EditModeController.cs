#if UNITY_EDITOR

using Angar.Importers;
using Angar.Importers.Views;
using Angar.Views;
using Uniforms;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.SceneManagement;


public static class EditorModeControllerCaller
{
    [MenuItem("Window/Angar")]
    public static void Open()
    {
        var window = EditorWindow.GetWindow<Angar.EditModeController>(false, "Angar :: editor");
    }
}


namespace Angar
{
    public class EditModeController : Window
    {

        private Stopwatch _stopwatch;


        public PoolEditorModel Model { get; set; }
        public PoolEditorView View { get; set; }


        public override void PostInit()
        {
            var model = new PoolEditorModel();

            var sceneImporterView = new ScenePrefabImporterView();
            var importerController = new ScenePrefabImporterController(sceneImporterView);

            var view = new PoolEditorView(model, sceneImporterView);

            Initialize(model, view);

            MainControll = view;
        }


        public void Initialize(PoolEditorModel model, PoolEditorView view)
        {
            Model = model;
            View = view;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();


            View.EventSelectAll += View_EventSelectAll;
            View.EventChangeActiveUpdater += ViewOnEventChangeActiveUpdater;
            View.EventChangeEditMode += ViewOnEventChangeEditMode;
            View.EventRemoveObjects += ViewOnEventRemoveObjects;
            view.EventCreateRoot += ViewOnEventCreateRoot;

            SceneManager.sceneLoaded += EditorSceneManagerOnSceneLoaded;
            SceneManager.sceneUnloaded += EditorSceneManagerOnSceneUnloaded;

            AngarEditorSettings.AddedUpdaterEvent += AngarStaticSettingsOnAddedUpdaterEvent;
            AngarEditorSettings.RemovedUpdaterEvent += AngarStaticSettingsOnRemovedUpdaterEvent;

            EditorApplication.update += _Update;

            Model.RefreshUpdaters();
        }

        private void ViewOnEventCreateRoot()
        {
            Model.CreateRoot();
        }

        public void OnDestroy()
        {
            SceneManager.sceneLoaded -= EditorSceneManagerOnSceneLoaded;
            SceneManager.sceneUnloaded -= EditorSceneManagerOnSceneUnloaded;

            AngarEditorSettings.AddedUpdaterEvent -= AngarStaticSettingsOnAddedUpdaterEvent;
            AngarEditorSettings.RemovedUpdaterEvent -= AngarStaticSettingsOnRemovedUpdaterEvent;

            if (EditorApplication.update != null) EditorApplication.update -= _Update;
        }



        private void ViewOnEventRemoveObjects()
        {
            Model.RemoveFindedObjects();
        }



        private void ViewOnEventChangeEditMode(bool b)
        {
            if (b)
                Model.EnterEditMode();
            else
                Model.ExitEditMode();
        }

        private void ViewOnEventChangeActiveUpdater(IPositionUpdater positionUpdater1, bool b)
        {
            Model.SetUpdaterActivity(positionUpdater1, b);
        }

        private void View_EventSelectAll()
        {
            foreach (var editorPoolInstance in Model.UpdatersState)
            {
                Model.SetUpdaterActivity(editorPoolInstance.Updater, true);
            }
        }

        private void _Update()
        {
            Model.Update(_stopwatch.ElapsedMilliseconds / 1000.0f);
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        private void AngarStaticSettingsOnRemovedUpdaterEvent(IPositionUpdater positionUpdater1)
        {
            Model.RemoveUpdater(positionUpdater1);
        }

        private void AngarStaticSettingsOnAddedUpdaterEvent(IPositionUpdater positionUpdater1)
        {
            Model.AddUpdater(positionUpdater1);
        }

        private void EditorSceneManagerOnSceneUnloaded(Scene arg0)
        {
            Model.RefreshUpdaters();
        }

        private void EditorSceneManagerOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            Model.RefreshUpdaters();
        }

    }
}

#endif