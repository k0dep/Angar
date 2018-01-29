using System.Collections.Generic;
using System.Threading.Tasks;
using Angar.Data;
using UnityEngine;

namespace Angar.PositionEngine
{
    public class PositionUpdaterEngineOctree : IPositionUpdaterEngine
    {
        public float FarRadius { get; set; }
        public float NearRadius { get; set; }

        public IPostionTargetSource TargetSource { get; set; }

        public IPoolDataSet DataSet { get; set; }


        public HashSet<int> Loaded { get; set; }

        private Vector3 cachePosition;

        private Vector3 position;
        private Task passTask;
        private float nodeMinSize;
        private float worldStartSize;

        public Queue<int> LoadQueue { get; set; }
        public Queue<int> UnloadQueue { get; set; }

        public PointOctree<Integer> Octree { get; set; }




        public PositionUpdaterEngineOctree(float nearRadius, float farRadius, float worldStartSize = 10.0f, float nodeMinSize = 10.0f)
        {
            LoadQueue = new Queue<int>();
            UnloadQueue = new Queue<int>();

            Loaded = new HashSet<int>();
            FarRadius = farRadius;
            NearRadius = nearRadius;
            this.worldStartSize = worldStartSize;
            this.nodeMinSize = nodeMinSize;
        }



        public void Initialize(IPoolDataSet dataset, IPostionTargetSource postionTargetSource)
        {
            TargetSource = postionTargetSource;
            DataSet = dataset;

            Octree = new PointOctree<Integer>(worldStartSize, Vector3.zero, nodeMinSize);
            for (int i = 0; i < dataset.Count; i++)
            {
                Octree.Add(i, dataset[i].Position);
            }
        }





        protected void PassDataSet(Vector3 p_position)
        {
            if(passTask != null)
                if(!passTask.IsCompleted)
                    return;

            position = p_position;

            passTask = Task.Run(() =>
            {
                cachePosition = position;
                Octree.Pass(position, FarRadius, CheckFunc);
            });
        }

        private void CheckFunc(PointOctreeNode<Integer>.OctreeObject octreeObject)
        {
            var objectPosition = DataSet[octreeObject.Obj].Position;
            var distanceCurrent = Vector3.Distance(cachePosition, objectPosition);

            if (distanceCurrent > NearRadius && distanceCurrent < FarRadius && !Loaded.Contains(octreeObject.Obj))
            {
                Loaded.Add(octreeObject.Obj);
                lock (LoadQueue)
                {
                    LoadQueue.Enqueue(octreeObject.Obj);
                }
            }
            else if ((distanceCurrent < NearRadius || distanceCurrent > FarRadius) &&
                     Loaded.Contains(octreeObject.Obj))
            {
                Loaded.Remove(octreeObject.Obj);
                lock (UnloadQueue)
                {
                    UnloadQueue.Enqueue(octreeObject.Obj);
                }
            }
        }


        public void Load(int i)
        {
            if (Loaded.Contains(i))
                return;

            Loaded.Add(i);
        }

        public void Unload(int i)
        {

            if (!Loaded.Contains(i))
                return;

            Loaded.Remove(i);

        }

        public void UpdateRange(float delta)
        {
            var position = TargetSource.Position;

            PassDataSet(position);
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
