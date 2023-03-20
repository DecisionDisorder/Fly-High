# Fly-High 
안드로이드 플랫폼 출시를 목표로 개발했던 모바일 게임 프로젝트 입니다.

[Developer]
- 김현종 (guswhd990507@naver.com)

## 프로젝트 소개
### [개발 동기]
- 이전 2D UI 기반의 게임에서 나아가 2D Sprite 기반의 게임을 공부하며 개발해보고, 물리적 상호작용을 적극적으로 사용해보고자 하였습니다.
- 쇼핑카트 히어로에서 영감을 받아, 아예 우주로 끝 없이 컨트롤하며 날아가는 게임을 만들어보고자 하였습니다.

### [게임 설명]
- 발사대에서 로켓의 발사 각도와 파워를 설정하여 발사를 시작합니다.
- 로켓이 발사되고 대기권을 벗어나면, 로켓을 조작할 수 있습니다.
- 로켓 조작은 좌/우로 회전시키거나, 파워를 조절할 수 있습니다.
- 로켓에는 내구도(HP)가 있으며, 내구도가 모두 닳으면 로켓이 폭발하며 게임이 종료됩니다.
- 로켓의 내구도를 떨어트리는 요소는 다음과 같습니다.
	- 땅에 떨어지기 (-100%)
	- 장애물과 부딪히기 (-30%)
- 비행 중에 다음과 같은 보너스 오브젝트를 획득할 수 있습니다.
	- 코인으로 비행하여 보너스 코인을 획득할 수 있습니다.
	- 반짝이는 별로 비행하여 보너스 점수를 획득할 수 있습니다.
	- 기름통으로 비행하여 긴급 급유를 할 수 있습니다.
- 상점에서 무기, 탄환, 아이템과 로켓을 구매할 수 있습니다.
- 무기를 장착하고 탄환을 발사하여 우주에 분포한 장애물을 없앨 수 있습니다.
- 아이템을 사용하여 비행 중 특수한 효과를 발동할 수 있습니다.
- 각기 다른 스펙으로 구성된 로켓을 구매할 수 있습니다.
	- 2단 로켓, 연비 위주, 파워 위주 등 각 로켓 마다의 컨셉에 따라 마음에 드는 로켓을 선택할 수있습니다.

### [게임 플레이 시작]
본 게임은 안드로이드 배포를 기준으로 개발되어 iOS 빌드에 대한 테스트는 진행되지 않았습니다.  
게임 설치는 APK 디렉토리의 Alpha 0.0.11 버전의 APK를 다운로드 받아 설치할 수 있습니다.  
(Minimum API Level: Android 5.0)

### [스크린샷]
<figure>
	<img src="/Screenshots/Screenshot1.png" width=50% height=50%/>
	<img src="/Screenshots/Screenshot2.png" width=50% height=50%/>
	<img src="/Screenshots/Screenshot3.png" width=50% height=50%/>
	<img src="/Screenshots/Screenshot4.png" width=50% height=50%/>
	<img src="/Screenshots/Screenshot5.png" width=50% height=50%/>
	<img src="/Screenshots/Screenshot6.png" width=50% height=50%/>
	<img src="/Screenshots/Screenshot7.png" width=50% height=50%/>
	<img src="/Screenshots/Screenshot8.png" width=50% height=50%/>
</figure>

## 기술 관련 소개
### [개발환경 및 기술 스택]
- Unity 2020.2.4f1
- Visual Studio 2019
- VCS: Unity Collaborate → GitHub

### [개발 이슈]
1. 최적화
APK 빌드 테스트 때 프레임이 30에서 40으로 낮게 유지되는 문제가 있었습니다.  
프레임을 안정적으로 높이기 위해 프로파일링을 진행하여,  string.Format()를 StringBuilder.AppendFormat으로 교체하여 약 62%가량 개선하였습니다.  
또한, 자주 업데이트 되는 UI를 별도의 Canvas로 분리하였으며, physics나 빌드 설정들을 간소화하여 최적화를 진행했습니다.   
그 결과, 발사 직전까지 60프레임을 유지하는 수준으로 최적화가 진행되었으나, 발사 직후부터 다시 3~40 프레임으로 떨어지는 문제가 발생하였습니다.  
여러가지 세팅을 바꿔본 결과,  빌드 설정에서 Graphic API Auto를 true로 설정하여 안정적으로 60프레임을 유지하게 되었습니다.

2. 로컬라이징 시스템 도입
게임 개발 초기 단계부터 언어 설정을 고려하여 개발하기 시작했습니다.  
기본적으로 영어와 한국어 전환을 지원하며, 태블릿 UI의 설정에서 언어 설정이 가능합니다.  
Excel을 이용하여 key와 value를 정리했으며, key에는 언어 공통으로 사용할 코드, value에는 언어별 텍스트를 입력하였습니다.  
xlsx 파일을 json 형식으로 변환 후, C#의 Dictionary로 읽어들여 Text UI에 적용하였습니다.  
(관련 코드: [LocalizationManager.cs](https://github.com/DecisionDisorder/Fly-High/blob/master/Assets/Script/LocalizationManager.cs "LocalizationManager.cs"))

## 기타
### [외부 리소스 출처]
- 폰트1: 폰트랩(주) 'LAB디지털'
- 폰트2: (주)위메프 '위메프체'
- 폰트3: 비씨카드(주) 'BC카드 글꼴'
