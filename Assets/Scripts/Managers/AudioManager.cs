using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    #region Variables

    //main soundtrack
    [SerializeField] private AudioClip _mainMenuSoundtrack;
    [SerializeField] private AudioClip _waveSoundtrack;
    [SerializeField] private AudioClip _gameOverSoundtrack;
    [SerializeField] private AudioClip _cameraZoomClip;
    [SerializeField] private AudioClip _hoverClip;
    [SerializeField] private AudioClip _clickClip;
    [SerializeField] private AudioClip _reactorHitClip;
    [SerializeField] private AudioClip _deepImpactClip;
    [SerializeField] private AudioSource _mainMenuSoundtrackSource;
    [SerializeField] private AudioSource _waveSoundtrackSource;
    [SerializeField] private AudioSource _gameOverSoundtrackSource;

    //player shooting
    [SerializeField] private AudioSource _rifleSource;

    //cam zoom sound
    [SerializeField] private AudioSource _cameraZoomSource;

    //button sounds
    [SerializeField] private AudioSource _clickSouce;

    [SerializeField] private AudioSource _reactorHitSource;

    [SerializeField] private AudioSource _deepImpactSource;

    #endregion

    #region Start, OnDisable

    private void Start()
    {
        _mainMenuSoundtrackSource.Play();
        StartCoroutine(FadeInRoutine(_mainMenuSoundtrackSource, 10f));

        GameManager.onWaveStart += GameManager_onWaveStart;
        GameManager.onGameOver += GameManager_onGameOver;
        GameManager.onGameWin += GameManager_onGameWin;
    }

    private void OnDisable()
    {
        GameManager.onWaveStart -= GameManager_onWaveStart;
        GameManager.onGameOver -= GameManager_onGameOver; 
        GameManager.onGameWin -= GameManager_onGameWin;
    }

    #endregion

    #region Event Methods

    private void GameManager_onGameOver()
    {
        StartCoroutine(FadeOutRoutine(_waveSoundtrackSource, 5f));

        _gameOverSoundtrackSource.Play();

        StartCoroutine(FadeInRoutine(_gameOverSoundtrackSource, 5f));
    }

    private void GameManager_onGameWin()
    {
        StartCoroutine(FadeOutRoutine(_waveSoundtrackSource, 5f));

        _gameOverSoundtrackSource.Play();

        StartCoroutine(FadeInRoutine(_gameOverSoundtrackSource, 5f));
    }

    private void GameManager_onWaveStart()
    {
        StartCoroutine(FadeOutRoutine(_mainMenuSoundtrackSource, 5f));

        if (_waveSoundtrackSource.isPlaying == false)
        {
            _waveSoundtrackSource.Play();
            StartCoroutine(FadeInRoutine(_waveSoundtrackSource, 5f));
        }
    }

    #endregion

    #region Public Methods

    public void PlayRifleSound(AudioClip clip)
    {
        _rifleSource.PlayOneShot(clip);
    }

    public void PlayCameraSound()
    {
        _cameraZoomSource.PlayOneShot(_cameraZoomClip);
    }

    public void PlayReactorHitSound()
    {
        _reactorHitSource.PlayOneShot(_reactorHitClip);
        _deepImpactSource.PlayOneShot(_deepImpactClip);
    }

    public void PlayExplodingBarrelSound(AudioSource source1, AudioSource source2, AudioClip clip1, AudioClip clip2)
    {
        source1.PlayOneShot(clip1);
        source2.PlayOneShot(clip2);
    }

    public void PlayHoverSound()
    {
        _clickSouce.PlayOneShot(_hoverClip);
    }

    public void PlayClickSound()
    {
        _clickSouce.PlayOneShot(_clickClip);
    }

    public void PlayBarrierHitSound(AudioSource source, AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void PlayClickSFX(AudioClip clip)
    {
        _clickSouce.PlayOneShot(clip);
    }

    #endregion

    #region Coroutines

    private IEnumerator FadeInRoutine(AudioSource source, float timeToFade)
    {
        float timeElapsed = 0f;
        
        while(timeElapsed < timeToFade)
        {
            source.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutRoutine(AudioSource source, float timeToFade)
    {
        float timeElapsed = 0f;

        while (timeElapsed < timeToFade)
        {
            source.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        source.Stop();
    }

    #endregion
}
