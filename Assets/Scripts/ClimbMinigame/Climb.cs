using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour
{
    [SerializeField] private float speed;

    private bool win;


    void Awake()
    {
        win = false;
    }

    void Update()
    {
        if (!win)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Estoy subiendo");
                transform.Translate(transform.up * speed * Time.deltaTime, Space.World);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!win)
        {
            Win();
        }
    }

    public bool Win()
    {
        Debug.Log("He ganado");
        win = true;
        return win;
    }
}
