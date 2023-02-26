using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �÷��̾� ���� �ý��� ���� Ŭ����
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// ���� ������ 
    /// </summary>
    private LevelData levelData;
    /// <summary>
    /// ���� �� ����ġ �䱸��
    /// </summary>
    public int expReq { get; private set; }
    /// <summary>
    /// �ִ� ����
    /// </summary>
    private int maxLevel;

    /// <summary>
    /// ��å ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text position_text;
    /// <summary>
    /// ���� ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text level_text;
    /// <summary>
    /// ���� ����ġ ������ �ؽ�Ʈ
    /// </summary>
    public Text curExp_text;
    /// <summary>
    /// ���� ����ġ ������ �����̴�(������)
    /// </summary>
    public Slider exp_slider;
    /// <summary>
    /// ����ġ ���� ȿ�� ��� ť
    /// </summary>
    private Queue<IEnumerator> expSliderQueue;

    /// <summary>
    /// �������� �������� �ӵ�
    /// </summary>
    public float gageSpeed;
    /// <summary>
    /// ��å ���� ����ġ
    /// </summary>
    public int[] positionCriteria;

    public StatManager statManager;
    public DataManager dataManager;

    private void Awake()
    {
        // ����ġ ���� ȿ�� ��⿭ �ʱ�ȭ
        expSliderQueue = new Queue<IEnumerator>();
        // ����, ����ġ ���� UI ������Ʈ
        SetLevelText();
        SetExpReq();
        UpdateExpSlider();
    }

    /// <summary>
    /// ���� �� ����ġ �䱸�� ������Ʈ
    /// </summary>
    private void SetExpReq()
    {
        // �ִ� ������ ���, ����ġ �䱸�� 0���� ����
        if (levelData.level >= GetMaxLevel())
            expReq = 0;
        else
        {
            expReq = 1000;// TODO: ���� �ۼ�
            exp_slider.maxValue = expReq;
        }
    }

    /// <summary>
    /// ����ġ ����
    /// </summary>
    /// <param name="add">������ ����ġ ��</param>
    public void AddExp(int add)
    {
        // �ִ� ���� ������ ��쿡�� ����ġ ����
        if (levelData.level < GetMaxLevel())
        {
            // ����ġ ���� ȿ�� ��⿭ ��� �� ����ġ ����
            IEnumerator expGageEffect = ExpGageEffect(levelData.currentExp, levelData.currentExp + add);
            expSliderQueue.Enqueue(expGageEffect);
            levelData.currentExp += add;
            // ������ ���� �˻�
            if (levelData.currentExp >= expReq)
            {
                LevelUp();
            }
            else
            {
                dataManager.SaveData();
            }
            SetLevelText();
        }
    }

    /// <summary>
    /// ������ ó��
    /// </summary>
    private void LevelUp()
    {
        // ���� ����
        levelData.level++;
        // �ִ� ���� ���� ���� �˻�
        if (levelData.level <= GetMaxLevel())
        {
            // ���� ����Ʈ ���� �� ���� �� �䱸 ����ġ�� ������Ʈ
            levelData.totalStatPoint++;
            levelData.unusedStatPoint++;
            //IEnumerator expGageEffect = ExpGageEffect(expReq, levelData.currentExp - expReq);
            levelData.currentExp -= expReq;
            if (levelData.level.Equals(GetMaxLevel()))
            {
                levelData.currentExp = 0;
            }
            else
                SetExpReq();
            // ����ġ ���� ����Ʈ ��⿭ ��� �� ���� �� ���� �� �˻�
            IEnumerator expGageEffect = ExpGageEffect(0, levelData.currentExp);
            expSliderQueue.Enqueue(expGageEffect);
            if (levelData.currentExp >= expReq)
                LevelUp();
            else
                dataManager.SaveData();
        }
        else
        {
            levelData.level = GetMaxLevel();
            levelData.currentExp = 0;
        }
    }

    /// <summary>
    /// (�׽�Ʈ ��) �ٷ��� ���� �߰�
    /// </summary>
    /// <param name="add">���� ������</param>
    public void AddLevel(int add)
    {
        levelData.level += add;
        if (levelData.level > GetMaxLevel())
            levelData.level = GetMaxLevel();
        else
        {
            SetExpReq();
            levelData.totalStatPoint += add;
            levelData.unusedStatPoint += add;
        }
    }

    /// <summary>
    /// ���� ���� UI ������Ʈ
    /// </summary>
    public void SetLevelText()
    {
        if (level_text != null && LocalizationManager.instance != null)
        {
            // ���� UI ������Ʈ
            statManager.SetUnusedStatText();
            // ���� �ؽ�Ʈ ������Ʈ
            SetLevelText(level_text, curExp_text);
            // ��å ���� ������Ʈ
            position_text.text = LocalizationManager.instance.GetLocalizedValue("position_" + GetPositionIndex());
        }
    }

    /// <summary>
    /// ���� �ؽ�Ʈ ������Ʈ
    /// </summary>
    /// <param name="level_text">������Ʈ ��� ���� �ؽ�Ʈ</param>
    /// <param name="curExp_text">����ġ ������ �ؽ�Ʈ</param>
    public void SetLevelText(Text level_text, Text curExp_text)
    {
        // ���� �� ����ġ �䱸�� ������Ʈ
        SetExpReq();
        // �ִ� ������ ���� �ؽ�Ʈ ������Ʈ ó��
        if (levelData.level.Equals(GetMaxLevel()))
        {
            level_text.text = "Level: " + GetMaxLevel() + " (MAX)";
            curExp_text.text = "0 / 0 (0%)";
        }
        // �ִ� ���� �̸��� ���� �ؽ�Ʈ ������Ʈ ó��
        else
        {
            level_text.text = "Level: " + levelData.level;
            curExp_text.text = PlayManager.SetCommaOnDigit(GetCurrentExp()) + " / " + PlayManager.SetCommaOnDigit(expReq) + " (" + PlayManager.SetCommaOnDigit((float)GetCurrentExp() / expReq * 100) + "%)";
        }
    }

    /// <summary>
    /// �÷��̾ ������ ���� ��å �ε����� ���Ѵ�.
    /// </summary>
    /// <returns>�÷��̾� ��å �ε���</returns>
    public int GetPositionIndex()
    {
        for (int i = 0; i < positionCriteria.Length; i++)
        {
            if (levelData.level < positionCriteria[i])
                return i;
        }
        return positionCriteria.Length;
    }

    /// <summary>
    /// ����ġ �����̴� ������Ʈ
    /// </summary>
    private void UpdateExpSlider()
    {
        if(SceneManager.GetActiveScene().buildIndex.Equals(0))
            exp_slider.value = levelData.currentExp;
    }

    /// <summary>
    /// ����ġ ���� ����Ʈ ����
    /// </summary>
    public void StartGageEffect()
    {
        StartCoroutine(expSliderQueue.Dequeue());
    }

    /// <summary>
    /// ����ġ ���� ����Ʈ
    /// </summary>
    /// <param name="from">���� ��ġ</param>
    /// <param name="to">���� ��ġ</param>
    IEnumerator ExpGageEffect(int from, int to)
    {
        yield return new WaitForEndOfFrame();

        float currentPercentage;
        float deltaSpeed;

        // ������ ���
        if (from < to)
        {
            // ���� �ۼ�Ƽ�� ���
            currentPercentage = (exp_slider.value - from) / (to - from) * 100f;

            // �������� 90% �̸��� ��� �Ϲ� �ӵ��� ����
            if (currentPercentage < 90)
                deltaSpeed = (to - from) * gageSpeed * Time.deltaTime;
            // �������� 90%�� ���� ��� 50%�� �ӵ��� ����
            else
                deltaSpeed = (to - from) / 2f * gageSpeed * Time.deltaTime;

            // �����̴� �� ����
            exp_slider.value += deltaSpeed;
            if (exp_slider.value > to)
                exp_slider.value = to;

            // ���� �ݺ�
            if (exp_slider.value != to && exp_slider.value < exp_slider.maxValue)
                StartCoroutine(ExpGageEffect(from, to));
            // ������ ���� �Ŀ� ���� ��⿭�� ���� ȿ�� ����
            else if (expSliderQueue.Count > 0)
            {
                exp_slider.value = 0f;
                StartCoroutine(expSliderQueue.Dequeue());
            }
        }
        // ������ ���
        else
        {
            // ���� �ۼ�Ƽ�� ���
            currentPercentage = exp_slider.value / (from + to) * 100f;

            // ���ҷ��� 10% �̻��� ��� �Ϲ� �ӵ��� ����
            if (currentPercentage > 10)
                deltaSpeed = (from - to) * Time.deltaTime;
            // ���ҷ��� 10% �̸����� ���� ��� 50%�� �ӵ��� ����
            else
                deltaSpeed = (from - to) / 2f * Time.deltaTime;

            // �����̴� �� ����
            exp_slider.value -= deltaSpeed;
            if (exp_slider.value < to)
                exp_slider.value = to;

            // ���� �ݺ�
            if (exp_slider.value != to && exp_slider.value > exp_slider.minValue)
                StartCoroutine(ExpGageEffect(from, to));
            // ���Ұ� ���� �Ŀ� ���� ��⿭�� ���� ȿ�� ����
            else if (expSliderQueue.Count > 0)
            {
                exp_slider.value = 0f;
                StartCoroutine(expSliderQueue.Dequeue());
            }
        }
    }

    /// <summary>
    /// �÷��̾��� ������ ��´�.
    /// </summary>
    /// <returns>�÷��̾� ����</returns>
    public int GetLevel()
    {
        return levelData.level;
    }
    /// <summary>
    /// ���� �������� ����ġ�� ��´�.
    /// </summary>
    /// <returns>�������� ����ġ</returns>
    public int GetCurrentExp()
    {
        return levelData.currentExp;
    }
    /// <summary>
    /// ������� ���� ���� ����Ʈ�� ��´�.
    /// </summary>
    /// <returns>������� ���� ���� ����Ʈ</returns>
    public int GetUnusedStatPoint()
    {
        return levelData.unusedStatPoint;
    }
    /// <summary>
    /// ���� ����Ʈ ��� ó��
    /// </summary>
    /// <returns>ó�� ���</returns>
    public bool UseStatPoint()
    {
        if(levelData.unusedStatPoint > 0)
        {
            levelData.unusedStatPoint--;
            return true;
        }

        return false;
    }

    /// <summary>
    /// �ִ뷹���� ����Ͽ� ��ȯ�Ѵ�.
    /// </summary>
    /// <returns>�ִ� ����</returns>
    public int GetMaxLevel()
    {
        // maxLevel ���� ����� �ȵǾ��ִ� ��쿡�� ����Ͽ� ����
        if (maxLevel.Equals(0))
        {
            int _maxLevel = 1;
            for (int i = 0; i < statManager.stats.Length; i++)
                _maxLevel += statManager.stats[i].statAbilityData.MaxLevel;
            maxLevel = _maxLevel;
        }

        return maxLevel;
    }

    /// <summary>
    /// ���� ������ ����
    /// </summary>
    /// <param name="data">����� ������</param>
    public void SaveData(ref Data data)
    {
        data.levelData = levelData;
    }
    /// <summary>
    /// ���� ������ �ҷ�����
    /// </summary>
    /// <param name="data">�ҷ��� ������</param>
    public void LoadData(Data data)
    {
        if (data.levelData != null)
        {
            levelData = data.levelData;
        }
        else 
        {
            levelData = new LevelData();
        }
        SetLevelText();
    }
}
/// <summary>
/// �÷��̾��� ���� ������
/// </summary>
[System.Serializable]
public class LevelData
{
    /// <summary>
    /// �÷��̾� ����
    /// </summary>
    public int level = 1;
    /// <summary>
    /// �÷��̾ ���� ���� ����ġ��
    /// </summary>
    public int currentExp;
    /// <summary>
    /// ���� ���� ����Ʈ
    /// </summary>
    public int totalStatPoint;
    /// <summary>
    /// ������� ���� ���� ����Ʈ
    /// </summary>
    public int unusedStatPoint;
}
