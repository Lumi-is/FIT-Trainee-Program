using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEditor.Analytics;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] LevelPreset CurrentLevelStat;
    [SerializeField] EnemyStatPreset weakEnemyStats;
    [SerializeField] EnemyStatPreset avgEnemyStats; // Get these three from the rescource folder
    [SerializeField] EnemyStatPreset strongEnemyStats;
    [SerializeField] Transform Player;

    [SerializeField] float minDistanceFromPlayer;
    [SerializeField] float maxDistanceFromPlayer;
    [SerializeField] FindObjectsSortMode sortmode;

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
        Vector3 TrySpawnPos = Player.position + Player.forward * UnityEngine.Random.Range(minDistanceFromPlayer,maxDistanceFromPlayer+1) + Vector3.right * RandomOffset;
        Vector3 SpawnPos = new Vector3(0,0,0);
        NavMeshHit hit;
        bool canSpawn;
        if(NavMesh.SamplePosition(TrySpawnPos, out hit, 5.0f, NavMesh.AllAreas))
        {
            SpawnPos = hit.position;
            canSpawn = true;
        } else {
            canSpawn = false;
        }

        if(canSpawn)
        {
            Instantiate(EnemyType, new Vector3(SpawnPos.x, SpawnPos.y, SpawnPos.z), Quaternion.identity);
        } else {
            StartCoroutine(FailedSpawnAttemptCooldown(EnemyType));
        } 
    }

    private IEnumerator FailedSpawnAttemptCooldown(GameObject EnemyType)
    {
        yield return new WaitForSeconds(1f);
        SpawnEnemy(EnemyType);
    }

    private IEnumerator EnemySpawning(GameObject[] LevelEnemies)
    {
        var EnemyList = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        NumberOfEnemiesInScene = EnemyList.Length;

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
                LoadNextLevel();
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

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
