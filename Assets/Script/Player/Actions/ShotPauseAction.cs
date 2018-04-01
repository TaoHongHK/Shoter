using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shot Pause Action Data", menuName = "Pause Action Data/Shot")]
public class ShotPauseAction : BasePauseAction
{
    public GameObject m_bulletLightingPathNodePrefab;

    public float m_bulletSpeed;

    public int m_bulletDamage;

    public float m_playerLookAngle;

    public Vector3 m_playerPosition;

    public List<GameObject> m_bullets;

    public List<GameObject> m_bulletPathNodes;

    public List<GameObject> m_enemies;

    public override Type ActionType
    {
        get
        {
            return this.GetType();
        }
    }


    public ShotPauseAction(int actionPoint, int actionTimeFrame) : base(actionPoint, actionTimeFrame)
    {
        m_bullets = new List<GameObject>();
        m_bulletPathNodes = new List<GameObject>();
        m_enemies = new List<GameObject>();
    }

    public ShotPauseAction() : base()
    {
        m_bullets = new List<GameObject>();
        m_bulletPathNodes = new List<GameObject>();
        m_enemies = new List<GameObject>();
    }

    public override bool ExcuteAction(PlayerProperty playerProperty)
    {
        CurrentTimeFrame++;

        if(CurrentTimeFrame == 1)
        {
            var playerSpriteTrans = playerProperty.m_spriteReference.transform;
            playerSpriteTrans.rotation = Quaternion.Euler(0, 0, m_playerLookAngle);
            playerProperty.transform.position = m_playerPosition;
        }

        if (CurrentTimeFrame == (m_actionTimeFrame / 2))
        {                                                   
            foreach (var bulletGameObject in m_bullets)
            {
                bulletGameObject.SetActive(true);

                UbhBullet bulletComponent = bulletGameObject.GetComponent<UbhBullet>();
                if (bulletComponent == null)
                {
                    bulletComponent = bulletGameObject.AddComponent<UbhBullet>();
                }

                bulletComponent.m_damage = m_bulletDamage;
                bulletComponent.Shot(m_bulletSpeed,
                        UbhUtil.GetAngleFromTwoPosition(Vector3.zero, bulletGameObject.transform.up),  // shot angle
                        0, 0,
                        false, null, 0, 0,
                        false, 0, 0,
                        false, 0, 0, true);
            }

            foreach (var bulletPathNode in m_bulletPathNodes)
            {
                UbhObjectPool.Instance.ReleaseGameObject(bulletPathNode);
            }

            return false;
        }

        if (CurrentTimeFrame > m_actionTimeFrame)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public override bool RollBackAction(PlayerProperty playerProperty)
    {
        foreach(var bullet in m_bullets)
        {
            UbhObjectPool.Instance.ReleaseGameObject(bullet);
        }

        foreach(var bulletLightPathNode in m_bulletPathNodes)
        {
            UbhObjectPool.Instance.ReleaseGameObject(bulletLightPathNode);
        }

        return true;
    }
}
