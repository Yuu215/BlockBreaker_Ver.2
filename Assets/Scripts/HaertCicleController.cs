using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaertCicleController : MonoBehaviour
{
    //ïœêî
    [SerializeField] private float rotateSpeed;

    void Start()
    {
        
    }

    
    void Update()
    {
        this.gameObject.transform.Rotate(new Vector3(0, 0, rotateSpeed) * Time.deltaTime);
    }
}
