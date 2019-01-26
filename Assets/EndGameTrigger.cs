using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered by " + other);

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER " + other + " won the game");
        }

    }
}
