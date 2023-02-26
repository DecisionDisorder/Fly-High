using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아이템 상품 세부사항 클래스
/// </summary>
[System.Serializable]
public class ProductDetails
{
    /// <summary>
    /// 상품 종류 이름
    /// </summary>
    public string name;
    /// <summary>
    /// 세부사항 UI 관련 게임 오브젝트
    /// </summary>
    public GameObject detail_obj;
    /// <summary>
    /// 아이템의 스펙별 최대치 배열
    /// </summary>
    public float[] maxValue;
    /// <summary>
    /// 아이템의 스펙별 수치 텍스트
    /// </summary>
    public Text[] value_text;
    /// <summary>
    /// 아이템의 스펙별 수치 비율 슬라이더
    /// </summary>
    public Slider[] value_slider;
    /// <summary>
    /// 구매 확정 관리 오브젝트
    /// </summary>
    public PurchaseConfirm purchaseConfirm;
    
    /// <summary>
    /// 특정 스펙의 수치를 문자열로 변환
    /// </summary>
    /// <param name="list">한 스펙에서의 수치들</param>
    /// <returns>문자열로 변환된 스펙 수치</returns>
    private string GetSpecListData(float[] list)
    {
        string spec = "";
        for (int i = 0; i < list.Length; i++)
        {
            spec += "(" + (i + 1) + LocalizationManager.instance.GetLocalizedValue("rocket_stage") + ") ";
            spec += list[i];
            if (i < list.Length - 1)
                spec += ", ";
        }

        return spec;
    }

    /// <summary>
    /// 로켓의 스펙 정보를 업데이트 한다
    /// </summary>
    /// <param name="data">로켓 정보</param>
    /// <param name="index">로켓 인덱스</param>
    public void UpdateSepcDisplay(RocketData data, int index)
    {
        purchaseConfirm.UpdateImage(data.imagePreset.rocketSprite);
        purchaseConfirm.UpdateText(data.itemData.price, data);

        value_slider[0].value = data.Hp;
        value_slider[1].value = data.MaxPower[0];
        value_slider[2].value = data.Weight[0];
        value_slider[3].value = data.FuelTotal;
        value_slider[4].value = data.FuelEfficienty;

        value_text[0].text = data.Hp.ToString();
        value_text[1].text = GetSpecListData(data.MaxPower);
        value_text[2].text = GetSpecListData(data.Weight);
        value_text[3].text = data.FuelTotal.ToString();
        value_text[4].text = data.FuelEfficienty.ToString();
    }

    /// <summary>
    /// 무기의 스펙 정보를 업데이트 한다
    /// </summary>
    /// <param name="data">무기 정보</param>
    /// <param name="index">무기 인덱스</param>
    public void UpdateSpecDisplay(WeaponSpecData data, int index)
    {
        purchaseConfirm.UpdateImage(data.weaponSprite);
        purchaseConfirm.UpdateText(data.weaponData.price, LocalizationManager.instance.GetLocalizedValue("weapon_name_" + index));

        value_slider[0].value = data.BulletCapacity;
        value_slider[1].value = data.Damage;
        value_slider[2].value = data.RPM;

        value_text[0].text = data.BulletCapacity.ToString();
        value_text[1].text = data.Damage.ToString();
        value_text[2].text = data.RPM.ToString();
    }
    /// <summary>
    /// 슬라이더의 최대값 설정
    /// </summary>
    public void InitializeSliderMax()
    {
        for (int j = 0; j < value_slider.Length; j++)
        {
            value_slider[j].maxValue = maxValue[j];
        }
    }

}
