using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CreateRoomUI : MonoBehaviour
{
    public Networking networking;
    public GameObject inputField;

    public GameObject thisPanel;
    public GameObject nextPanel;

    public void CreateRoom()
    {
        string name = inputField.GetComponent<InputField>().text;

        networking.CreateRoom(name);

        thisPanel.SetActive(false);
        nextPanel.SetActive(true);
    } 
}
