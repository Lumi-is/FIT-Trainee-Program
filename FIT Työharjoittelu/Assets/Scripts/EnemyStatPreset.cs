using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatPreset", menuName = "Enemy/NewEnemyStatPreset")]
public class EnemyStatPreset : ScriptableObject
{
    public string PrefabName;
    public bool canDodge;

    public float dodgeDistance;
    public float MaxHP;
    public float HP;
    public float MaxDamageTaken;
    public float Speed;
    public float AttackSpeed;
    public bool isActiveAtSpawn;
}