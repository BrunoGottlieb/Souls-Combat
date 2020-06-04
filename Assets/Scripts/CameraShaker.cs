using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam;
    public CinemachineFreeLook lockedCam;

    void Start()
    {
        Noise(false); // inicia o jogo sem shake
    }

    public void ShakeCamera(float time) // metodo chamado por outros scripts
    {
        StartCoroutine(Shaker(time));
    }

    IEnumerator Shaker(float time) // corotina para controlar o tempo do shake
    {
        Noise(true);
        yield return new WaitForSeconds(time);
        Noise(false);
    }

    private void Noise(bool b) // ativa e desativa o noise nas cameras
    {
        freeLookCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().enabled = b;
        freeLookCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().enabled = b;
        freeLookCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().enabled = b;
        lockedCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().enabled = b;
        lockedCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().enabled = b;
        lockedCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().enabled = b;
    }

}
