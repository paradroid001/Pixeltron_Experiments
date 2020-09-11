using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    float speed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        transform.Rotate(my, mx, 0.0f);
        transform.position += (v * transform.forward + h * transform.right).normalized * speed * Time.deltaTime; 

    }
}
