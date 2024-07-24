using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "New level/Level template")]
public class LevelPreset : ScriptableObject
{
    public int CustomEnemySpawnType;
    public float delayBeforeStart;
    public int MaxEnemyAmount;
    public int weakEnemyAmount;
    public int avgEnemyAmount;
    public int strongEnemyAmount;

    public int MaxConcurrentEnemies;
    public float enemySpawnCooldown;
}