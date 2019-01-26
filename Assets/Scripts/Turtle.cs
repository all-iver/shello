using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour {

    bool swipedRightSide, swipedLeftSide;
	Rigidbody2D rb;
    public float lurchForce = 1;
    public Transform leftFlipper, rightFlipper;
    [System.NonSerialized]
    public bool useKeyboardInput;

	void Start (){
		rb = GetComponent<Rigidbody2D>();
	}

	public void ButtonInput(string input) {
		switch (input) {
		case "swipeRightEnd":
			swipedRightSide = true;
			break;
		case "swipeLeftEnd":
			swipedLeftSide = true;
			break;
		}
	}

    void Update() {
        if (useKeyboardInput) {
            if (Input.GetKeyDown("right"))
                swipedRightSide = true;
            if (Input.GetKeyDown("left"))
                swipedLeftSide = true;
        }
    }

	void FixedUpdate() {
		if (swipedLeftSide) {
			rb.AddForceAtPosition(transform.up * lurchForce, leftFlipper.position, ForceMode2D.Impulse);
        }
		if (swipedRightSide) {
			rb.AddForceAtPosition(transform.up * lurchForce, rightFlipper.position, ForceMode2D.Impulse);
        }
        swipedLeftSide = swipedRightSide = false;
	}
}
