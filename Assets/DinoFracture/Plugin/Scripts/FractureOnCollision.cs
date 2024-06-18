using UnityEngine;

namespace DinoFracture
{
    /// <summary>
    /// This component will cause a fracture to happen at the point of impact.
    /// </summary>
    public class FractureOnCollision : MonoBehaviour
    {
        /// <summary>
        /// The minimum amount of force required to fracture this object.
        /// Set to 0 to have any amount of force cause the fracture.
        /// </summary>
        [UnityEngine.Tooltip("The minimum amount of force required to fracture this object. Set to 0 to have any amount of force cause the fracture.")]
        public float ForceThreshold;

        /// <summary>
        /// Falloff radius for transferring the force of the impact
        /// to the resulting pieces. Any piece outside of this falloff
        /// from the point of impact will have no additional impulse
        /// set on it.
        /// </summary>
        [UnityEngine.Tooltip("Falloff radius for transferring the force of the impact to the resulting pieces. Any piece outside of this falloff from the point of impact will have no additional impulse set on it.")]
        public float ForceFalloffRadius = 1.0f;

        /// <summary>
        /// If true and this is a kinematic body, an impulse will be
        /// applied to the colliding body to counter the effects of
        /// hitting a kinematic body. If false and this is a kinematic
        /// body, the colliding body will bounce off as if this were an
        /// unmovable wall.
        /// </summary>
        [UnityEngine.Tooltip("If true and this is a kinematic body, an impulse will be applied to the colliding body to counter the effects of' hitting a kinematic body. If false and this is a kinematic body, the colliding body will bounce off as if this were an unmovable wall.")]
        public bool AdjustForKinematic = true;

        /// <summary>
        /// If true, the force calculations will become more consistent
        /// with all incoming collisions. The force will be the same
        /// given the same incoming rigid body properties instead of
        /// changing as this mass changes.
        /// </summary>
        /// <remarks>
        /// Recommended to set to true for consistency. Defaults to false for
        /// backwards compatibility.
        /// 
        /// The incoming force calculations will change, requiring re-tweaking
        /// of the ForceThreshold value.
        /// </remarks>
        [UnityEngine.Tooltip("If true, the force calculations will become more consistent with all incoming collisions. The force will be the same given the same incoming rigid body properties instead of changing as this mass changes.\n\nRecommended to set to true for consistency. Defaults to false for backwards compatibility.\n\nThe incoming force calculations will change, requiring re-tweaking of the ForceThreshold value.")]
        public bool ConsistentImpactForce = false;

        /// <summary>
        /// The collision layers that are allowed to cause a fracture.
        /// </summary>
        [UnityEngine.Tooltip("The collision layers that are allowed to cause a fracture.")]
        public LayerMask CollidableLayers = (LayerMask)(-1);

        /// <summary>
        /// If true, collision energies will be output in the editor to help tune required force thresholds.
        /// </summary>
        [UnityEngine.Tooltip("If true, collision energies will be output in the editor to help tune required force thresholds.")]
        public bool OutputDebugCollisionInfo = false;

        private Vector3 _impactImpulse;
        private float _impactMass;
        private Vector3 _impactPoint;
        private Rigidbody _impactBody;
        private Vector3 _impactVelocity;
        private Vector3 _thisVelocity;

        private FractureGeometry _fractureGeometry;
        private Rigidbody _thisBody;
        private float _thisMass;

        private bool _fireFracture = false;

        public void CopyFrom(FractureOnCollision other)
        {
            ForceThreshold = other.ForceThreshold;
            ForceFalloffRadius = other.ForceFalloffRadius;
            AdjustForKinematic = other.AdjustForKinematic;
            ConsistentImpactForce = other.ConsistentImpactForce;
            CollidableLayers = other.CollidableLayers;
            OutputDebugCollisionInfo = other.OutputDebugCollisionInfo;
        }

        private void Awake()
        {
            TryGetComponent(out _fractureGeometry);
            RefreshRigidBody();

            this.enabled = false;
        }

        private void RefreshRigidBody()
        {
            if (_thisBody == null)
            {
                _thisBody = GetComponentInParent<Rigidbody>();

                if (_thisBody != null)
                {
                    _thisMass = _thisBody.mass;
                }
            }
        }

        private void OnCollisionEnter(Collision col)
        {
            if (_fractureGeometry != null && !_fractureGeometry.IsProcessingFracture)
            {
                GatherCollisionInfo(col);
            }
        }

