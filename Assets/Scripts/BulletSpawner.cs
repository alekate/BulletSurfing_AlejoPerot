using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private float layerOffset = 0.5f;
    public Rigidbody attachedObj;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = attachedObj.position;

        transform.rotation = Quaternion.LookRotation(attachedObj.velocity);

        if (Input.GetButtonDown("Fire1"))
        {
            GameObject bullet = BulletPool.Instance.RequestBullet();

            if (bullet != null)
            {
                bullet.transform.position = transform.position + transform.forward * layerOffset;
            }
        }
    }
}
