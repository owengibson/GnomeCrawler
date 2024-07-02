using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GnomeCrawler
{
    public class Adds : IState
    {
        public float AddsTestTimer;

        private readonly Boss _boss;
        private readonly Animator _animator;

        private static readonly int PhaseShiftHash = Animator.StringToHash("PhaseShift");
        private static readonly int ReturnHash = Animator.StringToHash("Return");
        public Adds(Boss boss, Animator animator)
        {
            _boss = boss;
            _animator = animator;
        }
        public void Tick()
        {
            AddsTestTimer += Time.deltaTime;
        }

        public void OnEnter()
        {
            //_boss.gameObject.transform.position = new Vector3(_boss.gameObject.transform.position.x, 10, _boss.gameObject.transform.position.z);
            _animator.SetTrigger(PhaseShiftHash);
            AddsTestTimer = 0;
        }

        public void OnExit()
        {
            _animator.SetTrigger(ReturnHash);
        }
    }
}
