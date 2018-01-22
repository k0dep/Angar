using System.Collections.Generic;
using System.Threading.Tasks;
using Angar.Data;
using UnityEngine;

namespace Angar
{
    public class PositionUpdaterEngineOctree : PositionUpdaterEngine
    {
        private Vector3 cachePosition;

        private Vector3 position;
        private Task passTask;



        public Queue<int> LoadQueue { get; set; }
        public Queue<int> UnloadQueue { get; set; }

        public PointOctree<Integer> Octree { get; set; }

        public PositionUpdaterEngineOctree(float nearRadius, float farRadius, IPostionTargetSource targetSource, IPoolDataSet dataSetSource, float worldStartSize = 10.0f, float nodeMinSize = 10.0f)
            : base(nearRadius, farRadius, targetSource, dataSetSource)
        {
            Octree = new PointOctree<Integer>(worldStartSize, Vector3.zero, nodeMinSize);
            for (int i = 0; i < dataSetSource.Count; i++)
            {
                Octree.Add(i, dataSetSource[i].Position);
            }

            LoadQueue = new Queue<int>();
            UnloadQueue = new Queue<int>();
            Loaded = new HashSet<int>();
        }


        protected override void PassDataSet(Vector3 p_position)
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

        public override void Load(int i)
        {
            FireLoad(i);
        }

        public override void Unload(int i)
        {
            FireUnload(i);
        }
        
    }
}
