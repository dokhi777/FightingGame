using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanelController : MonoBehaviour
{
    public GameObject optionPanel; // �ɼ� Panel ����

    // �ɼ� Panel ��Ȱ��ȭ
    public void CloseOptionPanel()
    {
        optionPanel.SetActive(false);
    }
}