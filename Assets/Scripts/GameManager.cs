using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RamailoGames;
using UnityEngine.UI;

[System.Serializable]
public struct SpawnPos
{
    public Transform spawnPosition;

    [Range(-90,90)]
    public int forceAngleLeft;
    [Range(-90, 90)]
    public int forceAngleRight;
    [Range(0,1000)]
    public float force;
}
[System.Serializable]
public struct Levels
{
    public int Level;
    public float timescale;
    public int score;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Tooltip("Position To spwan fruits from")]
    public SpawnPos[] spawnPos;

    [Space(10)]
    public GameObject[] fruitsPrefabs;
    public GameObject bombPrefab;
    public GameObject[] PowerupPrefab;
    public GameObject powerUpScreen;

    public TMP_Text gameTimerText;
    public TMP_Text pauseMenugameTimerText;

    [Space(10)]
    public GameObject[] splashEffects;

    [Header("UI Texts")]
    public TMP_Text pausefruitsCutText;
    public TMP_Text pausehighscoreText;
    public TMP_Text pauseScoreText;
    public TMP_Text GameOverScoreText;
    public TMP_Text GameOverhighscoreText;
    public TMP_Text GameOverFruitsCutText;
    public TMP_Text scoreText;
    public TMP_Text gamePlayhighscoreText;

    public List<GameObject> lifesPrefab;

    public List<Levels> levels;

    [SerializeField]private float spawnDuration=0.5f;

    [Space(10)]
    [Range(0,100)]
    [SerializeField]private int bombSpawnChance=10;
    [Range(0, 100)]
    [SerializeField]private int powerUpSpawnChance=10;

    public Image levelUIImage;

    [HideInInspector] public int lifes;
    [HideInInspector] public int score;
    [HideInInspector] public int combo=1;
    public float timeScale;
    [HideInInspector] public float fruitscut;
    public Levels activeLevel;

    private int maxCombo;
    private int startTime;

    private float gameTimer;
    [SerializeField] private bool paused;
    [SerializeField] private bool slowmo;
    private bool canSpawn;
    public float highscore;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SpawnFruits");
        timeScale = 0.75f;
        Time.timeScale = timeScale;
        activeLevel = levels[0];
        startTime =(int) Time.unscaledTime;
        gameTimer = 300;
        paused = false;
        slowmo = false;
        powerUpScreen.gameObject.SetActive(false);
        canSpawn = true;
        ScoreAPI.GameStart((bool s) => {
        });
        ScoreAPI.GetData((bool s, Data_RequestData d) => {
            if (s)
            {
                highscore = d.high_score;
            }
        });
        setHighScore(gamePlayhighscoreText);
    }

    private void Update()
    {
        if (!paused)
        {
            gameTimer -= Time.unscaledDeltaTime;
            int min = (int)gameTimer / 60;
            int sec = (int)gameTimer % 60;
            gameTimerText.text = min.ToString() + ":" + sec.ToString();

            if (gameTimer <= 0)
            {
                GameOver();
            }
        }
    }

    public void PauseGame()
    {
        UIManager.instance.DisableCombo();
        setHighScore(pausehighscoreText);
        int min = (int)gameTimer / 60;
        int sec = (int)gameTimer % 60;
        pauseMenugameTimerText.text = min.ToString() + ":" + sec.ToString();
        pauseScoreText.text = score.ToString();
        pausefruitsCutText.text =":"+fruitscut.ToString();
        Time.timeScale = 0;
        paused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = timeScale;
        paused = false;
    }

    public void SlowGame(float value,float duration)
    {
        Time.timeScale = value;
        slowmo = true;
        powerUpScreen.gameObject.SetActive(true);
        StartCoroutine("ResumeSlow", duration);
    }

    IEnumerator ResumeSlow(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = timeScale;
        slowmo = false;
        powerUpScreen.gameObject.SetActive(false);
    }
    public void PauseWithCollision(float duration)
    {
        powerUpScreen.gameObject.SetActive(true);
        canSpawn = false;
        fruitController[] controllers = FindObjectsOfType<fruitController>();
        foreach (fruitController fruit in controllers)
        {
            fruit.GetComponent<Rigidbody2D>().bodyType =RigidbodyType2D.Static;
            fruit.canRotate = false;
        }
        Invoke(nameof(ResumeWithCollsion), duration);
    }

    public void ResumeWithCollsion()
    {
        powerUpScreen.gameObject.SetActive(false);
        canSpawn = true;
        fruitController[] controllers = FindObjectsOfType<fruitController>();
        foreach (fruitController fruit in controllers)
        {
            if (fruit.fruitType != FruitType.comboMelon)
            {
                fruit.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
    IEnumerator SpawnFruits()
    {
        while (true)
        {
            if (canSpawn && !paused)
            {
                int spawnIndex = Random.Range(0, spawnPos.Length);
                float a = Random.Range(spawnPos[spawnIndex].forceAngleLeft * Mathf.Deg2Rad, spawnPos[spawnIndex].forceAngleRight * Mathf.Deg2Rad);
                Vector3 dir = (transform.up * Mathf.Cos(a) + transform.right * Mathf.Sin(a)).normalized;
                GameObject fruit;
                if (Random.Range(0, 100) <= powerUpSpawnChance)
                {
                    fruitController[] scenefruits = FindObjectsOfType<fruitController>();
                    foreach (fruitController scenefruit in scenefruits)
                    {
                        if(scenefruit.fruitType!=FruitType.lighteningPowerUp && scenefruit.fruitType != FruitType.comboMelon)
                        {
                            int powerUpIndex = Random.Range(0, PowerupPrefab.Length);
                            fruit = Instantiate(PowerupPrefab[powerUpIndex], spawnPos[spawnIndex].spawnPosition.position, Quaternion.Euler(new(0, 0, -71.4f)));
                            fruit.GetComponent<fruitController>().AddForce(dir, spawnPos[spawnIndex].force);
                            break;
                        }
                        else
                        {
                            int fruitIndex = Random.Range(0, fruitsPrefabs.Length);
                            fruit = Instantiate(fruitsPrefabs[fruitIndex], spawnPos[spawnIndex].spawnPosition.position, Quaternion.identity);
                            fruit.GetComponent<fruitController>().AddForce(dir, spawnPos[spawnIndex].force);
                            break;
                        }

                    }

                }
                else if (Random.Range(0, 100) <= bombSpawnChance)
                {
                    fruit = Instantiate(bombPrefab, spawnPos[spawnIndex].spawnPosition.position, Quaternion.identity);
                    fruit.GetComponent<fruitController>().AddForce(dir, spawnPos[spawnIndex].force);

                }
                else
                {
                    int fruitIndex = Random.Range(0, fruitsPrefabs.Length);
                    fruit = Instantiate(fruitsPrefabs[fruitIndex], spawnPos[spawnIndex].spawnPosition.position, Quaternion.identity);
                    fruit.GetComponent<fruitController>().AddForce(dir, spawnPos[spawnIndex].force);

                }
            }
            if(slowmo)
                yield return new WaitForSeconds(spawnDuration);
            else
                yield return new WaitForSecondsRealtime(spawnDuration);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (SpawnPos pos in spawnPos)
        {
            float a = pos.forceAngleLeft * Mathf.Deg2Rad;
            Vector3 dir = (transform.up * Mathf.Cos(a) + transform.right *Mathf.Sin(a)).normalized;
            Gizmos.DrawRay(pos.spawnPosition.position, dir*pos.force);
            float b = pos.forceAngleRight * Mathf.Deg2Rad;
            Vector3 dirb = (transform.up * Mathf.Cos(b) + transform.right * Mathf.Sin(b)).normalized;
            Gizmos.DrawRay(pos.spawnPosition.position, dirb*pos.force);
        }
    }

    public void PlaySplash(Vector2 position,Color splashColor,float scale)
    {
        int splashEffectIndex = Random.Range(0, splashEffects.Length);
        GameObject splashObject = Instantiate(splashEffects[splashEffectIndex], position, Quaternion.identity);
        splashObject.GetComponent<SpriteRenderer>().color = splashColor;
        splashObject.transform.localScale = splashObject.transform.localScale*scale;
        Destroy(splashObject, 5);
    }
    private void SwitchLevel()
    {
        if (!slowmo)
        {
            timeScale = activeLevel.timescale;
            Time.timeScale = timeScale;
            levelUIImage.rectTransform.localScale = new Vector3((float)activeLevel.Level / levels.Count, 1, 1);
        }
    }
    void setHighScore(TMP_Text highscroreTextUI)
    {
        if (score >= highscore)
        {
            highscore = score;

        }
        highscroreTextUI.text = highscore.ToString();
    }
    public void AddScore(int amount)
    {
        score += amount*combo;
        scoreText.text = score.ToString();

        setHighScore(gamePlayhighscoreText);
        for (int i = levels.Count-1; i >= 0; i--)
        {
            if(score>levels[i].score )
            {
                activeLevel = levels[i];
                SwitchLevel();
                return;
            }
        }
        
    }

    public void increaseCombo()
    {
        combo++;
        if (combo > maxCombo)
            maxCombo = combo;
    }

    //public void DecreseLife()
    //{
    //    lifes--;

    //    //GameOver State
    //    if (lifes <= 0)
    //    {
    //        GameOver();
    //    }
    //    else
    //    {
    //        Destroy(lifeSpace.GetChild(0).gameObject);
    //    }
    //}

    public void GameOver()
    {
        UIManager.instance.SwitchCanvas(UIPanelType.GameOver);
        PauseGame();
        GameOverFruitsCutText.text =":"+fruitscut.ToString();
        GameOverScoreText.text = score.ToString();
        int playTime =(int)Time.unscaledTime - startTime;
        ScoreAPI.SubmitScore(score,playTime, (bool s, string msg) => { });
        GetHighScore();
    }

    void GetHighScore()
    {
        ScoreAPI.GetData((bool s, Data_RequestData d) => {
            if (s)
            {
                if (score >= d.high_score)
                {
                    GameOverhighscoreText.text = score.ToString();

                }
                else
                {
                    GameOverhighscoreText.text =d.high_score.ToString();
                }

            }
        });

    }

}
