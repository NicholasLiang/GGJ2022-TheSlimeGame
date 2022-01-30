using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public Networking networking;

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("fall in trap");
        networking.Dead();
    }
}
