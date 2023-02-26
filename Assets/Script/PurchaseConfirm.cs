using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������� ������ ���Ÿ� ó���ϴ� ���� Ŭ����
/// </summary>
public class PurchaseConfirm : MonoBehaviour
{
    public StoreManager storeManager;
    public AndroidController androidController;
    public ItemManager itemManager;
    public WeaponManager weaponManager;
    public DataManager dataManager;

    /// <summary>
    /// ���� Ȯ�� �޴� ������Ʈ
    /// </summary>
    public GameObject confirm_obj;
    /// <summary>
    /// ������ �̸� �ؽ�Ʈ
    /// </summary>
    public Text name_text;
    /// <summary>
    /// ������ ���� �ؽ�Ʈ
    /// </summary>
    public Text price_text;
    /// <summary>
    /// ���� �� ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text afterPurchase_text;
    /// <summary>
    /// ��ǰ �̹���
    /// </summary>
    public Image product_img;
    /// <summary>
    /// ���� ��� ������
    /// </summary>
    private Item item;
    /// <summary>
    /// ���� ��� ������ ������
    /// </summary>
    public ScriptableObject currentItemData { private set; get; }

    /// <summary>
    /// ������ ������ ���� ����
    /// </summary>
    public ProductDetails[] productDetails; 

    /// <summary>
    /// ���� ���� �����̴�
    /// </summary>
    public Slider[] purchasingQuantitySliders;
    /// <summary>
    /// ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text[] purchasingQuantityTexts;
    /// <summary>
    /// ������ ���� �ؽ�Ʈ
    /// </summary>
    public Text itemDescription_text;
    /// <summary>
    /// ������ ���� ����
    /// </summary>
    private int quantity = -1;

    /// <summary>
    /// 1ȸ �ִ� ���� ����
    /// </summary>
    public int itemMaxQuantity = 50;

    /// <summary>
    /// ���� ȿ����
    /// </summary>
    public AudioSource purchaseAudio;
    /// <summary>
    /// ���� ȿ���� Ŭ�� �迭
    /// </summary>
    public AudioClip[] purchaseClips;

