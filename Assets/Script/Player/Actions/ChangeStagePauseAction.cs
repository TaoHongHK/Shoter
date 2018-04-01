using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Change Stage Pause Action Data", menuName = "Pause Action Data/Change Stage")]
public class ChangeStagePauseAction : BasePauseAction
{
    public override Type ActionType
    {
        get
        {
            return this.GetType();
        }
    }

    public override bool ExcuteAction(PlayerProperty playerProperty)
    {
        CurrentTimeFrame++;

        if(CurrentTimeFrame == (m_actionTimeFrame / 2))
        {
            var playerChangeStateComponent = playerProperty.GetComponent<PlayerChangeState>();

            if (playerChangeStateComponent == null)
            {
                Debug.LogError("Can not change state because the PlayerChangeState component is not add to player!");
                return false;
            }

            playerChangeStateComponent.ChangeState();
        }

        return CurrentTimeFrame > m_actionTimeFrame;
    }

    public override bool RollBackAction(PlayerProperty playerProperty)
    {
        var playerChangeStateComponent = playerProperty.GetComponent<PlayerChangeState>();

        if (playerChangeStateComponent == null)
        {
            Debug.LogError("Can not roll back because the PlayerChangeState component is not add to player!");
            return false;
        }

        playerChangeStateComponent.ChangeState();

        return true;
    }

}
