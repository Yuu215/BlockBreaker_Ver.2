using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    //<�{�[���𓮂�������>
    //�ϐ�
    private Rigidbody2D myRigidbody2D;
    private LineGenerator _lineGenerator;
    private PlayerStatusManager _playerStatusManager;

    private float ballSpeed = 3f;
    private float minSpeed = 4f;   //�����̍ŏ��l���w�肷��ϐ���ǉ�
    private float maxSpeed = 7f;  //�����̍ő�l���w�肷��ϐ���ǉ�

    //�Œᑬ�x�ƍō����x�̕ύX�֐�
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
        //���݂̑��x���擾
        Vector2 velocity = myRigidbody2D.velocity;
        //�������v�Z
        float clampedSpeed = Mathf.Clamp(velocity.magnitude, minSpeed, maxSpeed);
        //���x�̕ω�
        myRigidbody2D.velocity = velocity.normalized * clampedSpeed;
    }

    //�{�[���ɗ͂������A�ړ���������
    public void StartSpeed()
    {
        myRigidbody2D.velocity = new Vector2(Random.Range(-ballSpeed, ballSpeed), Random.Range(-ballSpeed, ballSpeed));
    }

    //�{�[���ƃI�u�W�F�N�g���ڐG�����ꍇ�̏���
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //�p�h���ƐڐG���Ƀp�h��������
        if (collision.gameObject.tag == "Paddle")
            _lineGenerator.DeletePaddle();
    }

    //�{�[������ʊO�ɏo���ꍇ�̏���
    private void OnBecameInvisible()
    {
        Debug.Log("�{�[������ʊO�ɏo���I");
        this.gameObject.SetActive(false);
        _playerStatusManager.BallMiss();
    }
}
