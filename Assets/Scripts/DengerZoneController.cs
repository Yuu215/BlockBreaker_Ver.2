using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DengerZoneController : MonoBehaviour
{
    //ïœêî
    [SerializeField] private float rotateSpeed;

    void Start()
    {
        
    }

    
    void Update()
    {
        float rotation = rotateSpeed * Time.deltaTime;
        this.gameObject.transform.Rotate(new Vector3(0, 0, rotation));
    }
}
