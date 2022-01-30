using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayRooms : MonoBehaviour
{
    public Networking networking;
    public float updatePerSecond;
    private float timeRemaining;

    public GameObject thisPanel;
    public GameObject nextPanel;

    public List<GameObject> rooms;

    public GameObject roomPrefab;

    void Start()
    {
        timeRemaining = 2;
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        } else {

            // do you things
            networking.GetListOfRooms();
            UpdateRooms();

            timeRemaining = updatePerSecond;
        }
    }

    public void UpdateRooms()
    {
        foreach(var room in rooms)
        {
            Destroy(room);
        }
        rooms.Clear();

        for(var i = 0; i < networking.rooms.Count - 1; i++)
        {
            GameObject newRoom = Instantiate(roomPrefab, this.transform) as GameObject;
            rooms.Add(newRoom);

            newRoom.GetComponent<Button>().onClick.AddListener(() => {
                networking.JoinRoom((i-1).ToString());
                thisPanel.SetActive(false);
                nextPanel.SetActive(true);
            });
            newRoom.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = networking.rooms[i];
        }
        
    }

}
