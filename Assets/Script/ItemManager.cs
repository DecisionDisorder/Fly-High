using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ ���� Ŭ����
/// </summary>
public class ItemManager : MonoBehaviour
{
    /// <summary>
    /// ���Ϻ� ������
    /// </summary>
    public RocketData[] rocketDataSet;
    public EquipmentManager equipmentManager;
    public StatManager statManager;

    /// <summary>
    /// ������ ����
    /// </summary>
    public enum ItemType { EmergencyFueling, VernierEngine, BoosterEngine, Shield }
    /// <summary>
    /// ������ ������ (ScriptableObject)
    /// </summary>
    public ItemData[] items;
    /// <summary>
    /// ������ ������ ����
    /// </summary>
    public static ItemType? equippedItem;
    /// <summary>
    /// ������ ��� ��ư
    /// </summary>
    public Button item_button;
    /// <summary>
    /// ���� ������ ���� �ؽ�Ʈ
    /// </summary>
    public Text quantity_text;

    /// <summary>
    /// ���� �������� ���� HP
    /// </summary>
    public int shieldHp;
    /// <summary>
    /// ���� Ȱ��ȭ ����
    /// </summary>
    public bool shieldOn = false;

    /// <summary>
    /// �ν��� ȿ�� Ȱ��ȭ ����
    /// </summary>
    public bool boosterOn = false;
    /// <summary>
    /// �ν��� ���� �ð� ǥ�� Ÿ�̸� �̹���
    /// </summary>
    public Image boosterTimer;

    private void Start()
    {
        UpdateItemQuantityText();
    }

    /// <summary>
    /// ������ ��� ��ư ������
    /// </summary>
    public void UseItem()
    {
        UseItem(equippedItem);
    }

    /// <summary>
    /// ������ ���
    /// </summary>
    /// <param name="type">������ ���� �ε���</param>
    public void UseItem(int type)
    {
        UseItem((ItemType)type);
    }

    /// <summary>
    /// ������ ���
    /// </summary>
    /// <param name="type">������ ����</param>
    public void UseItem(ItemType? type)
    {
        // �� ������ ���� ȿ�� �ߵ�
        int index = (int)type;
        switch (type)
        {
            case ItemType.EmergencyFueling:
                EmergencyFueling(items[index].MainValue);
                break;
            case ItemType.BoosterEngine:
                // �ν��� ���� ���� Ȯ�� �� �ߵ�
                if (statManager.GetLevel(StatType.Booster) != 0)
                {
                    if (!boosterOn && Launch.instance.isAccerlate)
                        BoosterEngine();
                    else
                        return;
                }
                else
                {
                    LocalizationManager.instance.GetLocalizedValue("booster_unable");
                    return;
                }
                break;
            case ItemType.Shield:
                if (!shieldOn)
                    Shield();
                else
                    return;
                break;
        }

        // ������ ���� ���� �谨 
        items[index].item.quantity--;
        // ������ ���� Ȯ�� �� ���� ���� ó��
        if (items[index].item.quantity <= 0)
        {
            equippedItem = null;
            item_button.gameObject.SetActive(false);
            equipmentManager.UpdateEquipItemImage();
        }
        else
            UpdateItemQuantityText();
    }

    /// <summary>
    /// ������ ���� ���� ������Ʈ
    /// </summary>
    public void UpdateItemQuantityText()
    {
        if(equippedItem != null)
            quantity_text.text = items[(int)equippedItem].item.quantity.ToString();
    }

    /// <summary>
    /// ��� ���� ������ ���
    /// </summary>
    /// <param name="recover">������</param>
    private void EmergencyFueling(float recover)
    {
        RocketSet.instance.FuelRemain += RocketSet.instance.fuelMax[RocketSet.instance.currentStage] * recover;
    }

    /// <summary>
    /// �ν��� ������ ȿ�� �ߵ�
    /// </summary>
    private void BoosterEngine()
    {
        // �ν���/���� Ȱ��ȭ �� ȿ�� ���
        boosterOn = true;
        RocketSet.instance.BoosterFireEffectPlay();
        RocketSet.instance.isInvincible = true;
        boosterTimer.transform.parent.gameObject.SetActive(true);
        StartCoroutine(BoosterRemainTimer(items[(int)equippedItem].MainValue));
        StartCoroutine(BoosterTimer(items[(int)equippedItem].MainValue));
    }

