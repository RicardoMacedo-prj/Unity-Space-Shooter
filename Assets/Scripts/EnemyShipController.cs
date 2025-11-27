using UnityEngine;
using System.Collections;

public class EnemyShip : MonoBehaviour
{
    public float speed;
    public float waveFrequency;
    public float waveAmplitude;
    public float reverseTime;

    public GameObject enemyShot; 
    public Transform enemyShotSpawn;
    public float enemyShotInterval;

    public GameObject EnemyShootingSound;

    private float waveOffset;
    private bool isReversed = false;
    private GAMECONTROLLER gameController;

    void Start()
    {
        waveOffset = Random.Range(0f, 2f * Mathf.PI);

        StartCoroutine(ShootBullets());

        Invoke(nameof(ReverseDirection), reverseTime);

        gameController = FindFirstObjectByType<GAMECONTROLLER>();
    }

    void Update()
    {
        float forwardDirection = isReversed ? 1f : -1f;

        transform.position += Vector3.forward * forwardDirection * speed * Time.deltaTime;

        float xMovement = Mathf.Sin(Time.time * waveFrequency + waveOffset) * waveAmplitude;
        transform.position += new Vector3(xMovement, 0, 0) * Time.deltaTime;
    }

    private void ReverseDirection()
    {
        isReversed = true;
    }

    private IEnumerator ShootBullets()
    {
        while (true)
        {
            PlayEnemyWeaponSound();
            Instantiate(enemyShot, enemyShotSpawn.position, enemyShotSpawn.rotation);
            yield return new WaitForSeconds(enemyShotInterval);
        }
    }

    private void PlayEnemyWeaponSound()
    { 
        GameObject soundObject = Instantiate(EnemyShootingSound, transform.position, Quaternion.identity);
        Destroy(soundObject, soundObject.GetComponent<AudioSource>().clip.length);
    }
}
