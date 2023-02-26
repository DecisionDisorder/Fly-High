using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� �ý��� ���� Ŭ����
/// </summary>
public class EconomicMgr : MonoBehaviour
{
    /// <summary>
    /// ���� �ý��� ���� Ŭ������ �̱��� ������Ʈ
    /// </summary>
    public static EconomicMgr instance;

    /// <summary>
    /// ���� ������
    /// </summary>
    private EconomicData economicData = new EconomicData();
    /// <summary>
    /// ���� ������ �ִ�ġ
    /// </summary>
    private const int maxCoin = 2000000000; // 20�� �ѵ�(int ����)
    /// <summary>
    /// �� ���� ������ �ʵ忡�� ȹ���� ���� ����
    /// </summary>
    public int bonusCoins { private set; get; }
    /// <summary>
    /// �÷��̾ ���� ���� ����
    /// </summary>
    public int Coin { get { return economicData.coin; } private set {
            economicData.coin = value;
            SetText();
        } 
    }
    /// <summary>
    /// ���� ���� ���� ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text coin_text;

    /// <summary>
    /// ���� ���� ��ȭ(ĳ��)
    /// </summary>
    public int Cash { get { return economicData.cash; } private set
        {
            economicData.cash = value;
        } 
    }

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// ���� �߰� �Լ� (�ִ�ġ ���� ó�� ����) 
    /// </summary>
    /// <param name="add">ȹ���� ����</param>
    public void AddCoin(int add)
    {
        if (Coin + add < maxCoin)
        {
            Coin += add;
        }
        else
            Coin = maxCoin;
    }

    /// <summary>
    /// �ʵ忡�� ȹ���� ���� ���� ��� �Լ�
    /// </summary>
    /// <param name="add">ȹ���� ����</param>
    public void BonusCoin(int add)
    {
        bonusCoins += add;
        AddCoin(add);
    }

    /// <summary>
    /// ���� �ؽ�Ʈ ���� ������Ʈ
    /// </summary>
    private void SetText()
    {
        // ���� ���� �ٸ��� prefix ǥ��
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
            SetText(coin_text, "x");
        else
            SetText(coin_text, "Coin: ");
    }

    /// <summary>
    /// prefix�� ������ ���� ���ڿ��� �ؽ�Ʈ�� ������Ʈ ���ִ� �Լ�
    /// </summary>
    /// <param name="text">��� �ؽ�Ʈ</param>
    /// <param name="prefix">���λ�</param>
    public void SetText(Text text, string prefix)
    {
        text.text = prefix + PlayManager.SetCommaOnDigit(economicData.coin);
    }

    /// <summary>
    /// ���� �˻縦 �Ͽ� ���� ���� ó���� �Ѵ�.
    /// </summary>
    /// <param name="price">��ǰ�� ����</param>
    /// <returns>���� ���� ����</returns>
    public bool PurchaseByCoin(int price)
    {
        if (price <= Coin)
        {
            Coin -= price;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// ���� ������ ����
    /// </summary>
    /// <param name="data">��ĥ ������ ������Ʈ</param>
    public void SaveData(ref Data data)
    {
        data.economicData = economicData;
    }
    /// <summary>
    /// ���� ������ �ε�
    /// </summary>
    /// <param name="data">�ҷ��� ������</param>
    public void LoadData(Data data)
    {
        if (data.economicData != null)
        {
            economicData = data.economicData;
        }
        else
        {
            economicData = new EconomicData();
        }

        Coin += 0; // �ؽ�Ʈ ���ΰ�ħ
    }
}

/// <summary>
/// ���� ������
/// </summary>
[System.Serializable]
public class EconomicData
{
    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public int coin;
    /// <summary>
    /// ���� ���� ĳ��(���� ���� ��ȭ)
    /// </summary>
    public int cash;
}