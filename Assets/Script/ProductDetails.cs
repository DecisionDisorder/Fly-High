using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ ��ǰ ���λ��� Ŭ����
/// </summary>
[System.Serializable]
public class ProductDetails
{
    /// <summary>
    /// ��ǰ ���� �̸�
    /// </summary>
    public string name;
    /// <summary>
    /// ���λ��� UI ���� ���� ������Ʈ
    /// </summary>
    public GameObject detail_obj;
    /// <summary>
    /// �������� ���庰 �ִ�ġ �迭
    /// </summary>
    public float[] maxValue;
    /// <summary>
    /// �������� ���庰 ��ġ �ؽ�Ʈ
    /// </summary>
    public Text[] value_text;
    /// <summary>
    /// �������� ���庰 ��ġ ���� �����̴�
    /// </summary>
    public Slider[] value_slider;
    /// <summary>
    /// ���� Ȯ�� ���� ������Ʈ
    /// </summary>
    public PurchaseConfirm purchaseConfirm;
    
    /// <summary>
    /// Ư�� ������ ��ġ�� ���ڿ��� ��ȯ
    /// </summary>
    /// <param name="list">�� ���忡���� ��ġ��</param>
    /// <returns>���ڿ��� ��ȯ�� ���� ��ġ</returns>
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
    /// ������ ���� ������ ������Ʈ �Ѵ�
    /// </summary>
    /// <param name="data">���� ����</param>
    /// <param name="index">���� �ε���</param>
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
    /// ������ ���� ������ ������Ʈ �Ѵ�
    /// </summary>
    /// <param name="data">���� ����</param>
    /// <param name="index">���� �ε���</param>
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
    /// �����̴��� �ִ밪 ����
    /// </summary>
    public void InitializeSliderMax()
    {
        for (int j = 0; j < value_slider.Length; j++)
        {
            value_slider[j].maxValue = maxValue[j];
        }
    }

}
