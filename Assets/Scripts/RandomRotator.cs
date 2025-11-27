using UnityEngine;

public class RANDOMROTATOR : MonoBehaviour
{
    public Rigidbody rb;
    public float tumble;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.angularVelocity = Random. insideUnitSphere * tumble;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
