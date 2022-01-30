using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CreateRoomUI : MonoBehaviour
{
    public Networking networking;
    public GameObject inputField;

    public GameObject thisPanel;
    public GameObject nextPanel;

    public void CreateRoom()
    {
        string name = inputField.GetComponent<TMP_InputField>().text;

        networking.CreateRoom(name);

        thisPanel.SetActive(false);
        nextPanel.SetActive(true);
    } 
}
