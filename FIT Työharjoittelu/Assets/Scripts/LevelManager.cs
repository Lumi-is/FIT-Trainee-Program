using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;

public class LevelManager : MonoBehaviour
{
    [SerializeField] LevelPreset CurrentLevelStat;
    [SerializeField] GameObject weakEnemy;
    [SerializeField] GameObject avgEnemy; // Get these three from the rescource folder
    [SerializeField] GameObject strongEnemy;

    [SerializeField] Transform Player;

    // Start is called before the first frame update
    void Start()
    {
        int WeakEnemyAmount = CurrentLevelStat.weakEnemyAmount;
        int AvgEnemyAmount = CurrentLevelStat.avgEnemyAmount;

        GameObject[] LevelEnemies = new GameObject[CurrentLevelStat.MaxEnemyAmount];

        for(int i = 0; i < CurrentLevelStat.MaxEnemyAmount; i++)
        {
            if(WeakEnemyAmount > 0)
            {
                LevelEnemies[i] = weakEnemy;
                WeakEnemyAmount--;
            } else if(AvgEnemyAmount > 0)
            {
                LevelEnemies[i] = avgEnemy;
                AvgEnemyAmount--;
            } else {
                LevelEnemies[i] = strongEnemy;
            }

            int Random = UnityEngine.Random.Range(0,CurrentLevelStat.MaxEnemyAmount);

            SpawnEnemy(LevelEnemies[Random]);
            Debug.Log(LevelEnemies[Random].name);
        }   
    }

    private void SpawnEnemy(GameObject EnemyType)
    {
        Instantiate(EnemyType, new Vector3(0,0,0), Quaternion.identity);
    }
}
