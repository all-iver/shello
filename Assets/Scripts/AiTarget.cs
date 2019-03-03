using UnityEngine;

public class AiTarget : MonoBehaviour
{
	// Blank Script, serves as a beacon for FindGameObjects in the AI Turtle Movement script
	[Header("Target Order")]
	public int targetNum;
	public bool finalTarget;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.GetComponent<AiMovement>() != null)
		{
			var aiTurtle = collision.GetComponent<AiMovement>();

			if (!finalTarget)
			{
				aiTurtle.currrentPathTargetNUM = targetNum + 1;
				aiTurtle.NextTarget();
                aiTurtle.isReorientingSelf = true;
			}
		}
	}
}
