using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] LineGenerator _lineGenerator;
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] ZoneController _zoneController;
    [SerializeField] GameSoundEffects _gameSoundEffects;
    [SerializeField] WaveManager _waveManager;
    [SerializeField] PlayerStatusManager _playerStatusManager;
    [SerializeField] StetusPanelButton _stetusPanelButton;
    [SerializeField] GameObject shakeScreenObj;
    [SerializeField] GameObject heart;
    [SerializeField] GameObject countDownObj;
    [SerializeField] Text countDownText;
    [SerializeField] Text countDownText2;
    [SerializeField] GameObject playerBall;
    [SerializeField] Text scoreText;
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject endPanel;
    [SerializeField] GameObject gameoverText;
    [SerializeField] GameObject highScoreText;
    [SerializeField] GameObject[] rankingText;
    [SerializeField] Text[] scoreRankingText;
    [SerializeField] Text nowScoraText;
    private bool endTap = false;


    //《セーブ用》
    public const string SCORE_SAVE_NAME = "Score";

    private int score = 0;

    public bool canGameStart = false;
    private float startDelay = 1f;

    List<int> ranking = new List<int>(10) {0,0,0,0,0};

    private void Start()
    {
        StartCoroutine(StartCountdown());
        SetWaveText(0);

    }

    private IEnumerator StartCountdown()
    {
        for (int i = 3; i > 0; i--)
        {
            countDownText.text = i.ToString();
            countDownText2.text = i.ToString();
            _gameSoundEffects.Countdown();
            yield return new WaitForSeconds(startDelay);
        }

        countDownText.text = "START";
        countDownText2.text = "START";
        _gameSoundEffects.StartSound();

        yield return new WaitForSeconds(startDelay);

        countDownObj.SetActive(false);
        canGameStart = true;
        _playerStatusManager.BallGenerate();
    }

    private void Update()
    {
        if (canGameStart)
        {

            if (Input.GetKeyDown(KeyCode.P))
            {
                Time.timeScale = 0;
                _lineGenerator.CanLine(false);
                pauseCanvas.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Time.timeScale = 1;
                _lineGenerator.CanLine(true);
                pauseCanvas.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                ShakeScteen();
            }
        }

        if (endTap)
        {
            if(Input.GetMouseButton(0))
            {
                SceneManager.LoadScene("StartScene");
            }
        }
    }

    public void ShakeScteen()
    {
        shakeScreenObj.transform.DOKill(true);
        shakeScreenObj.transform.DOShakePosition(1f, 15f, 50, 1, false, true);

        heart.transform.DOKill(true);
        heart.transform.DOShakePosition(1f, 0.3f, 30, 1, false, true);
    }

    public void GameStop()
    {
        Time.timeScale = 0;
        _lineGenerator.CanLine(false);
    }

    public void GameMove()
    {
        Time.timeScale = 1;
        _lineGenerator.CanLine(true);
    }

    public void SetWaveText(int wave)
    {
        scoreText.text = "WAVE " + wave.ToString();

        //アニメーションするならここで挟む

        DOVirtual.DelayedCall(3.0f, () =>
         {
             SetScoreText(0);
         });
    }

    public void SetScoreText(int score)
    {
        this.score += score;

        scoreText.text = this.score.ToString("d8");
    }

    public void ThisGameEnd()
    {
        //ブロックのスポーン終了とオブジェクトの削除
        _waveManager.GameEND();
        canGameStart = false;
        _lineGenerator.CanLine(false);
        ScoreSave();
        gamePanel.SetActive(false);
        endPanel.SetActive(true);
        GameOverDirection();

        GameObject[] totalBall = GameObject.FindGameObjectsWithTag("PlayerBall");
        foreach (GameObject ball in totalBall)
        {
            if (ball.activeSelf)
            {
                ball.SetActive(false);
            }
        }

        DOVirtual.DelayedCall(2f, () =>
        {
            this.endTap = true;
        });
    }

    //特殊WAVE用テキスト
    [SerializeField] private GameObject specialTextFlame;
    [SerializeField] private Text specialText;
    private void OpenTextFlame()
    {
        specialTextFlame.SetActive(true);
    }

    public void SetSpecial(WaveManager.SpexialWAVEs waves)
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            OpenTextFlame();
            SetSpecialText(waves);
        });
    }

    private void SetSpecialText(WaveManager.SpexialWAVEs waves)
    {
        switch (waves)
        {
            case WaveManager.SpexialWAVEs.BIGBOSS:
                specialText.text = "BIGBOSSが現れた！";
                break;
        }

        DOVirtual.DelayedCall(3.5f, () =>
        {
            CloseTextFlame();
        });
    }

    private void CloseTextFlame()
    {
        specialText.text = "";
        specialTextFlame.SetActive(false);
    }

    //ボスHPUI
    [SerializeField] private GameObject bossUI;
    [SerializeField] private Slider bossHp;
    [SerializeField] private Text bossNameText;
    private int bossMaxHp;
    private int bossNowHp;

    //BIGBOSS
    public void SetBOSSHP(string bossName,int hp)
    {
        bossNameText.text = bossName;
        bossMaxHp = hp;
        bossNowHp = hp;
        bossHp.value = (float)bossNowHp / (float)bossMaxHp;

        DOVirtual.DelayedCall(4.5f, () =>
        {
            bossUI.SetActive(true);
        });
    }

    public void UpdateBossHP(int damage)
    {
        Debug.Log("HP更新");
        bossNowHp -= damage;
        bossHp.value = (float)bossNowHp / (float)bossMaxHp;

        if (bossNowHp <= 0)
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                bossUI.SetActive(false);
            });
        }
    }

    public void DeleteBossHP()
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            bossUI.SetActive(false);
        });
    }

    public void ScoreSave()
    {
        ranking.Add(score);
        ranking.Sort((a, b) => b - a);
        ranking.RemoveAt(ranking.Count - 1);

        for (int i = 0;i < ranking.Count; i++)
        {
            ranking.Add(PlayerPrefs.GetInt(SCORE_SAVE_NAME + i, 0));
            ranking.Sort((a, b) => b - a);
            ranking.RemoveAt(ranking.Count - 1);
        }
        
        for (int i = 0; i < ranking.Count; i++)
        {
            PlayerPrefs.SetInt(SCORE_SAVE_NAME + i, ranking[i]);
        }
    }

    private void GameOverDirection()
    {
        nowScoraText.text = score.ToString();

        for (int i = 0; i < rankingText.Length; i++)
        {
            rankingText[i].SetActive(true);
            int scora = PlayerPrefs.GetInt(SCORE_SAVE_NAME + i, 0);
            scoreRankingText[i].text = scora.ToString();
        }
    }


    
}
