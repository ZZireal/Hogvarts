using UnityEngine;

public class InternetConnectionControls : MonoBehaviour
{
    public bool IsInternetConnection()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public void ShowInternetConnectionErrorToast()
    {
        UnityAndroidExtras.instance.makeToast("Ошибка! Проверьте интернет-соединение и попробуйте снова.", 0);
    }

    public void ShowInternetConnectionErrorToast(string message)
    {
        UnityAndroidExtras.instance.makeToast(message, 0);
    }
}
