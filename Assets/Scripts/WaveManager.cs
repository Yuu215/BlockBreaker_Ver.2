using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Wave
{
    public enum SpawnType
    {
        Normal = 1,
        Bom = 2,
        Frame = 3,
        BlizzardAndSpark = 4,
    }

    
    public string waveName;         //ウエーブの名前
   
    public Transform[] spawnPoints; //スポーン位置
    public SpawnType spawnType;     //ブロックのタイプ
    public byte blockEXP;           //ブロックの経験値
}

public class WaveManager : MonoBehaviour
{
    public Wave[] waves;
    [SerializeField] private MyObjectPool _myObjectPool;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Transform StartPos;

    private Wave currentWave;       //現在のウェーブ
    private int currentWaveNumber;  //現在のウェーブのナンバー
    private float nextSpawnTime;    //次にエネミーがスポーンされる間隔

    private int TotalWave = 1;      //ウェーブの合計

    private bool canSpawn = true;   //生成していいかどうか
    private bool gameend = false;   //ゲームが終了しているかどうか

    private enum MAXLevel
    {
        MAXFnemies = 500,
    }

    private float menspawn = 0.1f;
    private float spawnInterval = 5.6f;         //次のブロックのスポーン感覚
    private int originalNoOfFnemies = 10;
    private int noOfFnemies = 10;               //ウェーブで出てくるエネミーの数

    void Update()
    {
        if (_gameManager.canGameStart)
        {
            currentWave = waves[currentWaveNumber];
            SpawnWave();
            GameObject[] totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            bool allEnemiesInactive = true;

            foreach (GameObject enemy in totalEnemies)
            {
                if (enemy.activeSelf)
                {
                    allEnemiesInactive = false;
                    break;  // 1つでもアクティブな敵が見つかればループを抜ける
                }
            }

            if (allEnemiesInactive && !canSpawn && !gameend)
            {
                SpawnNextWave();  //次のウェーブへ
            }
        }
    }

    void SpawnNextWave()  //次のウェーブへ
    {
        //ウェーブを知らせるアニメーション

        currentWaveNumber = UnityEngine.Random.Range(0, waves.Length);
        TotalWave++;
        _gameManager.SetWaveText(TotalWave);

        if (spawnInterval > menspawn)
        {
            spawnInterval -= 0.15f;

            if (spawnInterval < menspawn)
            {
                spawnInterval = menspawn;
            }
        }

        //特殊WAVEの処理
        int specialWaveProbaility = UnityEngine.Random.Range(0, 11);
        if (specialWaveProbaility >= 5)
        {
            SpecialWAVE();
        }

        canSpawn = true;
    }

    void SpawnWave()
    {
        if (!gameend && canSpawn && nextSpawnTime < Time.time)
        {
            int HP = (int)((TotalWave * 1.5) + (UnityEngine.Random.Range(1, 2f)));

            //エネミーの情報
            Vector2 randomPoint = currentWave.spawnPoints[UnityEngine.Random.Range(0, currentWave.spawnPoints.Length)].position;       //エネミーの位置を設定したポジションからランダムに決める
            GameObject block = _myObjectPool.GetBlockObj(randomPoint);                                                     //オブジェクトプールに位置、スポーンタイプを渡して生成
            BlockController _blockController = block.GetComponent<BlockController>();
            _blockController.SetParameter(HP, currentWave.blockEXP, currentWave.spawnType);
            _blockController.BlockTypeTable();

            //現在のウェーブ処理
            noOfFnemies--;　　　　　　　　　　　　　　　　//残りの生成するオブジェクトの数を更新
            nextSpawnTime = Time.time + spawnInterval;　　//次のブロック生成までの間隔を更新

            if (noOfFnemies == 0)                         //ウェーブのブロック数が無くなったら終了
            {
                canSpawn = false;

                if (originalNoOfFnemies < (int)MAXLevel.MAXFnemies)
                {
                    originalNoOfFnemies += (int)(TotalWave * 1.5f);

                    if (originalNoOfFnemies >= (int)MAXLevel.MAXFnemies)
                    {
                        originalNoOfFnemies = (int)MAXLevel.MAXFnemies;
                    }
                }
                
                noOfFnemies = originalNoOfFnemies;
                
            }
        }

    }

    public void GameEND()
    {
        gameend = true;

        //残っているオブジェクトを削除する
        GameObject[] totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in totalEnemies)
        {
            if (enemy.activeSelf)
            {
                enemy.SetActive(false);
            }
        }
        GameObject[] totalItem = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in totalItem)
        {
            if (item.activeSelf)
            {
                item.SetActive(false);
            }
        }
    }

    public enum SpexialWAVEs
    {
        BIGBOSS
    }

    private void SpecialWAVE()
    {
        int maxCount = Enum.GetNames(typeof(SpexialWAVEs)).Length;

        int number = UnityEngine.Random.Range(0, maxCount);

        SpexialWAVEs _spexialWAVEs = (SpexialWAVEs)Enum.ToObject(typeof(SpexialWAVEs), number);

        switch (_spexialWAVEs)
        {
            case SpexialWAVEs.BIGBOSS:
                BIGBOSSWAVE();
                break;
        }
    }

    //特殊WAVEの関数
    [SerializeField] private GameObject bigBlock;
    [SerializeField] private Transform[] bigBossPos;
    private GameObject bigBoss;
    private BIGBlockController _bIGBlockController;

    void Start()
    {
        //BIGBOSSの初期化
        bigBoss = Instantiate(bigBlock, StartPos.position, Quaternion.identity);
        _bIGBlockController = bigBoss.GetComponent<BIGBlockController>();
        bigBoss.SetActive(false);
    }

    private void BIGBOSSWAVE()
    {
        Vector2 pos = bigBossPos[UnityEngine.Random.Range(0, bigBossPos.Length)].position;
        bigBoss.transform.position = pos;
        bigBoss.SetActive(true);

        int hp = 20 + (TotalWave * 10);
        _bIGBlockController.SetParameter(hp);

        _gameManager.SetSpecial(SpexialWAVEs.BIGBOSS);
        _gameManager.SetBOSSHP("BIGBOSS", hp);
    }
}