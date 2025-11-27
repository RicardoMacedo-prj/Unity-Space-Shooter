using UnityEngine;

public class DESTROYBYCONTACT : MonoBehaviour
{
    public GameObject explosion;
    private GAMECONTROLLER gameController;
    public AudioSource asteroidExplosionAudio;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GAMECONTROLLER>();
        }
        else
        {
            Debug.LogError("GameController não encontrado! Certifique-se de que há um GameController na cena.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Boundary") || other.CompareTag("EnemyShot") || other.CompareTag("Asteroid") || other.CompareTag("Enemy"))
        {
            return;
        }

        
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayExplosionSound();
            Instantiate(explosion, transform.position, transform.rotation);
            gameController.ReduceActiveHazards();
        }

        else if (other.CompareTag("Shot"))
    {
        // Verifica a tag do objeto que foi atingido
        if (gameObject.CompareTag("Enemy"))
        {
            gameController.AddScoreEnemy();
        }
        else if (gameObject.CompareTag("Asteroid"))
        {
            gameController.AddScoreHazard();
        }

        Destroy(gameObject);
        Destroy(other.gameObject);
        PlayExplosionSound();
        Instantiate(explosion, transform.position, transform.rotation);
        gameController.ReduceActiveHazards();
        }
    }
    



    void PlayExplosionSound()
    {
        GameObject audioAsteroid = new GameObject("ExplosionAudioAsteroid");
        AudioSource audioSource = audioAsteroid.AddComponent<AudioSource>();
        audioSource.clip = asteroidExplosionAudio.clip;
        audioSource.Play();
        Destroy(audioAsteroid, asteroidExplosionAudio.clip.length);
    }
}
