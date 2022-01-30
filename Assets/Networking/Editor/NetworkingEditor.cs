using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(Networking))]
public class NetworkingEditor : Editor
{
    string roomName;
    string roomId;

    public override void OnInspectorGUI ()
    {
        Networking myTarget = (Networking)target;
        if (GUILayout.Button("StartConnection"))
        {
            myTarget.StartConnection(myTarget.server_url);
        }
        if (GUILayout.Button("StopConnection"))
        {
            myTarget.StopConnection();
        }
        roomName = EditorGUILayout.TextField("Room Name: ", roomName);
        if (GUILayout.Button("Create Room"))
        {
            myTarget.CreateRoom(roomName);
        }
        roomId = EditorGUILayout.TextField("Room ID to join: ", roomId);
        if (GUILayout.Button("Join Room"))
        {
            myTarget.JoinRoom(roomId);
        }
        if (GUILayout.Button("GetListOfRooms"))
        {
            myTarget.GetListOfRooms();
        }
        if (GUILayout.Button("Start"))
        {
            myTarget.StartRoom();
        }

        // Draw default inspector after button...
        base.OnInspectorGUI();
    }
}
