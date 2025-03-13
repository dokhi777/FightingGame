using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanelController : MonoBehaviour
{
    public GameObject optionPanel; // 可记 Panel 楷搬

    // 可记 Panel 厚劝己拳
    public void CloseOptionPanel()
    {
        optionPanel.SetActive(false);
    }
}