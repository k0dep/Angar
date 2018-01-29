using System.Collections.Generic;
using Angar.Data;
using UnityEngine;

namespace Angar.PositionEngine
{
    public class PositionUpdaterListEngine : IPositionUpdaterEngine
    {
        public float FarRadius { get; set; }
        public float NearRadius { get; set; }

        public IPostionTargetSource TargetSource { get; set; }

        public IPoolDataSet DataSet { get; set; }


        public HashSet<int> Loaded { get; set; }

        public Queue<int> LoadQueue { get; set; }
        public Queue<int> UnloadQueue { get; set; }


        public PositionUpdaterListEngine(float nearRadius, float farRadius)
        {
            LoadQueue = new Queue<int>();
            UnloadQueue = new Queue<int>();
            Loaded = new HashSet<int>();
            FarRadius = farRadius;
            NearRadius = nearRadius;
        }


        public void Initialize(IPoolDataSet dataset, IPostionTargetSource postionTargetSource)
        {
            TargetSource = postionTargetSource;
            DataSet = dataset;
        }



        public void Load(int i)
        {
            if (Loaded.Contains(i))
                return;

            Loaded.Add(i);
            LoadQueue.Enqueue(i);
        }

        public void Unload(int i)
        {

            if (!Loaded.Contains(i))
                return;

            Loaded.Remove(i);
            UnloadQueue.Enqueue(i);
        }

        public void UpdateRange(float delta)
        {
            var position = TargetSource.Position;

            PassDataSet(position);
        }

        protected void PassDataSet(Vector3 position)
        {
            for (var i = 0; i < DataSet.Count; i++)
            {
                var distance = (position - DataSet[i].Position).magnitude;
                if (distance < NearRadius || distance > FarRadius)
                    Unload(i);
                else
                    Load(i);
            }
        }

        public void UnloadAll()
        {
            var loadedSet = new List<int>(Loaded);

            foreach (var item in loadedSet)
            {
                Unload(item);
            }
        }
    }
}
