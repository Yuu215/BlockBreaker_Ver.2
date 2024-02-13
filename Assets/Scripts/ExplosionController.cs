using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private float time = 0.05f;
    private float nowTime;

    // Update is called once per frame
    void Update()
    {
        nowTime -= Time.deltaTime;

        if (nowTime < 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void ResetTime()
    {
        nowTime = time;
    }
}
