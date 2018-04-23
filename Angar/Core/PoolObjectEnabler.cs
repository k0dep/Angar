using UnityEngine;

namespace Angar
{
    [AddComponentMenu("Angar/Object enabler")]
    public class PoolObjectEnabler : MonoBehaviour, IPoolSystemClearable, IPoolSystemInitializable
    {
        [SerializeField]
        private Object _Pool;

        public IPool<GameObject> Pool
        {
            get { return _Pool as IPool<GameObject>; }
            set { _Pool = value as Object; }
        }




        public void Awake()
        {
            Initialize();
        }


        public void OnValidate()
        {
            if (!(_Pool is IPool<GameObject>))
                _Pool = null;
        }


        private void PoolEventEventPush(object o, GameObject obj)
        {
            obj.SetActive(false);
        }

        private void PoolEventEventPop(object o, GameObject obj)
        {
            obj.SetActive(true);
        }

        public void Clear()
        {
            Pool.EventPop -= PoolEventEventPop;
            Pool.EventPush -= PoolEventEventPush;
        }

        public void Initialize()
        {
            Pool.EventPop += PoolEventEventPop;
            Pool.EventPush += PoolEventEventPush;
        }
    }
}
