using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiMovement : MonoBehaviour
{

    public Transform finishLine;
    public Vector3 currentTarget;
    private float currentRotation;
    private Turtle turtleControl;
    public float speed;

    [Range(0, 360)]
    public float initialDegreeSpread;

    public float distanceToAim;

    // Start is called before the first frame update
    void Start()
    {
        turtleControl = GetComponent<Turtle>();
        finishLine = FindObjectOfType<EndGameTrigger>().gameObject.transform;
        RotateTurtle();
        SetTarget();
        StartCoroutine("Move");
    }

    void RotateTurtle()
    {
        float zRot = Random.Range(-initialDegreeSpread, initialDegreeSpread);
        transform.Rotate(0.0f, 0.0f, zRot);
    }

    void SetTarget()
    {
        Debug.Log("Setting Target");
        Vector3 turtlePosition = transform.position;
        Vector3 forwardDirection = transform.up;
       // Ray TargettingRay = new Ray(turtlePosition, forwardDirection);
        //RaycastHit interactionRayHit;
        float targettingRayLength = distanceToAim;

        Vector3 interactionRayEndpoint = forwardDirection * targettingRayLength;
        currentTarget = interactionRayEndpoint;
    }


    private IEnumerator Move()
    {
        //Debug.Log("Moving");
        while (true)
        {
            turtleControl.swipedRightSide = true;
            Debug.Log("Right");
            yield return new WaitForSeconds(speed);
            turtleControl.swipedLeftSide = true;
            Debug.Log("Left");
        }
    }

}
