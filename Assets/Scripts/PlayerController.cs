using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public float xmin, xmax, zmin, zmax;
    public float tilt;
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    private float nextFire;

    AudioSource shootSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shootSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizonta1 = Input.GetAxis("Horizontal");
        float moveVertica1= Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizonta1, 0, moveVertica1);
        rb.linearVelocity = movement*speed;
        rb.position = new Vector3(Mathf.Clamp(rb.position.x, xmin, xmax), 0, Mathf.Clamp(rb.position.z, zmin, zmax));
        rb.rotation = Quaternion.Euler( 0.0f, 0.0f, rb.linearVelocity.x*-tilt);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            shootSound.Play();
        }
    }
}
