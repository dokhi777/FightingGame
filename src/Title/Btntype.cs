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
                SceneManager.LoadSceneAsync("Character", LoadSceneMode.Single); // 캐릭터 선택 씬으로 이동
                break;

            case ButtonType.OptionBtn:
                optionPanel.SetActive(true);    // 옵션 Panel 활성화
                break;

            case ButtonType.QuitBtn:
                Application.Quit(); // 게임 종료
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
