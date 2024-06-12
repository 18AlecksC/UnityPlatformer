using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public float fallPanAmount = 0.25f;
    public float fallPanTime = 0.35f;
    public float fallSpeedThreshold = -15f;
    public bool IsLerping { get; private set; }
    public bool LerpedFromFall { get; set; }
    public CinemachineVirtualCamera[] virtualCameras;

    private CinemachineFramingTransposer framingTransposer;
    private CinemachineVirtualCamera currentCamera;
    private float normPanAmount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;   
        }

        for (int i = 0; i < virtualCameras.Length; i++)
        {
            if (virtualCameras[i].enabled)
            {
                currentCamera = virtualCameras[i];
                framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        normPanAmount = framingTransposer.m_YDamping;
    }

    public void LerpYDamping(bool isFalling)
    {
        StartCoroutine(LerpYAction(isFalling));
    }

    private IEnumerator LerpYAction(bool isFalling)
    {
        IsLerping = true;

        float startDampAmount = framingTransposer.m_YDamping;
        float endDampAmount;

        if (isFalling)
        {
            endDampAmount = fallPanAmount;
            LerpedFromFall = true;
        }
        else
        {
            endDampAmount = normPanAmount;
        }

        float elapsedTime = 0f;
        while (elapsedTime < fallPanTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, elapsedTime / fallPanTime);
            framingTransposer.m_YDamping = lerpedPanAmount;
            yield return null;
        }

        IsLerping = false;
    }
}
