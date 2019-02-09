using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMovement : MonoBehaviour
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
	public bool isReorientingSelf;
	private float distanceToAim;
	private float rotationalStep;

	[Header("AI Pathfinding")]
	public GameObject currentPath;
	public int currrentPathTargetNUM = 0;
	[SerializeField] private Transform currentPathTargetTransform;
	private AiTarget[] pathTargets;
	public List<Transform> pathTargetTransforms = new List<Transform>();
	private AiPathManager aiPaths;

    // Start is called before the first frame update
    void Start()
    {
        turtleMotionController = GetComponent<Turtle>();
		aiPaths = FindObjectOfType<AiPathManager>();
		//finishLineTarget = FindObjectOfType<EndGameTrigger>().gameObject.transform;
		RotateTurtle();
		//DrawTargetLine(transform.up, Color.red);
		SetPath();
		BuildTargetList();
		NextTarget();
        StartCoroutine("Move");
        StartCoroutine("PeriodicallyReorientSelf");
    }

	public void FixedUpdate()
    {
        if (isReorientingSelf)
        {
			//Debug.Log("ReSetting Target via UPDATE");
			//Set Direction
			Vector3 targetDir = currentPathTargetTransform.position - transform.position;
            rotationalStep = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.up, targetDir, rotationalStep, 0.0f);
			//Apply Direction
            transform.up = newDir;
        }
    }


	void SetPath()
	{
		var pathCount = aiPaths.numOfPaths;
		var currentPathNum = Random.Range(0, pathCount);
		currentPath = aiPaths.PossiblePaths[currentPathNum];
	}
	
	void BuildTargetList()
	{
		pathTargets = currentPath.GetComponentsInChildren<AiTarget>();

		foreach (AiTarget target in pathTargets)
		{
			pathTargetTransforms.Add(target.transform);
		}
	}

	public void NextTarget()
	{
		currentPathTargetTransform = pathTargetTransforms[currrentPathTargetNUM];
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
            isReorientingSelf = false;
            yield return new WaitForSeconds(timeBetweenReorients);
            //Debug.Log("ReSetting Target VIA enumerator");
            isReorientingSelf = true;
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
