﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerProperty))]
public class PlayerMove : MonoBehaviour
{
    // Centre point
    public Vector2 m_moveAreaCentre;
    // Width & height
    public Vector2 m_moveAreaShape;

    [Space]

    // Use for control "bullet time"
    public float m_resumeTime;
    public float m_pauseTime;
    public float m_timeScaleWhenPause;
                            
    private float _verticalSpeed;
    private float _horizontalSpeed;

    private PlayerProperty _playerProperty;


    private enum TimeState
    {
        Normal,
        Pausing,
        Resume
    };
    TimeState _timeState;

    private void OnEnable()
    {
        SetInitReference();

        _verticalSpeed = _playerProperty.m_horizontalSpeed;
        _horizontalSpeed = _playerProperty.m_verticalSpeed;

        UbhTimer.Instance.TimeScale = 1;
    }

    public Vector3 GetCurrentPlayerPosition()
    {
        UpdateSpeed();

        Vector2 playerPos = (Vector2)transform.position + new Vector2
                (InputManager.Instance.HorizontalInput * _horizontalSpeed,
                 InputManager.Instance.VerticalInput * _verticalSpeed) * Time.deltaTime;

        // player position is out of bound
        if (playerPos.x < m_moveAreaCentre.x - m_moveAreaShape.x / 2)
        {
            playerPos.x = m_moveAreaCentre.x - m_moveAreaShape.x / 2;
        }
        if (playerPos.x > m_moveAreaCentre.x + m_moveAreaShape.x / 2)
        {
            playerPos.x = m_moveAreaCentre.x + m_moveAreaShape.x / 2;
        }
        if (playerPos.y < m_moveAreaCentre.y - m_moveAreaShape.y / 2)
        {
            playerPos.y = m_moveAreaCentre.y - m_moveAreaShape.y / 2;
        }
        if (playerPos.y > m_moveAreaCentre.y + m_moveAreaShape.y / 2)
        {
            playerPos.y = m_moveAreaCentre.y + m_moveAreaShape.y / 2;
        }

        return new Vector3(playerPos.x, playerPos.y, transform.position.z);
    }

    public Quaternion GetCurrentRotation()
    {
        float lookAngle = UbhUtil.GetAngleFromTwoPosition(
            (Vector2)_playerProperty.m_spriteReference.transform.position,
            (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
        return Quaternion.Euler(0, 0, lookAngle);
    }

    private void Update()
    {                                                 
        transform.position = GetCurrentPlayerPosition();

        _playerProperty.m_spriteReference.transform.rotation = GetCurrentRotation();


        //// Time slow or resume control
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (_timeState == TimeState.Pausing)
        //    {
        //        _timeState = TimeState.Resume;
        //        StopAllCoroutines();
        //        StartCoroutine(Resuming(UbhTimer.Instance.TimeScale, m_resumeTime));
        //    }
        //    else if (_timeState == TimeState.Normal || _timeState == TimeState.Resume)
        //    {
        //        _timeState = TimeState.Pausing;
        //        StopAllCoroutines();
        //        StartCoroutine(Pausing(m_timeScaleWhenPause, m_pauseTime));
        //    }
        //}
    }


    IEnumerator Pausing(float endTimeScale, float lastTime)
    {
        while (UbhTimer.Instance.TimeScale >= endTimeScale)
        {
            UbhTimer.Instance.TimeScale -= (1 - endTimeScale) * Time.deltaTime / lastTime;
            yield return null;
        }

        UbhTimer.Instance.TimeScale = endTimeScale;
    }

    IEnumerator Resuming(float curTimeScale, float lastTime)
    {
        while (UbhTimer.Instance.TimeScale < 1f)
        {
            UbhTimer.Instance.TimeScale += (1 - curTimeScale) * Time.deltaTime / lastTime;
            yield return null;
        }
        UbhTimer.Instance.TimeScale = 1f;
        _timeState = TimeState.Normal;
    }


    void UpdateSpeed()
    {
        _horizontalSpeed = _playerProperty.m_horizontalSpeed;
        _verticalSpeed = _playerProperty.m_verticalSpeed;
    }


    void SetInitReference()
    {
        _playerProperty = GetComponent<PlayerProperty>();
        //_animator = GetComponent<Animator>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(m_moveAreaCentre, new Vector3(m_moveAreaShape.x, m_moveAreaShape.y, 0));

        //Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 10);
    }
}
