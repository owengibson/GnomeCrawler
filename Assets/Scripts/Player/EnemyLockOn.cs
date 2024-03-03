using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class EnemyLockOn : MonoBehaviour
    {
        public Camera playerCam;
        public CinemachineVirtualCamera lockOnCam;
        public Animator camAnimator;

        [SerializeField] LayerMask _targetLayers;
        [SerializeField] LayerMask _environmentLayers;
        [SerializeField] Transform _playerLockTransform;
        [SerializeField] private List<CombatBrain> _avaliableTargets = new List<CombatBrain>();
        [SerializeField] private CombatBrain _nearestLockOnTarget;

        [SerializeField] float _lockOnRadius = 30.0f;
        [SerializeField] float _minimumViewableAngle = -50.0f;
        [SerializeField] float _maximumViewableAngle = 50.0f;
        [SerializeField] bool _isLockedOn = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Slash))
            {
                if (!_isLockedOn)
                {
                    _isLockedOn = true;
                    HandleLocatingLockOnTargets();
                    lockOnCam.LookAt = _nearestLockOnTarget._lockOnTransform;
                    camAnimator.Play("LockCam");
                }
                else
                {
                    _isLockedOn = false;
                    camAnimator.Play("FollowCam");
                    ClearLockOnTargets();
                    return;
                }
            }
        }

        public void HandleLocatingLockOnTargets()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(_playerLockTransform.position, _lockOnRadius, _targetLayers);

            for (int i = 0; i < colliders.Length; i++)
            {
                CombatBrain lockOnTarget = colliders[i]?.GetComponent<CombatBrain>();

                if (lockOnTarget != null)
                {
                    Vector3 lockOnTargetDirection = lockOnTarget.transform.position - _playerLockTransform.position;
                    float distanceFromPlayer = Vector3.Distance(_playerLockTransform.position, lockOnTarget.transform.position);
                    float viewableAngle = Vector3.Angle(lockOnTargetDirection, playerCam.transform.forward);

                    if (viewableAngle > _minimumViewableAngle && viewableAngle < _maximumViewableAngle)
                    {
                        RaycastHit hit;

                        if (Physics.Linecast(_playerLockTransform.position, lockOnTarget._lockOnTransform.position, out hit, _environmentLayers))
                        {
                            continue;
                        }
                        else
                        {
                            _avaliableTargets.Add(lockOnTarget);
                        }
                    }
                }
            }

            for (int k = 0; k < _avaliableTargets.Count; k++)
            {
                if (_avaliableTargets[k] != null)
                {
                    float distanceFromTarget = Vector3.Distance(_playerLockTransform.position, _avaliableTargets[k].transform.position);
                    Vector3 lockTragetDirection = _avaliableTargets[k].transform.position - _playerLockTransform.position;

                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        _nearestLockOnTarget = _avaliableTargets[k];
                    }
                }
            }
        }

        public void ClearLockOnTargets()
        {
            _nearestLockOnTarget = null;
            _avaliableTargets.Clear();
        }
    }
}
