using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGaugeController : MonoBehaviour
{
    //<�A�C�e���Q�[�W>
    [SerializeField] float timeLimit;
    //�c�ꎞ�Ԃ�ێ�����ϐ�
    float seconds = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        _updateTimer();
    }

    void _updateTimer()
    {
        //�o�ߎ��Ԃ��擾
        seconds += Time.deltaTime;
        //�o�ߎ��Ԃ𐧌����ԂŊ���
        float timer = seconds / timeLimit;
    }
}
