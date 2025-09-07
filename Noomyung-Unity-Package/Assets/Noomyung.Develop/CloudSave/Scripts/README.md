# CloudSave 테스트 스크립트

CloudSave 패키지의 기능을 테스트하기 위한 개발용 스크립트들입니다.

## 포함된 파일들

### CloudSaveTestData.cs
테스트에 사용되는 데이터 클래스들을 정의합니다:
- `PlayerTestData`: 플레이어 정보를 저장하는 테스트 데이터 클래스
- `GameSettingsData`: 게임 설정을 저장하는 테스트 데이터 클래스

### CloudSaveTestManager.cs
CloudSave 기능을 종합적으로 테스트하는 매니저 클래스입니다.

#### 주요 기능:
- **기본 바이트 배열 테스트**: 바이트 데이터의 저장/로드 기능 테스트
- **JSON 직렬화 테스트**: 복잡한 객체의 JSON 직렬화/역직렬화 테스트
- **키 관리 테스트**: 키 존재 여부 확인, 키 목록 조회, 키 삭제 기능 테스트
- **오류 처리 테스트**: 예외 상황에 대한 적절한 처리 확인

#### 사용법:
1. GameObject에 `CloudSaveTestManager` 컴포넌트를 추가합니다
2. Inspector에서 테스트 설정을 조정합니다:
   - `Use Ugs Provider`: UGS 또는 PlayFab 제공업체 선택
   - `Run Tests On Start`: 시작 시 자동 테스트 실행 여부
   - `Enable Detailed Logging`: 상세 로깅 활성화 여부
3. Context Menu를 통해 개별 테스트를 실행할 수 있습니다

### CloudSaveTestUI.cs
테스트를 위한 UI 컨트롤러입니다.

#### 주요 기능:
- 테스트 버튼들을 통한 수동 테스트 실행
- 실시간 로그 출력 및 확인
- 클라우드 제공업체 선택 (UGS/PlayFab)
- 상세 로깅 설정

#### UI 구성 요소:
- **Initialize Button**: CloudSave 서비스 초기화
- **Run All Tests Button**: 모든 테스트 실행
- **Byte Array Test Button**: 바이트 배열 테스트만 실행
- **JSON Test Button**: JSON 직렬화 테스트만 실행
- **Key Management Test Button**: 키 관리 테스트만 실행
- **Error Handling Test Button**: 오류 처리 테스트만 실행
- **Clear Log Button**: 로그 지우기
- **Use UGS Toggle**: UGS/PlayFab 제공업체 선택
- **Detailed Logging Toggle**: 상세 로깅 설정

## 테스트 시나리오

### 1. 기본 바이트 배열 테스트
- UTF-8 인코딩된 문자열을 바이트 배열로 저장
- 저장된 데이터를 로드하여 원본과 비교
- 데이터 정리

### 2. JSON 직렬화 테스트
- **플레이어 데이터**: 이름, 레벨, 점수, 경험치, 프리미엄 여부
- **게임 설정**: 볼륨, 화면 모드, 품질, 언어 설정
- **복잡한 데이터**: 중첩된 딕셔너리와 리스트 구조

### 3. 키 관리 테스트
- 여러 테스트 키 생성
- 전체 키 목록 조회
- 접두사를 이용한 키 필터링
- 키 존재 여부 확인
- 키 삭제

### 4. 오류 처리 테스트
- 존재하지 않는 키 로드 시 null 반환 확인
- 빈 키로 저장 시도 시 예외 처리 확인
- null 데이터 저장 허용 여부 확인

## 사용 전 준비사항

### UGS 사용 시:
1. Unity Gaming Services 패키지 설치:
   - `Unity.Services.Core`
   - `Unity.Services.CloudSave`
2. Scripting Define Symbols에 `UGS_PACKAGE` 추가
3. Unity Gaming Services 프로젝트 설정

### PlayFab 사용 시:
1. PlayFab SDK 설치
2. Scripting Define Symbols에 `PLAYFAB_PACKAGE` 추가
3. PlayFab 프로젝트 설정

## 테스트 실행 방법

### 방법 1: 자동 테스트
1. `CloudSaveTestManager` 컴포넌트의 `Run Tests On Start` 옵션을 활성화
2. 씬을 실행하면 자동으로 모든 테스트가 실행됩니다

### 방법 2: 수동 테스트
1. `CloudSaveTestManager` 컴포넌트를 선택
2. Inspector에서 Context Menu를 열어 원하는 테스트를 선택하여 실행

### 방법 3: UI를 통한 테스트
1. `CloudSaveTestUI` 컴포넌트가 있는 UI를 설정
2. 각 버튼을 클릭하여 원하는 테스트를 실행

## 로그 확인

테스트 실행 중 모든 로그는 Unity Console과 UI 로그 창에 출력됩니다:
- 일반 정보: 흰색 텍스트
- 성공 메시지: 녹색 텍스트
- 오류 메시지: 빨간색 텍스트

## 주의사항

1. **네트워크 연결**: 클라우드 저장 서비스는 인터넷 연결이 필요합니다
2. **인증**: UGS나 PlayFab에 대한 적절한 인증 설정이 필요합니다
3. **테스트 데이터**: 테스트 실행 후 생성된 데이터는 자동으로 정리됩니다
4. **성능**: 대량의 테스트 데이터를 생성할 때는 네트워크 지연을 고려하세요

## 문제 해결

### 일반적인 문제들:
1. **초기화 실패**: 클라우드 서비스 패키지가 올바르게 설치되었는지 확인
2. **네트워크 오류**: 인터넷 연결 및 클라우드 서비스 설정 확인
3. **인증 오류**: UGS/PlayFab 프로젝트 설정 및 인증 정보 확인
4. **직렬화 오류**: 테스트 데이터 클래스의 직렬화 가능 여부 확인
