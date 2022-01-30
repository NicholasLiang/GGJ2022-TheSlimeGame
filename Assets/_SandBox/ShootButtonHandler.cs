using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Platformer.Mechanics;

public class ShootButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerController pc;

    //Detect current clicks on the GameObject (the one with the script attached)
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        pc.touchShoot = true;
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        pc.touchShoot = false;
    }
}
