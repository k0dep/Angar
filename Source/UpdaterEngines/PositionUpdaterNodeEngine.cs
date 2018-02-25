using Angar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Angar.PositionEngine
{
    public class PositionUpdaterNodeEngine : IPositionUpdaterEngine
    {
        public float FarRadius { get; set; }
        public float NearRadius { get; set; }
        public float NodeSize { get; set; }

        public IPostionTargetSource TargetSource { get; set; }

        public IPoolDataSet DataSet { get; set; }


        public HashSet<int> Loaded { get; set; }

        public Queue<int> LoadQueue { get; set; }
        public Queue<int> UnloadQueue { get; set; }

        private bool _isLoaded;
        private bool _isFirstLoading;
        private List<int>[,,] _space;

        private Vector3Int _offset;
        private Vector3Int _len;
        private Vector3 _last_pos;

        private HashSet<SpaceId> _current_nodes;
        private HashSet<SpaceId> _last_nodes;

        private Task _passTask;


        public PositionUpdaterNodeEngine(float nearRadius, float farRadius, float nodeSize)
        {
            Debug.Log("Create node engine");
            LoadQueue = new Queue<int>();
            UnloadQueue = new Queue<int>();
            Loaded = new HashSet<int>();
            FarRadius = farRadius;
            NearRadius = nearRadius;
            _isLoaded = false;
            NodeSize = nodeSize;

            _current_nodes = new HashSet<SpaceId>();
            _last_nodes = new HashSet<SpaceId>();
        }


        public void Initialize(IPoolDataSet dataset, IPostionTargetSource postionTargetSource)
        {
            Debug.Log("Initialize node engine");
            TargetSource = postionTargetSource;
            DataSet = dataset;

            Task.Run(() => load());
        }

        private void load()
        {
            var space = new Dictionary<SpaceId, List<int>>();

            for (int i = 0; i < DataSet.Count; i++)
            {
                var position = DataSet[i].Position;

                var x = (int) (position.x / NodeSize);
                var y = (int) (position.y / NodeSize);
                var z = (int) (position.z / NodeSize);

                var spaceId = new SpaceId(x, y, z);

                if (!space.ContainsKey(spaceId))
                    space[spaceId] = new List<int>();

                space[spaceId].Add(i);
            }

            _offset.x = -space.Min(pair => pair.Key.x);
            _len.x = space.Max(pair => pair.Key.x) + _offset.x;

            _offset.y = -space.Min(pair => pair.Key.y);
            _len.y = space.Max(pair => pair.Key.y) + _offset.y;

            _offset.z = -space.Min(pair => pair.Key.z);
            _len.z = space.Max(pair => pair.Key.z) + _offset.z;

            _space = new List<int>[_len.x+1, _len.y+1, _len.z+1];

            foreach (var spaceKey in space)
            {
                var x = spaceKey.Key.x + _offset.x;
                var y = spaceKey.Key.y + _offset.y;
                var z = spaceKey.Key.z + _offset.z;

                try
                {
                    _space[x, y, z] = spaceKey.Value;
                }
                catch (IndexOutOfRangeException)
                {
                    Debug.LogError("error add node at space, x:" + x + "/" + _len.x + " y:" + y + " / " + _len.y +
                                   " z:" + z + "/" + _len.z);
                }
            }

            _isLoaded = true;
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
            if (!_isLoaded)
                return;

            if (_isFirstLoading)
                PassDataSet(TargetSource.Position);

            _isFirstLoading = true;
        }

        protected void PassDataSet(Vector3 position)
        {
            if (_passTask != null)
                if (!_passTask.IsCompleted)
                    return;

            _passTask = Task.Run(() => pass(position));

            _last_pos = TargetSource.Position;
        }

        private void pass(Vector3 position)
        {
            _current_nodes.Clear();
            _last_nodes.Clear();

            var edge = (int) (FarRadius / NodeSize) + 1;

            var spacePos = new Vector3Int((int) (position.x / NodeSize), (int) (position.y / NodeSize),
                (int) (position.z / NodeSize));

            var spacePosLast = new Vector3Int((int) (_last_pos.x / NodeSize), (int) (_last_pos.y / NodeSize),
                (int) (_last_pos.z / NodeSize));

            for (int x = -edge; x < edge; x++)
            {
                for (int y = -edge; y < edge; y++)
                {
                    for (int z = -edge; z < edge; z++)
                    {
                        passNode(position, spacePos, spacePosLast, x, y, z);
                    }
                }
            }

            foreach (var currentNode in _current_nodes)
            {
                var arr = _space[currentNode.x, currentNode.y, currentNode.z];
                if (arr == null)
                    continue;

                foreach (var i in arr)
                {
                    Load(i);
                }
            }

            _last_nodes.ExceptWith(_current_nodes);
            foreach (var lastNode in _last_nodes)
            {
                var arr = _space[lastNode.x, lastNode.y, lastNode.z];
                if (arr == null)
                    continue;

                foreach (var i in _space[lastNode.x, lastNode.y, lastNode.z])
                {
                    Unload(i);
                }
            }
        }

        private void passNode(Vector3 position, Vector3Int spacePos, Vector3Int spacePosLast, int x, int y, int z)
        {
            var nodeCenter = new Vector3((spacePos.x + x + 0.5f) * NodeSize,
                (spacePos.y + y + 0.5f) * NodeSize, (spacePos.z + z + 0.5f) * NodeSize);

            var currentDistance = Vector3.Distance(nodeCenter, position);

            if (currentDistance < FarRadius && currentDistance > NearRadius)
            {
                var nodePos = spacePos + new Vector3Int(x, y, z);

                if (checkBoundsWorld(nodePos))
                    _current_nodes.Add(getSpacePos(nodePos));
            }


            var oldNodeCenter = new Vector3((spacePosLast.x + x + 0.5f) * NodeSize,
                (spacePosLast.y + y + 0.5f) * NodeSize, (spacePosLast.z + z + 0.5f) * NodeSize);

            var oldDistance = Vector3.Distance(oldNodeCenter, _last_pos);

            if (oldDistance < FarRadius && oldDistance > NearRadius)
            {
                var nodePosLast = spacePosLast + new Vector3Int(x, y, z);

                if (checkBoundsWorld(nodePosLast))
                    _last_nodes.Add(getSpacePos(nodePosLast));
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

        private bool checkBoundsWorld(Vector3Int node)
        {
            var spacePos = getSpacePos(node);

            return spacePos.x >= 0 && spacePos.x < _len.x
                   && spacePos.y >= 0 && spacePos.y < _len.y
                   && spacePos.z >= 0 && spacePos.z < _len.z
                   && _space[spacePos.x, spacePos.y, spacePos.z] != null;
        }

        private Vector3Int getSpacePos(int x, int y, int z)
        {
            return new Vector3Int(x + _offset.x, y + _offset.y, z + _offset.z);
        }

        private Vector3Int getSpacePos(Vector3Int world)
        {
            return getSpacePos(world.x, world.y, world.z);
        }

        private Vector3Int getWorldPos(int x, int y, int z)
        {
            return getWorldPos(new Vector3Int(x, y, z));
        }

        private Vector3Int getWorldPos(Vector3Int space)
        {
            return new Vector3Int(space.x - _offset.x, space.y - _offset.y, space.z - _offset.z);
        }

#if UNITY_EDITOR
        public void DrawBoxes()
        {
            for (int x = 0; x < _len.x; x++)
            {
                for (int y = 0; y < _len.y; y++)
                {
                    for (int z = 0; z < _len.z; z++)
                    {
                        if(_space[x, y, z] == null)
                            continue;

                        var world = getWorldPos(x, y, z);
                        var worldReal = new Vector3((world.x + 0.5f) * NodeSize, (world.y + 0.5f) * NodeSize, (world.z + 0.5f) * NodeSize);

                        Gizmos.DrawCube(worldReal, new Vector3(NodeSize * 0.9f, NodeSize * 0.9f, NodeSize * 0.9f));
                    }
                }
            }
        }
#endif

        struct SpaceId
        {
            public int x;
            public int y;
            public int z;

            public SpaceId(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static implicit operator SpaceId(Vector3Int source)
            {
                return new SpaceId(source.x, source.y, source.z);
            }
        }
    }

}

