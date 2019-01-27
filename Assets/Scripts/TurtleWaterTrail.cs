using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleWaterTrail : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        TurtleTrailManager turtleTrails = other.gameObject.GetComponentInChildren<TurtleTrailManager>();
        if (!turtleTrails)
            return;
        turtleTrails.waterTrail.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TurtleTrailManager turtleTrails = other.gameObject.GetComponentInChildren<TurtleTrailManager>();
        if (!turtleTrails)
            return;
        turtleTrails.waterTrail.enabled = false;
    }
}
