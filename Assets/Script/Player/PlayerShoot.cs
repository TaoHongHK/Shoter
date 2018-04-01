using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour 
{                                             
    // The position of the bullet emit point.
    public List<Transform> m_shootList = new List<Transform>();

    // Prefab of red status bullet
    public GameObject m_whiteBulletPrefab;

    // Prefab of black status bullet
    public GameObject m_blackBulletPrefab;

    // Use this property to control damage and shot interval.
    private PlayerProperty _playerProperty;

    // Timer for count whether it is enough time after last shot.
    private float _timer;

    [SerializeField]
    private Camera _camera;

    void OnEnable()
    {
        _playerProperty = GetComponent<PlayerProperty>();
    }


    void Update()
    {
        _timer += UbhTimer.Instance.DeltaTime;
        if (Input.GetMouseButton(0))
        {
            Shot();
        }
    }


    // Player wants to shot.
    void Shot()
    {                                              
        // it is not enough time after the last shot, shot cancel 
        if(_timer < _playerProperty.m_shootInterval)
            return;

        if(m_shootList == null || m_shootList.Count <= 0)
        {
            Debug.LogWarning("Cannot shot because ShotList is not set.");
            return;
        } 

        for(int i = 0; i < m_shootList.Count; i++)
        {
            Vector3 shotStartPoint = m_shootList[i].position ;
            Vector3 shotDirection = m_shootList[i].up;
            float angle = UbhUtil.GetAngleFromTwoPosition(Vector3.zero, shotDirection);

            UbhBullet bullet = GetBullet(shotStartPoint, Quaternion.Euler(0, 0, angle));
            if(bullet == null)  break;
            bullet.Shot(_playerProperty.m_bulletSpeed, angle, 
                    0, 0, 
                    false, null, 0, 0, 
                    false, 0, 0, 
                    false, 0, 0, false);
            bullet.m_damage = _playerProperty.m_bulletDamage;
        }

        // finish a shot, reset timer
        _timer = 0f;
    }


    // Get a template bullet in the object pool.
    // position: bullet worldspace position.
    // rotation: bullet worldspace rotation.
    // forceInstantiate: force to instantiate a bullet in object pool and get it.
    UbhBullet GetBullet(Vector3 position, Quaternion rotation, bool forceInstantiate = false)
    {
        GameObject bulletPrefab = (_playerProperty.m_playerState == PlayerProperty.PlayerStateType.Black) ? m_blackBulletPrefab : m_whiteBulletPrefab;

        if(bulletPrefab == null)
        {
            Debug.LogError("The bullet prefab is null!");
            return null;
        }

        // Get bullet gameobject from object pool
        var goBullet = UbhObjectPool.Instance.GetGameObject
                    (bulletPrefab, position, rotation, forceInstantiate);
        if(goBullet == null)
        {
            Debug.LogWarning("Fail to get the bullet from object pool!");
            return null;
        }    

        // Get or add UbhBullet component
        var bullet = goBullet.GetComponent<UbhBullet>();
        if(bullet == null)
        {
            bullet = goBullet.AddComponent<UbhBullet>();
        }

        return bullet;
    }

}
