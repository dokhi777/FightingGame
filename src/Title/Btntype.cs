using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Btntype : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum ButtonType
    {
        StartBtn,
        OptionBtn,
        QuitBtn
    }

    public ButtonType buttonType;
    public Transform buttonScale;
    public GameObject optionPanel;
    Vector3 defaultScale;

    private void Start()
    {
        defaultScale = buttonScale.localScale;
    }

    public void OnButtonClick()
    {
        switch (buttonType)
        {
            case ButtonType.StartBtn:
                SceneManager.LoadSceneAsync("Character", LoadSceneMode.Single); // ĳ���� ���� ������ �̵�
                break;

            case ButtonType.OptionBtn:
                optionPanel.SetActive(true);    // �ɼ� Panel Ȱ��ȭ
                break;

            case ButtonType.QuitBtn:
                Application.Quit(); // ���� ����
                break;

            default:
                Debug.LogError("Unknown buttontype");
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale * 1.3f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }
}
