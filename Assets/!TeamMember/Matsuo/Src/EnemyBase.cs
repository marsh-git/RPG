using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase
{
    [Header("Enemy Settings")]
    [SerializeField] protected int moveRange = 1;
    [SerializeField] protected int searchRange = 3;
    [SerializeField] protected int expReward = 10;

    protected EnemyState currentState = EnemyState.Idle;

    protected Transform target;
}