using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiMovement : MonoBehaviour
{
    //public Vector3 currentTarget;
    //private float currentRotation;

    [Header("AI Movement")]
    [Range(0, 360)]
    public float initialDegreeSpread;
    public float secondsBetweenMoves;
    public float rotationSpeed;
    public float timeReadjusting;
    public float timeBetweenReorients;

    private Turtle turtleMotionController;
	public List<Transform> targetTransformsArray = new List<Transform>();
    private Transform finishLineTarget;
    public bool isResettingTarget;
    private float distanceToAim;
    private float rotationalStep;

    // Start is called before the first frame update
    void Start()
    {
        turtleMotionController = GetComponent<Turtle>();
        //finishLineTarget = FindObjectOfType<EndGameTrigger>().gameObject.transform;
        RotateTurtle();
		//DrawTargetLine(transform.up, Color.red);
		SetTarget();
        StartCoroutine("Move");
        StartCoroutine("PeriodicallyReorientSelf");
    }

	public void FixedUpdate()
    {
        if (isResettingTarget)
        {
			//Debug.Log("ReSetting Target via UPDATE");
			//Set Direction
			Vector3 targetDir = finishLineTarget.position - transform.position;
            rotationalStep = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.up, targetDir, rotationalStep, 0.0f);
			//Apply Direction
            transform.up = newDir;
        }
    }

	void SetTarget()
	{
		//Build Target List
		foreach (AiTarget target in FindObjectsOfType<AiTarget>())
		{
			targetTransformsArray.Add(target.transform);
		}
		//Set Target
		int randomTarget = Random.Range(0, targetTransformsArray.Count);
		finishLineTarget = targetTransformsArray[randomTarget];
	}

	void RotateTurtle()
    {
        float zRot = Random.Range(-initialDegreeSpread, initialDegreeSpread);
        transform.Rotate(0.0f, 0.0f, zRot);
    }

    private IEnumerator Move()
    {
        while (true)
        {
           //Debug.Log("Moving");
            turtleMotionController.swipedRightSide = true;
            yield return new WaitForSeconds(secondsBetweenMoves);
            turtleMotionController.swipedLeftSide = true;
        }
    }

    private IEnumerator PeriodicallyReorientSelf()
    {
        while (true)
        {
            isResettingTarget = false;
            yield return new WaitForSeconds(timeBetweenReorients);
            //Debug.Log("ReSetting Target VIA enumerator");
            isResettingTarget = true;
            yield return new WaitForSeconds(timeReadjusting);
        }
    }

    void DrawTargetLine(Vector3 target, Color color)
    {
        distanceToAim = 10f;
        //Debug.Log("Drawing Target Line");
        Vector3 turtlePosition = transform.position;
        Vector3 RayEndpoint = target * distanceToAim;
        //Debug.DrawRay(turtlePosition, RayEndpoint, color, 20f);
    }
}
