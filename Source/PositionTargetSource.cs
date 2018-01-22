using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Angar
{
    [AddComponentMenu("Angar/Position source proxy")]
    public class PositionTargetSource : MonoBehaviour, IPostionTargetSource
    {
        public bool UseOther;
        public Transform OtherTransform;

#if UNITY_EDITOR
        public bool DontUseSceneVirtualCamera;
#endif

        public Vector3 Position
        {
            get
            {
                var pos = UseOther ? OtherTransform.position : transform.position;

#if UNITY_EDITOR
                return DontUseSceneVirtualCamera || !AngarEditorSettings.EditMode ? pos : ((SceneView)SceneView.sceneViews[0]).camera.transform.position;
#else
                return pos;
#endif
            }
        }
    }
}
