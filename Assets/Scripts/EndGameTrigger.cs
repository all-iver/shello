using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered by " + other);

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER " + other + " won the game");
        }
    }
}
