using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class Adds : IState
    {
        public float AddsTestTimer;

        private readonly Boss _boss;
        private readonly Animator _animator;
        private Collider _collider;

        private static readonly int PhaseShiftHash = Animator.StringToHash("PhaseShift");
        private static readonly int ReturnHash = Animator.StringToHash("Return");
        public Adds(Boss boss, Animator animator, Collider collider)
        {
            _boss = boss;
            _animator = animator;
            _collider = collider;
        }
        public void Tick()
        {
            /*foreach (GameObject enemy in _boss._currentEnemies)
            {
                if (enemy == null)
                {
                    _boss._currentEnemies.Remove(enemy);
                    _boss._currentEnemiesNo--;
                }
            }*/
        }

        public void OnEnter()
        {
            _boss._canEnterPhase2 = false;
            _boss._canEnterPhase3 = false;

            _boss._combatBrain.IsLockable = false;
            _collider.enabled = false;
            _animator.SetTrigger(PhaseShiftHash);
            SpawnAdds();
        }

        public void OnExit()
        {
            _boss._combatBrain.IsLockable = true;
            _animator.SetTrigger(ReturnHash);
            _collider.enabled = true;
            _boss._currentEnemiesNo = -1;
        }

        private void SpawnAdds()
        {
            foreach (Transform trans in _boss._addsSpawnPoints)
            {
                GameObject child = trans.GetChild(0).gameObject;
                child.SetActive(true);
            }
            _boss._currentEnemiesNo = _boss._addsSpawnPoints.Length;
        }
    }
}
