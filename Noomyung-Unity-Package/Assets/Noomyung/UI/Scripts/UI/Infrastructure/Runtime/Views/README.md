# UI View Composition System

UI 뷰의 책임을 분리하고 조합 가능한 컴포넌트 시스템입니다. 각 뷰가 명확한 단일 책임을 가지도록 설계되었습니다.

## 컴포넌트 개요

### 1. UiVisibilityView
- **책임**: GameObject의 active/inactive 상태 제어
- **기능**: 
  - `ShowAsync()` / `HideAsync()` - 가시성 토글
  - UiTransitionView가 연결되어 있으면 전환 효과 사용, 없으면 즉시 활성화/비활성화
  - `IsVisible` - 현재 가시성 상태 확인

### 2. UiTransitionView
- **책임**: 전환 효과 (페이드 인/아웃, 애니메이션)
- **기능**:
  - `PlayShowAsync()` / `PlayHideAsync()` - 전환 효과 재생
  - `IsTransitioning` - 전환 중인지 확인
  - CanvasGroup을 사용한 페이드 효과

### 3. UiHoverView
- **책임**: 호버 상호작용 효과
- **기능**:
  - `HoverEnterAsync()` / `HoverExitAsync()` - 호버 효과 재생
  - `IsHovered` - 현재 호버 상태 확인
  - IPointerEnterHandler, IPointerExitHandler 구현

### 4. UiStateView
- **책임**: UI 상태 전환 관리
- **기능**:
  - `ChangeStateAsync(UiState state)` - 상태 변경
  - `CurrentState` - 현재 상태 확인
  - Normal, Hovered, Pressed, Disabled, Selected 상태 지원

## 사용 예제

### 예제 A: UiVisibilityView만 사용
```csharp
// GameObject에 UiVisibilityView만 추가
var visibilityView = gameObject.GetComponent<IUiVisibilityView>();

// 즉시 On/Off (SetActive)
await visibilityView.ShowAsync();  // SetActive(true)
await visibilityView.HideAsync();  // SetActive(false)
```

### 예제 B: UiVisibilityView + UiTransitionView 조합
```csharp
// GameObject에 UiVisibilityView와 UiTransitionView 모두 추가
var visibilityView = gameObject.GetComponent<IUiVisibilityView>();
var transitionView = gameObject.GetComponent<IUiTransitionView>();

// 전환 효과와 함께 On/Off
await visibilityView.ShowAsync();  // TransitionView.PlayShowAsync() 호출
await visibilityView.HideAsync();  // TransitionView.PlayHideAsync() 호출
```

### 예제 C: UiHoverView만 사용
```csharp
// GameObject에 UiHoverView만 추가
var hoverView = gameObject.GetComponent<IUiHoverView>();

// 호버 효과
await hoverView.HoverEnterAsync();  // 스케일 확대 효과
await hoverView.HoverExitAsync();   // 원래 크기로 복원
```

### 예제 D: UiStateView 사용
```csharp
// GameObject에 UiStateView 추가
var stateView = gameObject.GetComponent<IUiStateView>();

// 상태 변경
await stateView.ChangeStateAsync(UiState.Hovered);
await stateView.ChangeStateAsync(UiState.Pressed);
await stateView.ChangeStateAsync(UiState.Disabled);
```

## 조합 패턴

### 1. 단순 가시성 제어
- **컴포넌트**: UiVisibilityView만
- **용도**: 간단한 On/Off 기능
- **특징**: 전환 효과 없이 즉시 활성화/비활성화

### 2. 가시성 + 전환 효과
- **컴포넌트**: UiVisibilityView + UiTransitionView
- **용도**: 부드러운 페이드 인/아웃 효과
- **특징**: UiVisibilityView가 UiTransitionView를 자동으로 사용

### 3. 호버 효과만
- **컴포넌트**: UiHoverView만
- **용도**: 마우스 호버 시 시각적 피드백
- **특징**: 가시성과 독립적인 호버 효과

### 4. 복합 상태 관리
- **컴포넌트**: UiStateView + (선택적) 다른 뷰들
- **용도**: 복잡한 UI 상태 전환
- **특징**: 여러 상태 간의 부드러운 전환

## 설정 옵션

### UiTransitionView
- `FadeInDuration`: 페이드 인 지속 시간
- `FadeOutDuration`: 페이드 아웃 지속 시간
- `FadeInCurve` / `FadeOutCurve`: 애니메이션 커브
- `IgnoreTimeScale`: 시간 스케일 무시 여부

### UiHoverView
- `HoverScale`: 호버 시 스케일 배율
- `HoverDuration`: 호버 효과 지속 시간
- `HoverCurve`: 애니메이션 커브
- `IgnoreTimeScale`: 시간 스케일 무시 여부

### UiStateView
- `InitialState`: 초기 상태
- `StateTransitionDuration`: 상태 전환 지속 시간
- `NormalColor` / `HoveredColor` / `PressedColor` / `DisabledColor` / `SelectedColor`: 각 상태별 색상

## 비동기 처리

모든 전환 효과는 UniTask를 사용하여 비동기로 처리됩니다:

```csharp
// 취소 토큰과 함께 사용
var cts = new CancellationTokenSource();
await visibilityView.ShowAsync(cts.Token);

// 취소
cts.Cancel();
```

## 확장성

각 뷰는 독립적으로 작동하므로 필요에 따라 조합할 수 있습니다:

- **최소 구성**: UiVisibilityView만
- **기본 구성**: UiVisibilityView + UiTransitionView
- **인터랙션 추가**: UiHoverView 추가
- **복합 상태**: UiStateView 추가

이러한 조합을 통해 다양한 UI 요구사항을 유연하게 처리할 수 있습니다.
