using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    AirController controller;

    void Start() {
        controller = FindObjectOfType<AirController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Turtle turtle = other.gameObject.GetComponent<Turtle>();
        if (!turtle)
            return;
        if (controller)
            controller.OnTurtleCrossedFinishLine(turtle);        
    }
}
