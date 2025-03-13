using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager.LoadSceneAsync("Battle2", LoadSceneMode.Single);
    }
}
