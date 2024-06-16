using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DinoFracture
{
    public class ChippedFractureRoot : MonoBehaviour
    {
        [SerializeField]
        internal float CollisionChipRadius = -1.0f;

        [SerializeField]
        internal int Id;

        [SerializeField]
        internal int SubId;

        [SerializeField]
        internal bool DetectDetachedChunks;

        [SerializeField]
        internal bool DestroyIfEmpty = true;

        private CoroutineHandle _scanSeparatedChunksCoroutine;
        private List<FracturedObject> _children = new List<FracturedObject>();
        private List<GroupedUnchippedObject> _groupedChildren = new List<GroupedUnchippedObject>();
        private UnchippedConnectivityGraph _connectivityGraph;

        // Forward collision events to the sub pieces
        private void OnCollisionEnter(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                var contact = collision.GetContact(i);
                if (contact.thisCollider.gameObject != gameObject)
                {
                    contact.thisCollider.gameObject.SendMessage("OnCollisionEnter", collision, SendMessageOptions.DontRequireReceiver);
                }
            }

            if (CollisionChipRadius >= 0.0f)
            {
                // We must manually chip off pieces
                ChipOnFracture.ChipOnCollision(this, collision, CollisionChipRadius);
            }
        }

        private void OnDestroy()
        {
            FractureEngine.CancelCoroutine(_scanSeparatedChunksCoroutine);
            CleanupSeparateChunksData();
        }

        internal void StartScanForSeparatedChunks()
        {
            if (DetectDetachedChunks)
            {
                FractureEngine.CancelCoroutine(_scanSeparatedChunksCoroutine);
                CleanupSeparateChunksData();

                _scanSeparatedChunksCoroutine = FractureEngine.QueueCoroutine(ScanForSeparatedChunks());
            }
        }

        private IEnumerator ScanForSeparatedChunks()
        {
            _connectivityGraph = UnchippedConnectivityGraphPool.Instance.GetInstance();

#if UNITY_2022_2_OR_NEWER || UNITY_2021_3 || UNITY_2020_3
            GetComponentsInChildren(includeInactive: false, result: _children);
#else
            _children.Clear();
            var children = GetComponentsInChildren<FracturedObject>(includeInactive: false);
            _children.Capacity = Mathf.Max(_children.Capacity, children.Length);
            foreach (var child in children)
            {
                _children.Add(child);
            }
#endif
            // We don't want us in the list
            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i].gameObject == gameObject)
                {
                    _children.RemoveFastAt(i);
                    break;
                }
            }

            if (_children.Count > 1)
            {
                _connectivityGraph.SetTotalCount(_children.Count);

                for (int c = 0; c < _children.Count; ++c)
                {
                    var child = _children[c];
                    Debug.Assert(child.gameObject != gameObject);

                    _connectivityGraph.AddNewObject(c, _children[c]);

                    for (int oc = c + 1; oc < _children.Count; ++oc)
                    {
                        var otherChild = _children[oc];
                        if (DoPiecesTouch(child, otherChild))
                        {
                            _connectivityGraph.Connect(c, oc);
                        }
                    }

                    yield return null;
                }

                yield return _connectivityGraph.GetGroups(_groupedChildren);

                // Items are always sorted by group in ascending order starting at group 0
                if (_groupedChildren[_groupedChildren.Count - 1].Group > 0)
                {
                    // We have more than one group and must split
                    yield return SeparateByGroups();
                }
                else
                {
                    // Just refresh our kinematic status
                    ChipOnFracture.RefreshKinematicStateForRoot(this);
                }
            }

            CleanupSeparateChunksData();
        }

        private IEnumerator SeparateByGroups()
        {
            // Give each new group a new root
            int startObjIdx = 0;
            int curGroup = 0;
            for (int i = 0; i < _groupedChildren.Count; ++i)
            {
                if (_groupedChildren[i].Group != curGroup)
                {
                    // Don't move objects from the first group. We'll just refresh our kinematic status
                    if (curGroup > 0)
                    {
                        ChipOnFracture.MoveObjectsToNewRoot(this, _groupedChildren, startObjIdx, i);
                    }

                    curGroup = _groupedChildren[i].Group;
                    startObjIdx = i;

                    yield return null;
                }
            }

            // Last group
            if (startObjIdx < _groupedChildren.Count)
            {
                ChipOnFracture.MoveObjectsToNewRoot(this, _groupedChildren, startObjIdx, _groupedChildren.Count);
            }

            // Appropriate children have been removed, so we can update the status of this root
            ChipOnFracture.RefreshKinematicStateForRoot(this);
            ChipOnFracture.RecalculateRootProperties(this, fracturedChildToIgnore: null);

            yield break;
        }

        private static bool DoPiecesTouch(FracturedObject x, FracturedObject y)
        {
            if (x.TryGetComponent(out Collider xCol) && y.TryGetComponent(out Collider yCol))
            {
                const float cPosAdjustment = 0.01f;
                var dir = (y.transform.position - x.transform.position);

                var xPos = x.transform.position + dir * cPosAdjustment;
                var yPos = y.transform.position;

                return Physics.ComputePenetration(xCol, xPos, x.transform.rotation, yCol, yPos, y.transform.rotation, out _, out _);
            }

            return false;
        }

        private void CleanupSeparateChunksData()
        {
            UnchippedConnectivityGraphPool.Instance.Release(_connectivityGraph);
            _connectivityGraph = null;

            _children.Clear();
            _groupedChildren.Clear();
        }
    }
}