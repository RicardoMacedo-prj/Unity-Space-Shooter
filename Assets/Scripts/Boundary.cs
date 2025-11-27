using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DestroyByBoundary : MonoBehaviour
{
    private GAMECONTROLLER gameController;

    void Start()
    {
        gameController = FindFirstObjectByType<GAMECONTROLLER>();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Asteroid") || other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            gameController.ReduceActiveHazards(); 
        }

        if (other.CompareTag("EnemyShot") || other.CompareTag("Shot"))
        {
            Destroy(other.gameObject);
        }
    }


}
