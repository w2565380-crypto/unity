using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    public float rotateSpeed = 360f; // 每秒旋转的角度

    void Update()
    {
        // 围绕自身 Z 轴旋转
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}