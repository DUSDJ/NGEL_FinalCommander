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
                for (int i = 0; i < slots; i++)
                {
                    if(now >= count)
                    {
                        yield break;
                    }


                    int rand = UnityEngine.Random.Range(0, monsterPrefabs.Count);
                    var randMonster = monsterPrefabs[rand];

                    int spawnPointRand = UnityEngine.Random.Range(0, UIManager.Instance.BattleGroundUI.SpawnPoint.Count);
                    var point = UIManager.Instance.BattleGroundUI.SpawnPoint[spawnPointRand];
                    MonsterManager.Instance.SetMonster(randMonster.name, point.position);

                    // 생성+1
                    now += 1;
                }
                

                yield return wait;
            }

        }



        #endregion





        #region Battle

        public ElementLocation BattleLocation;


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

            // UI 처리 & 유닛 목록 가져옴
            BattleLocation = UIManager.Instance.BattleGroundUI.SelectedLocation;
            BattleLocation.SetHero(UIManager.Instance.BattleGroundUI.EngageHeroList());            

            // 유닛이 하나라도 슬롯에 올라가 있어야 배틀 가능
            if (BattleLocation.HeroList == null || BattleLocation.HeroList.Count <= 0)
            {
                return;
            }


            Battle();
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


            // Hero는 잔류함
            // 사망한 Hero만 리스트에서 제거
            var list = new List<Hero>(BattleLocation.HeroList);
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

            // BattleGround Clean?
            // UIManager.Instance.BattleGroundUI.CleanElements();

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

            // Monster Clean
            MonsterManager.Instance.Clean();

            // BattleGround Clean
            UIManager.Instance.BattleGroundUI.CleanElements();

            // Alert
            UIManager.Instance.AlertUI.SetTextMiddleRed("패배!");
        }

        #endregion

    }
}