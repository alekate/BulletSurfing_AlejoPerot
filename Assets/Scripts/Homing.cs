using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing : MonoBehaviour
{

    public Transform target;
    public float speed = 5f;
    public float rotateSpeed = 0.3f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        while (Vector3.Distance(target.transform.position, rb.transform.position) > 0.3f)
        {
            rb.transform.position += (target.transform.position - rb.transform.position).normalized * speed * Time.deltaTime;
            rb.transform.LookAt(target.transform.position);
            return;
        }
    }
}
