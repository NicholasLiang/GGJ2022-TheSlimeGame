using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class NetworkingFollowMainPlayer : MonoBehaviour
{
    public void Follow()
    {
        GameObject mainPlayer = GameObject.FindWithTag("MainPlayer");
        GetComponent<CinemachineVirtualCamera>().m_LookAt = mainPlayer.transform;
        GetComponent<CinemachineVirtualCamera>().m_Follow = mainPlayer.transform;
    }

    public void Follow(GameObject go)
    {
        GetComponent<CinemachineVirtualCamera>().m_LookAt = go.transform;
        GetComponent<CinemachineVirtualCamera>().m_Follow = go.transform;
    }

    public void Follow(Rigidbody2D go)
    {
        GetComponent<CinemachineVirtualCamera>().m_LookAt = go.transform;
        GetComponent<CinemachineVirtualCamera>().m_Follow = go.transform;
    }
}
