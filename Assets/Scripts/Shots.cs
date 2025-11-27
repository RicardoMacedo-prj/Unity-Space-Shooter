using UnityEngine;

public class SHOT : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
  


    void Start()
    {
        rb.linearVelocity = transform.forward * speed;
    }



}
