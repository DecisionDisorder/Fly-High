using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingGameManager : MonoBehaviour
{
    public static ParkingGameManager instance;

    public Canvas tabletCanvas;
    public Animation tablet_ani;

    public GameObject successApp;
    public GameObject FailedApp;
    public GameObject settingsApp;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void OpenTablet()
    {
        OpenAni(tablet_ani, "OpenTablet", tabletCanvas);
    }

    public void CloseTablet()
    {
        CloseAni(tablet_ani, "OpenTablet", tabletCanvas);
    }

    public void StageFailed()
    {
        OpenTablet();
        FailedApp.SetActive(true);
    }

    public void StageSuccess()
    {
        OpenTablet();
        successApp.SetActive(true);
    }

    public void Pause()
    {
        OpenTablet();
        settingsApp.SetActive(true);
    }

    public void Resume()
    {
        CloseTablet();
        settingsApp.SetActive(false);
    }

    private void OpenAni(Animation ani, string aniName, Canvas cvs, bool aniIsMain = true)
    {
        cvs.enabled = true;
        ani[aniName].speed = 1;
        if (aniIsMain)
            ani.Play();
        else
            ani.Play(aniName);
    }
    private void CloseAni(Animation ani, string aniName, Canvas cvs, bool aniIsMain = true)
    {
        ani[aniName].speed = -1;
        ani[aniName].time = ani[aniName].length;
        ani.Play();
        if (aniIsMain)
        {
            ani.Play();
            PlayManager.instance.WaitAnimation(ani, AfterCloseCanvas, cvs);
        }
        else
        {
            ani.Play(aniName);
            PlayManager.instance.WaitAnimation(ani, AfterCloseCanvas, cvs);
        }
    }
    private void AfterCloseCanvas(Canvas cvs)
    {
        cvs.enabled = false;
    }
}
