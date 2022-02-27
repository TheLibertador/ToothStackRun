using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveCamDoTween : MonoBehaviour
{
    [SerializeField] Transform camTransform;
    [SerializeField] Transform collected;
    [SerializeField] private GameObject collectedThoot;

    private void Start()
    {
       
        StartCoroutine(ShowTongue());
    }
    private void OnTriggerEnter(Collider other)
    {
        int i = 0;
        if (other.gameObject.CompareTag("camTrigger"))
        {
            StopCoroutine(ShowTongue());
            Debug.Log("Çarpıştı");
            camTransform.DOLocalMove(new Vector3(1.2f, 11.58f, -11.13f), 1);
            collected.DOScale(0.08f, 0);
            collected.localPosition = new Vector3(0.53f, -9.494f, -13.936f);
            //collected.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            
            //collected.DOLocalMove(new Vector3(-1.25f, -9.38f, 0), 0);
            
            camTransform.DORotate(new Vector3(1.72f, 0, 0),1);
        }
    }

    private IEnumerator ShowTongue()
    {
        yield return new WaitForSeconds(1);
        camTransform.DOLocalMove(new Vector3(1.2f, 12.17f, 9.9f), 0.5f);
        camTransform.DORotate(new Vector3(22.04f, 0, 0), 0.5f);
        collectedThoot.GetComponent<Movement>().enabled = true;
    }
}
