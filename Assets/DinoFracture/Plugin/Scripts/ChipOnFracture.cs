#define LOOKUP_BY_NAME

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DinoFracture
{
    /// <summary>
    /// This component allows a fracture geometry to lose only bits of geometry
    /// at a time instead of completely collapsing after fracture. It works
    /// best when fracturing is done at a point instead and in conjunction with
    /// the <see cref="DinoFracture.GlueEdgeOnFracture">GlueEdgeOnFracture</see>
    /// script on the fracture template.
    /// </summary>
    public class ChipOnFracture : MonoBehaviour
    {
        struct ListReference<T> : IDisposable
        {
            private List<T> _ref;

            public void Dispose()
            {
                _ref.Clear();
            }

            public static ListReference<T> Get(ref List<T> item)
            {
                ListReference<T> ret = new ListReference<T>();
                ret._ref = item;
                return ret;
            }
        }

        private static List<FracturedObject> _sChippedObjectList = new List<FracturedObject>();
        private static List<FracturedObject> _sUnchippedObjectList = new List<FracturedObject>();
        private static List<ChippedFractureRoot> _sRootObjectList = new List<ChippedFractureRoot>();

        /// <summary>
        /// The radius, in world space units, around the point of fracture that
        /// we will consider fracture pieces to be chipped off.
        /// </summary>
        /// <remarks>
        /// A fracture piece must be fully contained within the radius to be
        /// considered chipped.
        /// 
        /// If this is set to <= 0.0, the FractureSize on the FractureGeometry will
        /// be used.
        /// </remarks>
        [Tooltip("The radius, in world space units, around the point of fracture that we will consider fracture pieces to be chipped off.\r\n\r\nA fracture piece must be fully contained within the radius to be considered chipped.\r\n\r\nIf this is set to <= 0.0, the FractureSize on the FractureGeometry will be used.")]
        public float Radius;

        /// <summary>
        /// Add this component to child items if not already present in the FractureTemplate.
        /// It is recommended to keep this true unless you explicitly add it to the FractureTemplate.
        /// </summary>
        [Tooltip("Add this component to child items if not already present in the FractureTemplate. It is recommended to keep this true unless you explicitly add it to the FractureTemplate.")]
        public bool EnsureChildComponents = true;

        /// <summary>
        /// If true, the collection of unchipped objects will be scanned when a fracture happens to
        /// see if major sections of the collection have been split apart. If so, each section will
        /// be separated with distinct rigid bodies. This allows for the chipping away of structural
        /// foundations to cause pieces above to collapse.
        /// </summary>
        /// <remarks>
        /// All detected separate chunks will be made non-kinematic. Use the GlueEdgeOnFracture script
        /// on the original fracture template to keep structure foundations from moving.
        /// </remarks>
        [Tooltip("If true, the collection of unchipped objects will be scanned when a fracture happens to see if major sections of the collection have been split apart. If so, each section will be separated with distinct rigid bodies. This allows for the chipping away of structural foundations to cause pieces above to collapse.\n\n All detected separate chunks will be made non-kinematic. Use the GlueEdgeOnFracture script on the original fracture template to keep structure foundations from moving.")]
        public bool DetectDetachedChunks = false;

        /// <summary>
        /// If true, unchipped roots will be destroyed if all the child objects have been fractured.
        /// It is recommended to set this to true.
        /// </summary>
        [Tooltip("If true, unchipped roots will be destroyed if all the child objects have been fractured. It is recommended to set this to true.")]
        public bool DestroyIfEmpty = true;

        public void CopyFrom(ChipOnFracture other)
        {
            Radius = other.Radius;
            DetectDetachedChunks = other.DetectDetachedChunks;
            DestroyIfEmpty = other.DestroyIfEmpty;
        }

        /// <summary>
        /// OnPostFracture is called by the fracture engine after all the regular OnFracture
        /// callbacks have fired. Because this method changes the generated fracture tree,
        /// it's important not to intefere with callbacks that might assume as certain
        /// tree structure.
        /// </summary>
        /// <param name="args"></param>
        private void OnPostFracture(OnFractureEventArgs args)
        {
            if (args.OriginalObject.gameObject == gameObject)
            {
                using (ListReference<FracturedObject>.Get(ref _sUnchippedObjectList))
                using (ListReference<FracturedObject>.Get(ref _sChippedObjectList))
                {
                    if (args.FractureDetails is ShatterDetails shatterDetails)
                    {
                        GetChippedObjects(args, shatterDetails);

                        var root = ProcessShatterChippedObjects(args);
                        if (root != null)
                        {
                            root.StartScanForSeparatedChunks();
                        }
                    }
                    else if (args.FractureDetails is SliceDetails sliceDetails)
                    {
                        GetChippedObjects(args, sliceDetails);

                        ProcessChippedObjects(args, sliceDetails);
                    }

                    if (args.FracturePiecesRootObject.transform.childCount == 0)
                    {
                        Destroy(args.FracturePiecesRootObject);
                    }
                }
            }
        }

        #region Shatter

        private void GetChippedObjects(OnFractureEventArgs args, ShatterDetails shatterDetails)
        {
            if (shatterDetails.FractureSize.Value > 0)
            {
                Vector3 fractureCenterWS = args.FracturePiecesRootObject.transform.TransformPoint(shatterDetails.FractureCenter);
                float radius = (Radius > 0) ? Radius : shatterDetails.FractureSize.GetWorldSpaceSize(args.OriginalMeshBounds.size).magnitude * 0.5f;

                for (int i = 0; i < args.FracturePiecesRootObject.transform.childCount; i++)
                {
                    var childTrans = args.FracturePiecesRootObject.transform.GetChild(i);
                    if (childTrans.TryGetComponent(out FracturedObject fo))
                    {
                        if (childTrans.TryGetComponent(out Collider childCollider))
                        {
                            Vector3 colliderCenter = childTrans.position;
                            float colliderRadius = childCollider.bounds.extents.magnitude;

                            if ((fractureCenterWS - colliderCenter).magnitude + colliderRadius > radius)
                            {
                                _sUnchippedObjectList.Add(fo);
                                continue;
                            }
                        }

                        // Chipped
                        _sChippedObjectList.Add(fo);
                    }
                }
            }
            else
            {
                // Everything is unchipped
                for (int i = 0; i < args.FracturePiecesRootObject.transform.childCount; i++)
                {
                    var childTrans = args.FracturePiecesRootObject.transform.GetChild(i);
                    if (childTrans.TryGetComponent(out FracturedObject fo))
                    {
                        _sUnchippedObjectList.Add(fo);
                    }
                }
            }
        }

        private ChippedFractureRoot ProcessShatterChippedObjects(OnFractureEventArgs args)
        {
            ChippedFractureRoot root = GetComponentInParent<ChippedFractureRoot>();

            if (_sUnchippedObjectList.Count > 0)
            {
                if (root == null)
                {
                    root = CreateNewRoot(null, 0, args);
                }

                for (int i = 0; i < _sUnchippedObjectList.Count; i++)
                {
                    FracturedObject fractureChild = _sUnchippedObjectList[i];

                    AddChildToRoot(fractureChild, root, args);
                }
            }

            if (root != null)
            {
                RecalculateRootProperties(root, args.OriginalObject.gameObject);
            }

            return root;
        }

        #endregion

        #region Slice

        private void GetChippedObjects(OnFractureEventArgs args, SliceDetails sliceDetails)
        {
            // Add all the newly sliced objects
            for (int i = 0; i < args.FracturePiecesRootObject.transform.childCount; i++)
            {
                FracturedObject fo = args.FracturePiecesRootObject.transform.GetChild(i).GetComponent<FracturedObject>();
                if (fo != null)
                {
                    _sUnchippedObjectList.Add(fo);
                }
            }

            // We also need to re-evaluate the existing pieces that have not been fractured
            ChippedFractureRoot curRoot = GetComponentInParent<ChippedFractureRoot>();
            if (curRoot != null)
            {
                for (int i = 0; i < curRoot.transform.childCount; i++)
                {
                    var child = curRoot.transform.GetChild(i);
                    if (child.gameObject.activeSelf && child.gameObject != gameObject)
                    {
                        FracturedObject childFracturedObject = child.GetComponent<FracturedObject>();
                        if (childFracturedObject != null)
                        {
                            _sUnchippedObjectList.Add(childFracturedObject);
                        }
                    }
                }
            }
        }

        private void ProcessChippedObjects(OnFractureEventArgs args, SliceDetails sliceDetails)
        {
            if (_sUnchippedObjectList.Count == 0)
            {
                return;
            }

            using (ListReference<ChippedFractureRoot>.Get(ref _sRootObjectList))
            {
                ChippedFractureRoot curRoot = GetComponentInParent<ChippedFractureRoot>();

                for (int i = 0; i < _sUnchippedObjectList.Count; i++)
                {
                    FracturedObject fractureChild = _sUnchippedObjectList[i];
                    int childId = GenerateFracturedChildSubId(fractureChild, sliceDetails);

                    ChippedFractureRoot root = GetChipRoot(curRoot, sliceDetails.FractureFrame, childId);
                    if (root == null)
                    {
                        root = CreateNewRoot(curRoot, childId, args);
                    }

                    AddChildToRoot(fractureChild, root, args);

                    if (!_sRootObjectList.Contains(root))
                    {
                        _sRootObjectList.Add(root);
                    }
                }

                for (int i = 0; i < _sRootObjectList.Count; i++)
                {
                    RecalculateRootProperties(_sRootObjectList[i], args.OriginalObject.gameObject);
                    RefreshKinematicStateForRoot(_sRootObjectList[i]);
                    _sRootObjectList[i].StartScanForSeparatedChunks();
                }
            }
        }

        private int GenerateFracturedChildSubId(FracturedObject fractureChild, SliceDetails sliceDetails)
        {
            // Create an ID based on which side of the planes we are on
            int id = 0;
            for (int i = 0; i < sliceDetails.SlicingPlanes.Count; i++)
            {
                var localSlicePlane = sliceDetails.SlicingPlanes[i].ToPlane();
                var worldSlicePlane = transform.localToWorldMatrix.TransformPlane(localSlicePlane);

                bool inFront = worldSlicePlane.GetSide(fractureChild.transform.position);
                if (inFront)
                {
                    id |= (1 << i);
                }
            }
            return id;
        }

        private static ChippedFractureRoot GetChipRoot(ChippedFractureRoot curRoot, int id, int subId)
        {
            if (curRoot == null)
            {
                return null;
            }

            if (subId == 0 && curRoot.Id != id)
            {
                curRoot.Id = id;
                curRoot.SubId = 0;
                curRoot.name = GetRootName(curRoot, id, 0);
                return curRoot;
            }

#if LOOKUP_BY_NAME
            var curRootParent = curRoot.transform.parent;
            if (curRootParent == null)
            {
                var foundChild = curRootParent.Find(GetRootName(curRoot, id, subId));
                if (foundChild != null)
                {
                    return foundChild.GetComponent<ChippedFractureRoot>();
                }
            }
            else
            {
                var foundGO = GameObject.Find(GetRootName(curRoot, id, subId));
                if (foundGO != null)
                {
                    return foundGO.GetComponent<ChippedFractureRoot>();
                }
            }
#else
            // Find sibilings of the cur root that have the same id
            var curRootParent = curRoot.transform.parent;
            if (curRootParent != null)
            {
                for (int i = 0; i < curRootParent.childCount; i++)
                {
                    ChippedFractureRoot root = curRootParent.GetChild(i).GetComponent<ChippedFractureRoot>();
                    if (root != null && root.Id == id && root.SubId == subId)
                    {
                        return root;
                    }
                }
            }
            else
            {
                var roots = FindObjectsOfType<ChippedFractureRoot>(true);
                for (int i = 0; i < roots.Length; i++)
                {
                    if (roots[i].transform.parent == null && roots[i].Id == id && roots[i].SubId == subId)
                    {
                        return roots[i];
                    }
                }
            }
#endif

            return null;
        }

        #endregion

        #region Collision

        internal static void ChipOnCollision(ChippedFractureRoot root, Collision collision, float radius)
        {
            // Transfer the collision to the chipped pieces
            if (root.TryGetComponent(out FractureOnCollision foc))
            {
                if (foc.GatherCollisionInfo(collision))
                {
                    using (ListReference<FracturedObject>.Get(ref _sChippedObjectList))
                    {
                        GetChippedObjects(root, collision, radius);

                        if (_sChippedObjectList.Count > 0)
                        {
                            GameObject newRoot = new GameObject("Fracture Root");
                            newRoot.transform.localPosition = root.transform.localPosition;
                            newRoot.transform.localRotation = root.transform.localRotation;
                            newRoot.transform.localScale = root.transform.localScale;
                            newRoot.transform.SetParent(root.transform.parent, worldPositionStays: false);

                            for (int i = 0; i < _sChippedObjectList.Count; i++)
                            {
                                FracturedObject fo = _sChippedObjectList[i];

                                // The new root has the same transform as the old parent, so we can keep the local transform
                                fo.transform.SetParent(newRoot.transform, worldPositionStays: false);

                                if (fo.TryGetComponent(out MeshCollider mc))
                                {
                                    mc.convex = true;
                                }

                                if (!fo.TryGetComponent(out Rigidbody rb))
                                {
                                    rb = fo.gameObject.AddComponent<Rigidbody>();
                                    rb.mass = fo.ThisMass;
                                    rb.isKinematic = false;
                                }

                                if (fo.TryGetComponent(out GlueEdgeOnFracture glueEdge))
                                {
                                    // Normally we check on awake, but we are already active at this point.
                                    // Also, we didn't have a rigid body before. So now we need to refresh.
                                    glueEdge.RefreshGluedStatus();
                                }
                            }

                            Physics.SyncTransforms();

                            OnFractureEventArgs fakeArgs = new OnFractureEventArgs(null, new Bounds(), newRoot, null);
                            foc.OnFracture(fakeArgs);

                            // Update what's left of the old root
                            RecalculateRootProperties(root, fracturedChildToIgnore: null);
                            root.StartScanForSeparatedChunks();
                        }
                    }
                }
            }
        }

        private static void GetChippedObjects(ChippedFractureRoot root, Collision collision, float radius)
        {
            if (collision.contactCount > 0)
            {
                var contact = collision.GetContact(0);
                Collider hitCollider = contact.thisCollider;

                // Always ensure the hit collider will be chipped
                if (hitCollider != null)
                {
                    if (hitCollider.TryGetComponent(out FracturedObject fo))
                    {
                        _sChippedObjectList.Add(fo);
                    }
                }

                if (radius > 0)
                {
                    for (int i = 0; i < root.transform.childCount; i++)
                    {
                        var childTrans = root.transform.GetChild(i);
                        if (childTrans.TryGetComponent(out FracturedObject fo))
                        {
                            if (childTrans.TryGetComponent(out Collider childCollider) && childCollider != hitCollider)
                            {
                                Vector3 colliderCenter = childTrans.position;
                                float colliderRadius = childCollider.bounds.extents.magnitude;

                                if ((contact.point - colliderCenter).magnitude + colliderRadius > radius)
                                {
                                    continue;
                                }
                            }

                            // Chipped
                            _sChippedObjectList.Add(fo);
                        }
                    }
                }
            }
        }

        #endregion

        #region Prefracture

        /// <summary>
        /// Returns the unchipped root gameobject
        /// </summary>
        internal GameObject OnPrefractureComplete(OnFractureEventArgs args)
        {
            using (ListReference<FracturedObject>.Get(ref _sUnchippedObjectList))
            using (ListReference<FracturedObject>.Get(ref _sChippedObjectList))
            {
                if (args.FractureDetails is ShatterDetails shatterDetails)
                {
                    GetChippedObjects(args, shatterDetails);
                }
                else if (args.FractureDetails is SliceDetails sliceDetails)
                {
                    GetChippedObjects(args, sliceDetails);
                }

                var root = ProcessShatterChippedObjects(args);

                // Move from out of the fracture pieces root
                if (root.transform.IsChildOf(args.FracturePiecesRootObject.transform))
                {
                    root.transform.SetParent(args.FracturePiecesRootObject.transform.parent, worldPositionStays: true);
                }

                if (args.FracturePiecesRootObject.transform.childCount == 0)
                {
                    DestroyImmediate(args.FracturePiecesRootObject);
                }

                return root?.gameObject;
            }
        }

        #endregion

        #region Separation

        /// <summary>
        /// Creates a new root for the selected children
        /// </summary>
        /// <param name="startIdx">Inclusive</param>
        /// <param name="endIdx">Exclusive</param>
        internal static void MoveObjectsToNewRoot(ChippedFractureRoot curRoot, List<GroupedUnchippedObject> allChildren, int startIdx, int endIdx)
        {
            var newRoot = DuplicateRoot(curRoot, Time.frameCount, curRoot.SubId);

            for (int i = startIdx; i < endIdx; ++i)
            {
                allChildren[i].Object.transform.SetParent(newRoot.transform, worldPositionStays: true);
            }

            RecalculateRootProperties(newRoot, fracturedChildToIgnore: null);
        }

        internal static void RefreshKinematicStateForRoot(Component root)
        {
            if (root.TryGetComponent(out GlueEdgeOnFracture glueEdges))
            {
                // Since we aren't being fractured ourselves we need to manually check
                // if this piece should be glued.
                glueEdges.RefreshGluedStatus();
            }
        }

        #endregion

        #region Common

        private void AddChildToRoot(FracturedObject unchippedChild, ChippedFractureRoot root, OnFractureEventArgs args)
        {
            var childGO = unchippedChild.gameObject;
            childGO.transform.SetParent(root.transform, worldPositionStays: true);

            if (Application.isPlaying)
            {
                Destroy(childGO.GetComponent<Rigidbody>());
            }
            else
            {
                DestroyImmediate(childGO.GetComponent<Rigidbody>());
            }

            FractureGeometry fractureGeom = unchippedChild.GetComponent<FractureGeometry>();
            if (fractureGeom == null && EnsureChildComponents)
            {
                fractureGeom = childGO.AddComponent<RuntimeFracturedGeometry>();
                fractureGeom.CopyFrom(args.OriginalObject);
                fractureGeom.ForceValidGeometry();
            }
            else if (fractureGeom != null)
            {
                // Allow us to fracture again as we haven't actually been removed
                fractureGeom.NumGenerations = args.OriginalObject.NumGenerations;

                fractureGeom.FractureSize = args.OriginalObject.FractureSize;
                fractureGeom.NumFracturePieces = args.OriginalObject.NumFracturePieces;
                fractureGeom.NumIterations = args.OriginalObject.NumIterations;

                fractureGeom.ForceValidGeometry();
            }

            if (EnsureChildComponents)
            {
                if (TryGetComponent(out FractureOnCollision thisColComp) && childGO.GetComponent<FractureOnCollision>() == null)
                {
                    FractureOnCollision fractureComp = childGO.AddComponent<FractureOnCollision>();
                    fractureComp.CopyFrom(thisColComp);
                }

                if (TryGetComponent(out FractureOnParticleCollision thisParticleColComp) && childGO.GetComponent<FractureOnParticleCollision>() == null)
                {
                    FractureOnParticleCollision childComp = childGO.AddComponent<FractureOnParticleCollision>();
                    childComp.CopyFrom(thisParticleColComp);
                }

                if (childGO.GetComponent<ChipOnFracture>() == null)
                {
                    ChipOnFracture chipComp = childGO.AddComponent<ChipOnFracture>();
                    chipComp.CopyFrom(this);
                }
            }
        }

        internal static void RecalculateRootProperties(ChippedFractureRoot root, GameObject fracturedChildToIgnore)
        {
            int childCount = 0;
            float totalMass = 0.0f;
            float totalVolume = 0.0f;
            Vector3 centerOfMass = Vector3.zero;

            for (int i = 0; i < root.transform.childCount; i++)
            {
                var child = root.transform.GetChild(i);
                if (child.gameObject.activeSelf && child.gameObject != fracturedChildToIgnore)
                {
                    FracturedObject childFracturedObject = child.GetComponent<FracturedObject>();

                    float childVolume = childFracturedObject.ThisMass;
                    float childMass = childFracturedObject.ThisVolume;

                    totalMass += childMass;
                    totalVolume += childVolume;

                    centerOfMass += child.localPosition * childMass;

                    childCount++;
                }
            }

            if (childCount > 0)
            {
                centerOfMass *= (1.0f / totalMass);

                Rigidbody rootRigidBody = root.GetComponent<Rigidbody>();
                rootRigidBody.mass = totalMass;
                rootRigidBody.centerOfMass = centerOfMass;

                var rootFracturedObject = root.GetComponent<FracturedObject>();
                rootFracturedObject.TotalMass = totalMass;
                rootFracturedObject.ThisMass = totalMass;
                rootFracturedObject.TotalVolume = totalVolume;
                rootFracturedObject.ThisVolume = totalVolume;
            }
            else if (root.DestroyIfEmpty)
            {
                Destroy(root.gameObject);
            }
            else
            {
                Destroy(root.GetComponent<Rigidbody>());
            }
        }

        private ChippedFractureRoot CreateNewRoot(ChippedFractureRoot curRoot, int subId, OnFractureEventArgs args)
        {
            Transform newRootParent;

            if (curRoot == null)
            {
                newRootParent = args.FracturePiecesRootObject.transform;
            }
            else
            {
                newRootParent = curRoot.transform.parent;
            }

            GameObject unchippedGO = new GameObject(GetRootName(curRoot, args.FractureDetails.FractureFrame, subId));
            unchippedGO.transform.SetParent(newRootParent, false);

            unchippedGO.transform.position = transform.position;
            unchippedGO.transform.localRotation = Quaternion.identity;
            unchippedGO.transform.localScale = Vector3.one;

            var chippedRoot = unchippedGO.AddComponent<ChippedFractureRoot>();
            chippedRoot.Id = args.FractureDetails.FractureFrame;
            chippedRoot.SubId = subId;
            chippedRoot.DetectDetachedChunks = DetectDetachedChunks;
            chippedRoot.DestroyIfEmpty = DestroyIfEmpty;

            if (curRoot != null)
            {
                CopyComponentsFrom(unchippedGO, curRoot, curRoot);
            }
            else
            {
                CopyComponentsFrom(unchippedGO, args.OriginalObject, args.FracturePiecesRootObject.transform);
            }

            if (!EnsureChildComponents)
            {
                // We didn't add a fracture component automatically. Assume we should manually chip off pieces.

                if (Radius > 0.0f)
                {
                    chippedRoot.CollisionChipRadius = Radius;
                }
                else
                {
                    if (args.OriginalObject.FractureSize.Value > 0)
                    {
                        Size fractureSize = new Size() { Space = args.OriginalObject.FractureSize.Space, Value = args.OriginalObject.FractureSize.Value };
                        chippedRoot.CollisionChipRadius = fractureSize.GetWorldSpaceSize(args.OriginalMeshBounds.size).magnitude * 0.5f;
                    }
                    else
                    {
                        chippedRoot.CollisionChipRadius = 0.0f;
                    }
                }
            }
            else
            {
                // We ensured a runtime fracture comp is available. No need to manually chip off pieces.
                chippedRoot.CollisionChipRadius = -1.0f;
            }

            return chippedRoot;
        }

        private static ChippedFractureRoot DuplicateRoot(ChippedFractureRoot curRoot, int id, int subId)
        {
            Transform newRootParent = curRoot.transform.parent;

            GameObject unchippedGO = new GameObject(GetRootName(curRoot, id, subId));
            unchippedGO.transform.SetParent(newRootParent, false);

            unchippedGO.transform.position = curRoot.transform.position;
            unchippedGO.transform.localRotation = Quaternion.identity;
            unchippedGO.transform.localScale = Vector3.one;

            var chippedRoot = unchippedGO.AddComponent<ChippedFractureRoot>();
            chippedRoot.Id = id;
            chippedRoot.SubId = subId;
            chippedRoot.DetectDetachedChunks = curRoot.DetectDetachedChunks;
            chippedRoot.CollisionChipRadius = curRoot.CollisionChipRadius;
            chippedRoot.DestroyIfEmpty = curRoot.DestroyIfEmpty;

            CopyComponentsFrom(unchippedGO, curRoot, curRoot);

            return chippedRoot;
        }

        private static void CopyComponentsFrom(GameObject root, Component fractureSource, Component pieceSource)
        {
            root.AddComponent<FracturedObject>();

            var newRigidBody = root.AddComponent<Rigidbody>();  // Must come before components that expect rigid bodies to be present

            // Keep us frozen if the original object was
            if (fractureSource.TryGetComponent(out Rigidbody rigidbody))
            {
                newRigidBody.isKinematic = rigidbody.isKinematic;
            }
            else
            {
                // No original rigid body. Remain static by being kinematic
                newRigidBody.isKinematic = true;
            }

            if (fractureSource.TryGetComponent(out FractureOnCollision foc))
            {
                var newFoc = root.AddComponent<FractureOnCollision>();
                newFoc.CopyFrom(foc);
            }

            // Glue edges is usually on the individual pieces instead of either on
            // the chipping root or the original fracture object.
            GlueEdgeOnFracture glueEdges = pieceSource.GetComponentInChildren<GlueEdgeOnFracture>(includeInactive: false);
            if (glueEdges != null)
            {
                var newGlueEdges = root.AddComponent<GlueEdgeOnFracture>();
                newGlueEdges.CopyFrom(glueEdges);

                // Since we aren't being fractured ourselves we need to manually check
                // if this piece should be glued.
                newGlueEdges.RefreshGluedStatus();
            }
        }

        private static string GetRootName(ChippedFractureRoot curRoot, int id, int subId)
        {
            if (curRoot == null)
            {
                return $"UnchippedRoot 0 {id} {subId}";
            }
            return $"UnchippedRoot {curRoot.GetInstanceID()} {id} {subId}";
        }

        #endregion
    }
}