using System;
using UnityEngine;
using UnityEngine.UI;

using Firebase.Auth;
using Firebase.Database;

public class AccountControls : MonoBehaviour
{
    public InternetConnectionControls internetConnectionControls;
    public StatisticsControls statisticsControls;
    public GameObject canvasAccount;
    public GameObject canvasEmail;

    public InputField signEmailField;
    public InputField signPasswordField;

    public Text helloText;

    void Start()
    {
        SetHelloTextCurrentUserEmail();
    }

    void ShowToast(string message)
    {
        UnityAndroidExtras.instance.makeToast(message, 0);
    }

    void ShowErrorToast()
    {
        UnityAndroidExtras.instance.makeToast("Ошибка! Проверьте правильность введенных данных и попробуйте снова.", 0);
    }

    public string GetCurrentUserEmail()
    {
        return FirebaseAuth.DefaultInstance.CurrentUser.Email;
    }

    bool IsUserLogined()
    {
        return FirebaseAuth.DefaultInstance.CurrentUser != null;
    }

    public void SetHelloTextCurrentUserEmail()
    {
        if (IsUserLogined())
        {
            helloText.text = "Добро пожаловать, " + GetCurrentUserEmail() + "!";
        }
        else
        {
            helloText.text = "Добро пожаловать! Авторизуйтесь, чтобы играть.";
        }
    }

    public void ShowCanvasAccount()
    {
        canvasAccount.SetActive(true);
    }

    public void CloseCanvasAccount()
    {
        canvasAccount.SetActive(false);
    }

    public void ShowCanvasEmail()
    {
        if (IsUserLogined())
        {
            ShowToast("Пожалуйста, выйдите из текущего аккаунта!");
        }
        else
        {
            canvasEmail.SetActive(true);
        }
    }

    public void CloseCanvasEmail()
    {
        canvasEmail.SetActive(false);
    }

    public async void SignUpProcess()
    {
        try
        {
            if (!internetConnectionControls.IsInternetConnection())
            {
                internetConnectionControls.ShowInternetConnectionErrorToast();
                return;
            }

            await FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(signEmailField.text + "", signPasswordField.text + "").ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    ShowErrorToast();
                    return;
                }
                else if (task.IsFaulted)
                {
                    ShowErrorToast();
                    return;
                }
                else if (task.IsCompleted)
                {
                    ShowToast("Вы успешно зарегистрировались, " + task.Result.Email + "!");
                    PrepareNewPlayer();
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
            });
        }
        catch (Exception ex)
        {
            ShowErrorToast();
        }

        signPasswordField.text = "";
    }

    public async void SignInProcess()
    {
        try
        {
            if (!internetConnectionControls.IsInternetConnection())
            {
                internetConnectionControls.ShowInternetConnectionErrorToast();
                return;
            }

            await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(signEmailField.text + "", signPasswordField.text + "").ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    ShowErrorToast();
                    return;
                }
                else if (task.IsFaulted)
                {
                    ShowErrorToast();
                    return;
                }
                else if (task.IsCompleted)
                {
                    statisticsControls.SetUserAndFacultyStatistics();
                    CloseCanvasEmail();
                    CloseCanvasAccount();
                    ShowToast("Вы успешно авторизовались, " + task.Result.Email + "!");
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                SetHelloTextCurrentUserEmail();
            });
        }
        catch (Exception ex)
        {
            ShowErrorToast();
        }
        signEmailField.text = "";
        signPasswordField.text = "";
    }

    public void SignOutProcess()
    {
        if (!IsUserLogined())
        {
            ShowToast("Вы не авторизованы :)");
        }
        else
        {
            try
            {
                FirebaseAuth.DefaultInstance.SignOut();
                ShowToast("Вы успешно вышли из аккаунта!");
                statisticsControls.ResetUserStatisticsUI();
                SetHelloTextCurrentUserEmail();
            }
            catch (Exception)
            {
                ShowErrorToast();
            }
        }
    }

    private async void PrepareNewPlayer()
    {
        await FirebaseDatabase.DefaultInstance.RootReference.Child("players").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).SetRawJsonValueAsync(GetNewPlayerJson());
        await FirebaseDatabase.DefaultInstance.GetReference("faculties").Child("noFaculty")
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  FirebaseDatabase.DefaultInstance.RootReference.Child("faculties").Child("noFaculty").SetRawJsonValueAsync((Convert.ToInt32(snapshot.Value) + 1).ToString());
              }
          });

        if (IsUserLogined()) FirebaseAuth.DefaultInstance.SignOut();
    }

    private string GetNewPlayerJson()
    {
        return JsonUtility.ToJson(new Player("0", "0", "0", "0", "не распределен", "0", "0"));
    }
}
