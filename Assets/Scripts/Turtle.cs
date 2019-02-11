using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour {

    private Animator turtleAnimController;

    public bool lastSwipe;
    public bool swipedRightSide, swipedLeftSide, swipedStraight;
    float swipePower;
	Rigidbody2D rb;
    public float lurchForce = 1;
    public Transform leftFlipper, rightFlipper;
    [System.NonSerialized]
    public bool useKeyboardInput;
    public TMPro.TMP_Text playerIDText;
    public int playerID {
        set {
            playerIDText.text = "" + value;
        }
    }
    public GameObject bow;

	void Start (){
		rb = GetComponent<Rigidbody2D>();
        turtleAnimController = GetComponent<Animator>();

    }

    public void Reset() {
        playerIDText.text = "";
    }

	public void ButtonInput(Newtonsoft.Json.Linq.JToken data) {
        string action = data["action"].ToString();
		switch (action) {
        case "swipeStraightEnd":
            swipedStraight = true;
            swipePower = float.Parse(data["power"].ToString());
            break;
		case "swipeRightEnd":
			swipedRightSide = true;
			break;
		case "swipeLeftEnd":
			swipedLeftSide = true;
			break;
		}
	}

    public void SetHasBow(bool hasBow) {
        bow.SetActive(hasBow);
        playerIDText.gameObject.SetActive(!hasBow);
    }

    void Update() {
        if (useKeyboardInput) {
            playerIDText.text = "K";
            if (Input.GetKeyDown("right"))
                swipedRightSide = true;
            if (Input.GetKeyDown("left"))
                swipedLeftSide = true;
            if (Input.GetKeyDown("up"))
                swipedStraight = true;
        }
    }

	void FixedUpdate() {
		if (swipedLeftSide) {
            //turtleAnimController.ResetTrigger("RightFlipper");
            turtleAnimController.SetTrigger("LeftFlipper");
			rb.AddForceAtPosition(transform.up * lurchForce * 0.5f, rightFlipper.position, ForceMode2D.Impulse);
		}
		if (swipedRightSide) {
            //turtleAnimController.ResetTrigger("LeftFlipper");
            turtleAnimController.SetTrigger("RightFlipper");
			rb.AddForceAtPosition(transform.up * lurchForce * 0.5f, leftFlipper.position, ForceMode2D.Impulse);
		}
		if (swipedStraight) {
            turtleAnimController.SetTrigger("LeftFlipper");
            turtleAnimController.SetTrigger("RightFlipper");
			rb.AddForceAtPosition(transform.up * lurchForce * swipePower, transform.position, ForceMode2D.Impulse);
        }
        swipedLeftSide = swipedRightSide = swipedStraight = false;
	}
}
