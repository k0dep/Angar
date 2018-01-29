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


        private void PoolOnOnPush(object o, GameObject obj)
        {
            obj.SetActive(false);
        }

        private void PoolOnOnPop(object o, GameObject obj)
        {
            obj.SetActive(true);
        }

        public void Clear()
        {
            Pool.OnPop -= PoolOnOnPop;
            Pool.OnPush -= PoolOnOnPush;
        }

        public void Initialize()
        {
            Pool.OnPop += PoolOnOnPop;
            Pool.OnPush += PoolOnOnPush;
        }
    }
}
