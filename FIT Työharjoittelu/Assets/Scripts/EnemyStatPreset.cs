using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatPreset", menuName = "Enemy")]
public class EnemyStatPreset : ScriptableObject
{
    public float MaxHP;
    public float HP;
    public float Speed;
    public float AttackSpeed;
    public bool isActiveAtSpawn;
}