using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WinScreen : MonoBehaviour
{
    public bool testStart;
    public RectTransform sizer, winnersList;

    // Start is called before the first frame update
    void Start()
    {
        sizer.anchoredPosition = new Vector2(0, -500);
    }

    void Update() {
        if (testStart) {
            Show();
            testStart = false;
        }
    }

    IEnumerator Go() {
        sizer.anchoredPosition = new Vector2(0, -500);
        Tween tween = sizer.DOAnchorPosY(0, 0.5f);
        yield return tween.WaitForCompletion();
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
        yield return s.WaitForCompletion();
        bow.SetParent(oldParent);
    }

    public void Show() {
        StartCoroutine(Go());
    }
}
