using System.Collections;
using UnityEngine;

/// <summary>
/// 진동 효과 도우미 클래스
/// </summary>
public static class Vibration
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass AndroidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject AndroidcurrentActivity = AndroidPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject AndroidVibrator = AndroidcurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
    /// <summary>
    /// 진동 시작
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
    /// 특정 시간 동안 진동
    /// </summary>
    /// <param name="milliseconds">진동 유지 시간</param>
    public static void Vibrate(long milliseconds)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", milliseconds);
#else
        Handheld.Vibrate();
#endif
    }
    /// <summary>
    /// 특정 패턴으로 진동
    /// </summary>
    /// <param name="pattern">패턴</param>
    /// <param name="repeat">반복 횟수</param>
    public static void Vibrate(long[] pattern, int repeat)
    {


#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", pattern, repeat);
#else
        Handheld.Vibrate();
#endif
    }

    /// <summary>
    /// 진동 취소
    /// </summary>
    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrator.Call("cancel");
#endif
    }
}
