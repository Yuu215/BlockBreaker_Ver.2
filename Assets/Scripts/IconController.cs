using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconController : MonoBehaviour
{
    [SerializeField] private float rotate;

    void Update()
    {
        //回転
        this.gameObject.transform.Rotate(new Vector3(0, 0, rotate) * Time.deltaTime);
    }

    private void OnEnable()
    {
        this.gameObject.transform.rotation = Quaternion.identity; // 回転を初期化
    }
}
