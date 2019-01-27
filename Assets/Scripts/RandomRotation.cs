using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [Header("Axis to rotate around:")]
    public bool xAxis = false;
    public bool yAxis = false;
    public bool zAxis = false;

    void Awake()
    {
        if (xAxis == true)
        {
            transform.Rotate(Random.Range(0.0f, 360.0f), 0.0f, 0.0f);
        }

        if (yAxis == true)
        {
            transform.Rotate(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
        }

        if (zAxis == true)
        {
            transform.Rotate(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        }
    }
}
