using System;
using System.Collections.Generic;
using UnityEngine;
public class CameraViewportController : MonoBehaviour
{
    public Camera mainCamera; // 메인 카메라
    public Camera secondCamera; // 분할화면에 사용할 두 번째 카메라
    public Camera thirdCamera;  // 분할화면에 사용할 세 번째 카메라

    public void EnableSplitScreen()
    {
        // 화면 분할 설정
        mainCamera.gameObject.SetActive(false);

        secondCamera.rect = new Rect(0, 0, 0.5f, 1); // 왼쪽 절반
        thirdCamera.rect = new Rect(0.5f, 0, 0.5f, 1); // 오른쪽 절반

        secondCamera.gameObject.SetActive(true); // 두 번째 카메라 활성화
        thirdCamera.gameObject.SetActive(true); // 세 번째 카메라 활성화

        secondCamera.transform.position = new Vector3(-5.03f, 2.19f, -4.32f);
        thirdCamera.transform.position = new Vector3(4.89f, 2.2f, -3.98f);
    }

    public void DisableSplitScreen()
    {
        // 전체 화면으로 복구
        mainCamera.rect = new Rect(0, 0, 1, 1); // 전체 화면
        secondCamera.gameObject.SetActive(false); // 두 번째 카메라 비활성화
    }
}
