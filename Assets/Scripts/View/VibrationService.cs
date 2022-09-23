using Game.Services;
using UnityEngine;

public class VibrationManager : IService
{
    private int               HapticFeedbackConstantsKey;
    private AndroidJavaObject UnityPlayer;

    public void Initialize()
    {
        if (Application.isEditor)
            return;

        HapticFeedbackConstantsKey =
            new AndroidJavaClass("android.view.HapticFeedbackConstants").GetStatic<int>("VIRTUAL_KEY");

        UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer");
    }

    public void Vibrate()
    {
        if (Application.isEditor)
        {
            Debug.Log("Vibrate brrrrr");
            return;
        }

        UnityPlayer.Call<bool>("performHapticFeedback", HapticFeedbackConstantsKey);
    }

    public void Clear()
    {
    }
}