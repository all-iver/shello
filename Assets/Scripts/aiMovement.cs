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
    public float rotationSpeed;
    private float z;
    public float timeBetweenResets;
    public bool isResettingTarget;

    // Start is called before the first frame update
    void Start()
    {
        z = 0.0f;
        turtleControl = GetComponent<Turtle>();
        finishLine = FindObjectOfType<EndGameTrigger>().gameObject.transform;
        RotateTurtle();
        SetTarget();
        StartCoroutine("Move");
        StartCoroutine("ResetTarget");
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

        Debug.DrawRay(turtlePosition, forwardDirection * distanceToAim, Color.red, 60f);

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
           //Debug.Log("Right");
            yield return new WaitForSeconds(speed);
            turtleControl.swipedLeftSide = true;
           //Debug.Log("Left");
        }
    }

    private IEnumerator ResetTarget()
    {

        while (true)
        {
            Debug.Log("ReSetting Target VIA enumerator");
            isResettingTarget = true;
            yield return new WaitForSeconds(timeBetweenResets);
            isResettingTarget = false;
            yield return new WaitForSeconds(timeBetweenResets);
        }
    }

    public void FixedUpdate()
    {
        if (isResettingTarget)
        {
            Debug.Log("ReSetting Target via UPDATE");

            //Vector3 turtlePosition = transform.position;
            //Vector3 lookDirection = finishLine.position - turtlePosition;
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection.normalized), Time.time * turnSpeed);
            //transform.Translate(Vector3.up * speed, Space.Self);

            //Vector3 forwardDirection = transform.up;
            //Debug.DrawRay(turtlePosition, forwardDirection * distanceToAim, Color.red, 3f);

            //then isResettingTarget=false;


            Vector3 targetDir = finishLine.position - transform.position;
            // The step size is equal to speed times frame time.
            float step = rotationSpeed * Time.deltaTime;
            Vector3 forwardDirection = transform.up;

            Vector3 newDir = Vector3.RotateTowards(forwardDirection, targetDir, step, 0.0f);
            Debug.DrawRay(transform.position, newDir, Color.green);

            Vector3 desiredRot = new Vector3(0, 0, newDir.z);

            Debug.DrawRay(transform.position, desiredRot, Color.red);
            // Move our position a step closer to the target.
            transform.rotation = Quaternion.LookRotation(desiredRot);


       

        }
    }

}
