﻿using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam;
    public CinemachineFreeLook lockedCam;
    public Transform listener;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float y_input = Input.GetAxis("Mouse Y") + Input.GetAxis("Right Stick Y");
        float x_input = Input.GetAxis("Mouse X") + Input.GetAxis("Right Stick X");

        freeLookCam.m_YAxis.m_InputAxisValue = y_input;
        freeLookCam.m_XAxis.m_InputAxisValue = x_input;

        lockedCam.m_YAxis.m_InputAxisValue = y_input;

        listener.position = freeLookCam.gameObject.transform.position;
        listener.transform.LookAt(player.position);
    }
}