    /// <summary>
    /// �ν��� ȿ�� ���� Ÿ�̸�
    /// </summary>
    /// <param name="fullTime">�� ���ӽð�</param>
    /// <param name="passedTime">���� �ð�</param>
    IEnumerator BoosterRemainTimer(float fullTime, float passedTime = 0)
    {
        yield return new WaitForEndOfFrame();

        passedTime += Time.deltaTime;

        // ���� �ν��� �ð� ǥ��
        boosterTimer.fillAmount = passedTime / fullTime;


        if (passedTime < fullTime)
        {
            StartCoroutine(BoosterRemainTimer(fullTime, passedTime));
        }
        else
            boosterTimer.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// �ν��� ȿ�� ���� Ÿ�̸�
    /// </summary>
    /// <param name="time">���ӽð�</param>
    IEnumerator BoosterTimer(float time)
    {
        yield return new WaitForSeconds(time);
        // ����Ʈ �ߴ� �� �ν���/���� ó�� ����
        RocketSet.instance.BoosterFireEffectStop(); 
        RocketSet.instance.isInvincible = false;
        boosterOn = false;
    }

    /// <summary>
    /// ��ȣ�� ������ Ȱ��ȭ
    /// </summary>
    private void Shield()
    {
        // ���� ȿ�� Ȱ��ȭ �� �ʱ� �� ����
        shieldOn = true;
        shieldHp = (int)items[(int)ItemType.Shield].MainValue;
        RocketSet.instance.shield.color = Color.white;
        RocketSet.instance.shield.gameObject.SetActive(true);
        RocketSet.instance.shield_ani.Play();
    }

    /// <summary>
    /// ��ȣ�� ��� ��� ó��
    /// </summary>
    /// <param name="dmg">������</param>
    public void HitShield(int dmg)
    {
        // ���差 �谨 �� ���� ���差 �������� ǥ��
        shieldHp -= dmg;
        float portion = shieldHp / items[(int)ItemType.Shield].MainValue;
        RocketSet.instance.shield.color = new Color(1, portion, portion);
        RocketSet.instance.isInvincible = true;
        // ���� ���� �� ���� ȿ�� ���� �� ���� ����Ʈ ����
        if (shieldHp <= 0)
        {
            RocketSet.instance.isInvincible = false;
            RocketSet.instance.shield_ani["ShieldAppear"].speed = -1;
            RocketSet.instance.shield_ani["ShieldAppear"].time = RocketSet.instance.shield_ani["ShieldAppear"].length;
            RocketSet.instance.shield_ani.Play();
            PlayManager.instance.WaitAnimation(RocketSet.instance.shield_ani, DisableShield);
        }
    }

    /// <summary>
    /// ���� ��Ȱ��ȭ
    /// </summary>
    private void DisableShield()
    {
        shieldOn = false;
        RocketSet.instance.shield.gameObject.SetActive(false);
    }

    /// <summary>
    /// ������ ���� ������
    /// </summary>
    /// <param name="data">��� ������ ������</param>
    /// <returns>����ȭ�� ������ ����</returns>
    public string GetDescription(ItemData data)
    {
        ItemType type = data.ItemType;
        switch(type)
        {
            case ItemType.EmergencyFueling:
                return string.Format(LocalizationManager.instance.GetLocalizedValue("item_desc_0"), data.MainValue * 100);
            case ItemType.VernierEngine:
                return string.Format(LocalizationManager.instance.GetLocalizedValue("item_desc_1"), data.MainValue, data.SubValues[0]);
            case ItemType.BoosterEngine:
                return string.Format(LocalizationManager.instance.GetLocalizedValue("item_desc_2"), data.MainValue);
            case ItemType.Shield:
                return string.Format(LocalizationManager.instance.GetLocalizedValue("item_desc_3"), data.MainValue);
        }

        return null;
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    /// <param name="data">������ ������</param>
    public void SaveData(ref Data data)
    {
        data.rocketItemData = new List<Item>();
        data.itemsData = new List<Item>();
        for (int i = 0; i < rocketDataSet.Length; i++)
        {
            data.rocketItemData.Add(rocketDataSet[i].itemData);
        }
        for(int i = 0; i < items.Length; i++)
        {
            data.itemsData.Add(items[i].item);
        }
    }
    /// <summary>
    /// ������ �ҷ�����
    /// </summary>
    /// <param name="data">�ҷ��� ������</param>
    public void LoadData(Data data)
    {
        // �ҷ��� �����Ͱ� ������ �ε� ����
        if (data != null)
        {
            // ���� ������ �����Ͱ� ������ ���� ������ �ε�
            if(data.rocketItemData != null)
            {
                for (int i = 0; i < data.rocketItemData.Count; i++)
                {
                    rocketDataSet[i].itemData = data.rocketItemData[i];
                }
            }
            // �Ϲ� ������ �����Ͱ� ������ ������ �ε�
            if(data.itemsData != null)
            {
                for (int i = 0; i < items.Length; i++)
                    items[i].item = data.itemsData[i];
            }
        }
        // �����Ͱ� ������ ������ ������ �ʱ�ȭ
        else
        {
            ResetItems();
        }
    }
    /// <summary>
    /// ������ �ʱ�ȭ �۾�
    /// </summary>
    public void ResetItems()
    {
        for (int i = 0; i < items.Length; i++)
            items[i].item.quantity = 0;

        for (int i = 1; i < rocketDataSet.Length; i++)
            rocketDataSet[i].itemData.hasItem = false;

        for (int i = 0; i < equipmentManager.weapons_prefab.Length; i++)
        {
            equipmentManager.weapons_prefab[i].weaponSpecData.weaponData.hasItem = false;
            equipmentManager.weapons_prefab[i].weaponSpecData.weaponData.numOfBulletLeft = 0;
        }
    }
}

/// <summary>
/// ������ ������
/// </summary>
[System.Serializable]
public class Item
{
    /// <summary>
    /// ������ �̸�
    /// </summary>
    public string itemName;
    /// <summary>
    /// ������ ����
    /// </summary>
    public int price;
    /// <summary>
    /// ������ ���� ����
    /// </summary>
    public bool hasItem;
    /// <summary>
    /// ������ ������
    /// </summary>
    public int quantity;
}