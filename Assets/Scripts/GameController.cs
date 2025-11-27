using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GAMECONTROLLER : MonoBehaviour
{
    private Renderer playerRenderer;
    private Collider playerCollider;
    private bool gameOver;
    private bool restart;
    private bool isPaused = false;
    private int playerLives;
    private int waveCounter = 0;
    private bool isGettingReady = false;
    private int bossWaveCounter = 0;
    private List<GameObject> heartIcons = new List<GameObject>();
    private Coroutine pauseCoroutine;
    private float enemyShootingInterval;
    private BossController bossController;
    private  float bossShootingInterval;
    private int  bossHP;

    [Header("Spawn Settings")]
    public GameObject[] hazards;
    public Vector3 spawnValues;
    public GameObject player;
    public ParticleSystem playerCoreParticles;
    public ParticleSystem playerFlareParticles;
    public GameObject enemy; 
    public int hazardCount;
    public int enemyCount ; 
    public float spawnDelay = 1f; 
    public float waveDelay = 3f; 
    public float startDelay = 2f; 
    public int activeHazards = 0;

    [Header("Boss Settings")]
    public GameObject boss;
    public float bossWaveDelay = 2f;

    [Header("Game Settings")]
    public AudioSource bgSound;
    public int maxLives = 3;
    public float invulnerabilityTime = 2f;


    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI restartText;
    public TextMeshProUGUI pauseText;
    public GameObject heartIcon;
    public Transform heartsParent;
    public Button playButton;
    public TextMeshProUGUI waveText;

    private int score;
    
    void Start()
    {
        gameOver = false;
        restart = false;

        score = 0;
        playerLives = maxLives;

        scoreText.text = "Score: 0";
        gameOverText.text = "";
        restartText.text = "";

        scoreText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        restartText.gameObject.SetActive(false);
        waveText.gameObject.SetActive(false);
        heartsParent.gameObject.SetActive(false);

        GenerateHeartsUI();
        heartIcons.ForEach(heart => heart.SetActive(false));

        Time.timeScale = 0;
        playButton.onClick.AddListener(StartGame);

        playerRenderer = player.GetComponent<Renderer>();
        playerCollider = player.GetComponent<Collider>();

        player.SetActive(false);
    }

    public void StartGame()
    {
        playButton.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        heartsParent.gameObject.SetActive(true);
        heartIcons.ForEach(heart => heart.SetActive(true));

        player.SetActive(true);

        Time.timeScale = 1;
        bgSound.Play();
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startDelay);

        while (!gameOver)
        {
            waveCounter++;

            if ((waveCounter) % 5 == 0)
            {
                bossWaveCounter++;
                Vector3 bossSpawnPosition = new Vector3(
                Random.Range(-spawnValues.x + 3f, spawnValues.x - 3f), spawnValues.y, 18f);

                 yield return SpawnBoss(bossSpawnPosition);
                 yield return new WaitUntil(() => activeHazards == 0);
            }

            else
            { 
                waveText.fontSize = 75;
                waveText.text = "Wave " + (waveCounter);
                waveText.gameObject.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                waveText.gameObject.SetActive(false);

                for (int i = 0; i < hazardCount; i++)
                {   
                    yield return new WaitForSeconds(spawnDelay);
                    Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                    Quaternion spawnRotation = Quaternion.identity;

                    int randomIndex = Random.Range(0, hazards.Length);
                    GameObject selectedHazard = hazards[randomIndex];

                    Instantiate(selectedHazard, spawnPosition, spawnRotation);
                    IncreaseActiveHazards();

                    yield return new WaitForSeconds(spawnDelay);
                }

                for (int i = 0; i < enemyCount; i++)
                {
                    Vector3 enemySpawnPosition = new Vector3(Random.Range(-spawnValues.x + 1.4f, spawnValues.x - 1.4f), spawnValues.y, spawnValues.z);
                    Instantiate(enemy, enemySpawnPosition, Quaternion.Euler(0, 180, 0));

                    IncreaseActiveHazards();
                    yield return new WaitForSeconds(spawnDelay);
                }
            }

            yield return new WaitUntil(() => activeHazards == 0);
            IncreaseDifficulty();
            yield return new WaitForSeconds(waveDelay - 2f);
        }
    }

    public void GameOver()
    {

        gameOverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        gameOver = true;
        bgSound.Stop();
        gameOverText.text = "Game Over";
        StartCoroutine(ShowRestartTextAfterDelay(2.5f));
    }

    private IEnumerator ShowRestartTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        restart = true;

        StartCoroutine(BlinkRestartText());
    }

    private IEnumerator BlinkRestartText()
    {
        while (restart)
        {
            restartText.text = "Press <R> to Restart";
            yield return new WaitForSeconds(0.5f);
            restartText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void PlayerDamaged()
    {
        playerLives--;
        UpdateLivesUI();

        if (playerLives > 0)
        {
            StartCoroutine(HandleInvulnerability());
        }
        else
        {
            GameOver();
        }
    }

    void GenerateHeartsUI()
    {
        foreach (GameObject heart in heartIcons)
        {
            Destroy(heart);
        }

        heartIcons.Clear();

        for (int i = 0; i < maxLives; i++)
        {
            GameObject newHeart = Instantiate(heartIcon, heartsParent);
            heartIcons.Add(newHeart);
        }
    }

    IEnumerator HandleInvulnerability()
    {
        playerCollider.enabled = false;
        bool isVisible = true;
        float elapsedTime = 0f;

        while (elapsedTime < invulnerabilityTime)
        {
            isVisible = !isVisible;
            playerRenderer.enabled = isVisible;

            if (playerCoreParticles != null)
            {
                var coreEmission = playerCoreParticles.emission;
                coreEmission.enabled = isVisible;
            }

            if (playerFlareParticles != null)
            {
                var flareEmission = playerFlareParticles.emission;
                flareEmission.enabled = isVisible;
            }

            elapsedTime += 0.2f;
            yield return new WaitForSeconds(0.2f);
        }

        playerRenderer.enabled = true;
        playerCollider.enabled = true;

        if (playerCoreParticles != null)
        {
            var coreEmission = playerCoreParticles.emission;
            coreEmission.enabled = true;
        }

        if (playerFlareParticles != null)
        {
            var flareEmission = playerFlareParticles.emission;
            flareEmission.enabled = true;
        }
    }

    public void AddScoreHazard()
    {
        score += 3;
        scoreText.text = "Score: " + score.ToString();
    }
    public void AddScoreEnemy()
    {
        score += 10;
        scoreText.text = "Score: " + score.ToString();
    }

    public void AddScoreBoss()
    {
        score += 100;
        scoreText.text = "Score: " + score.ToString();
    }

    void UpdateLivesUI()
    {
        for (int i = 0; i < heartIcons.Count; i++)
        {
            heartIcons[i].SetActive(i < playerLives);
        }
    }

    void Update()
    {
        if (restart && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

   private void TogglePause()
    {
        if (isGettingReady || playButton.gameObject.activeSelf)
        {
            return;
        }

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            bgSound.Pause();
            StartCoroutine(BlinkPause());
            if (pauseCoroutine != null)
            {
                StopCoroutine(pauseCoroutine);
            }
            pauseCoroutine = StartCoroutine(BlinkPause());
        }
        else
        {
            isGettingReady = true;
            StartCoroutine(ResumeGame());
        }
    }

    private IEnumerator BlinkPause()
    {

        pauseText.fontSize = 85;
        while (isPaused)
        {
            pauseText.text = "Paused";
            yield return new WaitForSecondsRealtime(0.2f);
            pauseText.text = "";
            yield return new WaitForSecondsRealtime(0.2f);
        }

        if (!isGettingReady)
        {
            pauseText.text = "";
        }
    }

    private IEnumerator ResumeGame()
    {
        pauseText.fontSize = 55;
        pauseText.text = "Get Ready...";
        yield return new WaitForSecondsRealtime(2f);


        pauseText.text = "";
        Time.timeScale = 1;
        bgSound.UnPause();
        isGettingReady = false;
    }

    public int GetPlayerLives()
    {
        return playerLives;
    }

    public void ReduceActiveHazards()
    {
        activeHazards--;

    }

    public void IncreaseActiveHazards()
    {
        activeHazards++;
    }

    void IncreaseDifficulty()
    {
        hazardCount += Mathf.RoundToInt(1f * waveCounter);
        enemyCount += Mathf.RoundToInt(0.6f * waveCounter);

        spawnDelay = Mathf.Max(0.4f, spawnDelay - (0.1f * waveCounter));

        EnemyShip enemyShip = enemy.GetComponent<EnemyShip>();
        
        enemyShootingInterval = enemyShip.enemyShotInterval;
        enemyShootingInterval = Mathf.Max(0.8f, enemyShootingInterval - (0.2f * waveCounter));

        BossController bossController = boss.GetComponent<BossController>();
        bossShootingInterval = bossController.bossShotInterval;
        bossHP = bossController.bossHealth;
        bossHP += Mathf.RoundToInt(0.6f * bossWaveCounter);
        bossShootingInterval = Mathf.Max(0.8f, enemyShootingInterval - (0.2f * bossWaveCounter));
        
    }


    private IEnumerator SpawnBoss(Vector3 bossSpawnPosition)
    {

        waveText.text = "Boss Wave! Prepare!";
        waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        waveText.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        BossController bossController = boss.GetComponent<BossController>();
        Quaternion rotation = Quaternion.Euler(180, 0, 0);
        Instantiate(boss, bossSpawnPosition, rotation);
        

        IncreaseActiveHazards();

        yield return null;

    }

    
}
