using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GnomeCrawler
{
    public class PlayerStateFactory
    {
        PlayerStateMachine stateMachine;

        public PlayerStateFactory(PlayerStateMachine currentStateMachine)
        {
            stateMachine = currentStateMachine;
        }
    }
}
