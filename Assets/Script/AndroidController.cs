using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 뒤로가기, 메시지 등의 안드로이드 일부 기능을 관리하는 컨트롤러 클래스
/// </summary>
public class AndroidController : MonoBehaviour
{
    /// <summary>
    /// 메시지 배경 오브젝트
    /// </summary>
    public GameObject message_obj;
    /// <summary>
    /// 메시지 등장 애니메이션 효과
    /// </summary>
    public Animation messageEffect_ani;
    /// <summary>
    /// 메시지 텍스트
    /// </summary>
    public Text message_text;
    /// <summary>
    /// 뒤로가기 버튼 클릭 횟수
    /// </summary>
    private int count = 0;
    /// <summary>
    /// 뒤로가기 버튼 클릭 중인지
    /// </summary>
    private bool backKeyDown = false;
    /// <summary>
    /// 애니메이션 효과가 끝날때 까지 대기하는 코루틴
    /// </summary>
    private IEnumerator waitForEndOfAnimation;
    /// <summary>
    /// 메시지 비활성화 효과 대기 코루틴
    /// </summary>
    private IEnumerator messageFloating;

    private void Start()
    {
        StartCoroutine(Backkey(0));
    }

    /// <summary>
    /// 뒤로가기 키 반응 코루틴
    /// </summary>
    /// <param name="delay">Backkey 인식 딜레이</param>
    IEnumerator Backkey(float delay)
    {
        // 딜레이가 없으면 프레임 간격으로 대기
        if (delay.Equals(0))
            yield return new WaitForEndOfFrame();
        // 딜레이가 있으면 실제 시간만큼(스케일링된 시간 미적용) 딜레이
        else
            yield return new WaitForSecondsRealtime(delay);

        // 뒤로가기 키가 인식되면
        if (Input.GetKey(KeyCode.Escape))
        {
            // backkey가 눌렸다는 것을 표시하고
            backKeyDown = true;
            // 여러번 눌린 경우에 앱 종료
            if (count > 0)
                Application.Quit();
            // 아니면 뒤로가기 횟수를 늘리고, 카운트 초기화 대기 코루틴 시작
            else
            {
                count++;
                ExitMessage();
                StartCoroutine(InitializeDoubleBack());
            }
            // 뒤로가기가 바로 다시 인식되지 않도록 0.1초 딜레이
            StartCoroutine(Backkey(0.1f));
        }

        // 일반적인 상태에서 딜레이 없이 코루틴 반복
        if (!backKeyDown)
            StartCoroutine(Backkey(0));
        else
            backKeyDown = false;
    }
    /// <summary>
    /// 게임 종료 안내 메시지를 현지화(한/영)에 맞추어 불러와서 적용한다.
    /// </summary>
    private void ExitMessage()
    {
        ShowMessage(LocalizationManager.instance.GetLocalizedValue("exit_message"));
    }

    /// <summary>
    /// 토스트 메시지처럼 메시지를 노출한다.
    /// </summary>
    /// <param name="message">메시지 내용</param>
    /// <param name="time">표기 시간</param>
    public void ShowMessage(string message, float time = 1f)
    {
        // 이전 메시지 비활성화 대기 코루틴 종료
        if (waitForEndOfAnimation != null)
            StopCoroutine(waitForEndOfAnimation);
        if (messageFloating != null)
            StopCoroutine(messageFloating);

        // 메시지 오브젝트 활성화 후 애니메이션 효과 재생
        message_obj.SetActive(true);
        message_text.text = message;
        messageEffect_ani["MessageEffect"].time = 0;
        messageEffect_ani["MessageEffect"].speed = 1f;
        messageEffect_ani.Play();

        // 메시지 비활성화 대기 코루틴 시작
        messageFloating = MessageFloating(time);
        StartCoroutine(messageFloating);
    }

    /// <summary>
    /// 뒤로가기 카운트 횟수 초기화 대기 코루틴
    /// </summary>
    IEnumerator InitializeDoubleBack()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        count = 0;
    }

    /// <summary>
    /// 메시지 비활성화 대기 코루틴
    /// </summary>
    /// <param name="time">메시지 대기 시간</param>
    IEnumerator MessageFloating(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        // 메시지 애니메이션 효과 역재생 설정 및 재생
        messageEffect_ani["MessageEffect"].speed = -1;
        messageEffect_ani["MessageEffect"].time = messageEffect_ani["MessageEffect"].length;
        messageEffect_ani.Play();
        // 메시지 애니메이션 종료 후 메시지 오브젝트 비활성화 대기 코루틴 등록
        waitForEndOfAnimation = PlayManager.instance.RWaitAnimation(messageEffect_ani, CloseWindow, message_obj);
        StartCoroutine(waitForEndOfAnimation);
    }

    /// <summary>
    /// 메뉴창 비활성화(delegate 등록용)
    /// </summary>
    /// <param name="obj">비활성화할 게임 오브젝트</param>
    private void CloseWindow(GameObject obj)
    {
        obj.SetActive(false);
    }
}
