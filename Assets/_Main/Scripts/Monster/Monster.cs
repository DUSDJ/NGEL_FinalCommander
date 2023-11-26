using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FC
{
    public class Monster : MonoBehaviour
    {

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

                if (nowHP <= 0)
                {
                    Dead();
                }
            }
        }

        public int Atk;
        public float AtkSpeed;

        public float MoveSpeed;

        public float SearchRange = 1.0f;

        
        public bool IsAlive = false;


        private IEnumerator routine;



        #region Collider

        [Header("Collider")]
        public Collider2D HitBox;

        /// <summary>
        /// from�� ���� ��Ʈ�ڽ����� �Ÿ� (���� ���̴� ���� �Ÿ�)
        /// ���� ����� �ڽ� ����Ʈ ����
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public float DistanceOfHitBox(Vector2 from)
        {
            return Vector2.Distance(HitBox.ClosestPoint(from), from);
        }

        /// <summary>
        /// Ư�� ���� ��Ʈ�ڽ� �ȿ� �ִ���
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointInHitBox(Vector3 point)
        {
            return HitBox.OverlapPoint(point);
        }


        /// <summary>
        /// ��Ʈ�ڽ��� �ٸ� �ڽ��� ��ġ����
        /// </summary>
        /// <param name="Box"></param>
        /// <returns></returns>
        public bool IsHitBoxIntersectBox(Vector2 pos, Vector2 size)
        {

            Bounds b = HitBox.bounds;
            Bounds b2 = new Bounds(pos, size);

            return b.Intersects(b2);
        }


        #endregion






        #region Update

        public void Update()
        {
            if (attackDelay >= 0f)
            {
                attackDelay -= Time.unscaledDeltaTime;
            }

        }


        #endregion





        public void SetMonster(Vector3 pos)
        {
            gameObject.SetActive(true);

            NowHP = MaxHP;
            transform.position = pos;
            IsAlive = true;

            if (routine != null)
            {
                StopCoroutine(routine);
            }
            routine = BattleRoutine();
            StartCoroutine(routine);
        }







        public void Clean()
        {
            // MaxHP = 0;
            nowHP = 0;
            // SearchRange = 0;
            IsAlive = false;

            // anim.enabled = false;
            // anim.runtimeAnimatorController = null;
            // spr.sprite = null;

            // transform.position = GameManager.Instance.LevelManager.SpawnPosition[1].position;

            gameObject.SetActive(false);
        }



        #region Battle

        private Hero target;   // ��� ��

        private float attackDelay = 0f;



        /// <summary>
        /// ���� ���� ������ ���� �ִ°� (������ ���ۿ��� Ȯ��)
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool CheckAttackStartRange(Hero e, Vector2 myPos)
        {
            float dis = Vector2.Distance(myPos, e.transform.position);
            //float dis = e.DistanceOfHitBox(playerPos);
            return dis <= SearchRange;
        }

        /// <summary>
        /// ���� ���� ���� ���� �ִ°� (�ǰ�Ȯ��)
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool CheckAttackRange(Hero e, Vector2 myPos)
        {
            float dis = Vector2.Distance(myPos, e.transform.position);
            //float dis = e.DistanceOfHitBox(playerPos);
            return dis <= SearchRange;
        }




        private void MoveTo(Vector2 pos)
        {
            // �ִϸ��̼� �����      
            // SetAnimationBool("Move", true);

            CheckLookTarget(pos);

            transform.position =
                Vector2.MoveTowards(transform.position,
                    pos,
                    MoveSpeed * Time.deltaTime);
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

            var gm = GameManager.Instance;

            while (true)
            {
                Vector3 myPos = transform.position;

                var heroes = gm.GetActiveHeroes();
                if (heroes.Count <= 0)
                {
                    target = null;
                    yield return null;
                    continue;
                }



                // ����� ������ ����
                var list = heroes.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToList();

                if (list.Count > 0)
                {
                    target = list[0];
                }
                else
                {
                    target = null;
                }


                // 1. �� Ž�� : ����� ���°�?
                if (target == null)
                {
                    yield return null;
                    continue;
                }
                else
                {
                    // 2. �� ���� : ���ݽ��� ��Ÿ� ���ΰ�?
                    bool inAttackRange = CheckAttackStartRange(target, myPos);
                    if (inAttackRange == false)
                    {
                        MoveTo(target.transform.position);
                        Debug.DrawLine(transform.position, target.transform.position, Color.blue, 0.05f);
                    }
                    else
                    {
                        // 3. ���� ����
                        CheckLookTarget(target.transform.position);

                        // �ĵ��� �ִٸ� ���Ѵ��
                        if (attackDelay > 0)
                        {
                            yield return null;
                            continue;
                        }

                        // ��� ���ݹ��� ���� �� ��� ������ ó��
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (!list[i].IsAlive)
                            {
                                continue;
                            }

                            if (CheckAttackRange(list[i], myPos))
                            {
                                list[i].Damaged(Atk);
                            }
                        }

                        // ����
                        // AudioManager.Instance.PlayOneShot();
                        // ����Ʈ
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

                        // 3-1. ���� �ĵ� ����
                        // �ĵ� ó���� ���� ���۴ܰ迡�� �Ѵ�.
                        attackDelay = 1 / AtkSpeed;
                        // yield return new WaitForSeconds(PlayerDataManager.Instance.StatusMachine.GetResultAttackDelayTime());


                        // 3-2. ���� �ִϸ��̼� ���� (3-1�� �ĵ� �ݿ�)
                        /* ���� �ִϸ��̼� ����ð� ���� 
                           �ִϸ��̼� ���ӱ����� �״�� ���ݸ�� ���
                           AttackDelay���� ū�� ������ ����ؼ�, ��ǽð��� ���̱⸸ �Ѵ�.
                           �׷��� ��� �� ��� �ʱ�ȭ�� �� �ִ� (�ִϸ��̼� EndAction �̺�Ʈ ����)
                         */

                        // float originAttackAnimTime = PlayerCharacter.GetAnimationTime(EnumCharacterAnimation.Attack);
                        float originAttackAnimTime = 0.5f; // ��� ���� ����� 0.5�ʷ� �����Ǿ��ִ�
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

            Clean();
        }


        #endregion

    }
}