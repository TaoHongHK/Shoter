using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerProperty))]
[RequireComponent(typeof(PlayerShoot))]
[RequireComponent(typeof(PlayerMove))]
public class SuperStateController : MonoBehaviour
{
    public LinkedList<BasePauseAction> m_actions;

    [SerializeField] private int _totalActionPoint;
    public int TotalActionPoint
    {
        get
        {
            return _totalActionPoint;
        }
        set
        {
            _totalActionPoint = value;
        }
    }

    [SerializeField] private int _currentActionPoint;
    public int CurrentActionPoint
    {
        get
        {
            return _currentActionPoint;
        }
        set
        {
            _currentActionPoint = value;
        }
    }                                              

    [Space]

    [SerializeField]
    private bool _lockInput = true;

    [SerializeField]
    private bool _inSuperState = false;

    [SerializeField]
    private bool _completeAction = false;

    private PlayerProperty _playerBackup;

    private PlayerProperty _playerProperty;
    private PlayerShoot _playerShotProperty;
    private PlayerMove _playerMoveComponent;
    private PlayerChangeState _playerChangeStateComponent;
    private SpriteRenderer _playerSprite;

    private BasePauseAction _currentAction;

    [Space]
    [Header("Template Action Data:")]

    [SerializeField]
    private MovePauseAction _templateMoveAction;
    [SerializeField]
    private ShotPauseAction _templateShotAction;
    [SerializeField]
    private ChangeStagePauseAction _templateChangeStateAction;

    [Space]
    [Header("Other Reference")]

    [SerializeField]
    private Camera _mainCamera;            
    public Slider m_actionPointSlider;


    public void Start()
    {
        _playerProperty = GetComponent<PlayerProperty>();
        _playerShotProperty = GetComponent<PlayerShoot>();
        _playerMoveComponent = GetComponent<PlayerMove>();
        _playerChangeStateComponent = GetComponent<PlayerChangeState>();
        _playerSprite = _playerProperty.m_spriteReference;

        m_actions = new LinkedList<BasePauseAction>();

        _lockInput = true;
        _inSuperState = false;
        _completeAction = false;
    }


