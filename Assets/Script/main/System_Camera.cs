using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class System_Camera : MonoBehaviour
{
    public float baseVerticalSize = 10f; // 基準となるサイズ（高さ）
    public float baseAspect = 16f / 9f;  // 基準のアスペクト比（横/縦）

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        UpdateCameraSize();
    }

    void Update()
    {
        UpdateCameraSize();
    }

    void UpdateCameraSize()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        float currentAspect = (float)Screen.width / Screen.height;
        float scaleFactor = baseAspect / currentAspect;

        // 縦方向のサイズをアスペクト比に応じて調整
        cam.orthographicSize = baseVerticalSize * scaleFactor;
    }
}
