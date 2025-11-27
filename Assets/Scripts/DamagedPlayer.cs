using UnityEngine;

public class PlayerDamaged : MonoBehaviour
{
    private GAMECONTROLLER GAMECONTROLLER;
    public GameObject playerExplosion;
    public AudioSource explosionAudio;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
    
        GAMECONTROLLER = gameControllerObject.GetComponent<GAMECONTROLLER>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid") || other.CompareTag("Enemy") || (other.CompareTag("EnemyShot")))
        {

            GAMECONTROLLER.PlayerDamaged();

            if (GAMECONTROLLER.GetPlayerLives() <= 0)
            {
                Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                PlayExplosionSound();

                GAMECONTROLLER.GameOver();
                Destroy(gameObject);
            }
            
            Destroy(other.gameObject);

        }

    }

    void PlayExplosionSound()
    {
        GameObject audioPlayer = new GameObject("ExplosionAudioPlayer");
        AudioSource audioSource = audioPlayer.AddComponent<AudioSource>();
        audioSource.clip = explosionAudio.clip;
        audioSource.Play();
        Destroy(audioPlayer, explosionAudio.clip.length);

    }
}
