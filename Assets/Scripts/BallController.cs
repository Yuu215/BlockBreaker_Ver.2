using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    //<ボールを動かす処理>
    //変数
    private Rigidbody2D myRigidbody2D;
    private LineGenerator _lineGenerator;
    private PlayerStatusManager _playerStatusManager;

    private float ballSpeed = 3f;
    private float minSpeed = 4f;   //速さの最小値を指定する変数を追加
    private float maxSpeed = 7f;  //速さの最大値を指定する変数を追加

    //最低速度と最高速度の変更関数
    public void ChangeBallSpeed(float minSpeed,float maxSpeed)  
    {
        this.minSpeed = minSpeed;
        this.maxSpeed = maxSpeed;
    }

    void Start()
    {
        myRigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        StartSpeed();

        _lineGenerator = GameObject.Find("LineGenerator").GetComponent<LineGenerator>();
        _playerStatusManager = GameObject.Find("Manager").GetComponent<PlayerStatusManager>();
    }

    void Update()
    {
        //現在の速度を取得
        Vector2 velocity = myRigidbody2D.velocity;
        //速さを計算
        float clampedSpeed = Mathf.Clamp(velocity.magnitude, minSpeed, maxSpeed);
        //速度の変化
        myRigidbody2D.velocity = velocity.normalized * clampedSpeed;
    }

    //ボールに力を加え、移動をさせる
    public void StartSpeed()
    {
        myRigidbody2D.velocity = new Vector2(Random.Range(-ballSpeed, ballSpeed), Random.Range(-ballSpeed, ballSpeed));
    }

    //ボールとオブジェクトが接触した場合の処理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //パドルと接触時にパドルを消す
        if (collision.gameObject.tag == "Paddle")
            _lineGenerator.DeletePaddle();
    }

    //ボールが画面外に出た場合の処理
    private void OnBecameInvisible()
    {
        Debug.Log("ボールが画面外に出た！");
        this.gameObject.SetActive(false);
        _playerStatusManager.BallMiss();
    }
}
