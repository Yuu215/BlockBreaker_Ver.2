using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIGBlockController : MonoBehaviour
{
    //<�u���b�N�̏���>
    //�u���b�N�̃X�e�[�^�X
    [SerializeField] private int blockHP = 1;             //�u���b�N��HP
    [SerializeField] private int blockMaxHP = 1;           //�u���b�N�̍ő�HP
    private byte blockATK = 10;            //�u���b�N�̍U����
    private byte blockEXP = 10;            //�u���b�N�̌o���l
    private byte bonusEXP = 1;            //�o���l�{�[�i�X�̐��l
    private int blockDamage = 0;          //�u���b�N�̃_���[�W
    private int blockDamage2 = 0;         //�u���b�N�̃_���[�W�\��(���_���[�W�Ȃ�)
    private float hp_ratio;

    //�R���|�[�l���g�n
    private Rigidbody2D myRigidbody2D;
    private PlayerStatusManager _playerStatusManager;
    private Transform target;              //���S�̃Q�[���I�u�W�F�N�g�̈ʒu
    private MyObjectPool _myObjectPool;    //�I�u�W�F�N�g�v�[��
    private SpriteRenderer _spriteRenderer;
    private GameObject startPos;
    private GameManager _gameManager;

    //�u���b�N�̑��x��
    [SerializeField] private float blockSpeed;
    [SerializeField] private float rotate;
    [SerializeField] private Color color_1, color_2, color_3, color_4, freeze;

    //�u���b�N�������Ă�����
    private bool canBlockMove = true;
    [SerializeField] private float blockStopTime;
    private float blockStopCount = 0;

    //���d�_���[�W�͈̔�
    [SerializeField] private float damageRadius = 3.5f;
    //�]�[���̉���_���[�W�^�C��
    [SerializeField] private float frameTime = 0.7f;
    private float frameTimeCount;
    //�]�[�����X��Ԃ��ǂ���
    private bool canBlizzardZone = false;

    //���ʉ�
    private GameSoundEffects _gameSoundEffects;

    public void SetParameter(int HP)
    {
        this.blockMaxHP = HP;
        this.blockHP = HP;
    }

    private void Awake()
    {
        this.myRigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        this._spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _playerStatusManager = GameObject.Find("Manager").GetComponent<PlayerStatusManager>();
        this.target = GameObject.Find("Haert").GetComponent<Transform>();
        startPos = GameObject.Find("CreatePosition");
        this._myObjectPool = GameObject.Find("ObjectPool").GetComponent<MyObjectPool>();
        _gameSoundEffects = GameObject.Find("SoundEffect").GetComponent<GameSoundEffects>();
        _gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
    }

    void Update()
    {
        BlockStop();

        //�u���b�N�̉�]�ƒ��S�ւ̈ړ�
        if (canBlockMove && !canBlizzardZone)
        {
            this.gameObject.transform.Rotate(new Vector3(0, 0, rotate) * Time.deltaTime);
            this.gameObject.transform.position = Vector3.MoveTowards(transform.position, target.position, blockSpeed * Time.deltaTime);
        }
        if (!canBlockMove)
        {
            this.gameObject.transform.Rotate(new Vector3(0, 0, 0) * Time.deltaTime);
            this.gameObject.transform.position = Vector3.MoveTowards(transform.position, target.position, 0 * Time.deltaTime);
        }
        if (canBlizzardZone && canBlockMove)
        {
            this.gameObject.transform.Rotate(new Vector3(0, 0, rotate / 2.5f) * Time.deltaTime);
            this.gameObject.transform.position = Vector3.MoveTowards(transform.position, target.position, (blockSpeed / 2.5f) * Time.deltaTime);
        }

        hp_ratio = (float)blockHP / (float)blockMaxHP;

        if (hp_ratio > 0.75f)
        {
            _spriteRenderer.color = Color.Lerp(color_2, color_1, Mathf.Clamp01((hp_ratio - 0.75f) * 4f));
        }
        else if (hp_ratio > 0.25f)
        {
            _spriteRenderer.color = Color.Lerp(color_3, color_2, Mathf.Clamp01((hp_ratio - 0.25f) * 4f));
        }
        else
        {
            _spriteRenderer.color = Color.Lerp(color_4, color_3, Mathf.Clamp01(hp_ratio * 4f));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "PlayerBall")             //�{�[���ƃu���b�N���ڐG�������A�{�[���̍U���͕��AHP���������
        {
            float randomValue = Random.Range(0.7f, 1.3f);         //�_���[�W����
            if (_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.Frame)
            {
                blockDamage = (int)((_playerStatusManager.playerATK * 2.5f) * randomValue);

                //���ʉ�
                _gameSoundEffects.FlameBlockCollisionSound();
            }
            else if(_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.none)
            {
                blockDamage = (int)(_playerStatusManager.playerATK * randomValue);

                //���ʉ�
                _gameSoundEffects.BlockCollisionSound();
            }
            //�X��Ԃ̂Ƃ��͒�~����
            else if (_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.Blizzard)
            {
                blockDamage = (int)(_playerStatusManager.playerATK * randomValue);
                this.blockStopCount = blockStopTime;

                _gameSoundEffects.BlizzardBlockCollisionSound();
            }
            //�d�C��Ԃ̂Ƃ��͎��͂̃I�u�W�F�N�g�Ƀ_���[�W��^����
            else if (_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.Plasma)
            {
                blockDamage = (int)(_playerStatusManager.playerATK * randomValue);
                SurroundingsEnemy();

                //���ʉ�
                _gameSoundEffects.BlockCollisionSound();
            }

            this.blockHP -= blockDamage;
            _gameManager.UpdateBossHP(blockDamage);

            //�X�R�A�i���u���j
            _gameManager.SetScoreText(2);

            //�_���[�W�e�L�X�g
            GameObject EnemyTextPbj = _myObjectPool.GetEnemyTextObj();
            EnemyTextPbj.SetActive(true);
            GameTextController _gameTextController = EnemyTextPbj.GetComponent<GameTextController>();
            if (_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.Frame)
                _gameTextController.SetDamageText(blockDamage, collision.transform.position, "Frame");
            else
                _gameTextController.SetDamageText(blockDamage, collision.transform.position, "Normal");

            

            if (blockHP <= 0)                                     //HP�������Ȃ�����u���b�N���A�N�e�B�u�ɂ���
            {
                EnemyBreak();

            }

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Heart")
        {
            _playerStatusManager.ReceiveDamage(this.blockATK);     //�u���b�N��Base�ɓ���������_���[�W��H�炤
            _gameManager.DeleteBossHP();
            this.gameObject.SetActive(false);
            this.gameObject.transform.position = startPos.transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DengerZone")              //DengerZone�Ōo���l��{
        {
            this.bonusEXP = 2;

            if (_playerStatusManager.playerTypeZone == PlayerStatusManager.Strengthening.Frame)
            {
                frameTimeCount -= Time.deltaTime;

                if (frameTimeCount < 0)
                {
                    blockDamage2 = (int)((_playerStatusManager.playerATK / 6) + 1);
                    blockHP -= blockDamage2;
                    _gameManager.UpdateBossHP(blockDamage2);

                    //�_���[�W�e�L�X�g
                    GameObject EnemyTextPbj2 = _myObjectPool.GetEnemyTextObj();
                    EnemyTextPbj2.SetActive(true);
                    GameTextController _gameTextController2 = EnemyTextPbj2.GetComponent<GameTextController>();
                    _gameTextController2.SetDamageText(blockDamage2, this.transform.position, "Frame");

                    frameTimeCount = frameTime;

                    if (blockHP <= 0)
                    {
                        EnemyBreak();
                    }

                }
            }

            if (_playerStatusManager.playerTypeZone == PlayerStatusManager.Strengthening.Blizzard)
            {
                canBlizzardZone = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)             //DengerZone�𔲂���ƌo���l���{
    {
        if (collision.gameObject.tag == "DengerZone")
        {
            this.bonusEXP = 1;
            canBlizzardZone = false;
        }
    }

    //���d�_���[�W�p
    private void SurroundingsEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, damageRadius);

        Debug.Log("Number of Colliders: " + colliders.Length);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                if (collider.gameObject == this.gameObject)
                {
                    continue;
                }

                //�߂��̃u���b�N�Ɋ��d�_���[�W��^���鏈��
                collider.GetComponent<BlockController>().ElectricShockDamage(blockDamage);
            }
        }
    }

    public void ElectricShockDamage(int damege)
    {
        this.blockDamage = (damege * 2) + 1;

        this.blockHP -= blockDamage;
        _gameManager.UpdateBossHP(blockDamage);
        //�_���[�W�e�L�X�g
        GameObject EnemyTextPbj = _myObjectPool.GetEnemyTextObj();
        EnemyTextPbj.SetActive(true);
        GameTextController _gameTextController = EnemyTextPbj.GetComponent<GameTextController>();
        _gameTextController.SetDamageText(blockDamage, this.transform.position, "Plasma");

        if (blockHP <= 0)
            EnemyBreak();
    }

    private void EnemyBreak()
    {
        //�X�R�A�i���u���j
        _gameManager.SetScoreText(5);

        int EXP = blockEXP * bonusEXP;
        _playerStatusManager.LevelUp(EXP);

        //�o���l�e�L�X�g
        GameObject expText = _myObjectPool.GetEnemyTextObj();
        expText.SetActive(true);
        GameTextController _gameTextControllerEXP = expText.GetComponent<GameTextController>();
        _gameTextControllerEXP.SetEXPText(EXP, this.transform.position);

        this.gameObject.transform.position = startPos.transform.position;
        this.gameObject.SetActive(false);
    }

    public void CounterDamage()
    {
        if (bonusEXP == 2)
        {
            blockDamage2 = (_playerStatusManager.playerATK * 3);
            blockHP -= blockDamage2;
            _gameManager.UpdateBossHP(blockDamage2);

            //�_���[�W�e�L�X�g
            GameObject EnemyTextPbj2 = _myObjectPool.GetEnemyTextObj();
            EnemyTextPbj2.SetActive(true);
            GameTextController _gameTextController2 = EnemyTextPbj2.GetComponent<GameTextController>();
            _gameTextController2.SetDamageText(blockDamage2, this.transform.position, "Plasma");

            if (blockHP <= 0)
            {
                EnemyBreak();
            }
        }
    }

    private void BlockStop()
    {
        this.blockStopCount -= Time.deltaTime;
        if (blockStopCount > 0)
        {
            this.canBlockMove = false;
        }
        else if (blockStopCount < 0)
        {
            this.canBlockMove = true;
        }
    }
}
