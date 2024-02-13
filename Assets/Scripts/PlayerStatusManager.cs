using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerStatusManager : MonoBehaviour
{
    //<プレイヤーのステータス管理>
    private byte playerLV = 1;           //プレイヤーのレベル
    private int playerMaxHP = 10;        //プレイヤーの最大体力
    private int playerHP = 1;            //プレイヤーの体力
    public  int playerATK = 1;           //プレイヤーの攻撃力
    private int nextEXP = 20;            //次のレベルになるために必要な経験値
    private int nowEXP = 0;              //現在の経験値
    private int ballMiss = 3;            //ボールミス時のダメージ
    private int sp = 10;                  //スキルポイント

    

    //<アイテムゲージ>
    [SerializeField] float timeLimit;
    private float secondsBall = 0f;
    private float sexondsZone = 0f;
    public Strengthening playerTypeBall = Strengthening.none;
    public Strengthening playerTypeZone = Strengthening.none;

    //コンポーネント系
    [SerializeField] private Image hpGauge;    //プレイヤーのHPゲージSlider;
    private Slider expGauge;                   //現在の経験値ゲージSlider
    [SerializeField] private GameObject expGaugeUI;
    private Text lvText;                       //現在のレベルを表示するテキスト
    [SerializeField] private GameObject lvTextUI;
    [SerializeField] private ZoneController _zoneController;
    [SerializeField] private GameSoundEffects _gameSoundEffects;
    [SerializeField] private MyObjectPool _myObjectPool;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Image itemGaugeBall;               //アイテムゲージボール
    [SerializeField] private Image itemGaugeHeart;              //アイテムゲージハート
    [SerializeField] private GameObject frameIconBall;          //炎ボールアイコン
    [SerializeField] private GameObject blizzardIconBall;       //氷ボールアイコン
    [SerializeField] private GameObject plasmaIconBall;         //雷ボールアイコン
    [SerializeField] private GameObject frameIconZone;          //炎ゾーンアイコン
    [SerializeField] private GameObject blizzardIconZone;       //氷ゾーンアイコン
    [SerializeField] private GameObject plasmaIconZone;         //雷ゾーンアイコン

    //ステータスボード用
    [SerializeField] private Text boardLVText;
    [SerializeField] private Text boardATKText;
    [SerializeField] private Text boardHPText;
    [SerializeField] private Text boardSPText;

    void Start()
    {
        lvText = lvTextUI.GetComponent<Text>();

        lvText.text = playerLV.ToString();
        boardLVText.text = playerLV.ToString();
        boardATKText.text = playerATK.ToString();
        boardSPText.text = "SP " + sp.ToString();

        hpGauge.fillAmount = 1;
        playerHP = playerMaxHP;
        boardHPText.text = playerHP.ToString() + " / " + playerMaxHP.ToString();

        RandomPowerUpSelect();
        Debug.Log("Start HP : " + playerHP);
        Debug.Log(playerTypeBall);
    }

    public enum Strengthening
    {
        none,
        Frame,
        Blizzard,
        Plasma,
        Healing,
    }

    void Update()
    {
        updateTimerBall();
        updateTimerZone();
    }

    public void SetPlayerBuffBall(Strengthening type)
    {
        switch (type)
        {
            case Strengthening.Frame:
                this.playerTypeBall = Strengthening.Frame;
                SetIconBallFalse();
                this.secondsBall = timeLimit;
                this.frameIconBall.SetActive(true);
                _gameSoundEffects.FlameItemSound();
                break;
            case Strengthening.Blizzard:
                this.playerTypeBall = Strengthening.Blizzard;
                SetIconBallFalse();
                this.secondsBall = timeLimit;
                this.blizzardIconBall.SetActive(true);
                _gameSoundEffects.BlizzardItemSound();
                break;
            case Strengthening.Plasma:
                this.playerTypeBall = Strengthening.Plasma;
                SetIconBallFalse();
                this.secondsBall = timeLimit;
                this.plasmaIconBall.SetActive(true);
                _gameSoundEffects.PlasmaItemSound();
                break;
            case Strengthening.Healing:
                HPHealing(3);
                break;
            default:
                this.playerTypeBall = Strengthening.none;
                break;
        }
    }

    public void SetPlayerBuffZone(Strengthening type)
    {
        switch (type)
        {
            case Strengthening.Frame:
                this.playerTypeZone = Strengthening.Frame;
                SetIconZoneFalse();
                this.sexondsZone = timeLimit;
                this.frameIconZone.SetActive(true);
                _zoneController.ExpansionZone("Frame");
                break;
            case Strengthening.Blizzard:
                this.playerTypeZone = Strengthening.Blizzard;
                SetIconZoneFalse();
                this.sexondsZone = timeLimit;
                this.blizzardIconZone.SetActive(true);
                _zoneController.ExpansionZone("Blizzard");
                break;
            case Strengthening.Plasma:
                this.playerTypeZone = Strengthening.Plasma;
                SetIconZoneFalse();
                this.sexondsZone = timeLimit;
                this.plasmaIconZone.SetActive(true);
                _zoneController.ExpansionZone("Plasma");
                break;
            case Strengthening.Healing:
                HPHealing(5);
                break;
            default:
                this.playerTypeZone = Strengthening.none;
                break;
        }
    }

    public void ReceiveDamage(byte damage) //ダメージ
    {
        _gameSoundEffects.PlayerDamageSound();
        this.playerHP -= damage;
        Debug.Log("現在のHPは" + playerHP);

        _gameManager.ShakeScteen();

        if (playerTypeZone == Strengthening.Plasma)
        {
            GameObject[] totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            for (int i = 0; i < totalEnemies.Length; i++)
            {
                totalEnemies[i].GetComponent<BlockController>().CounterDamage();
            }
        }

        if (playerHP <= 0)
        {
            playerHP = 0;
            //ゲーム終了処理
            _gameManager.ThisGameEnd();
        }

        hpGauge.fillAmount = (float)playerHP / (float)playerMaxHP;
        boardHPText.text = playerHP.ToString() + " / " + playerMaxHP.ToString();
    }

    public void HPHealing(byte healing)
    {
        this.playerHP += healing;
        Debug.Log("現在のHPは" + playerHP);

        if(playerHP >= playerMaxHP)
        {
            playerHP = playerMaxHP;
        }

        hpGauge.fillAmount = (float)playerHP / (float)playerMaxHP;
        boardHPText.text = playerHP.ToString() + " / " + playerMaxHP.ToString();
    }

    public void BallGenerate()
    {
        GameObject ball = _myObjectPool.GetBallObj(Vector2.zero);
        BallController _ballController = ball.GetComponent<BallController>();
        _ballController.StartSpeed();
    }

    public void BallMiss()  //ボール残基
    {
        _gameSoundEffects.PlayerDamageSound();
        this.playerHP -= ballMiss;
        Debug.Log("現在のHPは" + playerHP);
        _gameManager.ShakeScteen();

        if (playerHP <= 0)
        {
            //ゲーム終了の処理
            _gameManager.ThisGameEnd();
        }
        else
        {
            BallGenerate();
        }

        hpGauge.fillAmount = (float)playerHP / (float)playerMaxHP;
        boardHPText.text = playerHP.ToString() + " / " + playerMaxHP.ToString();
    }

    public void LevelUp(int exp)
    {
        this.nowEXP += exp;
        Debug.Log("現在のEXPは" + nowEXP);

        while (nowEXP >= nextEXP)
        {
            nowEXP -= nextEXP;
            playerLV++;
            sp += 5;
            lvText.text = playerLV.ToString();
            boardLVText.text = playerLV.ToString();
            boardSPText.text = "SP " + sp.ToString();
            _gameSoundEffects.LevelUpSound();
        }

        //expGauge.value = (float)nowEXP / (float)nextEXP;
    }

    void updateTimerBall()
    {
        secondsBall -= Time.deltaTime;
        itemGaugeBall.fillAmount = (float)secondsBall / (float)timeLimit;
        if (secondsBall < 0)
        {
            playerTypeBall = Strengthening.none;
            SetIconBallFalse();
        }
    }

    void updateTimerZone()
    {
        sexondsZone -= Time.deltaTime;
        itemGaugeHeart.fillAmount = (float)sexondsZone / (float)timeLimit;
        if (sexondsZone < 0)
        {
            _zoneController.ReductionZone();
            playerTypeZone = Strengthening.none;
            SetIconZoneFalse();
        }
    }

    void SetIconBallFalse()
    {
        this.frameIconBall.SetActive(false);
        this.blizzardIconBall.SetActive(false);
        this.plasmaIconBall.SetActive(false);
    }

    void SetIconZoneFalse()
    {
        this.frameIconZone.SetActive(false);
        this.blizzardIconZone.SetActive(false);
        this.plasmaIconZone.SetActive(false);
    }

    //ステータス強化要素

    private int[] powerUpSelect = new int[3];
    [SerializeField] private GameObject[] iconPos;
    [SerializeField] private GameObject[] parentObject;
    [SerializeField] private Text[] skillNameText;
    [SerializeField] private Text skillExplanationText;

    [SerializeField] private Skill[] skills;
    private int RedrawSP = 5;

    private void RandomPowerUpSelect()
    {
        for (int i = 0; i < powerUpSelect.Length; i++)
        {
            //SKillsでリメイク
            int number = UnityEngine.Random.Range(0, skills.Length);
            powerUpSelect[i] = number;
            GameObject Icon = (GameObject)Instantiate(skills[number].powerUpIcon, iconPos[i].transform.position, Quaternion.identity);
            Icon.transform.SetParent(parentObject[i].transform, false);
            skillNameText[i].text = skills[number].skillName;
        }
    }

    public void ResetPowerUpSelect(bool canUp)
    {
        if (sp >= RedrawSP || canUp)
        {
            if (!canUp)
                this.sp -= 5;

            boardSPText.text = "SP " + sp.ToString();
            GameObject[] deleteIcon = GameObject.FindGameObjectsWithTag("PowerUpIcon");
            foreach (GameObject ball in deleteIcon)
            {
                Destroy(ball);
            }

            RandomPowerUpSelect();
        }
    }

    public void SetSkillExplanation(StetusPanelButton.Frame frame)
    {
        switch (frame)
        {
            case StetusPanelButton.Frame.Left:
                Rewrite(0);
                break;
            case StetusPanelButton.Frame.Middle:
                Rewrite(1);
                break;
            case StetusPanelButton.Frame.Right:
                Rewrite(2);
                break;
        }
    }

    private void Rewrite(int i)
    {
        skillExplanationText.text = skills[powerUpSelect[i]].skillExplanation;
    }

    //選択してパワーアップ
    public void PowerUpDecision(StetusPanelButton.Frame frame)
    {
        switch (frame)
        {
            case StetusPanelButton.Frame.Left:
                PowerUp(0);
                break;
            case StetusPanelButton.Frame.Middle:
                PowerUp(1);
                break;
            case StetusPanelButton.Frame.Right:
                PowerUp(2);
                break;
        }
    }

    private void PowerUp(int i)
    {

        switch (powerUpSelect[i])
        {
            case 0:
                if (sp >= skills[powerUpSelect[i]].sp)
                {
                    this.sp -= skills[powerUpSelect[i]].sp;
                    ATKUP();
                }
                else
                {
                    return;
                }
                break;
            case 1:
                if (sp >= skills[powerUpSelect[i]].sp)
                {
                    this.sp -= skills[powerUpSelect[i]].sp;
                    HPUP();
                }
                else
                {
                    return;
                }
                break;
            case 2:
                if (sp >= skills[powerUpSelect[i]].sp)
                {
                    this.sp -= skills[powerUpSelect[i]].sp;
                    TimeUP();
                }
                else
                {
                    return;
                }
                break;
            case 3:
                if (sp >= skills[powerUpSelect[i]].sp)
                {
                    this.sp -= skills[powerUpSelect[i]].sp;
                    MAXHealing();
                }
                else
                {
                    return;
                }
                break;
        }

        ResetPowerUpSelect(true);
    }


    //強化用関数

    private void ATKUP()
    {
        int up = UnityEngine.Random.Range(1, 4);
        this.playerATK += up;
        boardATKText.text = playerATK.ToString();
    }
    private void HPUP()
    {
        this.playerMaxHP += 2;
        this.playerHP += 2;
        hpGauge.fillAmount = (float)playerHP / (float)playerMaxHP;
        boardHPText.text = playerHP.ToString() + " / " + playerMaxHP.ToString();
    }
    private void TimeUP()
    {
        this.timeLimit += 2f;
    }
    private void MAXHealing()
    {
        this.playerHP = this.playerMaxHP;
        hpGauge.fillAmount = (float)playerHP / (float)playerMaxHP;
        boardHPText.text = playerHP.ToString() + " / " + playerMaxHP.ToString();
    }
}