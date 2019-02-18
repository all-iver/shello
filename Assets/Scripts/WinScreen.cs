using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WinScreen : MonoBehaviour
{
    public bool testStart;
    public RectTransform sizer, winnersList;
    public float secondsToShow = 10;
    public GameObject backdrop;
    public LeaderboardTurtle turtlePrefab;
    public Transform turtlesParent;
    bool showing;
    public int maxTurtlesToShow = 5;
    public TMPro.TMP_Text nextRaceTimer;
    float waitToCloseTimer;

    // Start is called before the first frame update
    void Start()
    {
        sizer.anchoredPosition = new Vector2(0, -500);
        backdrop.SetActive(false);
        sizer.gameObject.SetActive(false);
    }

    void Update() {
        if (testStart) {
            StartCoroutine(Show());
            testStart = false;
        }

        if (showing) {
            waitToCloseTimer -= Time.deltaTime;
            nextRaceTimer.text = string.Format("Next race in {0}...", Mathf.CeilToInt(waitToCloseTimer));
        }
    }

    public IEnumerator Show() {
        if (showing)
            throw new System.Exception("Win screen is already showing");

        // show
        waitToCloseTimer = secondsToShow;
        showing = true;
        backdrop.SetActive(true);
        sizer.gameObject.SetActive(true);
        sizer.anchoredPosition = new Vector2(0, -500);
        Tween tween = sizer.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutCubic);
        yield return tween.WaitForCompletion();

        // put the bow on the winner
        yield return new WaitForSeconds(1);
        LeaderboardTurtle turtle = winnersList.GetChild(0).GetComponent<LeaderboardTurtle>();
        RectTransform bow = turtle.bow.GetComponent<RectTransform>();
        bow.gameObject.SetActive(true);
        Vector3[] corners = new Vector3[4];
        sizer.GetWorldCorners(corners);
        Transform oldParent = bow.parent;
        bow.SetParent(sizer);
        Vector2 center = (corners[0] + corners[1] + corners[2] + corners[3]) / 4;
        Sequence s = DOTween.Sequence();
        s.Append(bow.DOMove(center, 1).From());
        s.Join(bow.DOScale(new Vector2(10, 10), 1).From());
        s.SetDelay(1);
        s.SetEase(Ease.OutCubic);
        yield return s.WaitForCompletion();
        bow.SetParent(oldParent);

        // wait
        while (waitToCloseTimer > 0)
            yield return new WaitForEndOfFrame();

        // hide
        tween = sizer.DOAnchorPosY(-500, 0.5f).SetEase(Ease.OutCubic);
        yield return tween.WaitForCompletion();
        backdrop.SetActive(false);
        sizer.gameObject.SetActive(false);
        showing = false;
    }

    // call this first to reset and prep, then call SetRank() for each turtle
    public void SetNumPlayers(int numPlayers) {
        Debug.Log("Num players = " + numPlayers);
        while (turtlesParent.childCount > 0) {
            Transform t = turtlesParent.GetChild(0);
            t.SetParent(null);
            Destroy(t.gameObject);
        }
        for (int i = 0; i < numPlayers; i++) {
            if (i >= maxTurtlesToShow)
                continue;
            LeaderboardTurtle turtle = Instantiate(turtlePrefab);
            turtle.transform.SetParent(turtlesParent);
            turtle.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // rank is 0-based here
    public void SetRank(int rank, Sprite body, int number, float time, bool hasBow, bool isKeyboard = false) {
        if (rank >= maxTurtlesToShow)
            return;
        if (rank >= turtlesParent.childCount)
            throw new System.Exception("No leaderboard turtle found for rank - call SetNumPlayers()");
        LeaderboardTurtle turtle = turtlesParent.GetChild(rank).GetComponent<LeaderboardTurtle>();
        turtle.gameObject.SetActive(true);
        turtle.body.sprite = body;
        turtle.number.text = isKeyboard ? "K" : ("" + number);
        turtle.bow.gameObject.SetActive(false);
        if (time <= 0) {
            turtle.time.gameObject.SetActive(true);
            turtle.time.text = "---";
        } else {
            turtle.time.gameObject.SetActive(true);
            turtle.time.text = string.Format("{0:F2}s", time);
        }
    }
}
