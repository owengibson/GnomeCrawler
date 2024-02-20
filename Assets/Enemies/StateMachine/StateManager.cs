using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{

    BaseState currentState;
    StateFactory states;

    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private GameObject _playerCharacter;
    [SerializeField] private GameObject _currentEnemy;
    [SerializeField] private float _distanceToPlayer;

    public BaseState CurrentState { get => currentState; set => currentState = value; }
    public GameObject PlayerCharacter { get => _playerCharacter; set => _playerCharacter = value; }
    public GameObject CurrentEnemy { get => _currentEnemy; set => _currentEnemy = value; }
    public float DistanceToPlayer { get => _distanceToPlayer; set => _distanceToPlayer = value; }
    public EnemyMovement EnemyMovement { get => enemyMovement; set => enemyMovement = value; }

    void Start()
    {
        states = new StateFactory(this);
        currentState = states.IdleState();
        currentState.EnterState();
    }

    void Update()
    {
        currentState.UpdateState();
        Debug.Log(currentState);
    }
}
