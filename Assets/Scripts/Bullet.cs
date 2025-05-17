using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private Rigidbody bulletRB;
    [SerializeField]  BulletPool bulletPool;

    // Start is called before the first frame update
    void Start()
    {
        bulletPool = BulletPool.Instance; // CHATGPT instancio el Singelton
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable()
    {
        bulletRB.velocity = transform.forward * bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("se devuelve bullet");

        bulletPool.ReturnBullet(gameObject); // Pasar esta bala al BulletPool para ser reciclada
        
    }
}
