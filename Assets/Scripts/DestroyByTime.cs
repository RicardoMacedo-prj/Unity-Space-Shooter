using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DestroyByTime : MonoBehaviour
{
    
    public float lifetime;
    void Start()
    {
        Destroy (gameObject, lifetime);
    }

}
    