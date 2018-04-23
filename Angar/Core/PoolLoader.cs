using System.Collections.Generic;
using UnityEngine;

namespace Angar
{
    public interface IMonoPoolLoader
    {
        IPool<GameObject> Pool { get; set; }
        IPositionUpdater PositionUpdater { get; set; }
    }

    [AddComponentMenu("Angar/Loaders/Single loader")]
    public class PoolLoader : MonoBehaviour, IPoolSystemInitializable, IPoolSystemClearable, IMonoPoolLoader
    {
        [SerializeField]
        protected Object _Pool;

        [SerializeField]
        protected Object _PositionUpdater;

        private Dictionary<int, GameObject> objToId;
        


        public IPool<GameObject> Pool
        {
            get { return _Pool as IPool<GameObject>; }
            set { _Pool = value as Object; }
        }

        public IPositionUpdater PositionUpdater
        {
            get { return _PositionUpdater as IPositionUpdater; }
            set { _PositionUpdater = value as Object; }
        }



        public void Awake()
        {
            Initialize();
        }

        public void OnValidate()
        {
            if (!(_Pool is IPool<GameObject>))
                _Pool = null;

            if (!(_PositionUpdater is IPositionUpdater))
                _PositionUpdater = null;
        }



        private void PositionUpdaterOnEventLoad(int i)
        {
            if(objToId.ContainsKey(i))
                return;

            var obj = Pool.Pop();
            objToId.Add(i, obj);

            obj.transform.position = PositionUpdater.DataSet[i].Position;
            obj.transform.rotation = Quaternion.Euler(PositionUpdater.DataSet[i].Rotation);
            obj.transform.localScale = PositionUpdater.DataSet[i].Scale;

            var instancePool = obj.GetComponents<IPoolObjectEvents>();
            foreach (var poolObjectEventse in instancePool)
            {
                poolObjectEventse.PoolInitialize(i, PositionUpdater.DataSet, PositionUpdater.DataSet[i]);
            }
        }

        private void PositionUpdaterOnEventUnload(int i)
        {
            if(!objToId.ContainsKey(i))
                return;

            var obj = objToId[i];
            objToId.Remove(i);
            Pool.Push(obj);

            var instancePool = obj.GetComponents<IPoolObjectEvents>();
            foreach (var poolObjectEventse in instancePool)
            {
                poolObjectEventse.PoolDeinitialize();
            }
        }

        public void Initialize()
        {
            objToId = new Dictionary<int, GameObject>();

            PositionUpdater.EventUnload += PositionUpdaterOnEventUnload;

            PositionUpdater.EventLoad += PositionUpdaterOnEventLoad;
        }

        public void Clear()
        {
            PositionUpdater.EventUnload -= PositionUpdaterOnEventUnload;

            PositionUpdater.EventLoad -= PositionUpdaterOnEventLoad;
        }

    }
}
