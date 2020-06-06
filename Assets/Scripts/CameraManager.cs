using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam;
    public CinemachineFreeLook lockedCam;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float y_input = Input.GetAxis("Mouse Y") + Input.GetAxis("Right Stick Y");
        float x_input = Input.GetAxis("Mouse X") + Input.GetAxis("Right Stick X");

        freeLookCam.m_YAxis.m_InputAxisValue = y_input;
        freeLookCam.m_XAxis.m_InputAxisValue = x_input;

        lockedCam.m_YAxis.m_InputAxisValue = y_input;


        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vKey))
            {
                print(vKey);
            }
        }

    }
}