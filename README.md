# Unity 액션 RPG 코드 설명서

이 문서는 프로젝트 내 주요 스크립트들의 역할과 기능을 정리한 문서입니다.  
---

##  Scripts

### Player 관련

- **Player.cs**  
  플레이어의 상태(Controller, Condition, Equipment) 초기화 및 소모 아이템 효과 처리

- **PlayerController.cs**  
  이동, 점프, 시선 회전, 벽 타기/매달리기 등 기본 조작 구현  
  `ToggleCursor()`, `ClimbWall()` 등 기능적 분리 우수

- **PlayerCondition.cs**  
  체력/스태미나/배고픔 관리, 상태 효과(Rainbow, Poison), 무적 처리

- **CharacterManager.cs**  
  싱글톤 패턴으로 플레이어 객체를 전역 접근 가능하게 관리

---

###  전투 / 장비

- **Equip.cs**  
  장비 아이템의 기본 인터페이스. `OnAttackInput()` 오버라이드 구조 제공

- **EquipTool.cs**  
  근접 무기용 장비. 자원 채집/전투 데미지 여부 설정 가능

- **Bow.cs / Arrow.cs**  
  원거리 무기 시스템. 화살 생성, 발사, 충돌 후 루팅 전환 처리

---

###  NPC 및 AI

- **NPC.cs**  
  AI 상태(Wandering, Attacking 등), 진영(Faction), 탐색/공격/도주 행동 포함  
  `NavMeshAgent` 기반으로 동작하며, 아군/적군 인식 및 전투 가능

---

###  아이템 / 상호작용

- **ItemObject.cs**  
  `IInteractable` 인터페이스를 구현, 아이템 루팅 처리

- **Interaction.cs**  
  카메라 전방에 있는 상호작용 가능한 오브젝트 탐색 및 UI 프롬프트 출력

- **Resource.cs**  
  자원 채집 처리. `capacity` 기반으로 드랍 후 파괴됨

---

###  기타 유틸

- **ThirdPersonCamera.cs**  
  3인칭 카메라 시스템

- **PlayerRotation.cs**  
  플레이어가 카메라 시선 방향으로 회전하도록 처리

- **FootSteps.cs**  
  Rigidbody 속도를 기반으로 걷기 소리 출력

---

##  싱글톤 구조

- **CharacterManager.cs** 는 `Player`에 접근하기 위한 글로벌 싱글톤으로 동작합니다.  
  추후 확장 시 `GameManager`, `UIManager` 등의 싱글톤도 상속 구조로 분리 가능

---

##  참고 사항

- 대부분의 스크립트는 `Player`, `Equip`, `NPC`, `UI` 등 역할별 폴더에 정리되어 있습니다.
- Input System 기반이며, `InputAction.CallbackContext`를 활용해 입력 처리

---

