using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using Firebase.Auth;

public class MainMenuControls : MonoBehaviour
{
    public InternetConnectionControls internetConnectionControls;
    public GameObject emailCanvas;
    public GameObject statisticsButton;
    public GameObject accountButton;
    public GameObject playButton;
    public GameObject duelButton;
    public GameObject exitButton;
    public GameObject loadingText;
    public GameObject loadingCanvas;

    private void Awake()
    {
        Debug.Log("Awake");
    }

    public void PlayPressed()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            if (!internetConnectionControls.IsInternetConnection())
            {
                internetConnectionControls.ShowInternetConnectionErrorToast();
                return;
            }

            loadingCanvas.SetActive(true);
            SceneManager.LoadSceneAsync("Game");
        }
        else
        {
            emailCanvas.SetActive(true);
            UnityAndroidExtras.instance.makeToast("Пожалуйста, авторизуйтесь, чтобы играть!", 0);
        }
    }

    public void DuelClubPressed()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            if (!internetConnectionControls.IsInternetConnection())
            {
                internetConnectionControls.ShowInternetConnectionErrorToast();
                return;
            }

            loadingCanvas.SetActive(true);
            SceneManager.LoadSceneAsync("DuelClub");
        }
        else
        {
            UnityAndroidExtras.instance.makeToast("Пожалуйста, авторизуйтесь, чтобы войти в дауэльный клуб!", 0);
        }
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}