        internal bool GatherCollisionInfo(Collision col)
        {
            if (!_fireFracture && col.contactCount > 0 && (CollidableLayers.value & (1 << col.gameObject.layer)) != 0)
            {
                _impactBody = col.rigidbody;
                _impactMass = (col.rigidbody != null) ? col.rigidbody.mass : 0.0f;

                _impactPoint = Vector3.zero;

                Vector3 avgNormal = Vector3.zero;
                if (!ConsistentImpactForce)
                {
                    float sumSeparation = 0.0f;
                    for (int i = 0; i < col.contactCount; i++)
                    {
                        var contact = col.GetContact(i);
                        if ((_fractureGeometry == null) || (contact.thisCollider.gameObject == gameObject))
                        {
                            float separation = Mathf.Max(1e-3f, contact.separation);

                            _impactPoint += contact.point * separation;
                            avgNormal -= contact.normal * separation;
                            sumSeparation += separation;
                        }
                    }
                    _impactPoint *= 1.0f / sumSeparation;
                    avgNormal = avgNormal.normalized;
                }
                else
                {
                    for (int i = 0; i < col.contactCount; i++)
                    {
                        var contact = col.GetContact(i);
                        if ((_fractureGeometry == null) || (contact.thisCollider.gameObject == gameObject))
                        {
                            _impactPoint += contact.point;
                            avgNormal -= contact.normal;
                        }
                    }
                    _impactPoint *= (1.0f / col.contactCount);
                    avgNormal = avgNormal.normalized;
                }

                bool doFracture = false;
                float forceMag;

                if (!ConsistentImpactForce)
                {
                    _impactImpulse = -avgNormal * col.impulse.magnitude;
                    forceMag = 0.5f * _impactImpulse.sqrMagnitude;
                }
                else
                {
                    RefreshRigidBody();
                    float invThisMass = (_thisBody != null) ? 1.0f / _thisBody.mass : 0.0f;
                    float invOtherMass = (_impactMass > 0.0f) ? 1.0f / _impactMass : 0.0f;

                    _thisVelocity = (_thisBody != null) ? _thisBody.velocity : Vector3.zero;
                    _impactVelocity = col.relativeVelocity - _thisVelocity;

                    _impactImpulse = avgNormal * (-Mathf.Max(0.0f, Vector3.Dot(-avgNormal, col.relativeVelocity)) / (invOtherMass + invThisMass));
                    forceMag = ((_impactMass > 0.0f) ? _impactMass : 1.0f) * Mathf.Max(0.0f, Vector3.Dot(col.relativeVelocity, -avgNormal));
                }

#if UNITY_EDITOR
                if (OutputDebugCollisionInfo)
                {
                    const float cSmallImpactForce = 1e-4f;  // Prevent spam from bodies that are settling
                    if (ForceThreshold <= cSmallImpactForce || forceMag > cSmallImpactForce)
                    {
                        Debug.Log($"FractureOnCollision: [This Obj: {gameObject.name}] [Colliding Obj: {col.gameObject.name}] [Impact Force: {forceMag}] [Force Threshold: {ForceThreshold}] [Do Fracture: {(forceMag >= ForceThreshold ? "True" : "False")}]", gameObject);
                    }
                }
#endif

                if (forceMag >= ForceThreshold)
                {
                    doFracture = true;
                }
                else
                {
                    _impactMass = 0.0f;
                }

                if (_fractureGeometry != null)
                {
                    _fireFracture = doFracture;
                    this.enabled = doFracture;
                }

                return doFracture;
            }

            return false;
        }

        private void Update()
        {
            if (_fireFracture && _fractureGeometry != null)
            {
                _fireFracture = false;

                Vector3 localPoint = transform.worldToLocalMatrix.MultiplyPoint(_impactPoint);
                _fractureGeometry.FractureType = FractureType.Shatter;
                _fractureGeometry.Fracture(localPoint);
            }

            this.enabled = false;
        }

        internal void OnFracture(OnFractureEventArgs args)
        {
            if (args.IsValid && (_fractureGeometry == null || args.OriginalObject.gameObject == gameObject) && _impactMass > 0.0f)
            {
                Vector3 thisImpulse;
                if (!ConsistentImpactForce)
                {
                    thisImpulse = _impactImpulse * _thisMass / (_thisMass + _impactMass);
                }
                else
                {
                    Vector3 desiredVelocity = _impactImpulse / _impactMass;
                    desiredVelocity += _thisVelocity - ((_thisBody != null) ? _thisBody.velocity : Vector3.zero);
                    thisImpulse = desiredVelocity * _thisMass;
                }

                for (int i = 0; i < args.FracturePiecesRootObject.transform.childCount; i++)
                {
                    Transform piece = args.FracturePiecesRootObject.transform.GetChild(i);

                    Rigidbody rb = piece.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        float percentForce = FractureUtilities.GetFracturePieceRelativeMass(piece.gameObject);

                        if (ForceFalloffRadius > 0.0f)
                        {
                            float dist = (piece.position - _impactPoint).magnitude;
                            percentForce *= Mathf.Clamp01(1.0f - (dist / ForceFalloffRadius));
                        }

                        rb.AddForce(thisImpulse * percentForce, ForceMode.Impulse);
                    }
                }

                if (AdjustForKinematic)
                {
                    RefreshRigidBody();

                    // If the fractured body is kinematic, the collision for the colliding body will
                    // be as if it hit an unmovable wall.  Try to correct for that by adding the same
                    // force to colliding body.
                    if (_thisBody != null && _thisBody.isKinematic && _impactBody != null && _impactMass > 0.0f)
                    {
                        Vector3 impactBodyImpulse;
                        if (!ConsistentImpactForce)
                        {
                            impactBodyImpulse = _impactImpulse * _impactMass / (_thisMass + _impactMass);
                        }
                        else
                        {
                            Vector3 desiredVelocity = -_impactImpulse / _impactMass;
                            desiredVelocity += _impactVelocity - _impactBody.velocity;

                            impactBodyImpulse = desiredVelocity * _impactMass;
                        }
                        _impactBody.AddForce(impactBodyImpulse, ForceMode.Impulse);
                    }
                }

                // Allow another fracture for pre-fractured unchipped objects
                _fireFracture = false;
            }
        }
    }
}
