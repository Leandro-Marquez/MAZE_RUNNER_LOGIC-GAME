using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : MonoBehaviour
{
    public float speed = 150f;
    public float rotationSpeed = 50f;
    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        // Vector3 movementDirection = new Vector3(horizontalInput,0,verticalInput);
        // movementDirection.Normalize();
        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        transform.Translate(movement * speed * Time.deltaTime);
        
        // transform.position = transform.position + movementDirection * speed * Time.deltaTime;
        // if(movementDirection != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(movementDirection), rotationSpeed * Time.deltaTime);
        
    }
}
