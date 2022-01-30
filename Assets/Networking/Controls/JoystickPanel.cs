using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Platformer.Mechanics;

public class JoystickPanel : MonoBehaviour
{
    PlayerController pc;

    public float speedInverse;
    private float width;
    private float height;

    private int pointerId;
    private bool isSelected;
    private Vector2 position;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    void Awake()
    {
        m_Raycaster = GameObject.FindObjectOfType<GraphicRaycaster>();
        m_EventSystem = GameObject.FindObjectOfType<EventSystem>();

        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;

        isSelected = false;
        pointerId = -1;
        position = new Vector2(0, 0);


    }

    void OnGUI()
    {
        // Compute a fontSize based on the size of the screen width.
        GUI.skin.label.fontSize = (int)(Screen.width / 50.0f);
        GUI.color = Color.red;

        GUI.Label(new Rect(20, 70, width, height * 0.25f),
            "x = " + position.x.ToString("f2") +
            ", y = " + position.y.ToString("f2") +
            ", isTouched = " + isSelected.ToString() +
            ", ID = " + pointerId.ToString());
    }


    // Update is called once per frame
    void Update()
    {
        if (pc == null)
        {
            pc = GameObject.FindWithTag("MainPlayer").GetComponent<PlayerController>();
        } else {
            // Handle screen touches.
            if (Input.touchCount > 0)
            {
                if (isSelected)
                {
                    Touch touch = Input.GetTouch(pointerId);

                    if (touch.phase == TouchPhase.Moved)
                    {
                        Vector2 pos = touch.position;
                        float speed = pos.x - position.x;

                        Debug.Log(pos.x - position.x);

                        if (speed > 50)
                        {
                            speed = 50;
                        }

                        if (speed < -50)
                        {
                            speed = -50;
                        }

                        pc.move = new Vector2((speed) / speedInverse, 0);
                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        isSelected = false;
                        pointerId = -1;
                        position = new Vector2(0, 0);
                        pc.move = new Vector2(0, 0);
                    }
                }
                else
                {
                    for (var i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);

                        //Set up the new Pointer Event
                        m_PointerEventData = new PointerEventData(m_EventSystem);
                        //Set the Pointer Event Position to that of the mouse position
                        m_PointerEventData.position = touch.position;

                        //Create a list of Raycast Results
                        List<RaycastResult> results = new List<RaycastResult>();

                        //Raycast using the Graphics Raycaster and mouse click position
                        m_Raycaster.Raycast(m_PointerEventData, results);

                        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
                        foreach (RaycastResult result in results)
                        {
                            Debug.Log("Hit " + result.gameObject.name);
                            Debug.Log(touch.fingerId);

                            if (result.gameObject.tag == "JoystickPanel")
                            {
                                isSelected = true;
                                pointerId = i;
                                position = touch.position;
                            }
                        }
                    }
                }
            }
        }
    }
}
