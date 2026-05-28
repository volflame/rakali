using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    private CharacterController controller;
    // private Transform playerTransform;
    private Vector3 velocity;
    public bool isLocked = false;
    public static PlayerMovement instance;
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        // playerTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocked) return;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = Camera.main.transform.right * x + Camera.main.transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void LockMovement() => isLocked = true;
    public void UnlockMovement() => isLocked = false;
}
