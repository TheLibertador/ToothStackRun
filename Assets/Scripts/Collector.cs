using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using Tabtale.TTPlugins;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Collector : MonoBehaviour
{
    [SerializeField] private Transform collectedTooth;
    [SerializeField] private Transform collected;
    [SerializeField] private Transform camera;
    
    public  GameObject[] locationArray = new GameObject[15];
    public  GameObject[] teethArray = new GameObject[15];
    public  GameObject[] damakdakiDislerArray = new GameObject[15];
    public int currentTeethCount = 1;

    [SerializeField] GameObject teethPrefab;
    [SerializeField] GameObject locationPrefab;
    [SerializeField] GameObject dislerr;
    [SerializeField] GameObject failCanvas;
    [SerializeField] GameObject winCanvas;
   
    [SerializeField] Material whiteTeethMaterial;

    [SerializeField] private RuntimeAnimatorController walk2;
    [SerializeField] private RuntimeAnimatorController walk3;

    [SerializeField] private GameObject goodParticle;
    [SerializeField] private GameObject badParticle;
    
    public Slider slider;
    public Image fill;
    
    private bool isZoomed = true;
    public int maxTeethCount = 1;
    void Start()
    {
       // updateSlider();
    }

    void Update()
    {

    }
    private void Awake()
    {
        // Initialize CLIK Plugin
        TTPCore.Setup();
        // Your code here
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Tooth"))
        {
            Destroy(other.gameObject);
            if (currentTeethCount +1 != 32)
            {
                AddTeeth(currentTeethCount +1);
            }
            else
            {
                AddTeeth(32-currentTeethCount);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("damakTrigger"))
        {
            collectedTooth.gameObject.GetComponent<Movement>().enabled = false;
            StartCoroutine(Jump());
        }
        if (other.gameObject.CompareTag("X2"))
        {
            if (currentTeethCount * 2+1 != 32)
            {
                AddTeeth(currentTeethCount*2+1);
            }
            else
            {
                AddTeeth(32-currentTeethCount);               
            }
            Destroy(other.gameObject);
            //ya destroy ya collider kapat
        }
        else if (other.gameObject.CompareTag("+10"))
        {
            if (currentTeethCount +10 != 32)
            {
                AddTeeth(currentTeethCount +10);
            }
            else
            {
                AddTeeth(32-currentTeethCount);
            }
            Destroy(other.gameObject);
            //ya destroy ya collider kapat
        }
        else if (other.gameObject.CompareTag("-3"))
        {
            if (currentTeethCount >= 3)
            {
                removeTeeth(3);
            }
            else
            {
                removeTeeth(currentTeethCount);
            }
            Destroy(other.gameObject);
            //ya destroy ya collider kapat
        }
        else if (other.gameObject.CompareTag("-5"))
        {
            if (currentTeethCount >= 5)
            {
                removeTeeth(5);
            }
            else
            {
                removeTeeth(currentTeethCount);
            }
            Destroy(other.gameObject);
            //ya destroy ya collider kapat
        }
        else if (other.gameObject.CompareTag("-20"))
        {
            if (currentTeethCount >= 5)
            {
                removeTeeth(20);
            }
            else
            {
                removeTeeth(currentTeethCount);
            }
            Destroy(other.gameObject);
            //ya destroy ya collider kapat
        }
        else if (other.gameObject.CompareTag("X3"))
        {
            if (currentTeethCount * 3+1 != 32)
            {
                AddTeeth(currentTeethCount*2+1);
            }
            else
            {
                AddTeeth(32-currentTeethCount);               
            }
            Destroy(other.gameObject);
            //ya destroy ya collider kapat
        }

    }

    /*
    private void updateSlider()
    {
        if (slider.value == slider.maxValue)
        {
            collectedTooth.GetComponent<Movement>().enabled = false;
            failCanvas.SetActive(true);
        }
    }
*/
    IEnumerator Jump()
    {
        int i = 0;
        while (maxTeethCount > i)
        {
            Debug.Log(i);
            if (teethArray[i] != null)
            {
                teethArray[i].transform.parent = dislerr.transform;
                teethArray[i].GetComponent<Animator>().enabled = false;
                teethArray[i].transform.DOMove(damakdakiDislerArray[i].transform.position, 0.3f);
                teethArray[i].transform.DOScale(0, 0.2f);
                yield return new WaitForSeconds(0.2f);
                damakdakiDislerArray[i].SetActive(true);
                i++;
                Destroy(teethArray[i]);
                currentTeethCount--;

                //yield return new WaitForSeconds(0.4f);
            }
            i++;
            
        }

        yield return new WaitForSeconds(0.15f);
        winCanvas.SetActive(true);
    }

    private void AddTeeth(int newCount)
    {
        int randomNumber;
        if (currentTeethCount < maxTeethCount)
        {
            int index = 0;
            while (index < maxTeethCount && currentTeethCount < newCount && currentTeethCount + 1 < 32)
            {
                if (teethArray[index] == null)
                {
                    randomNumber = Random.Range(1, 4);
                    currentTeethCount++;
                    Debug.Log(currentTeethCount);
                    teethArray[index] = Instantiate(teethPrefab, locationArray[index].transform.position,
                        Quaternion.identity);
                    teethArray[index].transform.parent = collected;
                    teethArray[index].tag = "CollectedTeeth";
                    teethArray[index].gameObject.GetComponent<Animator>().updateMode =
                        AnimatorUpdateMode.UnscaledTime;
                    if (randomNumber == 2)
                    {
                        teethArray[index].GetComponent<Animator>().runtimeAnimatorController =
                            walk2 as RuntimeAnimatorController;
                    }

                    if (randomNumber == 3)
                    {
                        teethArray[index].GetComponent<Animator>().runtimeAnimatorController =
                            walk3 as RuntimeAnimatorController;
                    }
                }

                index++;
            }

        }
        else {

            while (currentTeethCount < newCount && currentTeethCount + 1 < 32)
            {
                randomNumber = Random.Range(1, 4);
                currentTeethCount++;
                Debug.Log(currentTeethCount);
                teethArray[currentTeethCount] = Instantiate(teethPrefab, locationArray[currentTeethCount].transform.position,
                    Quaternion.identity);
                teethArray[currentTeethCount].transform.parent = collected;
                teethArray[currentTeethCount].tag = "CollectedTeeth";
                teethArray[currentTeethCount].gameObject.GetComponent<Animator>().updateMode =
                    AnimatorUpdateMode.UnscaledTime;
                if (randomNumber == 2)
                {
                    teethArray[currentTeethCount].GetComponent<Animator>().runtimeAnimatorController =
                        walk2 as RuntimeAnimatorController;
                }

                if (randomNumber == 3)
                {
                    teethArray[currentTeethCount].GetComponent<Animator>().runtimeAnimatorController =
                        walk3 as RuntimeAnimatorController;
                }

            }
        }

        if (currentTeethCount > maxTeethCount)
            maxTeethCount = currentTeethCount;
        /*
        if (currentTeethCount > 15 && isZoomed == true)
        {
            isZoomed = false;
            camera.GetComponent<Camera>().fieldOfView = 70;
            //collectedTooth.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
        else if (currentTeethCount < 15 && isZoomed == false)
        {
            isZoomed = true;
            camera.GetComponent<Camera>().fieldOfView = 60;
            //collectedTooth.transform.localScale = new Vector3(1f, 1f, 1f); 
        }
        */
    }

    private void removeTeeth(int removeCount)
    {
        if (currentTeethCount - removeCount >= 1)
        {
            int index = teethArray.Length -1;
            int removedCount = 0; 
            while (removedCount <= removeCount)
            {
                if (teethArray[index] != null)
                {
                    Destroy(teethArray[index]);
                    currentTeethCount--;
                    removedCount++;
                }
                index--;
            }
            
            
            
        }
        else
        {
            Debug.Log("game ended");
            collectedTooth.GetComponent<Movement>().enabled = false;
            failCanvas.SetActive(true);
        }
    }
}

