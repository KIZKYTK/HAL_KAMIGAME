//ozaki
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Ground : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.right;  // ˆÚ“®•ûŒü
    public float moveDistance = 5f;               // ˆÚ“®‹——£
    public float moveSpeed = 2f;                  // ˆÚ“®‘¬“x

    private Vector3 startPosition;
    private bool movingForward = true;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float moveStep = moveSpeed * Time.deltaTime;

        if (movingForward)
        {
            transform.position += moveDirection.normalized * moveStep;
            if (Vector3.Distance(startPosition, transform.position) >= moveDistance)
            {
                movingForward = false;
            }
        }
        else
        {
            transform.position -= moveDirection.normalized * moveStep;
            if (Vector3.Distance(startPosition, transform.position) <= 0.1f)
            {
                movingForward = true;
            }
        }
    }

}
