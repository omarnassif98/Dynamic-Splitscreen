using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private string upKey, downKey, leftKey, rightKey;
    [SerializeField] private float speed;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(upKey))
            transform.position += Vector3.forward * speed * Time.deltaTime;
        if(Input.GetKey(leftKey))
            transform.position += Vector3.left * speed * Time.deltaTime;
        if(Input.GetKey(rightKey))
            transform.position += Vector3.right * speed * Time.deltaTime;
        if(Input.GetKey(downKey))
            transform.position += Vector3.back * speed * Time.deltaTime;
    }
}
