using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move Pause Action Data", menuName = "Pause Action Data/Move")]
public class MovePauseAction : BasePauseAction
{
    public float m_moveSpeed;

    public GameObject m_pathLightingNodePrefab;

    public List<Vector3> m_pathNodes;

    public List<GameObject> m_pathLightingNodes;

    private float _currentMoveFrameFloat = 0;

    private Transform _playerSpriteTrans;

    public override System.Type ActionType
    {
        get
        {
            return this.GetType();
        }
    }


    public Vector3 LastLightingNodePosition
    {
        get; set;
    }

    public MovePauseAction() : base()
    {
        m_pathNodes = new List<Vector3>();
        m_pathLightingNodes = new List<GameObject>();
    }

    public MovePauseAction(int actionPoint, int actionTimeFrame) : base(actionPoint, actionTimeFrame)
    {
        m_pathNodes = new List<Vector3>();
        m_pathLightingNodes = new List<GameObject>();
    }


    public override bool ExcuteAction(PlayerProperty playerProperty)
    {
        if (CurrentTimeFrame == 0)
        {
            foreach (var pathLightingNode in m_pathLightingNodes)
            {
                pathLightingNode.GetComponent<CircleCollider2D>().enabled = true;
            }                                                                    
            _playerSpriteTrans = playerProperty.m_spriteReference.transform;
        }

        _currentMoveFrameFloat += m_moveSpeed;
        CurrentTimeFrame = ((int)_currentMoveFrameFloat) + 1;

        if(CurrentTimeFrame < m_actionTimeFrame)
        {
            Vector3 preNode = m_pathNodes[CurrentTimeFrame - 1];
            Vector3 nextNode = m_pathNodes[CurrentTimeFrame];
            Vector3 currentNode = Vector3.Lerp(preNode, nextNode, _currentMoveFrameFloat - CurrentTimeFrame);

            float lookAngle = UbhUtil.GetAngleFromTwoPosition(preNode, nextNode);

            playerProperty.transform.position = currentNode;
            _playerSpriteTrans.transform.rotation = Quaternion.Euler(0, 0, lookAngle);

            return false;
        }
        else
        {
            playerProperty.transform.position = m_pathNodes[m_pathNodes.Count - 1];
            return true;
        }

        
    }

    public override bool RollBackAction(PlayerProperty playerProperty)
    {                         
        playerProperty.transform.position = m_pathNodes[0];
                         
        foreach(var pathLightingNode in m_pathLightingNodes)
        {
            UbhObjectPool.Instance.ReleaseGameObject(pathLightingNode);
        }

        m_pathNodes.Clear();

        return true;
    }
}
