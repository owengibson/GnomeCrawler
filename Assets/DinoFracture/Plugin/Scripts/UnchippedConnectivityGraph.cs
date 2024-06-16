using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DinoFracture
{
    internal class UnchippedConnectivityGraphPool : SimpleObjectPool<UnchippedConnectivityGraph>
    {
        public static readonly UnchippedConnectivityGraphPool Instance = new UnchippedConnectivityGraphPool();

        private UnchippedConnectivityGraphPool()
            : base(() => new UnchippedConnectivityGraph(), (graph) => graph.Clear())
        {
        }
    }

    internal struct UnchippedConnectivityGraphNode
    {
        public FracturedObject Object { get; private set; }
        public bool Visited;

        public void Clear()
        {
            Object = null;
            Visited = false;
        }

        public void SetObject(FracturedObject obj)
        {
            Object = obj;
        }
    }

    internal readonly struct GroupedUnchippedObject
    {
        public readonly FracturedObject Object;
        public readonly int Group;

        public GroupedUnchippedObject(FracturedObject obj, int group)
        {
            Object = obj;
            Group = group;
        }
    }

    internal class UnchippedConnectivityGraph
    {
        private int _nodeCount = 0;
        private UnchippedConnectivityGraphNode[] _allNodes;
        private Stack<int> _searchItems = new Stack<int>();
        private bool[,] _connections;

        public static SimplePoolItemWrapper<UnchippedConnectivityGraph> GetEphemeralInstance()
        {
            return UnchippedConnectivityGraphPool.Instance.GetEphemeralInstance();
        }

        public void Clear()
        {
            for (int i = 0; i < _nodeCount; ++i)
            {
                for (int j = 0; j < _nodeCount; ++j)
                {
                    _connections[i, j] = false;
                }
            }

            for (int i = 0; i < _nodeCount; ++i)
            {
                _allNodes[i].Clear();
            }
            _nodeCount = 0;
        }

        public void SetTotalCount(int totalCount)
        {
            _nodeCount = totalCount;
            if ((_allNodes == null) || _nodeCount > _allNodes.Length)
            {
                _allNodes = new UnchippedConnectivityGraphNode[_nodeCount];
                _connections = new bool[_nodeCount, _nodeCount];
            }
        }

        public void Connect(int a, int b)
        {
            _connections[a, b] = true;
            _connections[b, a] = true;
        }

        public void AddNewObject(int index, FracturedObject obj)
        {
            _allNodes[index].SetObject(obj);
        }

        public IEnumerator GetGroups(List<GroupedUnchippedObject> outItems)
        {
            outItems.Clear();
            outItems.Capacity = Mathf.Max(outItems.Capacity, _nodeCount);

            int curGroup = 0;
            for (int i = 0; i < _nodeCount; ++i)
            {
                if (!_allNodes[i].Visited)
                {
                    VisitAllConnections(i, curGroup, outItems);
                    curGroup++;
                }

                yield return null;
            }
        }

        private void VisitAllConnections(int nodeIdx, int group, List<GroupedUnchippedObject> outItems)
        {
            _searchItems.Push(nodeIdx);

            while (_searchItems.Count > 0)
            {
                var topIdx = _searchItems.Pop();
                ref var top = ref _allNodes[topIdx];
                outItems.Add(new GroupedUnchippedObject(top.Object, group));
                top.Visited = true;

                for (int i = 0; i < _nodeCount; ++i)
                {
                    if (_connections[topIdx, i])
                    {
                        ref var conn = ref _allNodes[i];
                        if (!conn.Visited)
                        {
                            conn.Visited = true;
                            _searchItems.Push(i);
                        }
                    }
                }
            }
        }
    }
}