using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerStatusManager : MonoBehaviour
{
    //<�v���C���[�̃X�e�[�^�X�Ǘ�>
    private byte playerLV = 1;           //�v���C���[�̃��x��
    private int playerMaxHP = 10;        //�v���C���[�̍ő�̗�
    private int playerHP = 1;            //�v���C���[�̗̑�
    public  int playerATK = 1;           //�v���C���[�̍U����
    private int nextEXP = 20;            //���̃��x���ɂȂ邽�߂ɕK�v�Ȍo���l
    private int nowEXP = 0;              //���݂̌o���l
    private int ballMiss = 3;            //�{�[���~�X���̃_���[�W
    private int sp = 10;                  //�X�L���|�C���g

    

    //<�A�C�e���Q�[�W>
    [SerializeField] float timeLimit;
    private float secondsBall = 0f;
    private float sexondsZone = 0f;
    public Strengthening playerTypeBall = Strengthening.none;
    public Strengthening playerTypeZone = Strengthening.none;

    //�R���|�[�l���g�n
    [SerializeField] private Image hpGauge;    //�v���C���[��HP�Q�[�WSlider;
    private Slider expGauge;                   //���݂̌o���l�Q�[�WSlider
    [SerializeField] private GameObject expGaugeUI;
    private Text lvText;                       //���݂̃��x����\������e�L�X�g
    [SerializeField] private GameObject lvTextUI;
    [SerializeField] private ZoneController _zoneController;
    [SerializeField] private GameSoundEffects _gameSoundEffects;
    [SerializeField] private MyObjectPool _myObjectPool;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Image itemGaugeBall;               //�A�C�e���Q�[�W�{�[��
    [SerializeField] private Image itemGaugeHeart;              //�A�C�e���Q�[�W�n�[�g
    [SerializeField] private GameObject frameIconBall;          //���{�[���A�C�R��
    [SerializeField] private GameObject blizzardIconBall;       //�X�{�[���A�C�R��
    [SerializeField] private GameObject plasmaIconBall;         //���{�[���A�C�R��
    [SerializeField] private GameObject frameIconZone;          //���]�[���A�C�R��
    [SerializeField] private GameObject blizzardIconZone;       //�X�]�[���A�C�R��
    [SerializeField] private GameObject plasmaIconZone;         //���]�[���A�C�R��

    //�X�e�[�^�X�{�[�h�p
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

    public void ReceiveDamage(byte damage) //�_���[�W
    {
        _gameSoundEffects.PlayerDamageSound();
        this.playerHP -= damage;
        Debug.Log("���݂�HP��" + playerHP);

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
            //�Q�[���I������
            _gameManager.ThisGameEnd();
        }

        hpGauge.fillAmount = (float)playerHP / (float)playerMaxHP;
        boardHPText.text = playerHP.ToString() + " / " + playerMaxHP.ToString();
    }

    public void HPHealing(byte healing)
    {
        this.playerHP += healing;
        Debug.Log("���݂�HP��" + playerHP);

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

    public void BallMiss()  //�{�[���c��
    {
        _gameSoundEffects.PlayerDamageSound();
        this.playerHP -= ballMiss;
        Debug.Log("���݂�HP��" + playerHP);
        _gameManager.ShakeScteen();

        if (playerHP <= 0)
        {
            //�Q�[���I���̏���
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
        Debug.Log("���݂�EXP��" + nowEXP);

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

    //�X�e�[�^�X�����v�f

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
            //SKills�Ń����C�N
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

    //�I�����ăp���[�A�b�v
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


    //�����p�֐�

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