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
using UnityEngine.InputSystem;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Level variables")]
    [SerializeField] LevelPreset CurrentLevelStat;
    [SerializeField] EnemyStatPreset weakEnemyStats;
    [SerializeField] EnemyStatPreset avgEnemyStats; // Get these three from the rescource folder
    [SerializeField] EnemyStatPreset strongEnemyStats;

    [Header("Player")]
    [SerializeField] Transform Player;

    [SerializeField] PowerUpManager powerUpManager;

    private bool PowerUpsInstantiated;

    [SerializeField] float minDistanceFromPlayer;
    [SerializeField] float maxDistanceFromPlayer;
    [SerializeField] FindObjectsSortMode sortmode;

    [Header("Boss")]
    [SerializeField] GameObject BossObject;
    private bool CrystalDestroyed;

    Enemy[] EnemyList;


    [Header("Debug")]
    public Enemy[] DebugEnemyList = new Enemy[100];
    public int DebugEnemyAmount;
    public int NumberOfEnemiesInScene;


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

        if(CurrentLevelStat.CustomEnemySpawnType == 0)
        {
            StartCoroutine(DelayBeforeFirstSpawn(LevelEnemies));
        } else if(CurrentLevelStat.CustomEnemySpawnType == 1){
            //Custom boss
            StartCoroutine(Boss(LevelEnemies));
        } else if(CurrentLevelStat.CustomEnemySpawnType == 2)
        {
            //Custom tutorial
            StartCoroutine(TutorialInputs());
            StartCoroutine(TutorialProgression());
        }
        
    }

    void Update()
    {
        EnemyList = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        NumberOfEnemiesInScene = EnemyList.Length;

        if(BossObject == null)
        {
            CrystalDestroyed = true;
        } else {
            CrystalDestroyed = false;
        }
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
            GameObject newObject = Instantiate(EnemyType, SpawnPos, Quaternion.identity);
            if(UnityEngine.Random.Range(0,2) == 1)
            {
                newObject.transform.localScale = new Vector3(-1,1,1);
            }
        } else {
            StartCoroutine(FailedSpawnAttemptCooldown(EnemyType));
        }
    }

    private IEnumerator FailedSpawnAttemptCooldown(GameObject EnemyType)
    {
        yield return new WaitForSeconds(1f);
        SpawnEnemy(EnemyType);
    }

    private IEnumerator DelayBeforeFirstSpawn(GameObject[] LevelEnemies)
    {
        yield return new WaitForSeconds(CurrentLevelStat.delayBeforeStart);
        StartCoroutine(EnemySpawning(LevelEnemies));
    }

    private IEnumerator EnemySpawning(GameObject[] LevelEnemies)
    {
        if(NumberOfEnemiesInScene < CurrentLevelStat.MaxConcurrentEnemies)
        {
            int Random = UnityEngine.Random.Range(0,LevelEnemies.Length);

            //If there is an enemy at index RANDOM 
            if(LevelEnemies[Random] != null)
            {
                SpawnEnemy(LevelEnemies[Random]);
                yield return new WaitForSeconds(CurrentLevelStat.enemySpawnCooldown);
                LevelEnemies[Random] = null;
                StartCoroutine(EnemySpawning(LevelEnemies));

            //If there is no enemy at index RANDOM but there are items still in the array
            } else if(!IsArrayEmpty(LevelEnemies)){
                yield return null;
                StartCoroutine(EnemySpawning(LevelEnemies));
                yield break;
            } else {
                CheckForLevelClear(NumberOfEnemiesInScene, LevelEnemies);
                yield return null;
                StartCoroutine(EnemySpawning(LevelEnemies));
            }
        } else {
            yield return null;
            StartCoroutine(EnemySpawning(LevelEnemies));
        }

        CheckForLevelClear(NumberOfEnemiesInScene, LevelEnemies);
    }

    void CheckForLevelClear(int NumberOfEnemiesInScene, GameObject[] LevelEnemies)
    {
        if(NumberOfEnemiesInScene == 0 && IsArrayEmpty(LevelEnemies) && !PowerUpsInstantiated)
        {
            NumberOfEnemiesInScene = EnemyList.Length;
            if(NumberOfEnemiesInScene == 0)
            {
                PowerUpsInstantiated = true;
                if(SceneManager.GetActiveScene().buildIndex != 9)
                {
                    Invoke(nameof(InstantiatePowerups),0.5f);
                } else {
                    //Ability to destroy crystal, after which ending
                    StartCoroutine(BossCrystalDestroy());
                }
            }
        }         
    }

    private bool IsArrayEmpty(GameObject[] LevelEnemies) {

        for (int i = 0; i < LevelEnemies.Length; i++){
            if (LevelEnemies[i] != null) {
                return false;
            } 
        }
        return true;
    }

    private IEnumerator Boss(GameObject[] LevelEnemies)
    {
        yield return new WaitForSeconds(2);

        //Play boss sfx

        yield return new WaitForSeconds(1);

        for(int i = 0; i < 5; i++)
        {
            SpawnEnemy(weakEnemyStats.Prefab);
        }

        yield return null;

        yield return new WaitUntil(()=> NumberOfEnemiesInScene == 0);

        yield return new WaitForSeconds(3);

        //Play boss sfx 2 
        //Change boss colour

        yield return new WaitForSeconds(1);

        for(int i = 0; i < 5; i++)
        {
            SpawnEnemy(avgEnemyStats.Prefab);
        }

        yield return null;

        yield return new WaitUntil(()=> NumberOfEnemiesInScene == 0);

        yield return new WaitForSeconds(3);

        //Play boss sfx 3
        //Change boss colour

        yield return new WaitForSeconds(2);

        for(int i = 0; i < 5; i++)
        {
            SpawnEnemy(strongEnemyStats.Prefab);
        }

        yield return null;

        yield return new WaitUntil(()=> NumberOfEnemiesInScene == 0);

        yield return new WaitForSeconds(3);

        //Play boss sfx 3
        //Change boss colour

        //Final phase starts

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(EnemySpawning(LevelEnemies));
    }

    private IEnumerator BossCrystalDestroy()
    {
        BossObject.layer = LayerMask.NameToLayer("Cuttable");
        yield return new WaitUntil(()=> CrystalDestroyed);
        LoadLevelDelay(5);
    }

    [Header("Tutorial")]

    [SerializeField] Enemy dummy;
    public bool hasDashed;
    public bool HasMoved;
    public int DummyHitAmount;
    public bool hasPunchedDummy;

    [SerializeField] InputActionReference leftHandStick;
    [SerializeField] InputActionReference buttonA;

    [SerializeField] TextMeshProUGUI tutorialText;

    private IEnumerator TutorialInputs()
    {
        if(buttonA.action.IsPressed())
        {
            hasDashed = true;
        }
        if(leftHandStick.action.ReadValue<Vector2>() != Vector2.zero)
        {
            HasMoved = true;
        }

        //Adds one to dummy hit amount every time they get hit in the dummy script

        //Changes haspuncheddummy to true when dummy is hit by fist

        yield return null;
        StartCoroutine(TutorialInputs());
    }

    private IEnumerator TutorialProgression()
    {
        tutorialText.text = "Welcome to a generic tutorial.";
        
        yield return new WaitForSeconds(5);

        tutorialText.text = "Move with joystick. Jump and dash with the primary button (A) of your controller.";

        yield return new WaitUntil(()=> HasMoved && hasDashed);
        yield return new WaitForSeconds(1);
        //Show guide to attack dummy
        tutorialText.text = "Try hitting the dummy with your sword :)";

        yield return new WaitUntil(()=> DummyHitAmount >= 1);
        yield return new WaitForSeconds(0.1f);
        tutorialText.text = "Faster swings deal more damage.";

        yield return new WaitForSeconds(2f);
        tutorialText.text = "Try hitting the dummy again, but faster.";

        yield return new WaitUntil(()=> DummyHitAmount >= 2);
        yield return new WaitForSeconds(0.1f);
        tutorialText.text = "One more time.";

        yield return new WaitUntil(()=> DummyHitAmount >= 3);
        yield return new WaitForSeconds(0.1f);
        tutorialText.text = "Great. Now try punching the dummy with your left fist.";

        yield return new WaitUntil(()=> hasPunchedDummy);
        yield return new WaitForSeconds(0.1f);
        
        tutorialText.text = "Punching enemies with a charged fist makes them fly and staggers them. Staggered enemies can be sliced in half easily.";

        yield return new WaitForSeconds(10);
        dummy.HP = 0;
        tutorialText.text = "Try slicing the dummy with your sword.";

        yield return new WaitUntil(()=> NumberOfEnemiesInScene == 0);
        yield return new WaitForSeconds(3);

        tutorialText.text = "Blocking attacks from enemies charges your fist, unless charged the fist does not do too much.";
        yield return new WaitForSeconds(6);
        tutorialText.text = "Try blocking this enemy and killing them.";

        yield return new WaitForSeconds(0.5f);
        SpawnEnemy(weakEnemyStats.Prefab);

        yield return new WaitForSeconds(1);
        yield return new WaitUntil(()=> NumberOfEnemiesInScene == 0);
        
        tutorialText.text = "Great. After defeating all the threats the level has to offer, you will be given a powerup of two choices.";

        yield return new WaitForSeconds(3);
        InstantiatePowerups();
        yield return new WaitForSeconds(6);
        

        tutorialText.text = "After taking the powerup, you will move to the next level. Slice the crystal you wish to choose when you want to proceed. ";
    }

    void InstantiatePowerups()
    {
        powerUpManager.PresentOptions();
    }

    public void LoadNextLevel(float delay)
    {
        StartCoroutine(LoadLevelDelay(delay));
    }

    private IEnumerator LoadLevelDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
