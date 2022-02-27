using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] private int toothArrayIndex = 0;

    [SerializeField] private GameObject collectorGameObject;
    [SerializeField] private Collector collector;
    [SerializeField] private GameObject particle;
    // Start is called before the first frame update
    void Start()
    {
        collectorGameObject = GameObject.Find("Collector");
        collector = collectorGameObject.GetComponent<Collector>();
        particle = Resources.Load("badParticle") as GameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("donut"))
        { 
            Instantiate(particle, gameObject.transform.position, Quaternion.identity);
           Destroy(gameObject);
           collector.currentTeethCount--;
        }
    }
    
}
