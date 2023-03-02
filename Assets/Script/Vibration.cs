using System.Collections;
using UnityEngine;

/// <summary>
/// ���� ȿ�� ����� Ŭ����
/// </summary>
public static class Vibration
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass AndroidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject AndroidcurrentActivity = AndroidPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject AndroidVibrator = AndroidcurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
    /// <summary>
    /// ���� ����
    /// </summary>
    public static void Vibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate");
#else
        Handheld.Vibrate();
#endif
    }

    /// <summary>
    /// Ư�� �ð� ���� ����
    /// </summary>
    /// <param name="milliseconds">���� ���� �ð�</param>
    public static void Vibrate(long milliseconds)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", milliseconds);
#else
        Handheld.Vibrate();
#endif
    }
    /// <summary>
    /// Ư�� �������� ����
    /// </summary>
    /// <param name="pattern">����</param>
    /// <param name="repeat">�ݺ� Ƚ��</param>
    public static void Vibrate(long[] pattern, int repeat)
    {


#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", pattern, repeat);
#else
        Handheld.Vibrate();
#endif
    }

    /// <summary>
    /// ���� ���
    /// </summary>
    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrator.Call("cancel");
#endif
    }
}
