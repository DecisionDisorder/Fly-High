using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestHelper : MonoBehaviour
{
    public InputField[] inputFields;
    public Button saveMode_button;
    public Text saveMode_text;
    public Button statTestMode_button;
    public Text statTestMode_text;

    public GameObject testData;
    public Text rocketDirection_text;
    public Text height_text;
    public Text addingScore_text;
    public Text velocity_text;
    public Text flyTime_text;

    private float flyTime = 0;

    public GameObject testMenu;
    public GameObject testCase;

    public LevelManager levelManager;
    public DataManager dataManager;
    public StatManager statManager;
    public ItemManager itemManager;
    public BackgroundControl backgroundControl;

    public bool testDisplayMode;

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
    }

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

    public void OpenTestMenu()
    {
        testMenu.SetActive(true);
    }

    public void CloseTestMenu()
    {
        testMenu.SetActive(false);
    }

    public void BackgroundColorTest()
    {
        backgroundControl.ChangeBackgroundColor(Universe.instance.milkyWayBackgroundColor, 10.0f);
    }

    public void CoinAdd()
    {
        EconomicMgr.instance.AddCoin(int.Parse(inputFields[0].text));
        inputFields[0].text = "";
    }

    public void EXPAdd()
    {
        levelManager.AddExp(int.Parse(inputFields[1].text));
        levelManager.StartGageEffect();
        inputFields[1].text = "";
    }

    public void LevelAdd()
    {
        levelManager.AddLevel(int.Parse(inputFields[2].text));
        inputFields[2].text = "";
        levelManager.SetLevelText();
    }

    public void ScoreAdd()
    {
        PlayManager.instance.AddScore(int.Parse(inputFields[3].text));
        inputFields[3].text = "";
    }

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
    public void ResetAll()
    {
        dataManager.ResetData();
    }

    public void OpenTestCase(bool open)
    {
        testCase.SetActive(open);
    }

    public void SetTestCase(int cases)
    {
        switch(cases)
        {
            case 0:
                ResetAll();
                break;
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

    public void SaveAll()
    {
        dataManager.SaveData();
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
