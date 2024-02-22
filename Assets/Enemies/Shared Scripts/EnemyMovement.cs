using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] public float _speed = 5f;
    [SerializeField] private GameObject _playerCharacter;

    public void ApproachPlayer()
    {
        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _playerCharacter.transform.position, step);
    }

}
