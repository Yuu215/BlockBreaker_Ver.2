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

    
    public string waveName;         //�E�G�[�u�̖��O
   
    public Transform[] spawnPoints; //�X�|�[���ʒu
    public SpawnType spawnType;     //�u���b�N�̃^�C�v
    public byte blockEXP;           //�u���b�N�̌o���l
}

public class WaveManager : MonoBehaviour
{
    public Wave[] waves;
    [SerializeField] private MyObjectPool _myObjectPool;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Transform StartPos;

    private Wave currentWave;       //���݂̃E�F�[�u
    private int currentWaveNumber;  //���݂̃E�F�[�u�̃i���o�[
    private float nextSpawnTime;    //���ɃG�l�~�[���X�|�[�������Ԋu

    private int TotalWave = 1;      //�E�F�[�u�̍��v

    private bool canSpawn = true;   //�������Ă������ǂ���
    private bool gameend = false;   //�Q�[�����I�����Ă��邩�ǂ���

    private enum MAXLevel
    {
        MAXFnemies = 500,
    }

    private float menspawn = 0.1f;
    private float spawnInterval = 5.6f;         //���̃u���b�N�̃X�|�[�����o
    private int originalNoOfFnemies = 10;
    private int noOfFnemies = 10;               //�E�F�[�u�ŏo�Ă���G�l�~�[�̐�

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
                    break;  // 1�ł��A�N�e�B�u�ȓG��������΃��[�v�𔲂���
                }
            }

            if (allEnemiesInactive && !canSpawn && !gameend)
            {
                SpawnNextWave();  //���̃E�F�[�u��
            }
        }
    }

    void SpawnNextWave()  //���̃E�F�[�u��
    {
        //�E�F�[�u��m�点��A�j���[�V����

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

        //����WAVE�̏���
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

            //�G�l�~�[�̏��
            Vector2 randomPoint = currentWave.spawnPoints[UnityEngine.Random.Range(0, currentWave.spawnPoints.Length)].position;       //�G�l�~�[�̈ʒu��ݒ肵���|�W�V�������烉���_���Ɍ��߂�
            GameObject block = _myObjectPool.GetBlockObj(randomPoint);                                                     //�I�u�W�F�N�g�v�[���Ɉʒu�A�X�|�[���^�C�v��n���Đ���
            BlockController _blockController = block.GetComponent<BlockController>();
            _blockController.SetParameter(HP, currentWave.blockEXP, currentWave.spawnType);
            _blockController.BlockTypeTable();

            //���݂̃E�F�[�u����
            noOfFnemies--;�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�c��̐�������I�u�W�F�N�g�̐����X�V
            nextSpawnTime = Time.time + spawnInterval;�@�@//���̃u���b�N�����܂ł̊Ԋu���X�V

            if (noOfFnemies == 0)                         //�E�F�[�u�̃u���b�N���������Ȃ�����I��
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

        //�c���Ă���I�u�W�F�N�g���폜����
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

    //����WAVE�̊֐�
    [SerializeField] private GameObject bigBlock;
    [SerializeField] private Transform[] bigBossPos;
    private GameObject bigBoss;
    private BIGBlockController _bIGBlockController;

    void Start()
    {
        //BIGBOSS�̏�����
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