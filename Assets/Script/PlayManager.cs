using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System;

/// <summary>
/// ���� �� �з�
/// </summary>
public enum GameScene { MainScene, LaunchScene }

/// <summary>
/// ���� �÷��� ���� Ŭ����
/// </summary>
public class PlayManager : MonoBehaviour
{
    /// <summary>
    /// ���� �÷��� ���� Ŭ������ �̱��� ������Ʈ
    /// </summary>
    public static PlayManager instance;

    /// <summary>
    /// ���� �÷��̾� ������ ���� ������ ����
    /// </summary>
    public static bool isSurvive = true;
    /// <summary>
    /// ���� ���ӿ��� ȹ���ϰ� �ִ� ����
    /// </summary>
    public int score { get; private set; }
    /// <summary>
    /// '����' �ܾ��� ���ö���¡�� �ؽ�Ʈ
    /// </summary>
    public string score_local;
    /// <summary>
    /// ���� ���޷� (�̵� �Ÿ��� ���� ���)
    /// </summary>
    public int deltaScore;
    /// <summary>
    /// ���� ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text score_text;
    /// <summary>
    /// �ְ� ���� ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text bestScore_text;
    /// <summary>
    /// �÷��� ������
    /// </summary>
    private PlayData playData = new PlayData();
    /// <summary>
    /// �ְ� ���� �޼� ���� ��Ʈ ũ��
    /// </summary>
    public int fontSizeOnBestScore = 35;
    /// <summary>
    /// �ְ� ���� �޼� ���� �ؽ�Ʈ ����
    /// </summary>
    public Color colorOnBestScore;

    /// <summary>
    /// ���� ����� ���� ���� ���� ��ġ
    /// </summary>
    private Vector3 priorPos; 
    /// <summary>
    /// ���� ��� ���� ���� ����
    /// </summary>
    public float coinPortion;
    /// <summary>
    /// ���� ��� ����ġ ���� ����
    /// </summary>
    public float expPortion;

    /// <summary>
    /// ���� ����
    /// </summary>
    private float rocketHeight;

    /// <summary>
    /// (���ӿ��� �޴�) ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text gOver_score_text;
    /// <summary>
    /// (���ӿ��� �޴�) ���� ȹ�淮 �ؽ�Ʈ
    /// </summary>
    public Text gOver_coin_text;
    /// <summary>
    /// (���ӿ��� �޴�) ���� �ؽ�Ʈ
    /// </summary>
    public Text gOver_level_text;
    /// <summary>
    /// (���ӿ��� �޴�) ��å �ؽ�Ʈ
    /// </summary>
    public Text gOver_position_text;
    /// <summary>
    /// (���ӿ��� �޴�) ȹ���� ����ġ �ؽ�Ʈ
    /// </summary>
    public Text gOver_addExp_text;
    /// <summary>
    /// (���ӿ��� �޴�) ����ġ �����̴�
    /// </summary>
    public Slider gOver_exp_slider;
    /// <summary>
    /// (���ӿ��� �޴�) ���� ����ġ ������ �ؽ�Ʈ
    /// </summary>
    public Text gOver_curExp_text;

    public DataManager dataManager;
    public Launch launch;
    public LevelManager levelManager;
    public TabletManager tabletManager;

    /// <summary>
    /// ���� ī�޶�
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// ���� ȿ����
    /// </summary>
    public AudioSource explosionAudio;

    /// <summary>
    /// ���ʽ� �ִϸ��̼� ȿ��
    /// </summary>
    public Animation bonusAnimation;
    /// <summary>
    /// ���ʽ� �˸� �ؽ�Ʈ
    /// </summary>
    public Text bonus_text;

    /// <summary>
    /// �ִϸ��̼� ���� �� ȣ��� �Լ��� ��������Ʈ
    /// </summary>
    public delegate void AfterAnimationPlay();
    /// <summary>
    /// �ִϸ��̼� ���� �� ȣ��� �Լ��� ��������Ʈ (���� ������Ʈ �Ű����� ����)
    /// </summary>
    /// <param name="obj">ó���� ���� ������Ʈ</param>
    public delegate void AfterAnimationPlay_obj(GameObject obj);
    /// <summary>
    /// �ִϸ��̼� ���� �� ȣ��� �Լ��� ��������Ʈ (ĵ���� �Ű����� ����)
    /// </summary>
    /// <param name="cvs">ó���� ĵ����</param>
    public delegate void AfterAnimationPlay_cvs(Canvas cvs);

    private void Awake()
    {
        instance = this;
        isSurvive = true;
    }

