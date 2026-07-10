using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DebugEventPlay : MonoBehaviour
{
    EventManager eventManager;

    // Start is called before the first frame update
    void Start()
    {
        eventManager = EventManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if(eventManager.GetDoingEvent() == true)
            {
                Debug.Log("イベント中です");
                return;
            }
            eventManager.StartEvent(Random.Range(0,2));
        }
    }
}
