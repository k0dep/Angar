using Angar.Data;
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


        public PositionUpdaterNodeEngine(float nearRadius, float farRadius, float nodeSize)
        {
            LoadQueue = new Queue<int>();
            UnloadQueue = new Queue<int>();
            Loaded = new HashSet<int>();
            FarRadius = farRadius;
            NearRadius = nearRadius;
            _isLoaded = false;

            _current_nodes = new HashSet<SpaceId>();
            _last_nodes = new HashSet<SpaceId>();
        }


        public void Initialize(IPoolDataSet dataset, IPostionTargetSource postionTargetSource)
        {
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

                var x = (int)(position.x % NodeSize);
                var y = (int)(position.y % NodeSize);
                var z = (int)(position.z % NodeSize);

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

            _space = new List<int>[_len.x, _len.y, _len.z];


            foreach (var spaceKey in space)
            {
                _space[spaceKey.Key.x, spaceKey.Key.y, spaceKey.Key.z] = spaceKey.Value;
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

            if (!_isFirstLoading)
                PassDataSet(TargetSource.Position);
            _isFirstLoading = true;
            _last_pos = TargetSource.Position;
        }

        protected void PassDataSet(Vector3 position)
        {
            _current_nodes.Clear();
            _last_nodes.Clear();

            var edge = (int)(FarRadius / NodeSize) + 1;
            var spacePos = new Vector3Int((int)(position.x / NodeSize), (int)(position.y / NodeSize),
                (int)(position.z / NodeSize));
            var spacePosLast = new Vector3Int((int)(_last_pos.x / NodeSize), (int)(_last_pos.y / NodeSize),
                (int)(_last_pos.z / NodeSize));

            for (int x = -edge; x < edge; x++)
            {
                for (int y = -edge; y < edge; y++)
                {
                    for (int z = -edge; z < edge; z++)
                    {
                        var nodeCenter = new Vector3((spacePos.x + x + 0.5f) * NodeSize,
                            (spacePos.x + x + 0.5f) * NodeSize, (spacePos.x + x + 0.5f) * NodeSize);

                        var currentDistance = Vector3.Distance(nodeCenter, position);

                        if (currentDistance > FarRadius || currentDistance < NearRadius)
                            continue;

                        var nodePos = spacePos + new Vector3Int(x, y, z);

                        if (checkBounds(nodePos))
                            _current_nodes.Add(new SpaceId(nodePos.x, nodePos.y, nodePos.z));

                        var nodePosLast = spacePosLast + new Vector3Int(x, y, z);

                        if (checkBounds(nodePosLast))
                            _last_nodes.Add(new SpaceId(nodePosLast.x, nodePosLast.y, nodePosLast.z));
                    }
                }
            }

            _last_nodes.ExceptWith(_current_nodes);

            foreach (var currentNode in _current_nodes)
            {
                foreach (var i in _space[currentNode.x, currentNode.y, currentNode.z])
                {
                    Load(i);
                }
            }

            foreach (var lastNode in _last_nodes)
            {
                foreach (var i in _space[lastNode.x, lastNode.y, lastNode.z])
                {
                    Unload(i);
                }
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

        private bool checkBounds(Vector3Int node)
        {
            return (node.x > -_offset.x) && (node.x < _len.x - _offset.x)
                   && (node.y > -_offset.y) && (node.y < _len.y - _offset.y)
                   && (node.z > -_offset.z) && (node.z < _len.z - _offset.z)
                   && _space[node.x, node.y, node.z] != null;
        }

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
        }
    }

}

