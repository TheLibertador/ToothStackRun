using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public PlayerState playerState;
    public LevelState levelState;
    
    public enum PlayerState
    {
        Stop,
        Move
    }
    public enum LevelState
    {
        NotFinished,
        Finished
    }
    
    
    //Touch Settings
    [SerializeField] bool isTouching;
    float touchPosX;
    Vector3 direction;
    [SerializeField, ReadOnly] private float movementSpeed;
    [SerializeField, ReadOnly] private float controlSpeed;
  
    void Start()
    {
        playerState = PlayerState.Move;
        levelState = LevelState.NotFinished;
    }

    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {

        if (playerState == PlayerState.Move)
        {
            transform.position += Vector3.forward * movementSpeed * Time.fixedDeltaTime;
        }
        if (isTouching)
        {
            touchPosX += Input.GetAxis("Mouse X") * controlSpeed * Time.fixedDeltaTime;
        }

        transform.position = new Vector3(touchPosX, transform.position.y, transform.position.z);
    }

    void GetInput()
    {
        if (Input.GetMouseButton(0))
        {
            isTouching = true;
        }
        else
        {
            isTouching = false;
        }
    }
}