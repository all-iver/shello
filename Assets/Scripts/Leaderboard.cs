using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 8; i++) {
            LeaderboardTurtle turtle = transform.GetChild(i).GetComponent<LeaderboardTurtle>();
            turtle.transform.SetParent(transform);
            turtle.gameObject.SetActive(false);
            turtle.rank.text = "" + (i + 1);
        }
        GetComponent<RectTransform>().ForceUpdateRectTransforms();
    }

    public void SetNumPlayers(int numPlayers) {
        for (int i = numPlayers; i < 8; i++) {
            LeaderboardTurtle turtle = transform.GetChild(i).GetComponent<LeaderboardTurtle>();
            turtle.gameObject.SetActive(false);
        }
    }

    public void SetRank(int rank, Sprite body, int number, float time, bool hasBow, bool isKeyboard = false) {
        if (rank >= 8)
            return; // hardcoded to 8 child turtles for now
        LeaderboardTurtle turtle = transform.GetChild(rank).GetComponent<LeaderboardTurtle>();
        turtle.gameObject.SetActive(true);
        turtle.body.sprite = body;
        turtle.number.text = isKeyboard ? "K" : ("" + number);
        turtle.bow.gameObject.SetActive(false);
        if (time <= 0) {
            turtle.time.gameObject.SetActive(hasBow);
        } else {
            turtle.time.gameObject.SetActive(true);
            turtle.time.text = string.Format("{0:F2}s", time);
        }
    }
}
