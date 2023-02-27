using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �׽�Ʈ ����� Ŭ����
/// </summary>
public class TestHelper : MonoBehaviour
{
    /// <summary>
    /// �� �׽�Ʈ ���� �Է� �ʵ�
    /// </summary>
    public InputField[] inputFields;
    /// <summary>
    /// ���� ��� ���� ��ư
    /// </summary>
    public Button saveMode_button;
    /// <summary>
    /// ���� ��� ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text saveMode_text;
    /// <summary>
    /// ���� �׽�Ʈ ��� ��ư
    /// </summary>
    public Button statTestMode_button;
    /// <summary>
    /// ���� �׽�Ʈ ��� �ؽ�Ʈ
    /// </summary>
    public Text statTestMode_text;

    /// <summary>
    /// �׽�Ʈ�� ��ġ ���� �׷�
    /// </summary>
    public GameObject testData;
    /// <summary>
    /// ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text rocketDirection_text;
    /// <summary>
    /// ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text height_text;
    /// <summary>
    /// ���� ȹ�淮 �ؽ�Ʈ
    /// </summary>
    public Text addingScore_text;
    /// <summary>
    /// ���� �ӵ� �ؽ�Ʈ
    /// </summary>
    public Text velocity_text;
    /// <summary>
    /// ���� �ð� �ؽ�Ʈ
    /// </summary>
    public Text flyTime_text;

    /// <summary>
    /// ���� �ð�
    /// </summary>
    private float flyTime = 0;

    /// <summary>
    /// �׽�Ʈ �޴� ������Ʈ
    /// </summary>
    public GameObject testMenu;
    /// <summary>
    /// �׽�Ʈ ���̽� �޴� ������Ʈ
    /// </summary>
    public GameObject testCase;

    public LevelManager levelManager;
    public DataManager dataManager;
    public StatManager statManager;
    public ItemManager itemManager;
    public BackgroundControl backgroundControl;

    /// <summary>
    /// �׽�Ʈ�� ��ġ ���� Ȱ��ȭ ����
    /// </summary>
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

    /// <summary>
    /// �׽�Ʈ ��ġ ������ ������Ʈ
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

    /// <summary>
    /// �׽�Ʈ �޴� Ȱ��ȭ
    /// </summary>
    public void OpenTestMenu()
    {
        testMenu.SetActive(true);
    }

    /// <summary>
    /// �׽�Ʈ �޴� ��Ȱ��ȭ
    /// </summary>
    public void CloseTestMenu()
    {
        testMenu.SetActive(false);
    }

    /// <summary>
    /// ��� ���� �׽�Ʈ
    /// </summary>
    public void BackgroundColorTest()
    {
        backgroundControl.ChangeBackgroundColor(Universe.instance.milkyWayBackgroundColor, 10.0f);
    }

    /// <summary>
    /// �Էµ� ��ġ ��ŭ ���� �߰�
    /// </summary>
    public void CoinAdd()
    {
        EconomicMgr.instance.AddCoin(int.Parse(inputFields[0].text));
        inputFields[0].text = "";
    }

    /// <summary>
    /// �Էµ� ��ġ ��ŭ ����ġ �߰�
    /// </summary>
    public void EXPAdd()
    {
        levelManager.AddExp(int.Parse(inputFields[1].text));
        levelManager.StartGageEffect();
        inputFields[1].text = "";
    }

    /// <summary>
    /// �Էµ� ��ġ ��ŭ ���� �߰�
    /// </summary>
    public void LevelAdd()
    {
        levelManager.AddLevel(int.Parse(inputFields[2].text));
        inputFields[2].text = "";
        levelManager.SetLevelText();
    }

    /// <summary>
    /// �Էµ� ��ġ ��ŭ ���� �߰�
    /// </summary>
    public void ScoreAdd()
    {
        PlayManager.instance.AddScore(int.Parse(inputFields[3].text));
        inputFields[3].text = "";
    }

    /// <summary>
    /// ���� ��� ����
    /// </summary>
    public void SetSaveMode()
    {
        DataManager.isSaveMode = !DataManager.isSaveMode;
        if(DataManager.isSaveMode)
        {
            saveMode_button.image.color = Color.green;
            saveMode_text.text = "������ ON";
        }
        else
        {
            saveMode_button.image.color = Color.red;
            saveMode_text.text = "������ OFF";
        }
    }

    /// <summary>
    /// ���� �׽�Ʈ ��� ����
    /// </summary>
    public void SetStatTestMode()
    {
        statManager.isTestMode = true;
        statManager.StatTestMode();

        if(statManager.isTestMode)
        {
            statTestMode_button.image.color = Color.green;
            statTestMode_text.text = "�������� ����!";

            if (SceneManager.GetActiveScene().buildIndex.Equals(1))
                statManager.ApplyAllAbility();
            statManager.InitializeAbilityText();
        }
        else
        {
            statManager.androidController.ShowMessage("reload �ϸ� ���� �׽�Ʈ ��尡 Ǯ���ϴ�.");
        }
    }
    /// <summary>
    /// ��ü ������ �ʱ�ȭ
    /// </summary>
    public void ResetAll()
    {
        dataManager.ResetData();
    }

    /// <summary>
    /// �׽�Ʈ ���̽� �޴� on/off
    /// </summary>
    /// <param name="open">Ȱ��ȭ ����</param>
    public void OpenTestCase(bool open)
    {
        testCase.SetActive(open);
    }

    /// <summary>
    /// �׽�Ʈ ���̽� ����
    /// </summary>
    /// <param name="cases">�׽�Ʈ ���̽�</param>
    public void SetTestCase(int cases)
    {
        switch(cases)
        {
            // ��ü �ʱ�ȭ
            case 0:
                ResetAll();
                break;
            // 1�� ���� ����, ���� ����, ����/�Ŀ� ���� ����
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
            // ��� ������, ����, ���� ����
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
    /// ������ ����
    /// </summary>
    public void SaveAll()
    {
        dataManager.SaveData();
    }

    /// <summary>
    /// �� ���� ��ħ
    /// </summary>
    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
