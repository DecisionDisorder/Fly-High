using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로켓의 인게임 데이터를 총괄하는 클래스
/// </summary>
[System.Serializable]
public class RocketSet : MonoBehaviour
{
    /// <summary>
    /// 배럴 단위를 톤으로 변환하는 상수
    /// </summary>
    private const float BarrelToTon = 0.128f;
    /// <summary>
    /// 로켓 총괄 데이터 오브젝트의 싱글톤 인스턴스
    /// </summary>
    public static RocketSet instance;

    /// <summary>
    /// 로켓 이름
    /// </summary>
    public string Name;
    /// <summary>
    /// 로켓의 최대 HP
    /// </summary>
    private int maxHp;
    /// <summary>
    /// 로켓의 실시간 HP
    /// </summary>
    private int hp;
    /// <summary>
    /// 로켓의 실시간 HP (Property)
    /// </summary>
    public int Hp { get { return hp; } 
        set 
        {
            // 변경되는 HP가 0 이하일 경우에 게임오버 처리
            if (value <= 0)
            {
                // 폭발 이펙트가 있는 경우, 폭발 이펙트 재생
                if (explosionEffect != null)
                {
                    explosionEffect.Play();
                    for (int i = 0; i < rocket_rends.Length; i++)
                        rocket_rends[i].color = Color.black;
                }
                PlayManager.isSurvive = false;
            }
            // HP가 1 이상인 경우에 HP바로 변경된 HP를 일정 시간동안 활성화 한다.
            else
            {
                hp = value;
                hpBar.SetActive(true);
                hpBar_SprtSlider.SetValue(value);
                SetRocketColorByHp();
                StartCoroutine(HpBarClose());
            }
        } 
    }
    /// <summary>
    /// 남은 연료량
    /// </summary>
    private float fuelRemain;
    /// <summary>
    /// 남은 연료량 (Property)
    /// </summary>
    public float FuelRemain { get { return fuelRemain; } 
        set
        {
            // 기존 남은 연료량 보다 큰 경우, 연료 보충으로 처리
            if (value > fuelRemain)
                if(Launch.instance != null)
                    Launch.instance.SupplyFuel(fuelRemain);
            // 최대치보다 커지는 경우, 최대치로 제한
            if (value > fuelMax[currentStage])
                fuelRemain = fuelMax[currentStage];
            else
            {
                // 남은 연료가 0 이하로 떨어진 경우
                if(value <= 0)
                {
                    // 마지막 단이 아닌 경우에 로켓 분리
                    if (currentStage < fuelMax.Count - 1)
                    {
                        fuelRemain = fuelMax[currentStage + 1];
                        DisconnectStage();
                    }
                    // 마지막 단이면, 남은 연료 0으로 처리
                    else
                        fuelRemain = 0;
                }
                else
                    fuelRemain = value;
            }
            // 남은 연료량에 따라 로켓 질량 계산
            rocketMain_rigid.mass = rocketData.Weight[weightStage] + GetTotalRemainFuel() * BarrelToTon;
        } 
    }
    /// <summary>
    /// 각 단의 최대 연료량 리스트
    /// </summary>
    public List<float> fuelMax;
    /// <summary>
    /// 무기를 생성하기 위한 상위 Transform
    /// </summary>
    public Transform weaponParent;

    /// <summary>
    /// 각 단의 최대 출력 리스트
    /// </summary>
    public List<float> powerMax;

    /// <summary>
    /// 발사대 기준 축
    /// </summary>
    public GameObject launcherAxis;
    /// <summary>
    /// 로켓의 메인 오브젝트
    /// </summary>
    public GameObject rocket;
    /// <summary>
    /// 로켓 전체 이미지
    /// </summary>
    public Sprite rocket_image;
    /// <summary>
    /// 각 단의 Sprite Renderer
    /// </summary>
    public SpriteRenderer[] rocket_rends;
    /// <summary>
    /// 각 단의 RigidBody2D
    /// </summary>
    public List<Rigidbody2D> rocket_rigids;
    /// <summary>
    /// 현재 추진 중인 RigidBody2D
    /// </summary>
    public Rigidbody2D rocketMain_rigid;
    /// <summary>
    /// 현재 추진 중인 단 (0: n단 / 1: n-1단 [tail부터 추진 시작])
    /// </summary>
    public int currentStage = 0;
    /// <summary>
    /// 질량 계산을 위한 단 Index
    /// </summary>
    public int weightStage = 0;
    /// <summary>
    /// 발사대 RigidBody2D
    /// </summary>
    public Rigidbody2D launcher_rigid;