    private void Start()
    {
        // �߻� ���� ��쿡 ���� ���̸� ����Ͽ� ���� �÷��� ���� �˻� �� ���� ȹ�� �ڷ�ƾ ����
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            rocketHeight = RocketSet.instance.rocket.GetComponent<RectTransform>().rect.height * RocketSet.instance.rocket.transform.localScale.y * 0.5f;
            StartCoroutine(OnPlaying());
            StartCoroutine(Scoring());
        }
        // ���� ���� ��� �ְ� ���� ������Ʈ
        else
            SetBestScoreText();
    }

    /// <summary>
    /// ���� ���� ���� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator OnPlaying()
    {
        yield return new WaitForSeconds(0.1f);

        if(!isSurvive)
        {
            GameOver();
        }
        else
        {
            // Ư�� ���� ���Ϸ� �������� ���� ���� ó��
            Vector3 targetScreenPos = mainCamera.WorldToScreenPoint(RocketSet.instance.rocket.transform.position + new Vector3(0, rocketHeight, 0));
            if (targetScreenPos.y < 0)
                isSurvive = false;

            StartCoroutine(OnPlaying());
        }
    }

    /// <summary>
    /// �ֱ������� �̵� �Ÿ��� ����Ͽ� ������ �߰��ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator Scoring()
    {
        yield return new WaitForSeconds(0.1f);

        // ���� �߿��� ���
        if (isSurvive)
        {
            // �߻簡 ���۵ǰ� ���Ḧ �������� ���� ��쿡�� ���
            if (launch.isStart && RocketSet.instance.FuelRemain > 0)
            {
                // �Ͻ����� ���°� �ƴ� ��쿡
                if (!TabletManager.isPause)
                {
                    // �����ϰ� �ִ� ���¿�
                    if (launch.isAccerlate)
                    {
                        // ���� ������ ��ġ�� ���� ��ġ�� �Ÿ��� ����Ͽ� deltaScore ��� �� ���� ����
                        Vector3 rocketPos = RocketSet.instance.rocket.transform.position;
                        deltaScore = (int)(Vector3.Distance(priorPos, rocketPos) * 4);
                        priorPos = rocketPos;
                        AddScore(deltaScore);
                    }
                }
            }
            else
                priorPos = RocketSet.instance.rocket.transform.position;
            StartCoroutine(Scoring());
        }
    }

    /// <summary>
    /// ���� ���޷� ��ŭ ���� �߰�
    /// </summary>
    /// <param name="add">���� ���޷�</param>
    public void AddScore(int add)
    {
        score += add;
        ReloadScore();
    }
    /// <summary>
    /// ���� �ؽ�Ʈ ���� ������Ʈ
    /// </summary>
    public void ReloadScore()
    {
        // 0,000 ������ �����ϸ� �ؽ�Ʈ ���� ������Ʈ
        score_text.text = score_local + ": " + SetCommaOnDigit(score);
        // �ְ� ������ �������� �� ��Ʈ ũ�� �� ���� ����
        if(score > playData.bestScore && score_text.fontSize != fontSizeOnBestScore)
        {
            score_text.fontSize = fontSizeOnBestScore;
            score_text.color = colorOnBestScore;
        }
    }

    /// <summary>
    /// ���� ���� ó��
    /// </summary>
    private void GameOver()
    {
        // ���� ��� ó��
        launch.AcceleratorUp();
        // ���� ���� ǥ�� �� ����ġ/���� ����
        gOver_score_text.text = score_local + ": " + SetCommaOnDigit(score);
        int addCoin = (int)(score * coinPortion);
        int addExp = (int)(score * expPortion);
        levelManager.AddExp(addExp);
        EconomicMgr.instance.AddCoin(addCoin);
        // �ְ����� ���� �� ���� ������Ʈ
        CheckBestScore(score); // TODO: ����Ʈ���ھ� ���Ž� ���� ���� ȿ��
        bestScore_text.text = LocalizationManager.instance.GetLocalizedValue("bestscore") + ": " + SetCommaOnDigit(playData.bestScore);
        gOver_coin_text.text = "+" + SetCommaOnDigit(addCoin + EconomicMgr.instance.bonusCoins);
        levelManager.SetLevelText(gOver_level_text, gOver_curExp_text);
        gOver_addExp_text.text = "Exp. +" + SetCommaOnDigit(addExp);
        // ��å ���� ������Ʈ
        gOver_position_text.text = LocalizationManager.instance.GetLocalizedValue("position_" + levelManager.GetPositionIndex());
        // GameOver �� Ȱ��ȭ
        tabletManager.OpenApp(TabletAppList.GameOver);
        // �º��� Ȱ��ȭ �� ���Ŀ� ����ġ ���� ȿ�� ����
        WaitAnimation(tabletManager.tablet_ani, levelManager.StartGageEffect);
        // �Ͻ������� �Ұ����ϵ��� ����
        tabletManager.pause_button.SetActive(false);
        // �߶����� ������ ���� ����
        RocketSet.instance.rocketMain_rigid.velocity = Vector3.zero;
        if (RocketSet.instance.rocket.transform.position.y > launch.heightCriteria[0])
            RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Kinematic;

        dataManager.SaveData();
    }

    /// <summary>
    /// ���ӿ��� ��ư ������
    /// </summary>
    /// <param name="key">�����ϰ��� �ϴ� ���</param>
    public void OnGameOverButton(int key)
    {
        switch (key)
        {
            case 0: // Game Reset
                ResetGame();
                break;
            case 1: // Go to Menu
                ResetGame();
                SceneManager.LoadScene((int)GameScene.MainScene);
                break;
            case 2: // Open Store
                Debug.Log("Store is not developed");
                break;
            case 3: // Open Equipment
                Debug.Log("Equipment is not developed");
                break;
            case 4: // Open Stat
                Debug.Log("Stat is not developed");
                break;
        }

    }

    /// <summary>
    /// ���� ȿ���� ���
    /// </summary>
    public void PlayExplosionSound()
    {
        explosionAudio.Play();
    }

    /// <summary>
    /// ���� �ʱ�ȭ
    /// </summary>
    public void ResetGame()
    {
        // ���� ������ ���� ���� �� ���ε�
        dataManager.SaveData();
        isSurvive = true;
        if (TabletManager.isPause)
            TabletManager.isPause = false;
        SceneManager.LoadScene((int)GameScene.LaunchScene, LoadSceneMode.Single);
    }

    /// <summary>
    /// �ְ� ���� ����
    /// </summary>
    /// <param name="score">������ ����</param>
    /// <returns>���� ���� ����</returns>
    private bool CheckBestScore(int score)
    {
        if (score > playData.bestScore)
        {
            playData.bestScore =  score;
            return true;
        }
        else
            return false;

    }

    /// <summary>
    /// �ְ� ���� �ؽ�Ʈ ���� ������Ʈ
    /// </summary>
    public void SetBestScoreText()
    {
        bestScore_text.text = "BestScore: " + SetCommaOnDigit(playData.bestScore);
    }

    /// <summary>
    /// �÷��� ������ ����
    /// </summary>
    /// <param name="data">������ ������</param>
    public void SaveData(ref Data data)
    {
        data.playData = playData;
    }
    /// <summary>
    /// �÷��� ������ �ҷ�����
    /// </summary>
    /// <param name="data">�ҷ��� ������</param>
    public void LoadData(Data data)
    {
        if (data.playData != null)
        { 
            playData = data.playData;
        }
        else
        {
            playData = new PlayData();
        }
    }

    /// <summary>
    /// ���ڸ� #,### ����� ���ڿ��� ��ȯ
    /// </summary>
    /// <param name="num">��ȯ�� ����</param>
    /// <returns>#,### ������ ���ڿ�</returns>
    public static string SetCommaOnDigit(int num)
    {
        StringBuilder result = new StringBuilder();
        string n = num.ToString();
        int len = n.Length;
        int front = len % 3;

        if (front != 0)
        {
            result.Append(n, 0, front);
            if (front < len)
                result.Append(',');
        }

        for (int i = front; i < len; i += 3)
        {
            result.Append(n, i, 3);
            if (i + 3 < len)
                result.Append(',');
        }
        return result.ToString();
    }
    /// <summary>
    /// �ε� �Ҽ����� #,### ����� ���ڿ��� ��ȯ
    /// </summary>
    /// <param name="num">��ȯ�� �ε� �Ҽ���</param>
    /// <returns>#,### ������ ���ڿ�</returns>
    public static string SetCommaOnDigit(float num)
    {
        if (num.Equals(0))
            return "0";

        return string.Format("{0:#,##0.00}", num);
    }

    /// <summary>
    /// ���ʽ� �˸� ���� ������Ʈ
    /// </summary>
    /// <param name="content">���ʽ� ����</param>
    /// <param name="textColor">�ؽ�Ʈ ����</param>
    public void SetBonusText(string content, Color textColor)
    {
        // ���ʽ� ȿ�� ��� �� ���� ����
        bonusAnimation.gameObject.SetActive(true);
        bonusAnimation.clip = bonusAnimation["BonusOn"].clip;
        bonusAnimation.Play();
        bonus_text.text = content;
        bonus_text.color = textColor;

        // ��� ���� ���ʽ� �ؽ�Ʈ ��Ȱ��ȭ �ڷ�ƾ ����
        if (waitBonusText != null)
            StopCoroutine(waitBonusText);
        // ���ʽ� �ؽ�Ʈ ��Ȱ��ȭ �ڷ�ƾ ����
        waitBonusText = WaitBonusText(1.5f);
        StartCoroutine(waitBonusText);
    }

    /// <summary>
    /// ���ʽ� �ؽ�Ʈ ��Ȱ��ȭ �ڷ�ƾ
    /// </summary>
    IEnumerator waitBonusText;
    /// <summary>
    /// ���ʽ� �ؽ�Ʈ ��Ȱ��ȭ �ڷ�ƾ
    /// </summary>
    /// <param name="time">��� �ð�</param>
    IEnumerator WaitBonusText(float time)
    {
        yield return new WaitForSeconds(time);

        bonusAnimation.clip = bonusAnimation["BonusOff"].clip;
        bonusAnimation.Play();
        StartCoroutine(WaitBonusOff());
    }

    /// <summary>
    /// ���ʽ� �ؽ�Ʈ ��Ȱ��ȭ ȿ���� ����� �� ��Ȱ��ȭ ��� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitBonusOff()
    {
        while (bonusAnimation.isPlaying)
            yield return new WaitForEndOfFrame();

        bonusAnimation.gameObject.SetActive(false);
    }

    /// <summary>
    /// �ִϸ��̼� ���� �� �Լ� �ݹ�
    /// </summary>
    /// <param name="animation">��� �ִϸ��̼�</param>
    /// <param name="afterAnimation">�ݹ��� �Լ�</param>
    public void WaitAnimation(Animation animation, AfterAnimationPlay afterAnimation)
    {
        StartCoroutine(WaitforAnimation(animation, afterAnimation));
    }
    /// <summary>
    /// �ִϸ��̼� ���� �� �Լ� �ݹ�
    /// </summary>
    /// <param name="animation">��� �ִϸ��̼�</param>
    /// <param name="afterAnimation">�ݹ��� �Լ�</param
    /// <param name="obj">���� ������Ʈ �Ķ����</param>
    public void WaitAnimation(Animation animation, AfterAnimationPlay_obj afterAnimation, GameObject obj)
    {
        StartCoroutine(WaitforAnimation(animation, afterAnimation, obj));
    }
    /// <summary>
    /// �ִϸ��̼� ���� �� �Լ� �ݹ�
    /// </summary>
    /// <param name="animation">��� �ִϸ��̼�</param>
    /// <param name="afterAnimation">�ݹ��� �Լ�</param
    /// <param name="cvs">ĵ���� �Ķ����</param>
    public void WaitAnimation(Animation animation, AfterAnimationPlay_cvs afterAnimation, Canvas cvs)
    {
        StartCoroutine(WaitforAnimation(animation, afterAnimation, cvs));
    }
    /// <summary>
    /// �ִϸ��̼� ���� �� �Լ� �ݹ� �� �ش� �ڷ�ƾ ��ȯ
    /// </summary>
    /// <param name="animation">��� �ִϸ��̼�</param>
    /// <param name="afterAnimation">�ݹ��� �Լ�</param>
    /// <param name="obj">���� ������Ʈ �Ķ����</param>
    public IEnumerator RWaitAnimation(Animation animation, AfterAnimationPlay_obj afterAnimation, GameObject obj)
    {
        IEnumerator coroutine = WaitforAnimation(animation, afterAnimation, obj);
        return coroutine;
    }

    /// <summary>
    /// �ִϸ��̼� ���� �� �Լ� �ݹ� �ڷ�ƾ
    /// </summary>
    /// <param name="animation">��� �ִϸ��̼�</param>
    /// <param name="afterAnimation">�ݹ��� �Լ�</param>
    IEnumerator WaitforAnimation(Animation animation, AfterAnimationPlay afterAnimation)
    {
        while(animation.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation();
    }
    /// <summary>
    /// �ִϸ��̼� ���� �� �Լ� �ݹ� �ڷ�ƾ
    /// </summary>
    /// <param name="animation">��� �ִϸ��̼�</param>
    /// <param name="afterAnimation">�ݹ��� �Լ�</param
    /// <param name="obj">���� ������Ʈ �Ķ����</param>
    IEnumerator WaitforAnimation(Animation animation, AfterAnimationPlay_obj afterAnimation, GameObject obj)
    {
        while (animation.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation(obj);
    }
    /// <summary>
    /// �ִϸ��̼� ���� �� �Լ� �ݹ� �ڷ�ƾ
    /// </summary>
    /// <param name="animation">��� �ִϸ��̼�</param>
    /// <param name="afterAnimation">�ݹ��� �Լ�</param
    /// <param name="cvs">ĵ���� �Ķ����</param>
    IEnumerator WaitforAnimation(Animation animation, AfterAnimationPlay_cvs afterAnimation, Canvas cvs)
    {
        while (animation.isPlaying)
            yield return new WaitForEndOfFrame();

        afterAnimation(cvs);
    }
}

/// <summary>
/// �÷��� ������
/// </summary>
[Serializable]
public class PlayData
{
    /// <summary>
    /// �ְ� ����
    /// </summary>
    public int bestScore;
}
