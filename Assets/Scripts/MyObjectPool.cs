using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObjectPool : MonoBehaviour
{
    //<オブジェクトプール>
    [SerializeField] GameObject enemyTextPrefabObj;
    [SerializeField] GameObject blockPrefabObj;
    [SerializeField] GameObject itemPrefabObj;
    //ボール
    [SerializeField] private GameObject ballPrefab;
    

    //リスト
    List<GameObject> enemyTextPool;
    List<GameObject> blockPool;
    List<GameObject> itemPool;
    List<GameObject> ballPool;

    //親オブジェクト
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
            //オブジェクト生成
            GameObject BallObj = Instantiate(ballPrefab);
            BallObj.SetActive(false);
            ballPool.Add(BallObj);
        }
    }

    public GameObject GetBallObj(Vector2 pos)
    {
        //非表示のオブジェクトを探す
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

        //プールのものを全部使ってたら新たに生成
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
            //オブジェクト生成
            GameObject EnemyTextObj = Instantiate(enemyTextPrefabObj,Canvas.transform);
            EnemyTextObj.SetActive(false);
            enemyTextPool.Add(EnemyTextObj);
        }
    }

    //使うときに場所を指定して表示する : プールの中から非表示のオブジェククトを探してくる
    public GameObject GetEnemyTextObj()
    {
        //非表示のオブジェクトを探す
        for (byte i = 0; i < enemyTextPool.Count; i++)
        {
            if (enemyTextPool[i].activeSelf == false)
            {
                GameObject EnemyTextObj = enemyTextPool[i];
                EnemyTextObj.SetActive(true);
                return EnemyTextObj;
            }
        }

        //プールのものを全部使ってたら新たに生成
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
            //オブジェクト生成
            GameObject blockObj = Instantiate(blockPrefabObj,createPosition.transform);
            blockObj.SetActive(false);
            blockPool.Add(blockObj);
        }
    }

    public GameObject GetBlockObj(Vector2 position)
    {
        //非表示のオブジェクトを探す
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

        //プールのものを全部使ってたら新たに生成
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
