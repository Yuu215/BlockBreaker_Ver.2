using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaertController : MonoBehaviour
{
    //�ϐ�
    [SerializeField] private float rotateSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        this.gameObject.transform.Rotate(new Vector3(0, rotateSpeed, 0) * Time.deltaTime);
    }
}
