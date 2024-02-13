using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGaugeController : MonoBehaviour
{
    //<アイテムゲージ>
    [SerializeField] float timeLimit;
    //慶賀時間を保持する変数
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
        //経過時間を取得
        seconds += Time.deltaTime;
        //経過時間を制限時間で割る
        float timer = seconds / timeLimit;
    }
}
