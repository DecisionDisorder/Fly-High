using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 경제 시스템 관리 클래스
/// </summary>
public class EconomicMgr : MonoBehaviour
{
    /// <summary>
    /// 경제 시스템 관리 클래스의 싱글톤 오브젝트
    /// </summary>
    public static EconomicMgr instance;

    /// <summary>
    /// 경제 데이터
    /// </summary>
    private EconomicData economicData = new EconomicData();
    /// <summary>
    /// 코인 보유량 최대치
    /// </summary>
    private const int maxCoin = 2000000000; // 20억 한도(int 범위)
    /// <summary>
    /// 한 게임 내에서 필드에서 획득한 누적 코인
    /// </summary>
    public int bonusCoins { private set; get; }
    /// <summary>
    /// 플레이어가 보유 중인 코인
    /// </summary>
    public int Coin { get { return economicData.coin; } private set {
            economicData.coin = value;
            SetText();
        } 
    }
    /// <summary>
    /// 보유 중인 코인 표시 텍스트
    /// </summary>
    public Text coin_text;

    /// <summary>
    /// 광고 보상 재화(캐시)
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
    /// 코인 추가 함수 (최대치 예외 처리 포함) 
    /// </summary>
    /// <param name="add">획득한 코인</param>
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
    /// 필드에서 획득한 코인 누적 계산 함수
    /// </summary>
    /// <param name="add">획득한 코인</param>
    public void BonusCoin(int add)
    {
        bonusCoins += add;
        AddCoin(add);
    }

    /// <summary>
    /// 코인 텍스트 정보 업데이트
    /// </summary>
    private void SetText()
    {
        // 씬에 따라 다르게 prefix 표기
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
            SetText(coin_text, "x");
        else
            SetText(coin_text, "Coin: ");
    }

    /// <summary>
    /// prefix와 코인을 더한 문자열을 텍스트에 업데이트 해주는 함수
    /// </summary>
    /// <param name="text">대상 텍스트</param>
    /// <param name="prefix">접두사</param>
    public void SetText(Text text, string prefix)
    {
        text.text = prefix + PlayManager.SetCommaOnDigit(economicData.coin);
    }

    /// <summary>
    /// 가격 검사를 하여 코인 구매 처리를 한다.
    /// </summary>
    /// <param name="price">상품의 가격</param>
    /// <returns>구매 성공 여부</returns>
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
    /// 경제 데이터 저장
    /// </summary>
    /// <param name="data">합칠 데이터 오브젝트</param>
    public void SaveData(ref Data data)
    {
        data.economicData = economicData;
    }
    /// <summary>
    /// 경제 데이터 로드
    /// </summary>
    /// <param name="data">불러온 데이터</param>
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

        Coin += 0; // 텍스트 새로고침
    }
}

/// <summary>
/// 경제 데이터
/// </summary>
[System.Serializable]
public class EconomicData
{
    /// <summary>
    /// 보유 중인 코인
    /// </summary>
    public int coin;
    /// <summary>
    /// 보유 중인 캐시(광고 보상 재화)
    /// </summary>
    public int cash;
}