using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NDream.AirConsole;

public class WinScreen : MonoBehaviour
{
    public TMPro.TMP_Text levelName, rules;
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
    public bool showTrophy;
    int winningPlayer;
    bool adCompleteTrigger, showingAd;
    public AudioSource ambientSounds;

    // Start is called before the first frame update
    void Start()
    {
        sizer.anchoredPosition = new Vector2(0, -700);
        backdrop.SetActive(false);
        sizer.gameObject.SetActive(false);

        AirConsole.instance.onAdComplete += OnAdComplete;
        AirConsole.instance.onAdShow += OnAdShow;
    }

    void OnDestroy() {
        if (AirConsole.instance) {
            AirConsole.instance.onAdComplete -= OnAdComplete;
            AirConsole.instance.onAdShow -= OnAdShow;
        }
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

        if (showingAd) 
            nextRaceTimer.text = "Waiting for ad...";
    }

    // note this doesn't always get called if AirConsole decides not to show an ad
    void OnAdShow() {
        Debug.Log("OnAdShow");
        showingAd = true;
        Time.timeScale = 0;
        if (ambientSounds)
            ambientSounds.Pause();
    }

    // afaik this always gets called after calling AirConsole.instance.ShowAd()
    void OnAdComplete(bool adWasShown) {
        Debug.Log("OnAdComplete " + adWasShown);
        showingAd = false;
        adCompleteTrigger = true;
        Time.timeScale = 1;
        if (ambientSounds)
            ambientSounds.UnPause();
    }

    IEnumerator ShowAd() {
        adCompleteTrigger = false;
        Debug.Log("Calling ShowAd()");
        AirConsole.instance.ShowAd();
        while (!adCompleteTrigger)
            yield return new WaitForEndOfFrame();
        Debug.Log("Got ad complete trigger");
    }

    public IEnumerator Show() {
        if (showing)
            throw new System.Exception("Win screen is already showing");

        if (showTrophy) {
            var nickname = winningPlayer >= 0 ? AirConsole.instance.GetNickname(winningPlayer) : "Keyboard Player";
            rules.text = string.Format("Congratulations {0}!", nickname);
        } else
            rules.text = string.Format("First to 5 wins is the champion!");

        // show
        waitToCloseTimer = secondsToShow;
        showing = true;
        backdrop.SetActive(true);
        sizer.gameObject.SetActive(true);
        sizer.anchoredPosition = new Vector2(0, -700);
        Tween tween = sizer.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutCubic);
        yield return tween.WaitForCompletion();

        // put the bow (or trophy) on the winner
        if (winnersList.childCount > 0) {
            yield return new WaitForSeconds(1);
            LeaderboardTurtle turtle = winnersList.GetChild(0).GetComponent<LeaderboardTurtle>();
            RectTransform award = turtle.bow.GetComponent<RectTransform>();
            if (showTrophy) {
                award = turtle.trophy.GetComponent<RectTransform>();
                // if (GameSounds.instance)
                //     GameSounds.instance.PlayWinSound();
            } else {
            }
            if (GameSounds.instance)
                GameSounds.instance.PlayTadaSound();
            award.gameObject.SetActive(true);
            Vector3[] corners = new Vector3[4];
            sizer.GetWorldCorners(corners);
            Transform oldParent = award.parent;
            award.SetParent(sizer);
            Vector2 center = (corners[0] + corners[1] + corners[2] + corners[3]) / 4;
            Sequence s = DOTween.Sequence();
            s.Append(award.DOMove(center, 1).From());
            if (showTrophy)
                s.Join(award.DOScale(new Vector2(8, 8), 1).From());
            else
                s.Join(award.DOScale(new Vector2(10, 10), 1).From());
            s.SetDelay(showTrophy ? 3 : 1);
            s.SetEase(Ease.OutCubic);
            yield return s.WaitForCompletion();
            award.SetParent(oldParent);
        }

        // wait
        while (waitToCloseTimer > 0)
            yield return new WaitForEndOfFrame();

        // show an ad, maybe
        yield return ShowAd();

        // hide
        tween = sizer.DOAnchorPosY(-700, 0.5f).SetEase(Ease.OutCubic);
        yield return tween.WaitForCompletion();
        backdrop.SetActive(false);
        sizer.gameObject.SetActive(false);
        showing = false;
    }

    // call this first to reset and prep, then call SetRank() for each turtle
    public void SetNumPlayers(int numPlayers) {
        Debug.Log("Num players = " + numPlayers);
        showTrophy = false;
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
    public void SetRank(int rank, Sprite body, int number, float time, bool hasBow, int wins, 
            bool isKeyboard = false) {
        if (rank >= maxTurtlesToShow)
            return;
        if (rank >= turtlesParent.childCount)
            throw new System.Exception("No leaderboard turtle found for rank - call SetNumPlayers()");
        LeaderboardTurtle turtle = turtlesParent.GetChild(rank).GetComponent<LeaderboardTurtle>();
        turtle.gameObject.SetActive(true);
        turtle.body.sprite = body;
        turtle.number.text = isKeyboard ? "K" : ("" + number);
        turtle.rank.text = "" + (rank + 1);
        turtle.bow.gameObject.SetActive(false);
        turtle.trophy.gameObject.SetActive(false);
        turtle.wins.text = "" + wins + "/5";
        if (wins >= 5) {
            showTrophy = true;
            winningPlayer = number;
        }
        if (time <= 0) {
            turtle.time.gameObject.SetActive(true);
            turtle.time.text = "---";
        } else {
            turtle.time.gameObject.SetActive(true);
            turtle.time.text = string.Format("{0:F2}s", time);
        }
    }
}
