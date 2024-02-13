using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BIGBlockController : MonoBehaviour
{
    //<ブロックの処理>
    //ブロックのステータス
    [SerializeField] private int blockHP = 1;             //ブロックのHP
    [SerializeField] private int blockMaxHP = 1;           //ブロックの最大HP
    private byte blockATK = 10;            //ブロックの攻撃力
    private byte blockEXP = 10;            //ブロックの経験値
    private byte bonusEXP = 1;            //経験値ボーナスの数値
    private int blockDamage = 0;          //ブロックのダメージ
    private int blockDamage2 = 0;         //ブロックのダメージ予備(炎ダメージなど)
    private float hp_ratio;

    //コンポーネント系
    private Rigidbody2D myRigidbody2D;
    private PlayerStatusManager _playerStatusManager;
    private Transform target;              //中心のゲームオブジェクトの位置
    private MyObjectPool _myObjectPool;    //オブジェクトプール
    private SpriteRenderer _spriteRenderer;
    private GameObject startPos;
    private GameManager _gameManager;

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
    //ゾーンの炎上ダメージタイム
    [SerializeField] private float frameTime = 0.7f;
    private float frameTimeCount;
    //ゾーンが氷状態かどうか
    private bool canBlizzardZone = false;

    //効果音
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
            else if(_playerStatusManager.playerTypeBall == PlayerStatusManager.Strengthening.none)
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
            _gameManager.UpdateBossHP(blockDamage);

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
                EnemyBreak();

            }

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Heart")
        {
            _playerStatusManager.ReceiveDamage(this.blockATK);     //ブロックがBaseに到着したらダメージを食らう
            _gameManager.DeleteBossHP();
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
                    _gameManager.UpdateBossHP(blockDamage2);

                    //ダメージテキスト
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

    private void OnTriggerExit2D(Collider2D collision)             //DengerZoneを抜けると経験値等倍
    {
        if (collision.gameObject.tag == "DengerZone")
        {
            this.bonusEXP = 1;
            canBlizzardZone = false;
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
        _gameManager.UpdateBossHP(blockDamage);
        //ダメージテキスト
        GameObject EnemyTextPbj = _myObjectPool.GetEnemyTextObj();
        EnemyTextPbj.SetActive(true);
        GameTextController _gameTextController = EnemyTextPbj.GetComponent<GameTextController>();
        _gameTextController.SetDamageText(blockDamage, this.transform.position, "Plasma");

        if (blockHP <= 0)
            EnemyBreak();
    }

    private void EnemyBreak()
    {
        //スコア（仮置き）
        _gameManager.SetScoreText(5);

        int EXP = blockEXP * bonusEXP;
        _playerStatusManager.LevelUp(EXP);

        //経験値テキスト
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

            //ダメージテキスト
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
