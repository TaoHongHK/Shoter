using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnterDestroy : MonoBehaviour {

    public string m_triggerTag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(m_triggerTag))
        {
            UbhObjectPool.Instance.ReleaseGameObject(this.gameObject);
        }
    }
}
