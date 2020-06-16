using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam;
    public CinemachineFreeLook lockedCam;
    public Transform listener;
    private Transform player;
    private Animator playerAnim;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerAnim = player.GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManagerScript.gameIsPaused)
        {
            freeLookCam.m_YAxis.m_InputAxisValue = 0;
            freeLookCam.m_XAxis.m_InputAxisValue = 0;
            lockedCam.m_YAxis.m_InputAxisValue = 0;
            return;
        }

        float y_input = Input.GetAxis("Mouse Y") + Input.GetAxis("Right Stick Y");
        float x_input = Input.GetAxis("Mouse X") + Input.GetAxis("Right Stick X");

        freeLookCam.m_YAxis.m_InputAxisValue = y_input;
        freeLookCam.m_XAxis.m_InputAxisValue = x_input;

        lockedCam.m_YAxis.m_InputAxisValue = y_input;

        if (!playerAnim.GetBool("LockedCamera"))
        {
            listener.position = freeLookCam.gameObject.transform.position;
            listener.transform.LookAt(player.position);
        } else
        {
            listener.position = lockedCam.gameObject.transform.position;
            listener.transform.LookAt(player.position);
        }

        
    }
}