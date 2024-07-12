using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using GameDevHQ.FileBase.Plugins.FPS_Character_Controller;

public class CameraManager : MonoSingleton<CameraManager>
{
    #region Varables

    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private float _reductionFactor = 1f;

    [SerializeField] private CinemachineVirtualCamera _farCam;
    [SerializeField] private CinemachineVirtualCamera _midCam;
    [SerializeField] private CinemachineVirtualCamera _closeCam;
    [SerializeField] private CinemachineVirtualCamera _veryCloseCam;
    [SerializeField] private CinemachineVirtualCamera[] _vCams;
    private CinemachineVirtualCamera _activeCam;
    [SerializeField] private Transform _followTarget;
    private Vector3 _startingPosition;
    private FPS_Controller _player;
    [SerializeField] private List<CinemachineBasicMultiChannelPerlin> _noises;

    #endregion

    #region Start

    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<FPS_Controller>();

        if (_player != null)
        {
            _followTarget = _player.GetFollowTarget();
        }
        else
        {
            Debug.Log("The Player is NULL on Camera Manager");
        }

        _farCam.Priority = 11;
        _cameraTransform = _farCam.transform;

        foreach (CinemachineVirtualCamera vCam in _vCams)
        {
            CinemachineBasicMultiChannelPerlin noise = vCam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

            _noises.Add(noise);
        }

        foreach (CinemachineBasicMultiChannelPerlin noise in _noises)
        {
            noise.m_AmplitudeGain = 0.2f;
            noise.m_FrequencyGain = 1f;
        }
    }

    #endregion

    #region Public Methods

    public void ZoomIn()
    {
        if (_farCam.Priority == 11)
        {
            _farCam.Priority = 10;
            _midCam.Priority = 11;

            _cameraTransform = _midCam.transform;

            AudioManager.Instance.PlayCameraSound();
        }
        else if (_midCam.Priority == 11)
        {
            _midCam.Priority = 10;
            _closeCam.Priority = 11;

            _cameraTransform = _closeCam.transform;

            AudioManager.Instance.PlayCameraSound();
        }
        else if (_closeCam.Priority == 11)
        {
            _closeCam.Priority = 10;
            _veryCloseCam.Priority = 11;

            _cameraTransform = _veryCloseCam.transform;

            AudioManager.Instance.PlayCameraSound();
        }
    }

    public void ZoomOut()
    {
        if (_veryCloseCam.Priority == 11)
        {
            _veryCloseCam.Priority = 10;
            _closeCam.Priority = 11;

            _cameraTransform = _closeCam.transform;

            AudioManager.Instance.PlayCameraSound();
        }
        else if (_closeCam.Priority == 11)
        {
            _closeCam.Priority = 10;
            _midCam.Priority = 11;

            _cameraTransform = _midCam.transform;

            AudioManager.Instance.PlayCameraSound();
        }
        else if (_midCam.Priority == 11)
        {
            _midCam.Priority = 10;
            _farCam.Priority = 11;

            _cameraTransform = _farCam.transform;

            AudioManager.Instance.PlayCameraSound();
        }
    }

    public void ShakeCamera(float shakeDuration, float shakeMagnitude)
    {
        StartCoroutine(ShakeCameraRoutine(shakeDuration, shakeMagnitude));
    }

    #endregion

    #region Corooutines

    private IEnumerator ShakeCameraRoutine(float shakeDuration, float shakeMagnitude)
    {
        _startingPosition = _farCam.transform.position;

        while (shakeDuration > 0f)
        {
            foreach (CinemachineBasicMultiChannelPerlin noise in _noises)
            {
                noise.m_AmplitudeGain = shakeDuration;
                noise.m_FrequencyGain = shakeMagnitude;
            }

            shakeDuration -= Time.deltaTime * _reductionFactor;
            shakeMagnitude -= Time.deltaTime * _reductionFactor;

            yield return null;
        }

        foreach (CinemachineBasicMultiChannelPerlin noise in _noises)
        {
            noise.m_AmplitudeGain = 0.2f;
            noise.m_FrequencyGain = 1f;
        }
    }

    #endregion
}
