using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.FileBase.Plugins.FPS_Character_Controller
{
    [RequireComponent(typeof(CharacterController))]
    public class FPS_Controller : MonoBehaviour
    {
        #region Variables

        [Header("Controller Info")]
        [SerializeField ][Tooltip("How fast can the controller walk?")]
        private float _walkSpeed = 3.0f; //how fast the character is walking
        [SerializeField][Tooltip("How fast can the controller run?")]
        private float _runSpeed = 7.0f; // how fast the character is running
        [SerializeField][Tooltip("Set your gravity multiplier")] 
        private float _gravity = 1.0f; //how much gravity to apply 
        [SerializeField][Tooltip("How high can the controller jump?")]
        private float _jumpHeight = 15.0f; //how high can the character jump
        [SerializeField]
        private bool _isRunning = false; //bool to display if we are running
        [SerializeField]
        private bool _crouching = false; //bool to display if we are crouched or not

        private CharacterController _controller; //reference variable to the character controller component
        private float _yVelocity = 0.0f; //cache our y velocity

        [Header("Headbob Settings")]       
        [SerializeField][Tooltip("Smooth out the transition from moving to not moving")]
        private float _smooth = 20.0f; //smooth out the transition from moving to not moving
        [SerializeField][Tooltip("How quickly the player head bobs")]
        private float _walkFrequency = 4.8f; //how quickly the player head bobs when walking
        [SerializeField][Tooltip("How quickly the player head bobs")]
        private float _runFrequency = 7.8f; //how quickly the player head bobs when running
        [SerializeField][Tooltip("How dramatic the headbob is")][Range(0.0f, 0.2f)]
        private float _heightOffset = 0.05f; //how dramatic the bobbing is
        private float _timer = Mathf.PI / 2; //This is where Sin = 1 -- used to simulate walking forward. 
        private Vector3 _initialCameraPos; //local position where we reset the camera when it's not bobbing

        [Header("Camera Settings")]
        [SerializeField][Tooltip("Control the look sensitivty of the camera")]
        private float _lookSensitivity = 5.0f; //mouse sensitivity 
        [SerializeField] private float _lookSensitivityFar = 5.0f; //mouse sensitivity
        [SerializeField] private float _lookSensitivityMid = 5.0f; //mouse sensitivity
        [SerializeField] private float _lookSensitivityClose = 5.0f; //mouse sensitivity
        [SerializeField] private float _lookSensitivityVeryClose = 5.0f; //mouse sensitivity

        [SerializeField] private Camera _fpsCamera;

        //Jared Stuff
        private Vector3 _centerPoint = new Vector3(0.5f, 0.5f, 0);
        [SerializeField] private LayerMask _hitMask;
        [SerializeField] private AudioClip _rifeShotClip;
        [SerializeField] private AudioClip _outOfAmmoClip;
        [SerializeField] private AudioClip _reloadClip;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private int _shotsFired;
        private bool _isWaveActive = false;
        [SerializeField] private int _ammo;
        [SerializeField] private int _maxAmmo = 7;
        [SerializeField] private int _ammoReserve;
        [SerializeField] private int _ammoRestock = 50;
        [SerializeField] private int _ammoIncreaseAmount = 10;
        [SerializeField] private bool _canFire = true;
        private const string _isWaveActiveName = "IsWaveActive";
        private const string _reloadCoolDownName = "ReloadCoolDown";
        private bool _isGameStarted = false;

        #endregion

        #region Start, OnDisable

        private void Start()
        {
            _controller = GetComponent<CharacterController>(); //assign the reference variable to the component
            _fpsCamera = GetComponentInChildren<Camera>();
            _initialCameraPos = _fpsCamera.transform.localPosition;
            _lookSensitivity = _lookSensitivityFar;

            GameManager.onWaveStart += GameManager_onWaveStart;
            GameManager.onWaveComplete += GameManager_onWaveComplete;
            GameManager.onGameStart += GameManager_onGameStart;
        }

        private void OnDisable()
        {
            GameManager.onWaveStart -= GameManager_onWaveStart;
            GameManager.onWaveComplete -= GameManager_onWaveComplete; 
            GameManager.onGameStart -= GameManager_onGameStart;
        }

        #endregion

        #region Update

        private void Update()
        {
            InputCheck();

            if (_isGameStarted)
            {
                FPSController();
                CameraController();
                HeadBobbing();
            }
        }

        #endregion

        #region Event Methods

        private void GameManager_onWaveComplete()
        {
            _isWaveActive = false;
        }

        private void GameManager_onWaveStart()
        {
            _ammo = _maxAmmo;

            _ammoReserve += _ammoRestock;

            _ammoRestock += _ammoIncreaseAmount;

            UIManager.Instance.UpdateAmmoUI(_ammo, _maxAmmo);

            UIManager.Instance.UpdateAmmoReserveUI(_ammoReserve);

            _isWaveActive = true;
        }

        private void GameManager_onGameStart()
        {
            _isGameStarted = true;
        }

        #endregion

        #region Input

        private void InputCheck()
        {
            //shooting
            if (Input.GetMouseButtonDown(0))
            {
                if (_isWaveActive)
                {
                    if (_ammo > 0 && _canFire)
                    {
                        Fire();
                    }
                    else
                        AudioManager.Instance.PlayRifleSound(_outOfAmmoClip);
                }
            }

            //reload
            if (Input.GetMouseButtonDown(1))
            {
                if (_isWaveActive)
                {
                    int difference = _maxAmmo - _ammo;
                    _ammo += Reload(difference);
                    UIManager.Instance.UpdateAmmoUI(_ammo, _maxAmmo);
                }
            }

            //zoom in
            if (Input.GetKeyDown(KeyCode.E))
            {
                //zoom in
                CameraManager.Instance.ZoomIn();

                if (_lookSensitivity == _lookSensitivityFar)
                {
                    _lookSensitivity = _lookSensitivityMid;
                }
                else if (_lookSensitivity == _lookSensitivityMid)
                {
                    _lookSensitivity = _lookSensitivityClose;
                }
                else if (_lookSensitivity == _lookSensitivityClose)
                {
                    _lookSensitivity = _lookSensitivityVeryClose;
                }
            }

            //zoom out
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //zoom out
                CameraManager.Instance.ZoomOut();

                if (_lookSensitivity == _lookSensitivityVeryClose)
                {
                    _lookSensitivity = _lookSensitivityClose;
                }
                else if (_lookSensitivity == _lookSensitivityClose)
                {
                    _lookSensitivity = _lookSensitivityMid;
                }
                else if (_lookSensitivity == _lookSensitivityMid)
                {
                    _lookSensitivity = _lookSensitivityFar;
                }
            }

            //quit game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            //skip cinematic
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                GameManager.Instance.SkipTutorial();
            }
        }

        #endregion

        #region Shooting, Reload

        private void Fire()
        {
            _shotsFired++;

            _ammo--;

            if (_ammo <= 0)
            {
                _ammo = 0;

                _ammo = Reload(_maxAmmo);
            }

            GameManager.Instance.AddShotsFired();

            UIManager.Instance.UpdateAmmoUI(_ammo, _maxAmmo);

            _muzzleFlash.Play();

            CameraManager.Instance.ShakeCamera(0.2f, 40f);

            //raycast forward from screen center
            Ray rayOrigin = Camera.main.ViewportPointToRay(_centerPoint);

            //store hit info
            RaycastHit hitInfo;

            if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity, _hitMask))
            {
                PoolManager.Instance.RequestBulletSparks(hitInfo.point);

                IHittable hit = hitInfo.collider.GetComponent<IHittable>();

                if (hit != null)
                {
                    hit.HitDamage(10, hitInfo.point);
                }
            }

            AudioManager.Instance.PlayRifleSound(_rifeShotClip);
        }

        private int Reload(int ammo)
        {
            _canFire = false;
            AudioManager.Instance.PlayRifleSound(_reloadClip);
            Invoke(_reloadCoolDownName, 1.5f);
            UIManager.Instance.ReloadCooldownUI(1.5f);

            //compare reserve ammmo to incoming request
            int difference = _ammoReserve - ammo;

            int remainingAmmo = _ammoReserve;

            _ammoReserve -= ammo;

            if (_ammoReserve < 0)
                _ammoReserve = 0;

            UIManager.Instance.UpdateAmmoReserveUI(_ammoReserve);

            if (_ammo == 0 && remainingAmmo == 0)
            {
                Invoke(_isWaveActiveName, 1f);
            }

            if (difference > 0)
            {
                //full reload
                return ammo;
            }
            else
            {
                //empty reserve. partial reload
                return remainingAmmo;
            }
        }

        #endregion

        #region Invoked Methods

        private void IsWaveActive()
        {
            if (_isWaveActive)
            {
                GameManager.Instance.GameOver();
                UIManager.Instance.SetOutOfAmmoTextActive();
            }
        }

        private void ReloadCoolDown()
        {
            _canFire = true;
        }

        #endregion

        #region Public Methods

        public Transform GetFollowTarget()
        {
            return _fpsCamera.transform;
        }

        void FPSController()
        {
            float h = Input.GetAxis("Horizontal"); //horizontal inputs (a, d, leftarrow, rightarrow)
            float v = Input.GetAxis("Vertical"); //veritical inputs (w, s, uparrow, downarrow)

            Vector3 direction = new Vector3(h, 0, v); //direction to move
            Vector3 velocity = direction * _walkSpeed; //velocity is the direction and speed we travel

            if (Input.GetKeyDown(KeyCode.C) && _isRunning == false)
            {

                if (_crouching == true)
                {
                    _crouching = false;
                    _controller.height = 2.0f;
                }
                else
                {
                    _crouching = true;
                    _controller.height = 1.0f;
                }
                
            }

            if (Input.GetKey(KeyCode.LeftShift) && _crouching == false) //check if we are holding down left shift
            {
                velocity = direction * _runSpeed; //use the run velocity 
                _isRunning = true;
            }
            else
            {
                _isRunning = false;
            }

            if (_controller.isGrounded == true) //check if we're grounded
            {
                if (Input.GetKeyDown(KeyCode.Space)) //check for the space key
                {
                    _yVelocity = _jumpHeight; //assign the cache velocity to our jump height
                }
            }
            else //we're not grounded
            {
                _yVelocity -= _gravity; //subtract gravity from our yVelocity 
            }

            velocity.y = _yVelocity; //assign the cached value of our yvelocity

            velocity = transform.TransformDirection(velocity);

            _controller.Move(velocity * Time.deltaTime); //move the controller x meters per second
        }

        void CameraController()
        {
            float mouseX = Input.GetAxis("Mouse X"); //get mouse movement on the x
            float mouseY = Input.GetAxis("Mouse Y"); //get mouse movement on the y

            Vector3 rot = transform.localEulerAngles; //store current rotation
            rot.y += mouseX * _lookSensitivity; //add our mouseX movement to the y axis
            transform.localRotation = Quaternion.AngleAxis(rot.y, Vector3.up); ////rotate along the y axis by movement amount

            Vector3 camRot = _fpsCamera.transform.localEulerAngles; //store the current rotation
            camRot.x += -mouseY * _lookSensitivity; //add the mouseY movement to the x axis
            _fpsCamera.transform.localRotation = Quaternion.AngleAxis(camRot.x, Vector3.right); //rotate along the x axis by movement amount
        }

        void HeadBobbing()
        {
            float h = Input.GetAxis("Horizontal"); //horizontal inputs (a, d, leftarrow, rightarrow)
            float v = Input.GetAxis("Vertical"); //veritical inputs (w, s, uparrow, downarrow)

            if (h != 0 || v != 0) //Are we moving?
            {
               
                if (Input.GetKey(KeyCode.LeftShift)) //check if running
                {
                    _timer += _runFrequency * Time.deltaTime; //increment timer for our sin/cos waves when running
                }
                else
                {
                    _timer += _walkFrequency * Time.deltaTime; //increment timer for our sin/cos waves when walking
                }

                Vector3 headPosition = new Vector3 //calculate the head position in our walk cycle
                    (
                        _initialCameraPos.x + Mathf.Cos(_timer) * _heightOffset, //x value
                        _initialCameraPos.y + Mathf.Sin(_timer) * _heightOffset, //y value
                        0 // z value
                    );

                _fpsCamera.transform.localPosition = headPosition; //assign the head position

                if (_timer > Mathf.PI * 2) //reset the timer when we complete a full walk cycle on the unit circle
                {
                    _timer = 0; //completed walk cycle. Reset. 
                }
            }
            else
            {
                _timer = Mathf.PI / 2; //reset timer back to 1 for initial walk cycle 

                Vector3 resetHead = new Vector3 //calculate reset head position back to initial cam pos
                    (
                    Mathf.Lerp(_fpsCamera.transform.localPosition.x, _initialCameraPos.x, _smooth * Time.deltaTime), //x vlaue
                    Mathf.Lerp(_fpsCamera.transform.localPosition.y, _initialCameraPos.y, _smooth * Time.deltaTime), //y value
                    0 //z value
                    );

                _fpsCamera.transform.localPosition = resetHead; //assign the head position back to the initial cam pos
            }
        }

        #endregion
    }
}

