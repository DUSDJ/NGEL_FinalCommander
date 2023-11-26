using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FC
{
    public class Hero : MonoBehaviour
    {
        [HideInInspector] public ScriptableHeroData Data = null;


        public int MaxHP;

        private int nowHP;
        public int NowHP
        {
            get
            {
                return nowHP;
            }
            set
            {
                value = Mathf.Clamp(value, 0, MaxHP);
                nowHP = value;

                if(nowHP <= 0)
                {
                    Dead();
                }
            }
        }

        public int Atk;
        public float AtkSpeed;


        public float SearchRange = 1.0f;

        public bool IsAlive = false;

        private IEnumerator routine;





        #region Update

        public void Update()
        {
            if (attackDelay >= 0f)
            {
                attackDelay -= Time.unscaledDeltaTime;
            }

        }

        #endregion





        public void SetHeroData()
        {
            MaxHP = Data.GetHP(Data.Level);
            Atk = Data.GetAtk(Data.Level);
            AtkSpeed = Data.BaseAtkSpeed;
        }

        public void SetHero()
        {
            SetHeroData();

            NowHP = MaxHP;
            IsAlive = true;
            
            if(routine != null)
            {
                StopCoroutine(routine);
            }            
            routine = BattleRoutine();
            StartCoroutine(routine);
        }


        public void Clean()
        {
            MaxHP = 0;
            NowHP = 0;
            SearchRange = 0;
            IsAlive = false;

            // anim.enabled = false;
            // anim.runtimeAnimatorController = null;
            // spr.sprite = null;

            // transform.position = GameManager.Instance.LevelManager.SpawnPosition[1].position;

            gameObject.SetActive(false);
        }




        #region Battle

        private Monster target;   // 대상 적

        private float attackDelay = 0f;



        /// <summary>
        /// 공격 시작 범위에 적이 있는가 (공격의 시작여부 확인)
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool CheckAttackStartRange(Monster e, Vector3 playerPos)
        {            
            float dis = e.DistanceOfHitBox(playerPos);
            return dis <= SearchRange;
        }

        /// <summary>
        /// 공격 범위 내에 적이 있는가 (피격확인)
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool CheckAttackRange(Monster e, Vector3 playerPos)
        {
            
            float dis = e.DistanceOfHitBox(playerPos);
            
            return dis <= SearchRange;
        }




        private void MoveTo(Vector2 pos)
        {
            // 애니메이션 연출부      
            // SetAnimationBool("Move", true);

            CheckLookTarget(pos);

            /*
            transform.position =
                Vector2.MoveTowards(transform.position,
                    pos,
                    moveSpeed * Time.deltaTime);
            */
        }

        private void CheckLookTarget(Vector2 pos)
        {
            if (transform.position.x > pos.x)
            {
                // Flip(EnumCharacterSide.Left);
            }
            else
            {
                // Flip(EnumCharacterSide.Right);
            }
        }






        private IEnumerator BattleRoutine()
        {
            attackDelay = 1 / AtkSpeed;

            var mm = MonsterManager.Instance;
            
            while (true)
            {
                if(GameManager.Instance.NowGameState != EnumGameState.Battle)
                {
                    yield return null;
                    continue;
                }


                Vector3 heroPos = transform.position;

                var enemies = mm.GetActiveMonster();

                if(enemies.Count <= 0)
                {
                    target = null;
                    yield return null;
                    continue;
                }


                // 가까운 순으로 정렬
                var list = enemies.OrderBy(x => x.DistanceOfHitBox(heroPos)).ToList();
                
                if (list.Count > 0)
                {
                    target = list[0];
                }
                else
                {
                    target = null;
                }



                // 1. 적 탐색 : 대상이 없는가?
                if (target == null)
                {
                    Debug.LogError("target Null");
                    yield return null;
                    continue;
                }
                else
                {
                    // 2. 적 추적 : 공격시작 사거리 밖인가?
                    bool inAttackRange = CheckAttackStartRange(target, heroPos);
                    if (inAttackRange == false)
                    {
                        // 영웅은 타워라서 추적하지 않음
                        // MoveTo(target.transform.position);
                    }
                    else
                    {
                        // 3. 적을 공격
                        CheckLookTarget(target.transform.position);

                        // 후딜이 있다면 무한대기
                        if (attackDelay > 0)
                        {                            
                            yield return null;
                            continue;
                        }

                        Debug.DrawLine(transform.position, target.transform.position, Color.red, 0.05f);

                        // Target 데미지 처리
                        if (target.IsAlive)
                        {
                            target.Damaged(Atk);
                        }

                        /*
                        // 즉시 공격범위 내의 적 모두 데미지 처리
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (!list[i].IsAlive)
                            {
                                continue;
                            }

                            if (CheckAttackRange(list[i], heroPos))
                            {
                                list[i].Damaged(Atk);
                            }
                        }
                        */


                        // 사운드
                        // AudioManager.Instance.PlayOneShot();
                        // 이펙트
                        /*
                        if (PlayerCharacter.NowLookSide == EnumCharacterSide.Left)
                        {
                            EffectManager.Instance.SetEffect("Effect_NormalAttack_L", PlayerCharacter.transform.position);
                        }
                        else
                        {
                            EffectManager.Instance.SetEffect("Effect_NormalAttack_R", PlayerCharacter.transform.position);
                        }
                        */

                        // 3-1. 공격 후딜 대입
                        // 후딜 처리는 공격 시작단계에서 한다.
                        attackDelay = 1 / AtkSpeed;
                        // yield return new WaitForSeconds(PlayerDataManager.Instance.StatusMachine.GetResultAttackDelayTime());


                        // 3-2. 공격 애니메이션 시작 (3-1의 후딜 반영)
                        /* 공격 애니메이션 재생시간 설정 
                           애니메이션 공속까지는 그대로 공격모션 사용
                           AttackDelay보다 큰가 작은가 계산해서, 모션시간을 줄이기만 한다.
                           그래도 모션 중 모션 초기화할 수 있다 (애니메이션 EndAction 이벤트 없음)
                         */

                        // float originAttackAnimTime = PlayerCharacter.GetAnimationTime(EnumCharacterAnimation.Attack);
                        float originAttackAnimTime = 0.5f; // 모든 공격 모션이 0.5초로 고정되어있다
                        if (originAttackAnimTime < attackDelay)
                        {
                            float animSpeed = attackDelay / originAttackAnimTime;
                            animSpeed = 1 / animSpeed;
                            //SetAnimationFloat("AttackMult", animSpeed);
                        }

                        int blend = UnityEngine.Random.Range(0, 3);

                        //SetAnimationFloat("AttackBlend", blend);
                        //SetAnimationTrigger("AttackTrigger");

                    }

                }



                yield return null;
            }
        }



        #endregion




        #region Damaged & Dead

        public void Damaged(int atk)
        {
            NowHP -= atk;
        }

        public void Dead()
        {
            IsAlive = false;
            gameObject.SetActive(false);

            GameManager.Instance.HeroDead(this);
        }


        #endregion






        /// <summary>
        /// SearchRange 사거리 표시
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Vector3 LineEnd = transform.position;
            LineEnd.x -= SearchRange;
            Gizmos.DrawLine(transform.position, LineEnd);
        }

    }
}