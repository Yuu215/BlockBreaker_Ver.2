using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardController : MonoBehaviour
{
    [SerializeField] GameObject scoreRanking;

    public void ActiveScoreRanking()
    {
        scoreRanking.SetActive(true);
    }

    public void NoActiveScoreRanking()
    {
        scoreRanking.SetActive(false);
    }
}
