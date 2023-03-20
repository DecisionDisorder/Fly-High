using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// 테스트 도우미 클래스
/// </summary>
public class TestHelper : MonoBehaviour
{
    /// <summary>
    /// 각 테스트 값의 입력 필드
    /// </summary>
    public InputField[] inputFields;
    /// <summary>
    /// 저장 모드 설정 버튼
    /// </summary>
    public Button saveMode_button;
    /// <summary>
    /// 저장 모드 현재 상태 텍스트
    /// </summary>
    public Text saveMode_text;
    /// <summary>
    /// 스탯 테스트 모드 버튼
    /// </summary>
    public Button statTestMode_button;
    /// <summary>
    /// 스탯 테스트 모드 텍스트
    /// </summary>
    public Text statTestMode_text;

    /// <summary>
    /// 테스트용 수치 정보 그룹
    /// </summary>
    public GameObject testData;
    /// <summary>
    /// 로켓 방향 텍스트
    /// </summary>
    public Text rocketDirection_text;
    /// <summary>
    /// 로켓 높이 텍스트
    /// </summary>
    public Text height_text;
    /// <summary>
    /// 점수 획득량 텍스트
    /// </summary>
    public Text addingScore_text;
    /// <summary>
    /// 로켓 속도 텍스트
    /// </summary>
    public Text velocity_text;
    /// <summary>
    /// 비행 시간 텍스트
    /// </summary>
    public Text flyTime_text;

    /// <summary>
    /// 비행 시간
    /// </summary>
    private float flyTime = 0;

    /// <summary>
    /// 테스트 메뉴 오브젝트
    /// </summary>
    public GameObject testMenu;
    /// <summary>
    /// 테스트 케이스 메뉴 오브젝트
    /// </summary>
    public GameObject testCase;

    public LevelManager levelManager;
    public DataManager dataManager;
    public StatManager statManager;
    public ItemManager itemManager;
    public BackgroundControl backgroundControl;

    /// <summary>
    /// 테스트용 수치 정보 활성화 여부
    /// </summary>
    public bool testDisplayMode;

