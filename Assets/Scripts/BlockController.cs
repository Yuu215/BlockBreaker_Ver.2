using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    //<�u���b�N�̏���>
    //�u���b�N�̃X�e�[�^�X
    [SerializeField] private int  blockHP = 1;             //�u���b�N��HP
    [SerializeField] private int blockMaxHP = 1;           //�u���b�N�̍ő�HP
    private byte blockATK = 1;            //�u���b�N�̍U����
    private byte blockEXP = 1;            //�u���b�N�̌o���l
    private byte bonusEXP = 1;            //�o���l�{�[�i�X�̐��l
    private Wave.SpawnType spawnType;     //�u���b�N�̃X�|�[���e�[�u��
    private BlockType BlockTypeDetermine; //�u���b�N�̃^�C�v
    private int blockDamage = 0;          //�u���b�N�̃_���[�W
    private int blockDamage2 = 0;         //�u���b�N�̃_���[�W�\��(���_���[�W�Ȃ�)
    private float hp_ratio;

    //�R���|�[�l���g�n
    private Rigidbody2D myRigidbody2D;
    private PlayerStatusManager _playerStatusManager;
    private GameManager _gameManager;
    private Transform target;              //���S�̃Q�[���I�u�W�F�N�g�̈ʒu
    private MyObjectPool _myObjectPool;    //�I�u�W�F�N�g�v�[��
    private GameObject frameIcon;          //���A�C�R��
    private GameObject blizzardIcon;       //�X�A�C�R��
    private GameObject plasmaIcon;         //���A�C�R��
    private GameObject healingIcon;        //�񕜃A�C�R��
    private GameObject bomIcon;            //���e�A�C�R��
    private SpriteRenderer _spriteRenderer;
    private GameObject startPos;

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
    //���j�_���[�W�͈̔�
    [SerializeField] private float damageRadiusEX = 5.0f;
    //�]�[���̉���_���[�W�^�C��
    [SerializeField] private float frameTime = 0.7f;
    private float frameTimeCount;
    //�]�[�����X��Ԃ��ǂ���
    private bool canBlizzardZone = false;

    //���ʉ�
    private GameSoundEffects _gameSoundEffects;

    //�u���b�N�̃^�C�v
    public enum BlockType
    {
        Default,
        Flame,
        Blizzard,
        Plasma,
        Healing,
        Bom,
        Small,
    }

    public void SetParameter(int HP, byte EXP, Wave.SpawnType spawnType)
    {
        this.blockMaxHP = HP;
        this.blockHP = HP;
        this.blockEXP = EXP;
        this.spawnType = spawnType;
        frameTimeCount = frameTime;
    }

    private void Awake()
    {
        this.myRigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        this._spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        //�A�C�R���擾
        this.frameIcon = this.gameObject.transform.Find("FrameIcon").gameObject;
        this.blizzardIcon = this.gameObject.transform.Find("BlizzardIcon").gameObject;
        this.plasmaIcon = this.gameObject.transform.Find("PlasmaIcon").gameObject;
        this.healingIcon = this.gameObject.transform.Find("Healing").gameObject;
        this.bomIcon = this.gameObject.transform.Find("Bom").gameObject;
    }

    void Start()
    {
        _playerStatusManager = GameObject.Find("Manager").GetComponent<PlayerStatusManager>();
        this.target = GameObject.Find("Haert").GetComponent<Transform>();
        this._myObjectPool = GameObject.Find("ObjectPool").GetComponent<MyObjectPool>();
        startPos = GameObject.Find("CreatePosition");
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
            else if (_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.none)
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
                if (this.BlockTypeDetermine == BlockType.Bom)
                {
                    ExplosionEnemy();
                }

                EnemyBreak();

            }

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Heart")
        {
            _playerStatusManager.ReceiveDamage(this.blockATK);     //�u���b�N��Base�ɓ���������_���[�W��H�炤
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

                    //�_���[�W�e�L�X�g
                    GameObject EnemyTextPbj2 = _myObjectPool.GetEnemyTextObj();
                    EnemyTextPbj2.SetActive(true);
                    GameTextController _gameTextController2 = EnemyTextPbj2.GetComponent<GameTextController>();
                    _gameTextController2.SetDamageText(blockDamage2, this.transform.position, "Frame");

                    frameTimeCount = frameTime;

                    if (blockHP <= 0)
                    {
                        if (this.BlockTypeDetermine == BlockType.Bom)
                        {
                            ExplosionEnemy();
                        }

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

    public void BlockTypeTable()
    {
        byte rand = (byte)Random.Range(0, 101);
        byte[] probabilities;
        BlockType[] blockTypeNames;

        switch (spawnType)
        {
            case Wave.SpawnType.Normal:
                probabilities = new byte[] { 81, 6, 6, 6, 1 };
                blockTypeNames = new BlockType[] { BlockType.Default, BlockType.Flame, BlockType.Blizzard, BlockType.Plasma, BlockType.Healing };
                break;
            case Wave.SpawnType.Bom:
                probabilities = new byte[] { 55, 44, 1 };
                blockTypeNames = new BlockType[] { BlockType.Small, BlockType.Bom, BlockType.Healing };
                break;
            case Wave.SpawnType.Frame:
                probabilities = new byte[] { 60, 10, 30 };
                blockTypeNames = new BlockType[] { BlockType.Default, BlockType.Flame, BlockType.Small };
                break;
            case Wave.SpawnType.BlizzardAndSpark:
                probabilities = new byte[] { 45, 35, 10, 10 };
                blockTypeNames = new BlockType[] { BlockType.Default, BlockType.Small, BlockType.Blizzard, BlockType.Plasma };
                break;
            default:
                probabilities = new byte[] { 100 };
                blockTypeNames = new BlockType[] { BlockType.Default };
                break;
        }

        //����
        byte cumulativeProbability = 0;
        for (byte i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (rand <= cumulativeProbability)
            {
                this.BlockTypeDetermine = blockTypeNames[i];
                break;
            }
        }

        switch (this.BlockTypeDetermine)
        {
            case BlockType.Default:
                SetDefault();
                break;
            case BlockType.Flame:
                SetFlame();
                break;
            case BlockType.Blizzard:
                SetBlizzard();
                break;
            case BlockType.Plasma:
                SetPlasma();
                break;
            case BlockType.Healing:
                SetHealing();
                break;
            case BlockType.Bom:
                SetBom();
                break;
            case BlockType.Small:
                SetSmall();
                break;
        }

        this.gameObject.transform.rotation = Quaternion.identity; // ��]��������
    }

    private void SetDefault()
    {
        this.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        frameIcon.SetActive(false);
        blizzardIcon.SetActive(false);
        plasmaIcon.SetActive(false);
        healingIcon.SetActive(false);
        bomIcon.SetActive(false);
    }

    private void SetFlame()
    {
        this.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        frameIcon.SetActive(true);
        blizzardIcon.SetActive(false);
        plasmaIcon.SetActive(false);
        healingIcon.SetActive(false);
        bomIcon.SetActive(false);
    }

    private void SetBlizzard()
    {
        this.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        frameIcon.SetActive(false);
        blizzardIcon.SetActive(true);
        plasmaIcon.SetActive(false);
        healingIcon.SetActive(false);
        bomIcon.SetActive(false);
    }

    private void SetPlasma()
    {
        this.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        frameIcon.SetActive(false);
        blizzardIcon.SetActive(false);
        plasmaIcon.SetActive(true);
        healingIcon.SetActive(false);
        bomIcon.SetActive(false);
    }

    private void SetHealing()
    {
        this.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        frameIcon.SetActive(false);
        blizzardIcon.SetActive(false);
        plasmaIcon.SetActive(false);
        healingIcon.SetActive(true);
        bomIcon.SetActive(false);
    }

    private void SetBom()
    {
        this.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        frameIcon.SetActive(false);
        blizzardIcon.SetActive(false);
        plasmaIcon.SetActive(false);
        healingIcon.SetActive(false);
        bomIcon.SetActive(true);
    }

    private void SetSmall()
    {
        this.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1);
        frameIcon.SetActive(false);
        blizzardIcon.SetActive(false);
        plasmaIcon.SetActive(false);
        healingIcon.SetActive(false);
        bomIcon.SetActive(false);
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
        //�_���[�W�e�L�X�g
        GameObject EnemyTextPbj = _myObjectPool.GetEnemyTextObj();
        EnemyTextPbj.SetActive(true);
        GameTextController _gameTextController = EnemyTextPbj.GetComponent<GameTextController>();
        _gameTextController.SetDamageText(blockDamage, this.transform.position, "Plasma");

        if (blockHP <= 0)
            EnemyBreak();
    }

    //���j�_���[�W�p
    private void ExplosionEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, damageRadiusEX);
        Debug.Log("Number of Colliders: " + colliders.Length);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                if (collider.gameObject == this.gameObject)
                {
                    continue;
                }

                //�߂��̃u���b�N�ɔ��j�_���[�W��^���鏈��
                collider.GetComponent<BlockController>().ExplosionDamage();
            }
        }
    }

    public void ExplosionDamage()
    {
        this.blockHP = 0;
        Debug.Log("�_���[�W");
        //�_���[�W�e�L�X�g
        GameObject EnemyTextPbj = _myObjectPool.GetEnemyTextObj();
        EnemyTextPbj.SetActive(true);
        GameTextController _gameTextController = EnemyTextPbj.GetComponent<GameTextController>();
        _gameTextController.SetDamageText(999, this.transform.position, "Frame");

        if (blockHP <= 0)
            EnemyBreak();
    }

    private void EnemyBreak()
    {
        //�X�R�A�i���u���j
        _gameManager.SetScoreText(5);

        //�u���b�N�̃^�C�v�œ�����ʂ��N�����@���A�X�A���A���A��
        int EXP = blockEXP * bonusEXP;
            _playerStatusManager.LevelUp(EXP);

            //�o���l�e�L�X�g
            GameObject expText = _myObjectPool.GetEnemyTextObj();
            expText.SetActive(true);
            GameTextController _gameTextControllerEXP = expText.GetComponent<GameTextController>();
            _gameTextControllerEXP.SetEXPText(EXP, this.transform.position);

            //�A�C�e���𐶐�
            switch (this.BlockTypeDetermine)
            {
                case BlockType.Default:
                    break;
                case BlockType.Flame:
                    GameObject item = _myObjectPool.GetItemObj(this.transform.position);
                    ItemController _itemController = item.GetComponent<ItemController>();
                    _itemController.SetItem(BlockTypeDetermine);
                    break;
                case BlockType.Blizzard:
                    GameObject item2 = _myObjectPool.GetItemObj(this.transform.position);
                    ItemController _itemController2 = item2.GetComponent<ItemController>();
                    _itemController2.SetItem(BlockTypeDetermine);
                    break;
                case BlockType.Plasma:
                    GameObject item3 = _myObjectPool.GetItemObj(this.transform.position);
                    ItemController _itemController3 = item3.GetComponent<ItemController>();
                    _itemController3.SetItem(BlockTypeDetermine);
                    break;
                case BlockType.Healing:
                    GameObject item4 = _myObjectPool.GetItemObj(this.transform.position);
                    ItemController _itemController4 = item4.GetComponent<ItemController>();
                    _itemController4.SetItem(BlockTypeDetermine);
                    break;
                case BlockType.Bom:
                    break;
                case BlockType.Small:
                    break;
            }

        this.gameObject.transform.position = startPos.transform.position;
        this.gameObject.SetActive(false);
    }

    public void CounterDamage()
    {
        if (bonusEXP == 2)
        {
            blockDamage2 = (_playerStatusManager.playerATK * 3);
            blockHP -= blockDamage2;

            //�_���[�W�e�L�X�g
            GameObject EnemyTextPbj2 = _myObjectPool.GetEnemyTextObj();
            EnemyTextPbj2.SetActive(true);
            GameTextController _gameTextController2 = EnemyTextPbj2.GetComponent<GameTextController>();
            _gameTextController2.SetDamageText(blockDamage2, this.transform.position, "Plasma");

            if (blockHP <= 0)
            {
                if (this.BlockTypeDetermine == BlockType.Bom)
                {
                    ExplosionEnemy();
                }

                EnemyBreak();

            }
        }
    }
}