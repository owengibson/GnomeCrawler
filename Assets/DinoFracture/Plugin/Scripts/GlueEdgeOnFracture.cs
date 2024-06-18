using System;
using UnityEngine;

namespace DinoFracture
{
    /// <summary>
    /// If the fracture pieces intersects with a specified trigger when
    /// created, the rigid body is destroyed and the piece becomes static.
    /// Otherwise, the piece will turn on gravity.  It’s best used if the
    /// FractureTemplate’s rigid body is set to not use gravity initially.  
    /// </summary>
    public class GlueEdgeOnFracture : MonoBehaviour
    {
        /// <summary>
        /// The piece will be glued if it intersects a trigger with this
        /// collision tag. Set to empty to allow any trigger to glue the piece.
        /// </summary>
        [Tooltip("The piece will be glued if it intersects a trigger with this collision tag. Set to empty to allow any trigger to glue the piece.")]
        public string CollisionTag = "";

        /// <summary>
        /// If true, the rigid body will be destroyed when glued. This will
        /// prevent further fractures that rely on collision events, but
        /// will reduce processing on Unity.
        /// </summary>
        [Tooltip("If true, the rigid body will be destroyed when glued. This will prevent further fractures that rely on collision events, but will reduce processing on Unity.")]
        public bool DestroyRigidBody = true;

        private const int cFractureFramesCountLeft = 2;
        private const int cRefreshFramesCountLeft = 2;

        private int _collisionCount;
        private int _frameCountLeft = 0;

        private RigidbodyConstraints _rigidBodyConstraints;
        private Vector3 _rigidBodyVelocity;
        private Vector3 _rigidBodyAngularVelocity;

        private Vector3 _impactPoint;
        private Vector3 _impactVelocity;
        private float _impactMass;
        private int _impactCount = 0;

        private void Awake()
        {
            // We need to ensure we are kinematic to begin
            // with so that we don't move because of collision
            // interactions on frame 1.
            if (TryGetComponent(out Rigidbody body))
            {
                _rigidBodyVelocity = body.velocity;
                _rigidBodyAngularVelocity = body.angularVelocity;
                _rigidBodyConstraints = body.constraints;

                body.isKinematic = true;
            }
        }

        public void RefreshGluedStatus()
        {
            if (Application.isPlaying)
            {
                if (TryGetComponent(out Rigidbody body))
                {
                    body.isKinematic = false;
                    body.constraints = RigidbodyConstraints.FreezeAll;
                    body.WakeUp();

                    _collisionCount = 0;
                    _frameCountLeft = cRefreshFramesCountLeft;

                    _impactPoint = Vector3.zero;
                    _impactVelocity = Vector3.zero;
                    _impactMass = 0;
                    _impactCount = 0;

                    this.enabled = true;
                }
            }
        }

        internal void CopyFrom(GlueEdgeOnFracture other)
        {
            CollisionTag = other.CollisionTag;
            DestroyRigidBody = other.DestroyRigidBody;
        }

        private void OnCollisionEnter(Collision col)
        {
            if (String.IsNullOrEmpty(CollisionTag) || col.collider.CompareTag(CollisionTag))
            {
                _collisionCount++;
                this.enabled = true;
            }
            else
            {
                _impactMass += (col.rigidbody != null) ? col.rigidbody.mass : 1.0f;
                _impactVelocity += col.relativeVelocity;

                Vector3 impactPoint = Vector3.zero;
                for (int i = 0; i < col.contacts.Length; i++)
                {
                    impactPoint += col.contacts[i].point;
                }
                _impactPoint += impactPoint * 1.0f / col.contacts.Length;
                ++_impactCount;
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            if (String.IsNullOrEmpty(CollisionTag) || col.CompareTag(CollisionTag))
            {
                _collisionCount++;
                this.enabled = true;
            }
        }

        private void OnTriggerStay(Collider col)
        {
            if (this.enabled)
            {
                if (String.IsNullOrEmpty(CollisionTag) || col.CompareTag(CollisionTag))
                {
                    _collisionCount++;
                }
            }
        }

        private void FixedUpdate()
        {
            if (_frameCountLeft > 0)
            {
                SetGlued(_collisionCount > 0);

                _frameCountLeft--;
                if (_frameCountLeft <= 0)
                {
                    if (_collisionCount == 0)
                    {
                        if (TryGetComponent(out Rigidbody body))
                        {
                            body.constraints = _rigidBodyConstraints;
                            body.angularVelocity = _rigidBodyAngularVelocity;
                            body.velocity = _rigidBodyVelocity;

                            body.WakeUp();

                            if (_impactCount > 0)
                            {
                                Vector3 force = _impactMass * _impactVelocity / (body.mass + _impactMass);
                                body.AddForceAtPosition(force * body.mass, (_impactPoint / _impactCount), ForceMode.Impulse);
                            }
                        }
                    }

                    // Stop receiving updates
                    this.enabled = false;
                    _collisionCount = 0;
                    _frameCountLeft = 0;
                }
            }
            else
            {
                this.enabled = false;
            }
        }

        private void OnFracture(OnFractureEventArgs fractureArgs)
        {
            if (TryGetComponent(out Rigidbody body))
            {
                body.isKinematic = false;   // Need to turn off kinematic to get collision events
                body.constraints = RigidbodyConstraints.FreezeAll;

                _impactPoint = Vector3.zero;
                _impactVelocity = Vector3.zero;
                _impactMass = 0.0f;

                _frameCountLeft = cFractureFramesCountLeft;
                this.enabled = true;
            }
        }

        private void SetGlued(bool glued)
        {
            if (glued)
            {
                if (TryGetComponent(out Rigidbody body))
                {
                    if (DestroyRigidBody)
                    {
                        Destroy(body);
                    }
                    else
                    {
                        body.isKinematic = true;
                    }
                }
            }
        }
    }
}