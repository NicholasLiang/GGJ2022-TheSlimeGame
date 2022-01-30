using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Platformer.Mechanics;

public class shootBullet : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerController pc;

    void Update()
    {
        if (pc == null)
        {
            pc = GameObject.FindWithTag("MainPlayer").GetComponent<PlayerController>();
        }
    }

    //Detect current clicks on the GameObject (the one with the script attached)
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (pc == null)
            return;

        pc.touchShoot = true;
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (pc == null)
            return;
            
        pc.touchShoot = false;
    }
}