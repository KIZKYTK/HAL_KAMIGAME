using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class System_Camera : MonoBehaviour
{
    public float baseVerticalSize = 10f; // ��ƂȂ�T�C�Y�i�����j
    public float baseAspect = 16f / 9f;  // ��̃A�X�y�N�g��i��/�c�j

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

        // �c�����̃T�C�Y���A�X�y�N�g��ɉ����Ē���
        cam.orthographicSize = baseVerticalSize * scaleFactor;
    }
}
