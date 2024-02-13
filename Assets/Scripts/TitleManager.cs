using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    //タイトル画面
    private bool firstTap = false;
    private bool rankingPanel = false;
    [SerializeField] GameObject startText;
    //メニューの項目
    [SerializeField] GameObject playText;
    [SerializeField] GameObject scoreText;

    [SerializeField] StartSound _startSound;
    [SerializeField] GameObject _blackPanel;
    [SerializeField] Image blackPanel;
    [SerializeField] Text[] rankingText;

    void Start()
    {
        SetScoraRanking();
    }


    void Update()
    {
        if (!firstTap && !rankingPanel && Input.GetMouseButtonUp(0))
        {
            startText.SetActive(false);
            firstTap = true;
            _startSound.FirstTapSound();
            SetMenu();
        }
        
    }

    private void SetScoraRanking()
    {
        for (int i = 0; i < rankingText.Length; i++)
        {
            int scora = PlayerPrefs.GetInt(GameManager.SCORE_SAVE_NAME + i, 0);
            rankingText[i].text = scora.ToString();
        }
    }

    private void SetMenu()
    {
        playText.SetActive(true);
        scoreText.SetActive(true);
    }

    public void GameSceneLoading()
    {
        _blackPanel.SetActive(true);
        _startSound.SceneMoveSound();
        blackPanel.DOFade(1f, 2f).OnComplete(() =>
         {
             SceneManager.LoadScene("SampleScene");
         });
    }
}
