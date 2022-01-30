using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectToServerUI : MonoBehaviour
{
    public Networking networking;
    public GameObject inputField;

    public GameObject thisPanel;
    public GameObject nextPanel;
    public GameObject ErrorMessage;

    void Update()
    {
        if (networking.serverState == ServerState.connected)
        {
            thisPanel.SetActive(false);
            nextPanel.SetActive(true);
        }
        else if (networking.serverState == ServerState.error)
        {
            ErrorMessage.SetActive(true);
        }
    }

    public void connect()
    {
        string url = inputField.GetComponent<InputField>().text;

        networking.StartConnection(url);
    }

}
