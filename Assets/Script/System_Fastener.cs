using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_Fastener : MonoBehaviour
{
    // ’·‰Ÿ‚µ‚Æ”»’è‚³‚ê‚éŽžŠÔ
    private float longPress_time = 0.5f;
    // “ü—ÍŽžŠÔ
    private float press_time = 0.0f;
    // “ü—Í”»’èƒtƒ‰ƒO
    private bool isPressing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            press_time = Time.time;
            isPressing = true;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            float heldDuration=Time.time- press_time;
            isPressing = true;

            if(heldDuration < longPress_time)
            {
                Debug.Log("short press");
                ShortPressAction();
            }
            else
            {
                Debug.Log("long press");
                LongPressAction();
            }
        }
    }

    // ’Z‰Ÿ‚µˆ—
    void ShortPressAction()
    {

    }
    
    // ’·‰Ÿ‚µˆ—
    void LongPressAction()
    {

    }
}