    /// <summary>
    /// 로켓 화염 이펙트
    /// </summary>
    public ParticleSystem fireEffect;
    /// <summary>
    /// 로켓 화염 이펙트 (일반 상태)
    /// </summary>
    public ParticleSystem normalFireEffect;
    /// <summary>
    /// 로켓 화염 이펙트 (부스터 상태)
    /// </summary>
    public ParticleSystem boosterFireEffect;
    /// <summary>
    /// 로켓에서 각 단의 상대 위치
    /// </summary>
    public List<Vector3> positionsOnStage;
    /// <summary>
    /// 발사 시의 연기 효과
    /// </summary>
    public ParticleSystem launchSmokeEffect;
    /// <summary>
    /// 로켓 스펙 데이터
    /// </summary>
    public RocketData rocketData;
    /// <summary>
    /// 로켓 HP바 오브젝트
    /// </summary>
    public GameObject hpBar;
    /// <summary>
    /// HP바 스프라이트 기반 슬라이더 (커스텀 슬라이더)
    /// </summary>
    public SpriteSlider hpBar_SprtSlider;
    /// <summary>
    /// 로켓 무적 처리 여부
    /// </summary>
    public bool isInvincible = false;
    /// <summary>
    /// HP에 따른 로켓 색상
    /// </summary>
    public Color[] rocketColorByHp;
    /// <summary>
    /// 로켓 화염 효과 관리 오브젝트
    /// </summary>
    public RocketFire rocketFire;
    /// <summary>
    /// 보조엔진의 RigidBody2D
    /// </summary>
    public Rigidbody2D vernierEngine;
    /// <summary>
    /// 보조엔진 로켓 화염 효과
    /// </summary>
    public ParticleSystem[] vernierEngineFires;
    /// <summary>
    /// 보호막 이미지 스프라이트 렌더러
    /// </summary>
    public SpriteRenderer shield;
    /// <summary>
    /// 보호막 애니메이션 효과 
    /// </summary>
    public Animation shield_ani;
    /// <summary>
    /// 대기권 돌파 이펙트
    /// </summary>
    public ParticleSystem atmosphereEffect;
    /// <summary>
    /// 로켓 폭발 이펙트
    /// </summary>
    public ParticleSystem explosionEffect;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        InitializeRocket();
    }
    
    /// <summary>
    /// 로켓 초기화
    /// </summary>
    public void InitializeRocket()
    {
        // 추진할 RigidBody 설정
        rocketMain_rigid = rocket_rigids[rocket_rigids.Count - 1];
        // 연료 최대 주유량 초기화 (스펙 데이터 복사)
        fuelMax = new List<float>(rocketData.Fuel);
        // 로켓 최대 출력량 초기화 (스펙 데이터 복사)
        powerMax = new List<float>(rocketData.MaxPower);
        // 현재 단에서의 연료 설정
        FuelRemain = fuelMax[currentStage];
        // 최대 HP / 실시간 HP 초기화
        maxHp = hp = rocketData.Hp;
        // 질량 초기화
        rocket_rigids[currentStage].mass = rocketData.Weight[weightStage];
        // 슬라이더 초기화
        hpBar_SprtSlider.maxValue = hp;
        hpBar_SprtSlider.SetValue(hp);
    }

    /// <summary>
    /// 로켓 방향이 오른쪽인지 여부 확인
    /// </summary>
    /// <returns>로켓의 진행 방향이 오른쪽인지 여부</returns>
    public bool IsRocketRight()
    {
        return rocketMain_rigid.velocity.x >= 0 ? true : false;
    }

    /// <summary>
    /// HP바 비활성화 대기 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator HpBarClose()
    {
        yield return new WaitForSeconds(1.0f);

        hpBar.SetActive(false);
    }

    /// <summary>
    /// HP 비율에 따라 로켓 색상 조정
    /// </summary>
    private void SetRocketColorByHp()
    {
        float per = hp / (float)maxHp;

        for (int i = 0; i < rocket_rends.Length; i++)
        {
            if (per <= 1)
                rocket_rends[i].color = rocketColorByHp[0];
            else if (per <= 0.5)
                rocket_rends[i].color = rocketColorByHp[1];
            else if (per < 0.3)
                rocket_rends[i].color = rocketColorByHp[2];
        }
    }

    /// <summary>
    /// 로켓 단 분리
    /// </summary>
    public void DisconnectStage()
    {
        // 추진 비활성화
        Launch.instance.AcceleratorUp();
        // 플레이어가 다시 추진하지 못하도록 버튼 비활성화
        Launch.instance.acceleratorButton.enabled = false;
        // 발사 버튼 비활성화
        Launch.instance.shot_button.SetActive(false);
        // 플레이어 회전 개입 비활성화
        Launch.instance.allowRotate = false;
        // 임시로 중력 0으로 처리
        float tempGravity = rocketMain_rigid.gravityScale;
        rocketMain_rigid.gravityScale = 0f;
        StartCoroutine(BeforeDisconnectDelay(1f, tempGravity));
    }

    /// <summary>
    /// 로켓 분리 직전 1차 딜레이 코루틴
    /// </summary>
    /// <param name="delay">지연 시간</param>
    /// <param name="gravity">기존 중력 값</param>
    IEnumerator BeforeDisconnectDelay(float delay, float gravity)
    {
        yield return new WaitForSeconds(delay);

        // 다음 분리될 단수의 물리적 설정
        rocket_rigids[currentStage].bodyType = RigidbodyType2D.Dynamic;
        rocket_rigids[currentStage].velocity = rocketMain_rigid.velocity;
        rocket_rigids[currentStage].GetComponent<Collider2D>().isTrigger = true;
        // 다음 단수로 pass
        currentStage++;
        // 보조엔진이 없으면 다음 질량 index로 pass
        if (!vernierEngine.gameObject.activeInHierarchy)
            weightStage++;
        // 로켓 분리 2차 딜레이 코루틴 시작
        StartCoroutine(DisconnectDelay(2f, gravity));
    }

    /// <summary>
    /// 로켓 분리 2차 딜레이 코루틴
    /// </summary>
    /// <param name="delay">지연 시간</param>
    /// <param name="gravity">기존 중력 값</param>
    /// <returns></returns>
    IEnumerator DisconnectDelay(float delay, float gravity)
    {
        yield return new WaitForSeconds(delay);
        // 분리된 로켓 비활성화
        rocket_rigids[currentStage - 1].gameObject.SetActive(false);
        // 중력 값 복원
        rocketMain_rigid.gravityScale = gravity;
        // 화염 이펙트 위치 조정
        fireEffect.transform.localPosition = positionsOnStage[currentStage];
        // 기준 출력 조정
        Launch.instance.stdPower = rocketData.MaxPower[currentStage];
        // 추진 활성화 및 유저 컨트롤 활성화 작업
        Launch.instance.AcceleratorDown();
        Launch.instance.acceleratorButton.enabled = true;
        Launch.instance.allowRotate = true;
        // 연료량 UI 업데이트
        Launch.instance.SetFuelStageText();
    }

    /// <summary>
    /// 로켓의 모든 연료량 합산
    /// </summary>
    /// <returns></returns>
    private float GetTotalRemainFuel()
    {
        float sum = 0;
        for (int i = currentStage + 1; i < fuelMax.Count; i++)
            sum += fuelMax[i];
        sum += fuelRemain;
        return sum;
    }

    /// <summary>
    /// 로켓 화염 이펙트 재생 시작
    /// </summary>
    public void FireEffectPlay()
    {
        if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine) && currentStage.Equals(0))
        {
            for (int i = 0; i < vernierEngineFires.Length; i++)
                vernierEngineFires[i].Play();
        }
        else
            fireEffect.Play();
    }
    /// <summary>
    /// 로켓 화염 이펙트 중단
    /// </summary>
    public void FireEffectStop()
    {
        if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine) && currentStage.Equals(0))
        {
            for (int i = 0; i < vernierEngineFires.Length; i++)
                vernierEngineFires[i].Stop();
        }
        else
            fireEffect.Stop();
    }

    /// <summary>
    /// 부스터 화염 효과 재생시작
    /// </summary>
    public void BoosterFireEffectPlay()
    {
        fireEffect.Stop();
        boosterFireEffect.gameObject.SetActive(true);
        boosterFireEffect.Play();
        fireEffect = boosterFireEffect;
    }

    /// <summary>
    /// 부스터 화염 효과 중단
    /// </summary>
    public void BoosterFireEffectStop()
    {
        boosterFireEffect.Stop();
        boosterFireEffect.gameObject.SetActive(false);
        fireEffect = normalFireEffect;
        if (Launch.instance.isAccerlate)
            fireEffect.Play();
    }

    /// <summary>
    /// 무적 효과 활성화
    /// </summary>
    /// <param name="time">활성화 시간</param>
    public void SetInvincible(float time)
    {
        isInvincible = true;
        StartCoroutine(InvincibleTime(time));
    }
    /// <summary>
    /// 무적 효과 비활성화 대기 코루틴
    /// </summary>
    /// <param name="time">활성화 시간</param>
    /// <returns></returns>
    IEnumerator InvincibleTime(float time)
    {
        yield return new WaitForSeconds(time);

        isInvincible = false;
    }
}

