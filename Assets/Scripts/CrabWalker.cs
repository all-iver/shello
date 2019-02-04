using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabWalker : MonoBehaviour
{
	// Movement speed in units/sec.
	public float speed = 1.0F;

	// Transforms to act as start and end markers for the journey.
	public Transform startMarker;
	public Transform endMarker;

	// Total distance between the markers.
	private float journeyLength;

	// Time when the movement started.
	private float startTime;


	public List<Transform> targetTransformsArray = new List<Transform>();

	//private Transform[] targetArray;
	private Transform currentTarget;
	public bool isResettingTarget;

	//public float yShift = 1.5f;
	//move left and right from the start point
	// public float xShift = 0f;
	//private Vector3 startV3Pos;


	void Start()
    {
		SetNewTarget();
		//startV3Pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		
		//  Vector3 v = startV3Pos;
		//  v.y += yShift * Mathf.Sin(Time.time * speed);
		//  v.x += xShift * Mathf.Sin(Time.time * speed);
		//  transform.position = v;
	}

	public void FixedUpdate()
	{
		if (!isResettingTarget)
		{
			//MOVE CRAB to new location
			
			// Distance moved = time * speed.
			float distCovered = (Time.time - startTime) * speed;
			// Fraction of journey completed = current distance divided by total distance.
			float fracJourney = distCovered / journeyLength;
			// Set our position as a fraction of the distance between the markers.
			transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);

			if (transform.position == endMarker.position)
			{
				isResettingTarget = true;
			}
		}
		
		if (isResettingTarget)
		{
			SetNewTarget();
			isResettingTarget = false;

			//Debug.Log("ReSetting Target via UPDATE");
			//Set Direction
			//Vector3 targetDir = currentTarget.position - transform.position;
			//rotationalStep = rotationSpeed * Time.deltaTime;
			//Vector3 newDir = Vector3.RotateTowards(transform.up, targetDir, rotationalStep, 0.0f);
			//Apply Direction
			//transform.up = newDir;
		}
	}

	void SetNewTarget()
	{
		//Build Target List
		//foreach (AiTarget target in FindObjectsOfType<AiTarget>())
		//{
		//	targetTransformsArray.Add(target.transform);
		//}
		//Set Target
		int randomTarget = Random.Range(0, targetTransformsArray.Count);
		currentTarget = targetTransformsArray[randomTarget];
		
		// Keep a note of the time the movement started.
		startTime = Time.time;

		startMarker = endMarker;
		endMarker = currentTarget;
		
		// Calculate the journey length.
		journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
	}
}