    public string folderName = "ScreenShots";
    public string fileName = "ScreenShot";
    public string extName = "png";
    private string RootPath { get { return Application.dataPath; } }
    private string FolderPath => $"{RootPath}/{folderName}";
    private string TotalPath => $"{FolderPath}/{fileName}_{DateTime.Now.ToString("MMdd_HHmmss")}.{extName}";

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            if (testDisplayMode)
            {
                testData.SetActive(true);
                StartCoroutine(TestDataUpdate());
            }
            else
                testData.SetActive(false);
        }

        StartCoroutine(InputUpdate());
    }

    IEnumerator InputUpdate()
    {
        yield return new WaitForEndOfFrame();

        if (Input.GetKey(KeyCode.F8))
            Screenshot();

        StartCoroutine(InputUpdate());
    }

    /// <summary>
    /// 테스트 수치 데이터 업데이트
    /// </summary>
    IEnumerator TestDataUpdate()
    {
        yield return new WaitForFixedUpdate();

        rocketDirection_text.text = string.Format("Direction: x:{0}, y:{1}", RocketSet.instance.rocketMain_rigid.velocity.x, RocketSet.instance.rocketMain_rigid.velocity.y);
        height_text.text = "Height: " + RocketSet.instance.rocket.transform.position.y;
        addingScore_text.text = "Score/0.1s: " + PlayManager.instance.deltaScore;
        velocity_text.text = string.Format("Velocity: {0:#,##0.00}", RocketSet.instance.rocketMain_rigid.velocity.magnitude);
        if(Launch.instance.isStart && PlayManager.isSurvive)
            flyTime_text.text = "Flying Time: " + string.Format("{0:0.00}", flyTime += Time.deltaTime);

        StartCoroutine(TestDataUpdate());
    }

    private void Screenshot()
    {
        string totalPath = TotalPath;
        Texture2D screenTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Rect area = new Rect(0f, 0f, Screen.width, Screen.height);
        screenTex.ReadPixels(area, 0, 0);
        try
        {
            if (Directory.Exists(FolderPath) == false)
            {
                Directory.CreateDirectory(FolderPath);
            }

            File.WriteAllBytes(totalPath, screenTex.EncodeToPNG());
            Debug.Log($"Screen Shot Saved : {totalPath}");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Screen Shot Save Failed : {totalPath}");
            Debug.LogWarning(ex);
        }


    }

    /// <summary>
    /// 테스트 메뉴 활성화
    /// </summary>
    public void OpenTestMenu()
    {
        testMenu.SetActive(true);
    }

    /// <summary>
    /// 테스트 메뉴 비활성화
    /// </summary>
    public void CloseTestMenu()
    {
        testMenu.SetActive(false);
    }

    /// <summary>
    /// 배경 색상 테스트
    /// </summary>
    public void BackgroundColorTest()
    {
        backgroundControl.ChangeBackgroundColor(Universe.instance.milkyWayBackgroundColor, 10.0f);
    }

    /// <summary>
    /// 입력된 수치 만큼 코인 추가
    /// </summary>
    public void CoinAdd()
    {
        EconomicMgr.instance.AddCoin(int.Parse(inputFields[0].text));
        inputFields[0].text = "";
    }

    /// <summary>
    /// 입력된 수치 만큼 경험치 추가
    /// </summary>
    public void EXPAdd()
    {
        levelManager.AddExp(int.Parse(inputFields[1].text));
        levelManager.StartGageEffect();
        inputFields[1].text = "";
    }

    /// <summary>
    /// 입력된 수치 만큼 레벨 추가
    /// </summary>
    public void LevelAdd()
    {
        levelManager.AddLevel(int.Parse(inputFields[2].text));
        inputFields[2].text = "";
        levelManager.SetLevelText();
    }

    /// <summary>
    /// 입력된 수치 만큼 점수 추가
    /// </summary>
    public void ScoreAdd()
    {
        PlayManager.instance.AddScore(int.Parse(inputFields[3].text));
        inputFields[3].text = "";
    }

    /// <summary>
    /// 저장 모드 설정
    /// </summary>
    public void SetSaveMode()
    {
        DataManager.isSaveMode = !DataManager.isSaveMode;
        if(DataManager.isSaveMode)
        {
            saveMode_button.image.color = Color.green;
            saveMode_text.text = "저장모드 ON";
        }
        else
        {
            saveMode_button.image.color = Color.red;
            saveMode_text.text = "저장모드 OFF";
        }
    }

    /// <summary>
    /// 스탯 테스트 모드 설정
    /// </summary>
    public void SetStatTestMode()
    {
        statManager.isTestMode = true;
        statManager.StatTestMode();

        if(statManager.isTestMode)
        {
            statTestMode_button.image.color = Color.green;
            statTestMode_text.text = "만랩스탯 적용!";

            if (SceneManager.GetActiveScene().buildIndex.Equals(1))
                statManager.ApplyAllAbility();
            statManager.InitializeAbilityText();
        }
        else
        {
            statManager.androidController.ShowMessage("reload 하면 스탯 테스트 모드가 풀립니다.");
        }
    }
    /// <summary>
    /// 전체 데이터 초기화
    /// </summary>
    public void ResetAll()
    {
        dataManager.ResetData();
    }

    /// <summary>
    /// 테스트 케이스 메뉴 on/off
    /// </summary>
    /// <param name="open">활성화 여부</param>
    public void OpenTestCase(bool open)
    {
        testCase.SetActive(open);
    }

    /// <summary>
    /// 테스트 케이스 선택
    /// </summary>
    /// <param name="cases">테스트 케이스</param>
    public void SetTestCase(int cases)
    {
        switch(cases)
        {
            // 전체 초기화
            case 0:
                ResetAll();
                break;
            // 1단 로켓 제공, 코인 제공, 방향/파워 스탯 제공
            case 1:
                ResetAll();
                for (int i = 0; i <= (int)RocketType.Powerful; i++)
                    itemManager.rocketDataSet[i].itemData.hasItem = true;
                EconomicMgr.instance.AddCoin(100000);
                levelManager.AddLevel(2);
                StatSelect statSelect = new StatSelect();
                statSelect.statType = StatType.Accelerate;
                statManager.StatLevelUp(statSelect);
                break;
            // 모든 아이템, 로켓, 레벨 제공
            case 2:
                for (int i = 0; i <= (int)RocketType.Ordinary; i++)
                    itemManager.rocketDataSet[i].itemData.hasItem = true;
                EconomicMgr.instance.AddCoin(1000000);
                SetStatTestMode();
                break;
        }
        dataManager.SaveData();
        Reload();
    }

    /// <summary>
    /// 데이터 저장
    /// </summary>
    public void SaveAll()
    {
        dataManager.SaveData();
    }

    /// <summary>
    /// 씬 새로 고침
    /// </summary>
    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