    private void Update()
    {
        HandleUIDisplay();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_inSuperState)
            {
                _inSuperState = true;
                _lockInput = false;
                StartSuperState();
            }
            else
            {
                _lockInput = true;
                EndSuperStateInput();
            }
            return;
        }

        if (_inSuperState && !_lockInput)
        {
            HandleInput();
            return;
        }
    }

    private void HandleUIDisplay()
    {
        m_actionPointSlider.maxValue = TotalActionPoint;
        m_actionPointSlider.value = CurrentActionPoint;

        m_actionPointSlider.gameObject.SetActive(_inSuperState);
    }


    private void StartSuperState()
    {
        RestorePlayerProperty();

        _currentAction = null;
        _playerShotProperty.enabled = false;
        _playerChangeStateComponent.enabled = false;
        _playerMoveComponent.enabled = false;

        UbhTimer.Instance.TimeScale = 0;
    }

    private void EndSuperStateInput()
    {
        if (m_actions.Count == 0)
        {
            _currentAction = null;
        }
        else
        {
            _currentAction = m_actions.First.Value;
            m_actions.RemoveFirst();
        }
        _completeAction = false;
        _playerChangeStateComponent.enabled = true;
        _mainCamera.GetComponent<MotionBlur>().enabled = true;

        UbhTimer.Instance.TimeScale = 0.02f;

        ResetPlayerProperty();
        StartCoroutine(StartExcuteAction());
    }


    private void EndSuperState()
    {                                           
        _inSuperState = false;

        _playerShotProperty.enabled = true;
        _playerMoveComponent.enabled = true;
        _mainCamera.GetComponent<MotionBlur>().enabled = false;

        UbhTimer.Instance.TimeScale = 1f;
    }


    private void RestorePlayerProperty()
    {
        _playerBackup = Instantiate(_playerProperty);
        _playerBackup.gameObject.SetActive(false);

        _playerBackup.m_playerState = _playerProperty.m_playerState;
    }

    private void ResetPlayerProperty()
    {
        _playerProperty.m_playerState = _playerBackup.m_playerState;

        Destroy(_playerBackup.gameObject);
    }

    IEnumerator StartExcuteAction()
    {
        while(_currentAction != null)
        {
            ExcuteAction();
            yield return null;
        }

        EndSuperState();
    }


    private void HandleInput()
    {
        _playerProperty.m_spriteReference.transform.rotation = _playerMoveComponent.GetCurrentRotation();

        // Handle the shot action
        if (Input.GetMouseButtonDown(0))
        {
            if (!CheckActionPointFill(_templateShotAction)) return;

            var shotAction = ScriptableObject.CreateInstance<ShotPauseAction>();

            shotAction.m_actionPoint = _templateShotAction.m_actionPoint;
            shotAction.m_actionTimeFrame = _templateShotAction.m_actionTimeFrame;
            shotAction.m_bulletLightingPathNodePrefab = _templateShotAction.m_bulletLightingPathNodePrefab;

            GameObject bulletPrefab = null;
            if (_playerProperty.m_playerState == PlayerProperty.PlayerStateType.Black)
            {
                bulletPrefab = _playerShotProperty.m_blackBulletPrefab;
            }
            else
            {
                bulletPrefab = _playerShotProperty.m_whiteBulletPrefab;
            }
            shotAction.m_bulletDamage = _playerProperty.m_bulletDamage;
            shotAction.m_bulletSpeed = _playerProperty.m_bulletSpeed;

            foreach (var shotPointTrans in _playerShotProperty.m_shootList)
            {
                float shotAngle = UbhUtil.GetAngleFromTwoPosition(Vector3.zero, shotPointTrans.up);

                Quaternion rotation = Quaternion.Euler(0, 0, shotAngle);
                Vector3 position = shotPointTrans.position;

                var bullet = UbhObjectPool.Instance.GetGameObject(bulletPrefab, position, rotation, true);
                bullet.SetActive(false);
                shotAction.m_bullets.Add(bullet);

                float bulletPathLength = 7;
                float deltLength = 0.3f;
                for (float curPathLength = 0; curPathLength <= bulletPathLength; curPathLength += deltLength)
                {
                    Vector3 nodePosition = shotPointTrans.position + shotPointTrans.up * curPathLength;
                    shotAction.m_bulletPathNodes.Add(
                        UbhObjectPool.Instance.GetGameObject(
                                shotAction.m_bulletLightingPathNodePrefab,
                                nodePosition, rotation));
                }
            }

            shotAction.m_playerLookAngle = UbhUtil.GetAngleFromTwoPosition(Vector3.zero, _playerSprite.transform.up);
            shotAction.m_playerPosition = _playerProperty.transform.position;

            m_actions.AddLast(shotAction);
            _currentActionPoint += shotAction.m_actionPoint;
            _currentAction = shotAction;
        }


        // Handle the move action
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            if (!CheckActionPointFill(_templateMoveAction)) return;

            _playerProperty.transform.position = _playerMoveComponent.GetCurrentPlayerPosition();

            MovePauseAction moveAction = _currentAction as MovePauseAction;

            if (moveAction == null)
            {
                moveAction = ScriptableObject.CreateInstance<MovePauseAction>();

                moveAction.m_pathLightingNodePrefab = _templateMoveAction.m_pathLightingNodePrefab;
                moveAction.m_moveSpeed = _templateMoveAction.m_moveSpeed;

                moveAction.LastLightingNodePosition = _playerProperty.transform.position;
                moveAction.m_actionPoint = 0;
                moveAction.m_actionTimeFrame = 0;

                _currentAction = moveAction;
                m_actions.AddLast(moveAction);
            }

            moveAction.m_actionTimeFrame++;
            moveAction.m_actionPoint += _templateMoveAction.m_actionPoint;
            _currentActionPoint += _templateMoveAction.m_actionPoint;


            Vector3 posToAdd = _playerProperty.transform.position;
            if ((posToAdd - moveAction.LastLightingNodePosition).magnitude > 0.2f)
            {
                moveAction.LastLightingNodePosition = posToAdd;
                var lightingNode = UbhObjectPool.Instance.GetGameObject(moveAction.m_pathLightingNodePrefab, posToAdd, Quaternion.identity);
                lightingNode.GetComponent<CircleCollider2D>().enabled = false;
                moveAction.m_pathLightingNodes.Add(lightingNode);
            }
            moveAction.m_pathNodes.Add(posToAdd);

            return;
        }


        // Handle the change state action
        if(Input.GetButtonDown("Change State"))
        {
            if (!CheckActionPointFill(_templateChangeStateAction)) return;

            var changeStateAction = ScriptableObject.CreateInstance<ChangeStagePauseAction>();

            changeStateAction.m_actionPoint = _templateChangeStateAction.m_actionPoint;
            changeStateAction.m_actionTimeFrame = _templateChangeStateAction.m_actionTimeFrame;

            m_actions.AddLast(changeStateAction);
            _currentAction = changeStateAction;
            _currentActionPoint += changeStateAction.m_actionPoint;

            if (_playerProperty.m_playerState == PlayerProperty.PlayerStateType.Black)
                _playerProperty.m_playerState = PlayerProperty.PlayerStateType.White;
            else
                _playerProperty.m_playerState = PlayerProperty.PlayerStateType.Black;

            return;
        }


        // Handle the action roll back
        if(Input.GetButtonDown("Roll Back Action"))
        {
            if (m_actions.Count != 0)
            {
                var lastAction = m_actions.Last.Value;
                m_actions.RemoveLast();
                _currentActionPoint -= lastAction.m_actionPoint;

                lastAction.RollBackAction(_playerProperty);
            }
            _currentAction = null;
            return ;
        }

    }



    private void ExcuteAction()
    {
        if (_completeAction)
        {
            _currentActionPoint -= _currentAction.m_actionPoint;

            if (m_actions.Count == 0)
            {
                _currentAction = null;
                EndSuperState();
                return;
            }
            else
            {
                _currentAction = m_actions.First.Value;
                m_actions.RemoveFirst();
            }
        }

        _completeAction = _currentAction.ExcuteAction(_playerProperty);
    }


    private bool CheckActionPointFill(BasePauseAction actionToTake)
    {
        if((CurrentActionPoint + actionToTake.m_actionPoint) > TotalActionPoint)
        {
            return false;
        }

        return true;
    }
    
}
