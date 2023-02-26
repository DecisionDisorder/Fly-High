using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 총알 관련 연산을 담당하는 클래스
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// 총알이 장전되어있는 무기
    /// </summary>
    public Weapon weapon;
    /// <summary>
    /// 메인 카메라
    /// </summary>
    private Camera mainCamera;
    /// <summary>
    /// 총알의 물리 연산을 도와줄 rigidbody
    /// </summary>
    private Rigidbody2D rigid;

    /// <summary>
    /// 총알 스폰 데이터 오브젝트
    /// </summary>
    private Spawnable spawnable;
    /// <summary>
    /// 카메라 영역 탐지 관련 계산 마진
    /// </summary>
    private float margin = 15f;
    /// <summary>
    /// 발사 되었는지 여부
    /// </summary>
    private bool isShot = false;
    /// <summary>
    /// 총알 이미지 스프라이트 렌더러
    /// </summary>
    public SpriteRenderer bulletImg;
    /// <summary>
    /// 폭발 이펙트 파티클
    /// </summary>
    public ParticleSystem explosionEffect;
    /// <summary>
    /// 발사 흔적 연기 이펙트 파티클
    /// </summary>
    public ParticleSystem rearSmokeEffect;
    
    /// <summary>
    /// 총알 멤버 변수 초기화 작업
    /// </summary>
    private void Initialize()
    {
        mainCamera = Camera.main;
        rigid = GetComponent<Rigidbody2D>();
        spawnable = weapon.bulletSpawnable;
    }

    /// <summary>
    /// 총알 격발 
    /// </summary>
    public void Shot()
    {
        // 첫 발에 초기화 작업
        if (rigid == null)
            Initialize();
        
        // 물리 연산 적용
        rigid.bodyType = RigidbodyType2D.Dynamic;
        rigid.velocity = RocketSet.instance.rocketMain_rigid.velocity; // 로켓 관성 적용
        rigid.AddForce(spawnable.parent.up * weapon.weaponSpecData.BulletForce, ForceMode2D.Impulse);
        isShot = true;

        // 발사 흔적 연기가 나오는 경우
        if (rearSmokeEffect != null)
            rearSmokeEffect.Play();

        // 카메라 영역 벗어났을 때 총알을 비활성화 할 코루틴
        StartCoroutine(DetectOutOfCamera());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 격발이 된 상태에서 장애물에 부딪혔을 때 반응
        if (collision.tag.Equals("Obstacle") && isShot)
        {
            // 히트 판정
            Hit(collision);
            // 휴대폰 진동
            Vibration.Vibrate(100); 

            // 폭발 이펙트가 없으면 게임 오브젝트 회수
            if (explosionEffect == null)
            {
                //cameraFollow.ShakeCamera(0.3f, 0.01f);
                SpawnManager.instance.ReturnObject(gameObject, spawnable);
            }
            // 폭발 이펙트가 있으면 관련 효과음 및 이펙트 재생 후 회수 대기
            else
            {
                //cameraFollow.ShakeCamera(0.3f, 0.02f);
                PlayManager.instance.PlayExplosionSound();
                rearSmokeEffect.Stop();
                explosionEffect.Play();
                bulletImg.color = new Color(0, 0, 0, 0);
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = 0;
                rigid.bodyType = RigidbodyType2D.Kinematic;
                StartCoroutine(WaitForExplosion(collision));
            }
        }
    }

    // 히트 판정
    private void Hit(Collider2D collision)
    {
        // 충돌 대상 오브젝트에서의 Obstacle 오브젝트 
        Obstacle obs = collision.gameObject.GetComponent<Obstacle>();
        // 해당 장애물의 체력(내구도)를 총알의 데미지만큼 차감
        obs.hp -= weapon.weaponSpecData.Damage;
        // 장애물의 데미지 효과 재생
        obs.DamageEffect();
        // 장애물의 체력(내구도)가 0 이하가 되면 파괴 효과 재생
        if (obs.hp <= 0)
            obs.Destroy();
    }

    /// <summary>
    /// 회수된 총알 초기화 작업
    /// </summary>
    private void ResetBullet()
    {
        isShot = false;
        rigid.bodyType = RigidbodyType2D.Kinematic;
        bulletImg.color = Color.white;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        SpawnManager.instance.ReturnObject(gameObject, spawnable);
    }

    /// <summary>
    /// 폭발 효과 대기 코루틴
    /// </summary>
    /// <param name="collision">충돌 대상</param>
    IEnumerator WaitForExplosion(Collider2D collision)
    {
        while (explosionEffect.isPlaying)
            yield return new WaitForEndOfFrame();

        // 폭발 효과 재생 후 총알 초기화
        ResetBullet();
    }

    /// <summary>
    /// 총알이 카메라 영역 밖으로 나가는지 확인하는 코루틴
    /// </summary>
    IEnumerator DetectOutOfCamera()
    {
        yield return new WaitForSeconds(0.1f);

        // 총알의 중력 값을 로켓의 중력 값과 동기화
        rigid.gravityScale = RocketSet.instance.rocketMain_rigid.gravityScale;

        // 총알의 위치를 카메라 좌표계로 환산하여 범위를 벗어나는지 확인 후 초기화
        Vector3 positonOnCamera = mainCamera.WorldToScreenPoint(gameObject.transform.position);
        if (positonOnCamera.x < -margin || positonOnCamera.y < -margin || positonOnCamera.x > Screen.width + 3 * margin || positonOnCamera.y > Screen.height + 3 * margin)
        {
            ResetBullet();
        }

        // 게임 오브젝트가 활성화 되어있으면 지속적 탐지
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DetectOutOfCamera());
        }
    }
}
