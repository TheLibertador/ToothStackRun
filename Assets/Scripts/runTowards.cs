using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class runTowards : MonoBehaviour
{
    private void Update()
    {
        transform.position += Vector3.back * 1.4f * Time.fixedDeltaTime;
    }
    
    
}

