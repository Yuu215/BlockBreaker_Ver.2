using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObjectPool : MonoBehaviour
{
    //<�I�u�W�F�N�g�v�[��>
    [SerializeField] GameObject enemyTextPrefabObj;
    [SerializeField] GameObject blockPrefabObj;
    [SerializeField] GameObject itemPrefabObj;
    //�{�[��
    [SerializeField] private GameObject ballPrefab;
    

    //���X�g
    List<GameObject> enemyTextPool;
    List<GameObject> blockPool;
    List<GameObject> itemPool;
    List<GameObject> ballPool;

    //�e�I�u�W�F�N�g
    [SerializeField] GameObject Canvas;

    [SerializeField] GameObject createPosition;

    private void Start()
    {
        CreateEnemyTextPool(10);
        CreateBlockPool(10);
        CreateItemPool(10);
        CreateBallPool(10);
    }

    public void CreateBallPool(int maxCount)
    {
        ballPool = new List<GameObject>();
        for (int i = 0; i < maxCount; i++)
        {
            //�I�u�W�F�N�g����
            GameObject BallObj = Instantiate(ballPrefab);
            BallObj.SetActive(false);
            ballPool.Add(BallObj);
        }
    }

    public GameObject GetBallObj(Vector2 pos)
    {
        //��\���̃I�u�W�F�N�g��T��
        for (byte i = 0; i < ballPool.Count; i++)
        {
            if (ballPool[i].activeSelf == false)
            {
                GameObject BallObj = ballPool[i];
                BallObj.transform.position = pos;
                BallObj.SetActive(true);
                return BallObj;
            }
        }

        //�v�[���̂��̂�S���g���Ă���V���ɐ���
        GameObject newBallObj = Instantiate(ballPrefab, pos, Quaternion.identity);
        newBallObj.SetActive(false);
        enemyTextPool.Add(newBallObj);
        return newBallObj;
    }

    public void CreateEnemyTextPool(int maxCount)
    {
        enemyTextPool = new List<GameObject>();
        for (int i = 0; i < maxCount; i++)
        {
            //�I�u�W�F�N�g����
            GameObject EnemyTextObj = Instantiate(enemyTextPrefabObj,Canvas.transform);
            EnemyTextObj.SetActive(false);
            enemyTextPool.Add(EnemyTextObj);
        }
    }

    //�g���Ƃ��ɏꏊ���w�肵�ĕ\������ : �v�[���̒������\���̃I�u�W�F�N�N�g��T���Ă���
    public GameObject GetEnemyTextObj()
    {
        //��\���̃I�u�W�F�N�g��T��
        for (byte i = 0; i < enemyTextPool.Count; i++)
        {
            if (enemyTextPool[i].activeSelf == false)
            {
                GameObject EnemyTextObj = enemyTextPool[i];
                EnemyTextObj.SetActive(true);
                return EnemyTextObj;
            }
        }

        //�v�[���̂��̂�S���g���Ă���V���ɐ���
        GameObject newEnemyTextObj = Instantiate(enemyTextPrefabObj, Canvas.transform);
        newEnemyTextObj.SetActive(false);
        enemyTextPool.Add(newEnemyTextObj);
        return newEnemyTextObj;
    }

    public void CreateBlockPool(int maxCount)
    {
        blockPool = new List<GameObject>();
        for (byte i = 0; i < maxCount; i++)
        {
            //�I�u�W�F�N�g����
            GameObject blockObj = Instantiate(blockPrefabObj,createPosition.transform);
            blockObj.SetActive(false);
            blockPool.Add(blockObj);
        }
    }

    public GameObject GetBlockObj(Vector2 position)
    {
        //��\���̃I�u�W�F�N�g��T��
        for (byte i = 0; i < blockPool.Count; i++)
        {
            if (blockPool[i].activeSelf == false)
            {
                GameObject blockObj = blockPool[i];
                blockObj.transform.position = position;
                blockObj.SetActive(true);
                return blockObj;
            }
        }

        //�v�[���̂��̂�S���g���Ă���V���ɐ���
        GameObject newBlockObj = Instantiate(blockPrefabObj, position, Quaternion.identity);
        newBlockObj.SetActive(false);
        blockPool.Add(newBlockObj);
        return newBlockObj;
    }

    public void CreateItemPool(int maxCount)
    {
        itemPool = new List<GameObject>();
        for (byte i = 0; i < maxCount; i++)
        {
            GameObject itemObj = Instantiate(itemPrefabObj);
            itemObj.SetActive(false);
            itemPool.Add(itemObj);
        }
    }

    public GameObject GetItemObj(Vector2 position)
    {
        for (byte i = 0; i < itemPool.Count; i++)
        {
            if(itemPool[i].activeSelf == false)
            {
                GameObject itemObj = itemPool[i];
                itemObj.transform.position = position;
                itemObj.SetActive(true);
                return itemObj;
            }
        }

        GameObject newItemObj = Instantiate(itemPrefabObj, position, Quaternion.identity);
        newItemObj.SetActive(false);
        itemPool.Add(newItemObj);
        return newItemObj;
    }

    
}
