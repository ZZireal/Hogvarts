using UnityEngine;

public class MultiplayerConnectionControls : MonoBehaviour
{
    public void ShowMultiplayerConnectionErrorToast()
    {
        UnityAndroidExtras.instance.makeToast("Ошибка!", 0);
    }

    public void ShowMultiplayerConnectionErrorToast(string message)
    {
        UnityAndroidExtras.instance.makeToast(message, 0);
    }
}