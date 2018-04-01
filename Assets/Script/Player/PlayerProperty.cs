using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperty : MonoBehaviour
{

    // god mode -- from Skyrim.
    public bool m_tgm = false;

    // movement
    public float m_verticalSpeed;
    public float m_horizontalSpeed;
    public float m_slowHorizontalSpeed;
    public float m_slowVerticalSpeed;

    // shoot 
    [Range(0.05f, 2.5f)]
    public float m_shootInterval;
    public int m_bulletDamage;
    public float m_bulletSpeed;
    
    
    // state
    public enum PlayerStateType
    {
        Black,
        White
    };
    private PlayerStateType _playerState;
    public PlayerStateType m_playerState
    {
        get
        {
            return _playerState;
        }
        set
        {
            _playerState = value;

            if (value == PlayerStateType.Black)
                m_spriteReference.sprite = GetComponent<PlayerChangeState>().m_blackSprite;
            else
                m_spriteReference.sprite = GetComponent<PlayerChangeState>().m_whiteSprite;
        }
    }

    public int m_playerHealth;

    public SpriteRenderer m_spriteReference;
}