    /// <summary>
    /// ���� ��� ������ �̹��� ������Ʈ
    /// </summary>
    /// <param name="sprite">������ �̹���</param>
    /// <param name="angle">������ ȸ�� ����</param>
    public void UpdateImage(Sprite sprite, float angle = 30f)
    {
        // ������ �̹��� ����
        product_img.sprite = sprite;
        // ������ �̹��� ���� ���
        float priorHeight = product_img.rectTransform.sizeDelta.y;
        // ����� ������ �̹��� ���� ���� �� ���� �����Ͽ� �̹��� ũ�� �� ���� ����
        product_img.SetNativeSize();
        float ratio = product_img.rectTransform.sizeDelta.x / product_img.rectTransform.sizeDelta.y;
        product_img.rectTransform.sizeDelta = new Vector2(priorHeight * ratio, priorHeight);
        product_img.transform.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    /// <summary>
    /// ������ ���� Ȯ�� �ؽ�Ʈ ���� ������Ʈ
    /// </summary>
    /// <param name="price">����</param>
    /// <param name="name">������ �̸�</param>
    public void UpdateText(int price, string name)
    {
        name_text.text = name;

        price_text.text = PlayManager.SetCommaOnDigit(price);

        int afterPrice = EconomicMgr.instance.Coin - price;
        if (afterPrice >= 0)
            afterPurchase_text.text = string.Format("<color=#afffb1>{0:#,##0}</color>", afterPrice);
        else
            afterPurchase_text.text = string.Format("<color=red>{0:#,##0}</color>", afterPrice);
    }

    /// <summary>
    /// ���� ���� Ȯ�� �ؽ�Ʈ ���� ������Ʈ
    /// </summary>
    /// <param name="price">����</param>
    /// <param name="rocketData">���� ��� ���� ������</param>
    public void UpdateText(int price, RocketData rocketData)
    {
        string name = LocalizationManager.instance.GetLocalizedValue("rocket_name_" + (int)rocketData.rocketType) + " [" + rocketData.MaxPower.Length + LocalizationManager.instance.GetLocalizedValue("rocket_stage") + "] ";
        UpdateText(price, name);
    }

    /// <summary>
    /// ���� ��� ������ �̸� ã��
    /// </summary>
    /// <returns>���� ��� ������ �̸�</returns>
    public string GetCurrentItemName()
    {
        string name = "";
        if(currentItemData is RocketData)
        {
            name = LocalizationManager.instance.GetLocalizedValue("rocket_name_" + (int)((RocketData)currentItemData).rocketType);
        }
        else if(currentItemData is WeaponSpecData)
        {
            name = LocalizationManager.instance.GetLocalizedValue("weapon_name_" + (int)((WeaponSpecData)currentItemData).weaponType);
        }
        else if(currentItemData is ItemData)
        {
            name = LocalizationManager.instance.GetLocalizedValue("item_name_" + (int)((ItemData)currentItemData).ItemType);
        }

        return name;
    }

    /// <summary>
    /// ���� ���� UI ������Ʈ
    /// </summary>
    /// <param name="data">��� ���� ������</param>
    /// <param name="index">���� �ε���</param>
    public void UpdateQuantityDisplay(WeaponSpecData data, int index)
    {
        UpdateImage(data.bulletSprite);
        UpdateText(data.BulletPrice, LocalizationManager.instance.GetLocalizedValue("bullet_name_" + index));

        // �ִ� ������ �̻� źȯ�� �������� ���ϵ��� ����
        purchasingQuantitySliders[0].maxValue = data.BulletCapacity - data.weaponData.numOfBulletLeft;
        purchasingQuantitySliders[0].value = 1;
        quantity = (int)purchasingQuantitySliders[0].value;
    }

    /// <summary>
    /// �Ϲ� ������ ���� ���� ������Ʈ
    /// </summary>
    /// <param name="data">������ ������</param>
    /// <param name="index">������ �ε���</param>
    public void UpdateQuantityDisplay(ItemData data, int index)
    {
        UpdateImage(data.IconSprite, 0);
        UpdateText(data.item.price, LocalizationManager.instance.GetLocalizedValue("item_name_" + index));

        // �ִ� ������ �̻� �������� �������� ���ϵ��� ����
        purchasingQuantitySliders[1].maxValue = itemMaxQuantity - data.item.quantity;
        purchasingQuantitySliders[1].value = 1;
        quantity = (int)purchasingQuantitySliders[1].value;
    }
    /// <summary>
    /// ������ ���� ȿ���� ���
    /// </summary>
    /// <param name="isQuantity">��ȸ ���ż� ������ ����</param>
    private void PurchaseSound(bool isQuantity)
    {
        if(isQuantity)
            purchaseAudio.clip = purchaseClips[0];
        else
            purchaseAudio.clip = purchaseClips[1];
        purchaseAudio.Play();
    }

    /// <summary>
    /// ���� Ȯ�� �޴� ��Ȱ��ȭ
    /// </summary>
    public void Close()
    {
        for (int i = 0; i < productDetails.Length; i++)
            if (productDetails[i].detail_obj.activeInHierarchy)
                productDetails[i].detail_obj.SetActive(false);
        confirm_obj.SetActive(false);
    }
    /// <summary>
    /// ���� Ȯ�� ó��
    /// </summary>
    /// <returns>���� ���� ����</returns>
    public bool Confirm()
    {
        // 1ȸ ���ż� ������
        if(quantity.Equals(-1))
        {
            // �̹� �������� ���������� Ȯ��
            if (!item.hasItem)
            {
                // ���� ���� ���� ó��
                if (EconomicMgr.instance.PurchaseByCoin(item.price))
                {
                    // ������ ���� ó�� �� �ȳ� �޽��� ���
                    item.hasItem = true;
                    if (currentItemData is RocketData || currentItemData is WeaponSpecData)
                        storeManager.UpdateAlreadyPurchased();
                    androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("purchase_success"), 0.75f);
                    dataManager.SaveData();
                    PurchaseSound(false);
                    Close();
                    return true;
                }
                // ���� ���� �ȳ� �޽��� ���
                else
                    androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_enough_coin"));
                Close();
            }
            // �̹� ������ ������ �ȳ� �޽��� ���
            else
            {
                androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("already_purchased"));
            }
        }
        else // �ټ� ���ż� ������
        {
            // ���� źȯ �������� ���
            if (currentItemData is WeaponSpecData)
            {
                // �ش� ���⸦ �����ߴ��� Ȯ��
                if (((WeaponSpecData)currentItemData).weaponData.hasItem)
                {
                    // ���ŷ��� 1�� �̻����� Ȯ��
                    if (quantity > 0)
                    {
                        // ������ �Ҹ��Ͽ� ��� ���� ó��
                        if (EconomicMgr.instance.PurchaseByCoin(((WeaponSpecData)currentItemData).BulletPrice * quantity))
                        {
                            // ���� źȯ ���� �� ���� �� ���� �ȳ� �޽��� ���
                            ((WeaponSpecData)currentItemData).weaponData.numOfBulletLeft += quantity;
                            if (weaponManager.weapon != null && (((WeaponSpecData)currentItemData).weaponData.numOfBulletLeft - quantity).Equals(0))
                                weaponManager.weapon.ReloadBullet();
                            androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("purchase_success"), 0.75f);
                            dataManager.SaveData();
                            PurchaseSound(true);
                            Close();
                            return true;
                        }
                        // ���� ���� �ȳ� �޽��� ���
                        else
                            androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_enough_coin"));
                    }
                    // 0�� ���� �Ұ� �ȳ� �޽��� ���
                    else
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("zero_quantity"), 1f);
                }
                // �ش� ���� ���� �ȳ� �޽��� ���
                else
                    androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_have_weapon"), 1f);
            }
            // �Ϲ� �������� ���
            else if(currentItemData is ItemData)
            {
                // ���� 1�� �̻����� Ȯ��
                if (quantity > 0)
                {
                    // ������ �Ҹ��Ͽ� ��� ���� ó��
                    if (EconomicMgr.instance.PurchaseByCoin(item.price * quantity))
                    {
                        // ������ ���� �߰� �� ���� �ȳ� �޽��� ���
                        item.quantity += quantity;
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("purchase_success"), 0.75f);
                        dataManager.SaveData();
                        PurchaseSound(true);
                        Close();
                        return true;
                    }
                    // ���� ���� �ȳ� �޽��� ���
                    else
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_enough_coin"));
                }
                // 0�� ���� �Ұ� �ȳ� �޽��� ���
                else
                    androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("zero_quantity"), 1f);
            }
            // ���� ������ �������� �ƴ�
            else
                Debug.LogError("Unavailable Item Type.");
            Close();
        }
        return false;
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    /// <param name="quantityIndex">źȯ/������ ���� �ε���</param>
    public void ControlQuantity(int quantityIndex)
    {
        // ���� ���� ����
        quantity = (int)purchasingQuantitySliders[quantityIndex].value;
        // źȯ ���ŷ� ���� ������Ʈ
        if (currentItemData is WeaponSpecData)
        {
            purchasingQuantityTexts[0].text = (((WeaponSpecData)currentItemData).weaponData.numOfBulletLeft + quantity) + "/" + ((WeaponSpecData)currentItemData).BulletCapacity + " (+" + quantity + ")";
            UpdateText(((WeaponSpecData)currentItemData).BulletPrice * quantity, LocalizationManager.instance.GetLocalizedValue("bullet_name_" + (int)((WeaponSpecData)currentItemData).weaponType));
        }
        // �Ϲ� ������ ���ŷ� ���� ������Ʈ
        else if(currentItemData is ItemData)
        {
            purchasingQuantityTexts[1].text = quantity + "/" + (int)purchasingQuantitySliders[1].maxValue + " (+" + quantity + ")"; ;
            UpdateText(item.price * quantity, LocalizationManager.instance.GetLocalizedValue("item_name_" + (int)((ItemData)currentItemData).ItemType));
        }
    }

    /// <summary>
    /// ���� Ȯ�� â Ȱ��ȭ
    /// </summary>
    /// <param name="data">���� ��� ������ ������</param>
    /// <param name="isBullet">�������� źȯ���� ����</param>
    public void OpenConfirm(ScriptableObject data, bool isBullet = false)
    {
        // ���� ��� ������ ������ ����
        currentItemData = data;
        // ���� ���� ���� UI ����
        if (currentItemData is RocketData)
        {
            item = ((RocketData)data).itemData;
            productDetails[0].detail_obj.SetActive(true);
            productDetails[0].UpdateSepcDisplay((RocketData)currentItemData, (int)((RocketData)currentItemData).rocketType);
            quantity = -1;
        }
        // ���� ���� ���� UI ����
        else if (currentItemData is WeaponSpecData)
        {
            // ���� ��ǰ ���� ���� UI ����
            item = ((WeaponSpecData)data).weaponData;
            if (!isBullet)
            {
                quantity = -1;
                productDetails[1].detail_obj.SetActive(true);
                productDetails[1].UpdateSpecDisplay((WeaponSpecData)currentItemData, (int)((WeaponSpecData)currentItemData).weaponType);
            }
            // źȯ ���� ���� UI ����
            else
            {
                productDetails[2].detail_obj.SetActive(true);
                UpdateQuantityDisplay((WeaponSpecData)data, (int)((WeaponSpecData)data).weaponType);
                ControlQuantity(0);
            }
        }
        // �Ϲ� ������ ���� ���� UI ����
        else if (currentItemData is ItemData)
        {
            item = ((ItemData)currentItemData).item;
            itemDescription_text.text = itemManager.GetDescription((ItemData)currentItemData);
            productDetails[3].detail_obj.SetActive(true);
            UpdateQuantityDisplay((ItemData)currentItemData, (int)((ItemData)currentItemData).ItemType);
        }
        confirm_obj.SetActive(true);
    }
}
