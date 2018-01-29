using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Angar
{
    [AddComponentMenu("Angar/Object pool")]
    public class Pool : MonoBehaviour, IPool<GameObject>, IPoolSystemClearable, IPoolSystemInitializable
    {
        [SerializeField]
        private GameObject _PoolingPrefab;

        [SerializeField]
        private int _MaxSize;

        [SerializeField]
        private int _MinSize;

        public int MaxSize
        {
            get { return _MaxSize; }
            set { _MaxSize = value;}
        }

        public int MinSize
        {
            get { return _MinSize; }
            set { _MinSize = value; }
        }

        public GameObject Prototype
        {
            get { return _PoolingPrefab; }
            set { _PoolingPrefab = value; }
        }

        public Queue<GameObject> ReservedInstances { get; set; }
        public HashSet<GameObject> ActiveInstances { get; set; }

        public int CurrentSize { get; set; }


        public event Action<object, GameObject> OnGenerate;
        public event Action<object, GameObject> OnPop;
        public event Action<object, GameObject> OnPush;


        public void Start()
        {
            Initialize();
        }


        public GameObject Pop()
        {
            if (ReservedInstances.Count == 0)
                Generate();

            var instance = ReservedInstances.Dequeue();
            ActiveInstances.Add(instance);

            if (OnPop != null)
                OnPop(this, instance);

            return instance;
        }

        public void Push(GameObject obj)
        {
            ReservedInstances.Enqueue(obj);
            if (OnPush != null)
                OnPush(this, obj);
        }


        public void Clear()
        {
            var listActive = new Queue<GameObject>(ActiveInstances.ToList());
            while (listActive.Count > 0)
            {
                GameObject.DestroyImmediate(listActive.Dequeue());
            }


            var listReserv = new Queue<GameObject>(ReservedInstances.ToList());
            while (listReserv.Count > 0)
            {
                GameObject.DestroyImmediate(listReserv.Dequeue());
            }

            ReservedInstances = null;
            ActiveInstances = null;
        }

        public void Initialize()
        {
            CurrentSize = 0;

            ReservedInstances = new Queue<GameObject>();
            ActiveInstances = new HashSet<GameObject>();

#if UNITY_EDITOR
            if (AngarEditorSettings.EditMode)
                return;            
#endif
            while (CurrentSize < MinSize)
            {
                Generate();
            }
        }

        private void Generate()
        {
            var instance = Instantiate(Prototype, transform);
            instance.SetActive(false);
            ReservedInstances.Enqueue(instance);

            CurrentSize++;

            if (OnGenerate != null)
                OnGenerate(this, instance);
        }
    }
}
