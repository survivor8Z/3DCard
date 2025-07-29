using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector2 cameraViewOffset;
    [SerializeField] private Vector2 mouseScreenPos;
    [SerializeField] private float cameraViewKX;
    [SerializeField] private float cameraViewKY;
    [SerializeField] private Transform viewDirPoint;
    void OnMousePosition(InputValue value)
    {
        mouseScreenPos = value.Get<Vector2>();
        cameraViewOffset = new Vector2(
            2*(mouseScreenPos.x / Screen.width - 0.5f),
            2*(mouseScreenPos.y / Screen.height - 0.5f)
        );
        //这里直接硬编码了
        viewDirPoint.localPosition = new Vector3(0 + cameraViewOffset.x * cameraViewKX,
                                            1.45f + cameraViewOffset.y * cameraViewKY,
                                            1);
    }
}
