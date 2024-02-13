using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    //<ブロックの処理>
    //ブロックのステータス
    [SerializeField] private int  blockHP = 1;             //ブロックのHP
    [SerializeField] private int blockMaxHP = 1;           //ブロックの最大HP
    private byte blockATK = 1;            //ブロックの攻撃力
    private byte blockEXP = 1;            //ブロックの経験値
    private byte bonusEXP = 1;            //経験値ボーナスの数値
    private Wave.SpawnType spawnType;     //ブロックのスポーンテーブル
    private BlockType BlockTypeDetermine; //ブロックのタイプ
    private int blockDamage = 0;          //ブロックのダメージ
    private int blockDamage2 = 0;         //ブロックのダメージ予備(炎ダメージなど)
    private float hp_ratio;

    //コンポーネント系
    private Rigidbody2D myRigidbody2D;
    private PlayerStatusManager _playerStatusManager;
    private GameManager _gameManager;
    private Transform target;              //中心のゲームオブジェクトの位置
    private MyObjectPool _myObjectPool;    //オブジェクトプール
    private GameObject frameIcon;          //炎アイコン
    private GameObject blizzardIcon;       //氷アイコン
    private GameObject plasmaIcon;         //雷アイコン
    private GameObject healingIcon;        //回復アイコン
    private GameObject bomIcon;            //爆弾アイコン
    private SpriteRenderer _spriteRenderer;
    private GameObject startPos;

    //ブロックの速度等
    [SerializeField] private float blockSpeed;
    [SerializeField] private float rotate;
    [SerializeField] private Color color_1, color_2, color_3, color_4, freeze;

    //ブロックが動いていいか
    private bool canBlockMove = true;
    [SerializeField] private float blockStopTime;
    private float blockStopCount = 0;

    //感電ダメージの範囲
    [SerializeField] private float damageRadius = 3.5f;
    //爆破ダメージの範囲
    [SerializeField] private float damageRadiusEX = 5.0f;
    //ゾーンの炎上ダメージタイム
    [SerializeField] private float frameTime = 0.7f;
    private float frameTimeCount;
    //ゾーンが氷状態かどうか
    private bool canBlizzardZone = false;

    //効果音
    private GameSoundEffects _gameSoundEffects;

    //ブロックのタイプ
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

        //アイコン取得
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

        //ブロックの回転と中心への移動
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
        
        if (collision.gameObject.tag == "PlayerBall")             //ボールとブロックが接触した時、ボールの攻撃力分、HPを消費させる
        {
             float randomValue = Random.Range(0.7f, 1.3f);         //ダメージ乱数
            if (_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.Frame)
            {
                blockDamage = (int)((_playerStatusManager.playerATK * 2.5f) * randomValue);

                //効果音
                _gameSoundEffects.FlameBlockCollisionSound();
            }
            else if (_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.none)
            {
                blockDamage = (int)(_playerStatusManager.playerATK * randomValue);

                //効果音
                _gameSoundEffects.BlockCollisionSound();
            }
            //氷状態のときは停止する
            else if (_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.Blizzard)
            {
                blockDamage = (int)(_playerStatusManager.playerATK * randomValue);
                this.blockStopCount = blockStopTime;

                _gameSoundEffects.BlizzardBlockCollisionSound();
            }
            //電気状態のときは周囲のオブジェクトにダメージを与える
            else if (_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.Plasma)
            {
                blockDamage = (int)(_playerStatusManager.playerATK * randomValue);
                SurroundingsEnemy();

                //効果音
                _gameSoundEffects.BlockCollisionSound();
            }

            this.blockHP -= blockDamage;

            //スコア（仮置き）
            _gameManager.SetScoreText(2);

            //ダメージテキスト
            GameObject EnemyTextPbj = _myObjectPool.GetEnemyTextObj();
            EnemyTextPbj.SetActive(true);
            GameTextController _gameTextController = EnemyTextPbj.GetComponent<GameTextController>();
            if (_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.Frame)
                _gameTextController.SetDamageText(blockDamage, collision.transform.position, "Frame");
            else
                _gameTextController.SetDamageText(blockDamage, collision.transform.position, "Normal");
            

            if (blockHP <= 0)                                     //HPが無くなったらブロックを非アクティブにする
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
            _playerStatusManager.ReceiveDamage(this.blockATK);     //ブロックがBaseに到着したらダメージを食らう
            this.gameObject.SetActive(false);
            this.gameObject.transform.position = startPos.transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DengerZone")              //DengerZoneで経験値二倍
        {
            this.bonusEXP = 2;

            if (_playerStatusManager.playerTypeZone == PlayerStatusManager.Strengthening.Frame)
            {
                frameTimeCount -= Time.deltaTime;

                if (frameTimeCount < 0)
                {
                    blockDamage2 = (int)((_playerStatusManager.playerATK / 6) + 1);
                    blockHP -= blockDamage2;

                    //ダメージテキスト
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

    private void OnTriggerExit2D(Collider2D collision)             //DengerZoneを抜けると経験値等倍
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

        //判定
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

        this.gameObject.transform.rotation = Quaternion.identity; // 回転を初期化
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

    //感電ダメージ用
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

                //近くのブロックに感電ダメージを与える処理
                collider.GetComponent<BlockController>().ElectricShockDamage(blockDamage);
            }
        }
    }

    public void ElectricShockDamage(int damege)
    {
        this.blockDamage = (damege * 2) + 1;

        this.blockHP -= blockDamage;
        //ダメージテキスト
        GameObject EnemyTextPbj = _myObjectPool.GetEnemyTextObj();
        EnemyTextPbj.SetActive(true);
        GameTextController _gameTextController = EnemyTextPbj.GetComponent<GameTextController>();
        _gameTextController.SetDamageText(blockDamage, this.transform.position, "Plasma");

        if (blockHP <= 0)
            EnemyBreak();
    }

    //爆破ダメージ用
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

                //近くのブロックに爆破ダメージを与える処理
                collider.GetComponent<BlockController>().ExplosionDamage();
            }
        }
    }

    public void ExplosionDamage()
    {
        this.blockHP = 0;
        Debug.Log("ダメージ");
        //ダメージテキスト
        GameObject EnemyTextPbj = _myObjectPool.GetEnemyTextObj();
        EnemyTextPbj.SetActive(true);
        GameTextController _gameTextController = EnemyTextPbj.GetComponent<GameTextController>();
        _gameTextController.SetDamageText(999, this.transform.position, "Frame");

        if (blockHP <= 0)
            EnemyBreak();
    }

    private void EnemyBreak()
    {
        //スコア（仮置き）
        _gameManager.SetScoreText(5);

        //ブロックのタイプで特殊効果を起こす　炎、氷、雷、癒、爆
        int EXP = blockEXP * bonusEXP;
            _playerStatusManager.LevelUp(EXP);

            //経験値テキスト
            GameObject expText = _myObjectPool.GetEnemyTextObj();
            expText.SetActive(true);
            GameTextController _gameTextControllerEXP = expText.GetComponent<GameTextController>();
            _gameTextControllerEXP.SetEXPText(EXP, this.transform.position);

            //アイテムを生成
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

            //ダメージテキスト
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