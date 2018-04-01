using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePauseAction: ScriptableObject
{
    // The behavior point that an action will consume
    public int m_actionPoint;

    // How much frame that this action will use
    public int m_actionTimeFrame;

    protected int CurrentTimeFrame = 0;

    public virtual System.Type ActionType
    {
        get
        {
            return this.GetType();
        }
    }

    public BasePauseAction(int actionPoint, int actionTimeFrame)
    {
        m_actionPoint = actionPoint;
        m_actionTimeFrame = actionTimeFrame;
        CurrentTimeFrame = 0;
    }

    public BasePauseAction()
    {
        m_actionPoint = m_actionTimeFrame = CurrentTimeFrame = 0;
    }

    // Return true if the action is done
    public virtual bool ExcuteAction(PlayerProperty playerProperty)
    {
        return false;    
    }

    public virtual bool RollBackAction(PlayerProperty playerProperty)
    {
        return false;    
    }

}
