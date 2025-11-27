using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public float speed = 5f; 
    public float waveFrequency = 1f; 
    public float waveAmplitude = 3f; 
    public float xLimit = 8f; 
    public float enterSceneTime = 4f; 
    public GameObject bossShot; 
    public Transform bossShotSpawn;
    public float bossShotInterval = 2f;
    public GameObject explosion; 
    public GameObject BossShootingSound;

    private bool inScene = false;
    private Vector3 waveOffset; 
    private Vector3 movementDirection = Vector3.right; 
    public int bossHealth = 10;
    private GAMECONTROLLER gameController;
    public AudioSource ExplosionAudio;

    void Start()
    {
        // Encontra o GameController e ajusta a posição inicial
        gameController = FindFirstObjectByType<GAMECONTROLLER>();
        Vector3 spawnPosition = gameController.spawnValues;
        Vector3 bossSpawnPosition = new Vector3(
            Random.Range(-spawnPosition.x + 3f, spawnPosition.x - 3f),
            spawnPosition.y,
            spawnPosition.z
        );

        transform.position = bossSpawnPosition;

        // Calcula o offset para o movimento oscilatório
        waveOffset = new Vector3(0, 0, Random.Range(0f, 2f * Mathf.PI));

        // Corrotinas para entrar na cena e disparar
        StartCoroutine(EnterScene());
        StartCoroutine(ShootBullets());
    }

    void Update()
    {
        if (!inScene) return;

        // Movimento oscilatório no eixo Z
        float zOscillation = Mathf.Sin(Time.time * waveFrequency + waveOffset.z) * waveAmplitude;

        // Movimento de um lado para o outro no eixo X
        transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);

        // Aplica o movimento oscilatório no eixo Z
        transform.position += new Vector3(0, 0, zOscillation) * Time.deltaTime;

        // Inverte a direção ao alcançar os limites no eixo X
        if (transform.position.x > xLimit)
        {
            transform.position = new Vector3(xLimit, transform.position.y, transform.position.z);
            movementDirection = Vector3.left;
        }
        else if (transform.position.x < -xLimit)
        {
            transform.position = new Vector3(-xLimit, transform.position.y, transform.position.z);
            movementDirection = Vector3.right;
        }
    }

    private IEnumerator EnterScene()
    {
        // Movimento inicial para entrar na cena
        float startTime = Time.time;
        while (Time.time - startTime < enterSceneTime)
        {
            transform.Translate(Vector3.forward * -speed * Time.deltaTime, Space.World);
            yield return null;
        }

        // Após entrar na cena, ativa o comportamento regular
        inScene = true;
    }

    private IEnumerator ShootBullets()
    {
        // Dispara periodicamente enquanto o boss está ativo
        while (true)
        {
            if (inScene)
            {
                Instantiate(bossShot, bossShotSpawn.position, bossShotSpawn.rotation);

                Vector3 leftPosition = bossShotSpawn.position + Vector3.left * 0.9f; 
                Instantiate(bossShot, leftPosition, bossShotSpawn.rotation);

                Vector3 rightPosition = bossShotSpawn.position + Vector3.right * 0.9f; 
                Instantiate(bossShot, rightPosition, bossShotSpawn.rotation);
                PlayBossWeaponSound();

            }
            yield return new WaitForSeconds(bossShotInterval);
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Shot"))
        {
            Destroy(other.gameObject);

            bossHealth--;


            if (bossHealth <= 0)
            {

                Destroy(gameObject);
                Instantiate(explosion, transform.position, transform.rotation);
                PlayExplosionSound();
                gameController.activeHazards--;
                gameController.AddScoreBoss();
            }
        }
    }

    void PlayExplosionSound()
    {
        GameObject audioAsteroid = new GameObject("ExplosionAudioAsteroid");
        AudioSource audioSource = audioAsteroid.AddComponent<AudioSource>();
        audioSource.clip = ExplosionAudio.clip;
        audioSource.Play();
        Destroy(audioAsteroid, ExplosionAudio.clip.length);
    }

    private void PlayBossWeaponSound()
    { 
        GameObject soundObject = Instantiate(BossShootingSound, transform.position, Quaternion.identity);
        Destroy(soundObject, soundObject.GetComponent<AudioSource>().clip.length);
    }
}