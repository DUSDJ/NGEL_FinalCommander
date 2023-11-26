using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FC
{
    public class GameManager : MonoBehaviour
    {

        #region SingleTone

        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();

                    if (!_instance)
                    {
                        var prefab = Resources.Load<GameManager>("Manager/GameManager");
                        _instance = Instantiate(prefab);

                        if (!_instance)
                        {
                            Debug.LogError("GameManager is null");
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion


        [HideInInspector] public EnumGameState NowGameState = EnumGameState.OutGame;



        [Header("인게임 BGM")]
        public string BGMKey;

        private IEnumerator mainRoutine;
        private IEnumerator locationRoutine;
        private IEnumerator battleRoutine;

        private bool isInit = false;


        [Header("적 증가할때까지의 시간(초)")]
        public float MonsterAddTime = 3.0f;

        [Header("적 생성 시간(초)")]
        public float MonsterSpawnTime = 2.0f;

        [Header("한번에 생성되는 몬스터 수")]
        public int NumOfMonsterSpawn = 2;


        [Header("골드")]
        public long gold = 10000;
        public long Gold
        {
            get
            {
                return gold;
            }
            set 
            {
                if(value <= 0)
                {
                    value = 0;
                }

                gold = value;
                UIManager.Instance?.InventoryUI?.UpdateGold(value);
            }
        }

        [Header("초당 획득 골드")]
        public float IncomeAddTime = 1.0f;
        public long Income = 30;        


        #region Awake & Init


        private void Awake()
        {
            #region SingleTon

            if (Instance == this)
            {
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            #endregion

            StartCoroutine(InitRoutine());
        }


        private IEnumerator InitRoutine()
        {
            if (isInit)
            {
                yield break;
            }
            else
            {
                isInit = true;
            }
            

            // 글로벌 화면 즉시 암전
            UIManager.Instance.InstantFade(false);

            // Load Manager
            /* DataBase */
            yield return Database.Instance.InitRoutine();

            /* Audio Manager */
            yield return AudioManager.Instance.InitRoutine();

            /* Effect Manager */
            yield return EffectManager.Instance.InitRoutine();

            /* UI Manager */
            yield return UIManager.Instance.InitRoutine();
            
            /* Monster Manager */
            yield return MonsterManager.Instance.InitRoutine();



            // 글로벌 즉시 암전 해제
            UIManager.Instance.InstantFade(true);



            // InCome
            mainRoutine = MainRoutine();
            StartCoroutine(mainRoutine);

            // Location
            locationRoutine = LocationRoutine();
            StartCoroutine(locationRoutine);

            Debug.Log("Init End!");

            yield return null;
        }




        private IEnumerator MainRoutine()
        {
            var wait = new WaitForSeconds(IncomeAddTime);
            

            while (true)
            {
                if(NowGameState != EnumGameState.OutGame)
                {                    
                    yield return null;
                    continue;
                }

                Gold += Income;
                UIManager.Instance.InventoryUI.SetGoldEffect(string.Format("+{0}", Income));

                yield return wait;
            }

        }



        private IEnumerator LocationRoutine()
        {
            var wait = new WaitForSeconds(MonsterAddTime);


            while (true)
            {
                if (NowGameState != EnumGameState.OutGame)
                {
                    yield return null;
                    continue;
                }

                UIManager.Instance.WorldMapUI.UpdateLocation();
                yield return wait;
            }

        }

        
        private IEnumerator BattleRoutine()
        {
            var wait = new WaitForSeconds(MonsterSpawnTime);
            // var slots = BattleLocation.NumOfSlots;
            var slots = NumOfMonsterSpawn;

            var monsterPrefabs = BattleLocation.MonsterPrefabs;

            while (true)
            {
                // 남은 생성값 확인
                var count = BattleLocation.NowMonsterCount;
                var now = MonsterManager.Instance.GetActiveMonster().Count;

                // 현존하는 수가 몬스터 수보다 많으면 생성 종료
                if(now >= count)
                {
                    yield break;
                }



                // N초당 Slot 수만큼 생성
                // 한 포인트에는 하나만 생성되는 방식 (일주하면 중복 가능)
                var spawnPointList = new List<Transform>();
                for (int i = 0; i < UIManager.Instance.BattleGroundUI.SpawnPoint.Count; i++)
                {
                    spawnPointList.Add(UIManager.Instance.BattleGroundUI.SpawnPoint[i]);
                }
                
                for (int i = 0; i < slots; i++)
                {
                    if(now >= count)
                    {
                        yield break;
                    }

                    if(spawnPointList.Count == 0)
                    {
                        for (int j = 0; j < UIManager.Instance.BattleGroundUI.SpawnPoint.Count; j++)
                        {
                            spawnPointList.Add(UIManager.Instance.BattleGroundUI.SpawnPoint[j]);
                        }
                    }

                    int rand = UnityEngine.Random.Range(0, monsterPrefabs.Count);
                    var randMonster = monsterPrefabs[rand];

                    int spawnPointRand = UnityEngine.Random.Range(0, spawnPointList.Count);
                    // int spawnPointRand = UnityEngine.Random.Range(0, UIManager.Instance.BattleGroundUI.SpawnPoint.Count);

                    var point = spawnPointList[spawnPointRand];
                    spawnPointList.RemoveAt(spawnPointRand);

                    MonsterManager.Instance.SetMonster(randMonster.name, point.position);

                    // 생성+1
                    now += 1;
                }
                

                yield return wait;
            }

        }



        #endregion





        #region Battle

        [HideInInspector] public ElementLocation BattleLocation;


        public List<Hero> GetActiveHeroes()
        {
            List<Hero> list = new List<Hero>();

            var heroes = BattleLocation.HeroList;

            for (int i = 0; i < heroes.Count; i++)
            {
                if (heroes[i].IsAlive)
                {
                    list.Add(heroes[i]);
                }
            }

            return list;
        }


        public void Engage()
        {
            // 이미 배틀중이면 처리X
            if(NowGameState == EnumGameState.Battle)
            {
                return;
            }

            // 출격
            if(UIManager.Instance.BattleGroundUI.SelectedLocation.NowOwner == EnumLocationOwner.Enemy)
            {
                if (!UIManager.Instance.BattleGroundUI.CheckCanEngage())
                {
                    return;
                }

                // UI 처리 & 유닛 목록 가져옴
                BattleLocation = UIManager.Instance.BattleGroundUI.SelectedLocation;
                BattleLocation.SetHero(UIManager.Instance.BattleGroundUI.EngageHeroList());

                Battle();
            }
            // 수납
            else
            {
                UIManager.Instance.BattleGroundUI.CleanHeroes();

                UIManager.Instance.BattleGroundUI.SelectedLocation.SetHero(
                    UIManager.Instance.BattleGroundUI.StoreHeroList()
                    );

                UIManager.Instance.BattleGroundUI.CleanElements();
                UIManager.Instance.BattleGroundUI.UpdateHero();
            }
           
        }


        public void Battle()
        {
            // Engage로 유발됨
            // State 변경
            NowGameState = EnumGameState.Battle;

            // 몬스터 생성 시작
            if (battleRoutine != null)
            {
                StopCoroutine(battleRoutine);
            }
            battleRoutine = BattleRoutine();
            StartCoroutine(battleRoutine);


            // ~ 모든 영웅이 죽거나 (Hero Dead Event)
            // ~ 모든 몬스터가 죽을 때까지 (Monster Dead Event)

        }


        public void HeroDead(Hero h)
        {
            // 승리조건
            int leftHero = 0;
            for (int i = 0; i < BattleLocation.HeroList.Count; i++)
            {
                if (BattleLocation.HeroList[i].IsAlive)
                {
                    leftHero += 1;
                }
            }

            if (leftHero <= 0)
            {
                BattleLost();
            }
        }


        public void MonsterDead(Monster m)
        {
            BattleLocation.DecreaseMonster(1);


            // 승리조건
            if(BattleLocation.NowMonsterCount <= 0)
            {
                BattleWin();
            }
        }

        

        private void BattleWin()
        {
            Debug.Log("Win");

            NowGameState = EnumGameState.OutGame;

            // 루틴 정지
            if (battleRoutine != null)
            {
                StopCoroutine(battleRoutine);
            }
            battleRoutine = null;


            // Reward
            Income += BattleLocation.RewardGold;


            // Hero는 잔류함
            // 사망한 Hero만 리스트에서 제거
            var list = new List<Hero>();
            for (int i = 0; i < BattleLocation.HeroList.Count; i++)
            {
                if (BattleLocation.HeroList[i].IsAlive)
                {
                    list.Add(BattleLocation.HeroList[i]);
                }
            }
            BattleLocation.HeroList = list;

            // Monster Clean
            MonsterManager.Instance.Clean();

            
            // Owner
            BattleLocation.NowOwner = EnumLocationOwner.Player;
            // BattleLocation.UpdateElement(); // 아래에서 자동 Update됨


            BattleLocation = null;
            

            // Owner 변경에 따른 Lock 체크 때문에 모든 ElementLocation UpdateElement
            UIManager.Instance.WorldMapUI.UpdateElement();

            // BattleGround Hero만 보여줌
            UIManager.Instance.BattleGroundUI.CleanElements();
            UIManager.Instance.BattleGroundUI.UpdateHero();

            // Alert
            UIManager.Instance.AlertUI.SetTextMiddleBlue("승리!");                        
        }

        private void BattleLost()
        {
            Debug.Log("Lost");

            NowGameState = EnumGameState.OutGame;

            // 루틴 정지
            if (battleRoutine != null)
            {
                StopCoroutine(battleRoutine);
            }
            battleRoutine = null;


            // Hero Clean
            BattleLocation.HeroList.Clear();

            BattleLocation = null;

            // 모든 ElementLocation UpdateElement
            UIManager.Instance.WorldMapUI.UpdateElement();

            // Monster Clean
            MonsterManager.Instance.Clean();

            // BattleGround Clean
            UIManager.Instance.BattleGroundUI.CleanElements();

            // Alert
            UIManager.Instance.AlertUI.SetTextMiddleRed("패배!");
        }

        #endregion



        #region Last Game Clear


        /// <summary>
        /// 월드맵에서 돌아갔을 때 발동
        /// </summary>
        public void AllStageClear()
        {                                    
            StopAllCoroutines();
            Time.timeScale = 0f;

            UIManager.Instance.AllClearUI.SetUI(true);
        }


        #endregion


        #region Camera Control


        [Header("메인카메라")]
        public Camera MainCamera;

        public void SetCameraMode(EnumUIState state)
        {
            if(MainCamera == null)
            {
                MainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            }


            switch (state)
            {
                case EnumUIState.WorldMap:
                    // CullingMask에 "Group" Layer를 제거합니다.
                    MainCamera.cullingMask = MainCamera.cullingMask & ~(1 << LayerMask.NameToLayer("Battle"));
                    break;

                case EnumUIState.BattleGround:
                    // CullingMask에 "Group" Layer를 추가합니다.
                    MainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Battle");
                    break;
            }
        }

        #endregion


    }
}