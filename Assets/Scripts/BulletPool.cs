using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private List<GameObject> bulletList = new List<GameObject>(); // Inicializar la lista. En vez de listas se puede usar Queue y Stack

    // Singleton
    private static BulletPool instance;
    public static BulletPool Instance { get { return instance; } }

    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AddBulletsToPool(poolSize); // poolSize se pasa como "amount"
    }

    private void AddBulletsToPool(int amount) // amount porque cuenta las instancias agregadas a la lista
    {
        // Instanciar los prefabs de balas y añadirlos a la pool
        for (int i = 0; i < amount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletList.Add(bullet);
            bullet.transform.parent = transform;
        }
    }

    public GameObject RequestBullet()
    {
        for (int i = 0; i < bulletList.Count; i++) 
        {
            if (!bulletList[i].activeSelf) // Verifica si el elemento en la lista NO está activo (usa !)
            {
                bulletList[i].SetActive(true); // Lo activa
                return bulletList[i]; // Lo devuelve
            }
        }
        return null; // Mover la declaración return fuera del bucle
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        Debug.Log("returned");
    }
}
