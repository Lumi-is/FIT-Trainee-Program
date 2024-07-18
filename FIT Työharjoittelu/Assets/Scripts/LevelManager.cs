using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class LevelManager : MonoBehaviour
{
    [SerializeField] LevelPreset CurrentLevelStat;
    [SerializeField] EnemyStatPreset weakEnemyStats;
    [SerializeField] EnemyStatPreset avgEnemyStats; // Get these three from the rescource folder
    [SerializeField] EnemyStatPreset strongEnemyStats;
    [SerializeField] Transform Player;

    [SerializeField] Transform enemyParent;

    [SerializeField] float minDistanceFromPlayer;
    [SerializeField] float maxDistanceFromPlayer;

    int NumberOfEnemiesInScene;

    // Start is called before the first frame update
    void Start()
    {
        int WeakEnemyAmount = CurrentLevelStat.weakEnemyAmount;
        int AvgEnemyAmount = CurrentLevelStat.avgEnemyAmount;

        GameObject weakEnemy = weakEnemyStats.Prefab;
        GameObject avgEnemy = avgEnemyStats.Prefab;
        GameObject strongEnemy = strongEnemyStats.Prefab;

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
        }   
        StartCoroutine(EnemySpawning(LevelEnemies));
    }

    private void SpawnEnemy(GameObject EnemyType)
    {
        int RandomOffset = UnityEngine.Random.Range(-2,3);
        Vector3 SpawnPos = Player.position + Player.forward * UnityEngine.Random.Range(minDistanceFromPlayer,maxDistanceFromPlayer+1) + Vector3.right * RandomOffset;
        Instantiate(EnemyType, new Vector3(SpawnPos.x, 1, SpawnPos.z), Quaternion.identity, enemyParent);
    }

    private IEnumerator EnemySpawning(GameObject[] LevelEnemies)
    {
        NumberOfEnemiesInScene = enemyParent.childCount;

        if(NumberOfEnemiesInScene < CurrentLevelStat.MaxConcurrentEnemies)
        {
            int Random = UnityEngine.Random.Range(0,LevelEnemies.Length);

            //If there is an enemy at index RANDOM 
            if(LevelEnemies[Random] != null)
            {
                yield return new WaitForSeconds(CurrentLevelStat.enemySpawnCooldown);
                SpawnEnemy(LevelEnemies[Random]);
                LevelEnemies[Random] = null;
                StartCoroutine(EnemySpawning(LevelEnemies));

            //If there is no enemy at index RANDOM but there are items still in the array
            } else if(!IsArrayEmpty(LevelEnemies)){
                yield return null;
                StartCoroutine(EnemySpawning(LevelEnemies));
                yield break;
            } else {
                yield break;
            }
        } else {
            yield return null;
            StartCoroutine(EnemySpawning(LevelEnemies));
        }
    }

    private bool IsArrayEmpty(GameObject[] LevelEnemies) {

        for (int i = 0; i < LevelEnemies.Length; i++){
            if (LevelEnemies[i] != null) {
                return false;
            }
        }

        if (LevelEnemies == null || LevelEnemies.Length == 0)
        {
            return true;
        } else {

            //Fallback
            return false;
        }
    }
}
