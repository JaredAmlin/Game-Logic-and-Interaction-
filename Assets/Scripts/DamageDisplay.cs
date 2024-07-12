using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour, IDisplay
{
    #region Variables

    private Camera _mainCamera;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _timeToDisplay = 2f;
    private const string _respawnName = "Respawn";

    #endregion

    #region Start, OnEnable

    void Start()
    {
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        Invoke(_respawnName,_timeToDisplay);
    }

    #endregion

    private void Update()
    {
        //face the camera
        transform.forward = -_mainCamera.transform.forward;

        //move up
        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }

    #region Methods

    public void DisplayText(string text)
    {
        _text.text = text;
    }

    private void Respawn()
    {
        this.gameObject.SetActive(false);
    }

    #endregion
}
