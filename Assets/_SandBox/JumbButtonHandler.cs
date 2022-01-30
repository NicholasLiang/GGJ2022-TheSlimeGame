using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Platformer.Mechanics;

public class JumbButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerController pc;

    //Detect current clicks on the GameObject (the one with the script attached)
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        pc.isBouncing = true;
    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        pc.isBouncing = false;
    }
}
