using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    #region Variables

    //handle to robot Ai OBJ
    [SerializeField] private GameObject _robot;
    [SerializeField] private GameObject _bigRobot;
    [SerializeField] private GameObject _gundam;

    //damage display
    [SerializeField] private GameObject _damageDisplay;

    //list of robots to pool 
    [SerializeField] private List<GameObject> _robotPool;
    [SerializeField] private List<GameObject> _bigRobotPool;
    [SerializeField] private List<GameObject> _gundamPool;

    //damage display
    [SerializeField] private List<GameObject> _damageDisplayPool;

    //cache Robot string name to variable
    private const string _robotName = "Robot";
    private const string _bigRobotName = "BigRobot";
    private const string _gundamName = "Gundam";
    private const string _damageDisplayName = "DamageDisplay";

    //assumed max robots to instantiate
    [SerializeField] private int _maxRobots = 20;
    [SerializeField] private int _maxBigRobots = 20;
    [SerializeField] private int _maxGundams = 20;

    //damage display
    [SerializeField] private int _maxDamageDisplay = 10;

    //parent to hold instantiated enemies
    [SerializeField] private GameObject _enemyContainer;

    //hitVFX
    [SerializeField] private ParticleSystem _impactSparks;
    [SerializeField] private ParticleSystem _bulletSparks;
    [SerializeField] private ParticleSystem _barrierHitFX;
    [SerializeField] private ParticleSystem _electricBarrerHitFX;
    [SerializeField] private List<ParticleSystem> _impactSparksPool;
    [SerializeField] private List<ParticleSystem> _bulletSparksPool;
    [SerializeField] private List<ParticleSystem> _barrierHitHFXPool;
    [SerializeField] private List<ParticleSystem> _electricalBarrierHitFXPool;
    private const string _sparksName = "ImpactSparks";
    private const string _bulletSparksName = "BulletSparks";
    private const string _barrierHitFXName = "BarrierHitFX";
    private const string _electricalBarrierHitFXName = "BarrierSparks";
    [SerializeField] private int _maxSparks = 10;
    [SerializeField] private int _maxBulletSparks = 10;
    [SerializeField] private int _maxBarrierHitFX = 10;
    [SerializeField] private int _maxElectricalBarrierHitFX = 10;
    [SerializeField] private GameObject _impactVFXcontainer;

    #endregion

    #region Start

    // Start is called before the first frame update
    void Start()
    {
        NullChecks();

        InitializeLists();
    }

    #endregion

    #region Initialization

    private void NullChecks()
    {
        if (_robot == null)
            _robot = Resources.Load(_robotName) as GameObject;

        if (_bigRobot == null)
            _bigRobot = Resources.Load(_bigRobotName) as GameObject;

        if (_impactSparks == null)
            _impactSparks = Resources.Load(_sparksName) as ParticleSystem;

        if (_bulletSparks == null)
            _bulletSparks = Resources.Load(_bulletSparksName) as ParticleSystem;

        if (_barrierHitFX == null)
            _barrierHitFX = Resources.Load(_barrierHitFXName) as ParticleSystem;

        if (_electricBarrerHitFX == null)
            _electricBarrerHitFX = Resources.Load(_electricalBarrierHitFXName) as ParticleSystem;

        if (_gundam == null)
            _gundam = Resources.Load(_gundamName) as GameObject;

        if (_damageDisplay == null)
            _damageDisplay = Resources.Load(_damageDisplayName) as GameObject;
    }

    private void InitializeLists()
    {
        _robotPool = CreateRobots(_maxRobots);

        _bigRobotPool = CreateBigRobots(_maxBigRobots);

        _gundamPool = CreateGundams(_maxGundams);

        _impactSparksPool = CreateSparks(_maxSparks);

        _bulletSparksPool = CreateBulletSparks(_maxBulletSparks);

        _barrierHitHFXPool = CreateBarrierHitFX(_maxBarrierHitFX);

        _electricalBarrierHitFXPool = CreateElectricalBarrierHitFX(_maxElectricalBarrierHitFX);

        _damageDisplayPool = CreateDamageVFX(_maxDamageDisplay);
    }

    #endregion

    #region Private Methods

    private List<GameObject> CreateRobots(int maxRobots)
    {
        for (int i = 0; i < maxRobots; i++)
        {
            Vector3 spawnPosition = SpawnManager.Instance.GetSpawnPosition();

            GameObject robot = Instantiate(_robot, spawnPosition, Quaternion.identity, _enemyContainer.transform);
            robot.SetActive(false);

            _robotPool.Add(robot);
        }

        return _robotPool;
    }

    private List<GameObject> CreateBigRobots(int maxBigRobots)
    {
        for (int i = 0; i < maxBigRobots; i++)
        {
            Vector3 spawnPosition = SpawnManager.Instance.GetSpawnPosition();

            GameObject robot = Instantiate(_bigRobot, spawnPosition, Quaternion.identity, _enemyContainer.transform);
            robot.SetActive(false);

            _bigRobotPool.Add(robot);
        }

        return _bigRobotPool;
    }

    private List<GameObject> CreateGundams(int maxGundams)
    {
        for (int i = 0; i < maxGundams; i++)
        {
            Vector3 spawnPosition = SpawnManager.Instance.GetSpawnPosition();

            GameObject gundam = Instantiate(_gundam, spawnPosition, Quaternion.identity, _enemyContainer.transform);

            gundam.SetActive(false);

            _gundamPool.Add(gundam);
        }

        return _gundamPool;
    }

    private List<ParticleSystem> CreateSparks(int maxSparks)
    {
        for (int i = 0; i < maxSparks; i++)
        {
            ParticleSystem sparks = Instantiate(_impactSparks, Vector3.zero, Quaternion.identity, _impactVFXcontainer.transform);
            sparks.gameObject.SetActive(false);
            _impactSparksPool.Add(sparks);
        }

        return _impactSparksPool;
    }

    private List<ParticleSystem> CreateBulletSparks(int maxSparks)
    {
        for (int i = 0; i < maxSparks; i++)
        {
            ParticleSystem bulletSparks = Instantiate(_bulletSparks, Vector3.zero, Quaternion.identity, _impactVFXcontainer.transform);
            bulletSparks.gameObject.SetActive(false);
            _bulletSparksPool.Add(bulletSparks);
        }

        return _bulletSparksPool;
    }

    private List<ParticleSystem> CreateBarrierHitFX(int maxHitFX)
    {
        for (int i = 0; i < maxHitFX; i++)
        {
            ParticleSystem hitFX = Instantiate(_barrierHitFX, Vector3.zero, Quaternion.identity, _impactVFXcontainer.transform);
            hitFX.gameObject.SetActive(false);
            _barrierHitHFXPool.Add(hitFX);
        }

        return _barrierHitHFXPool;
    }

    private List<ParticleSystem> CreateElectricalBarrierHitFX(int maxHitFX)
    {
        for (int i = 0; i < maxHitFX; i++)
        {
            ParticleSystem hitFX = Instantiate(_electricBarrerHitFX, Vector3.zero, Quaternion.identity, _impactVFXcontainer.transform);
            hitFX.gameObject.SetActive(false);
            _electricalBarrierHitFXPool.Add(hitFX);
        }

        return _electricalBarrierHitFXPool;
    }

    private List<GameObject> CreateDamageVFX(int maxDamageFX)
    {
        for (int i = 0; i < maxDamageFX; i++)
        {
            GameObject damageVFX = Instantiate(_damageDisplay, Vector3.zero, Quaternion.identity, _impactVFXcontainer.transform);
            damageVFX.gameObject.SetActive(false);
            _damageDisplayPool.Add(damageVFX);
        }

        return _damageDisplayPool;
    }

    #endregion

    #region Public Methods

    public GameObject RequestRobot()
    {
        foreach (GameObject robot in _robotPool)
        {
            if (robot.activeInHierarchy == false)
            {
                robot.SetActive(true);
                return robot;
            }
        }

        Vector3 spawnPosition = SpawnManager.Instance.GetSpawnPosition();
        GameObject additionalRobot = Instantiate(_robot, spawnPosition, Quaternion.identity, _enemyContainer.transform);
        _robotPool.Add(additionalRobot);
        return additionalRobot;
    }

    public GameObject RequestBigRobot()
    {
        foreach (GameObject bigRobot in _bigRobotPool)
        {
            if (bigRobot.activeInHierarchy == false)
            {
                bigRobot.SetActive(true);
                return bigRobot;
            }
        }

        Vector3 spawnPosition = SpawnManager.Instance.GetSpawnPosition();
        GameObject additionalBigRobot = Instantiate(_bigRobot, spawnPosition, Quaternion.identity, _enemyContainer.transform);
        _robotPool.Add(additionalBigRobot);
        return additionalBigRobot;
    }

    public GameObject RequestGundam()
    {
        foreach (GameObject gundam in _gundamPool)
        {
            if (gundam.activeInHierarchy == false)
            {
                gundam.SetActive(true);
                return gundam;
            }
        }

        Vector3 spawnPosition = SpawnManager.Instance.GetSpawnPosition();
        GameObject additionalGundam = Instantiate(_gundam, spawnPosition, Quaternion.identity, _enemyContainer.transform);
        _robotPool.Add(additionalGundam);
        return additionalGundam;
    }

    public ParticleSystem RequestImpactSparks(Vector3 spawnPosition)
    {
        foreach(ParticleSystem sparks in _impactSparksPool)
        {
            if (sparks.gameObject.activeInHierarchy == false)
            {
                sparks.transform.position = spawnPosition;
                sparks.gameObject.SetActive(true);
                return sparks;
            }
        }

        ParticleSystem additionalSparks = Instantiate(_impactSparks, spawnPosition, Quaternion.identity, _impactVFXcontainer.transform);
        _impactSparksPool.Add(additionalSparks);
        return additionalSparks;
    }

    public ParticleSystem RequestBulletSparks(Vector3 spawnPosition)
    {
        foreach (ParticleSystem bulletSparks in _bulletSparksPool)
        {
            if (bulletSparks.gameObject.activeInHierarchy == false)
            {
                bulletSparks.transform.position = spawnPosition;
                bulletSparks.gameObject.SetActive(true);
                return bulletSparks;
            }
        }

        ParticleSystem additionalBulletSparks = Instantiate(_bulletSparks, spawnPosition, Quaternion.identity, _impactVFXcontainer.transform);
        _bulletSparksPool.Add(additionalBulletSparks);
        return additionalBulletSparks;
    }

    public ParticleSystem RequestBarrierHitFX(Vector3 spawnPosition)
    {
        foreach (ParticleSystem hitFx in _barrierHitHFXPool)
        {
            if (hitFx.gameObject.activeInHierarchy == false)
            {
                hitFx.transform.position = spawnPosition;
                hitFx.gameObject.SetActive(true);
                return hitFx;
            }
        }

        ParticleSystem additionalHitFx = Instantiate(_barrierHitFX, spawnPosition, Quaternion.identity, _impactVFXcontainer.transform);
        _barrierHitHFXPool.Add(additionalHitFx);
        return additionalHitFx;
    }

    public ParticleSystem RequestElectricalBarrierHitFX(Vector3 spawnPosition)
    {
        foreach (ParticleSystem hitFx in _electricalBarrierHitFXPool)
        {
            if (hitFx.gameObject.activeInHierarchy == false)
            {
                hitFx.transform.position = spawnPosition;
                hitFx.gameObject.SetActive(true);
                return hitFx;
            }
        }

        ParticleSystem additionalHitFx = Instantiate(_electricBarrerHitFX, spawnPosition, Quaternion.identity, _impactVFXcontainer.transform);
        _electricalBarrierHitFXPool.Add(additionalHitFx);
        return additionalHitFx;
    }

    public GameObject RequestDamageVFX(Vector3 spawnPosition)
    {
        foreach (GameObject damageVFX in _damageDisplayPool)
        {
            if (damageVFX.activeInHierarchy == false)
            {
                damageVFX.transform.position = spawnPosition;
                damageVFX.SetActive(true);
                return damageVFX;
            }
        }

        GameObject additionalDamageVFX = Instantiate(_damageDisplay, spawnPosition, Quaternion.identity, _impactVFXcontainer.transform);
        _damageDisplayPool.Add(additionalDamageVFX);
        return additionalDamageVFX;
    }

    #endregion
}
