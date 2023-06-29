using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float souls;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            moveToStore();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            moveOutStore();
        }
    }

    private void moveToStore()
    {
        transform.position = new Vector3(-49, 1, 11);
    }

    private void moveOutStore()
    {
        transform.position = new Vector3(-44, 1, 11);
    }
}
