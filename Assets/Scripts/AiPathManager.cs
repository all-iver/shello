using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPathManager : MonoBehaviour
{
	public int numOfPaths;

	public GameObject[] PossiblePaths;

	private void Awake()
	{
		numOfPaths = PossiblePaths.Length;
	}
}
