//ozaki
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public float moveSpeed = 2f;  // 移動速度
    private bool isPlayerOn = false;  // プレイヤーが乗っているか

    void Update()
    {
        if (isPlayerOn)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    // プレイヤーが乗ったらフラグを立てる
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOn = true;
        }
    }

    // プレイヤーが降りたらフラグを下げる（必要なら）
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOn = false;
        }
    }
}