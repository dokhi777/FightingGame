using System;
using System.Collections.Generic;
using UnityEngine;
public class CameraViewportController : MonoBehaviour
{
    public Camera mainCamera; // ���� ī�޶�
    public Camera secondCamera; // ����ȭ�鿡 ����� �� ��° ī�޶�
    public Camera thirdCamera;  // ����ȭ�鿡 ����� �� ��° ī�޶�

    public void EnableSplitScreen()
    {
        // ȭ�� ���� ����
        mainCamera.gameObject.SetActive(false);

        secondCamera.rect = new Rect(0, 0, 0.5f, 1); // ���� ����
        thirdCamera.rect = new Rect(0.5f, 0, 0.5f, 1); // ������ ����

        secondCamera.gameObject.SetActive(true); // �� ��° ī�޶� Ȱ��ȭ
        thirdCamera.gameObject.SetActive(true); // �� ��° ī�޶� Ȱ��ȭ

        secondCamera.transform.position = new Vector3(-5.03f, 2.19f, -4.32f);
        thirdCamera.transform.position = new Vector3(4.89f, 2.2f, -3.98f);
    }

    public void DisableSplitScreen()
    {
        // ��ü ȭ������ ����
        mainCamera.rect = new Rect(0, 0, 1, 1); // ��ü ȭ��
        secondCamera.gameObject.SetActive(false); // �� ��° ī�޶� ��Ȱ��ȭ
    }
}
