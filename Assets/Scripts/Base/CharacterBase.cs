using Assets.Scripts.Bullet;
using Assets.Scripts.Controller;
using Assets.Scripts.Define;
using Spine;
using Spine.Unity;
using Spine.Unity.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct EffectSpeed
{
    public float percent;
    public float time;
}

public class CharacterBase : EntityBase
{
    protected enum EffectItem
    {
        IncrSpeed,
        Angry,
        SlowItem
    }

    protected string afterAnimName = "";
    protected string frontAnimName = "front";
    protected string backAnimName = "rear";
    protected string sideAnimName = "side";

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float shootSpeed = 10f;

    public float MoveSpeed { get { return moveSpeed; } }

    protected List<DynamicEntityState> subStates;

    private BoxBase curBoxToShoot;

    protected SkeletonAnimation skeletonAnimation;
    private SkeletonGhost skeletonGhost;
    protected Dictionary<EffectItem, EffectSpeed> allEffectSpeed;
    protected List<GameObject> effectGameObjects;
    private Coroutine blinkCoroutine;
    private Coroutine invisibleCoroutine, iceCoroutine;

    protected Vector3 animLocalScale;
    private float invisibleTimer, iceTimer;
    protected float shootTimer;
    protected string shootAnimName;
    protected List<ParticleSystem> invisibleEffects;
    protected Transform poisonEffectTf;

    protected override void Awake()
    {
        base.Awake();

        var anim = transform.Find("Anim");
        skeletonAnimation = anim.GetComponent<SkeletonAnimation>();
        skeletonGhost = anim.GetComponent<SkeletonGhost>();

        animLocalScale = skeletonAnimation.transform.localScale;
        effectGameObjects = new List<GameObject>();
        allEffectSpeed = new Dictionary<EffectItem, EffectSpeed>();
        invisibleEffects = new List<ParticleSystem>();
        subStates = new List<DynamicEntityState>();

        if (entityType == EntityType.Enemy) moveSpeed *= 0.7f;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        for (int k = 0; k < effectGameObjects.Count; k++)
        {
            ObjectPool.instance.Disable(effectGameObjects[k]);
        }
    }

    public override void Init()
    {
        base.Init();

        isDie = false;
        effectGameObjects.Clear();
        allEffectSpeed.Clear();
        invisibleEffects.Clear();
        subStates.Clear();

        movePointTf.parent = null;
        movePointTf.position = transform.position;
        oldMovePointPos = transform.position;

        direction = DynamicEntityDirection.Down;
        state = DynamicEntityState.Idle;

        currentMoveSpeed = moveSpeed;

        AnimInit();
    }

    protected void AnimInit()
    {
        skeletonAnimation.skeleton.A = 1f;
        skeletonAnimation.loop = true;
        //skeletonAnimation.timeScale = 1;
        skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = 0;
        skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0);
        skeletonGhost.ghostingEnabled = false;

        skeletonAnimation.AnimationState.Event -= HandleEvent;
        skeletonAnimation.AnimationState.Event += HandleEvent;
        skeletonAnimation.AnimationState.Complete -= HandleCompleteAnim;
        skeletonAnimation.AnimationState.Complete += HandleCompleteAnim;
    }

    protected virtual void HandleCompleteAnim(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name.Equals(this.shootAnimName))
        {
            //Debug.Log("HandleCompleteAnim end shoot");
            this.shootAnimName = "";
            if (!isDie)
            {
                Idle(0);
                skeletonAnimation.loop = true;
            }
        }
        else if (trackEntry.Animation.Name.Equals("die_start"))
        {
            ObjectPool.instance.Disable(gameObject);
            this.PostEvent(entityType == EntityType.Enemy ? EventID.EnemyDie : EventID.PlayerDie, this);
        }
    }

    protected virtual void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name.Equals("shoot"))
        {
            //Debug.Log("shoot event");
            if (curBoxToShoot != null)
            {
                if (curBoxToShoot.CanKickFly || curBoxToShoot.CanBreak)
                {
                    curBoxToShoot.Setup(new ItemParam()
                    {
                        characterId = this.EntityId,
                        characterBase = this,
                        direct = direction,
                        speed = shootSpeed
                    });
                }
                curBoxToShoot = null;
            }
        }
    }

    protected override void Update()
    {
        if (isDie)
        {
            ProcessAnim();
            return;
        }
        //if (GameManager.instance.IsGameOver)
        //{
        //    state = DynamicEntityState.Idle;
        //    ProcessAnim();
        //    return;
        //}

        float percent = 0;
        var keys = allEffectSpeed.Keys;
        for (int k = 0; k < keys.Count; k++)
        {
            var key = keys.ElementAt(k);
            var esp = allEffectSpeed[key];
            if (esp.time > Time.time)
                percent += esp.percent;
            else
            {
                if (key == EffectItem.SlowItem)
                {
                    if(poisonEffectTf != null)
                    {
                        ObjectPool.instance.Disable(poisonEffectTf.gameObject);
                        poisonEffectTf = null;
                    }
                }
                allEffectSpeed.Remove(key);
            }
        }
        if (percent != 0)
        {
            if (!skeletonGhost.ghostingEnabled && percent > 0) skeletonGhost.ghostingEnabled = true;
            if (percent < 0 && allEffectSpeed.ContainsKey(EffectItem.SlowItem))
            {
                if (poisonEffectTf == null)
                    poisonEffectTf = ObjectPool.instance.GetGameObject(PrefabCache.instance.poisonPf, transform.position, Quaternion.identity).transform;
            }
            //Debug.Log("percent:" + percent);
            currentMoveSpeed = moveSpeed * (1 + percent * 0.01f);
        }
        else
        {
            if (skeletonGhost.ghostingEnabled) skeletonGhost.ghostingEnabled = false;
            currentMoveSpeed = moveSpeed;
        }
        if (poisonEffectTf != null) poisonEffectTf.position = transform.position;

        if (state == DynamicEntityState.Walk && skeletonAnimation.timeScale != 0)
        {
            var ts = currentMoveSpeed / moveSpeed;

            if (direction == DynamicEntityDirection.Up || direction == DynamicEntityDirection.Down)
                skeletonAnimation.timeScale = ts * 1.3f;
            else skeletonAnimation.timeScale = ts;
        }

        if (invisibleEffects.Count > 0)
        {
            for (int k = 0; k < invisibleEffects.Count; k++)
            {
                var ang = invisibleEffects[k];
                if (ang.isStopped)
                {
                    invisibleEffects.RemoveAt(k);
                    k--;
                    ObjectPool.instance.Disable(ang.gameObject);
                }
                else ang.transform.position = transform.position;
            }
            if (invisibleEffects.Count() == 0)
                subStates.Remove(DynamicEntityState.Invisible);
        }

        base.Update();

        ProcessLocalScale();
        ProcessAnim();
    }

    protected override void MoveLeft()
    {
        direction = DynamicEntityDirection.Left;
        var tmpPos = movePointTf.position + Vector3.left;
        if (GameManager.instance.IsShowHint && !GameManager.instance.CheckNextHintIsAttack())
        {
            GameManager.instance.EndHint(TypeHint.Left);
        }
        CanMove(tmpPos);
        //Debug.Log("canMove:" + canMove);
    }

    protected override void MoveDown()
    {
        direction = DynamicEntityDirection.Down;
        var tmpPos = movePointTf.position + Vector3.down;
        if (GameManager.instance.IsShowHint && !GameManager.instance.CheckNextHintIsAttack())
        {
            GameManager.instance.EndHint(TypeHint.Down);
        }
        CanMove(tmpPos);
        //Debug.Log("canMove:" + canMove);
    }

    protected override void MoveRight()
    {
        direction = DynamicEntityDirection.Right;
        var tmpPos = movePointTf.position + Vector3.right;
        if (GameManager.instance.IsShowHint && !GameManager.instance.CheckNextHintIsAttack())
        {
            GameManager.instance.EndHint(TypeHint.Right);
        }
        CanMove(tmpPos);
        //Debug.Log("canMove:" + canMove);
    }

    protected override void MoveUp()
    {
        direction = DynamicEntityDirection.Up;
        var tmpPos = movePointTf.position + Vector3.up;
        if (GameManager.instance.IsShowHint && !GameManager.instance.CheckNextHintIsAttack())
        {
            GameManager.instance.EndHint(TypeHint.Up);
        }
        CanMove(tmpPos);
        //Debug.Log("canMove:" + canMove);
    }

    protected virtual void CanMove(Vector3 tmpPos)
    {
    }

    public virtual bool Shoot()
    {
        //|| GameManager.instance.IsGameOver
        if (isDie || state == DynamicEntityState.Ice) return false;

        var location = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]);
        if (location.tileType == TileType.Dynamic)
        {
            if (location.entityBase is BoxBase box && box.CanShoot())
            {
                state = DynamicEntityState.Shoot;
                skeletonAnimation.loop = false;
                curBoxToShoot = box;
                return true;
            }
        }
        //else
        //{
        //    state = DynamicEntityState.Shoot;
        //    skeletonAnimation.loop = false;
        //}
        return false;
    }

    public bool CheckBox(DynamicEntityDirection direction)
    {
        if (isDie || state == DynamicEntityState.Ice) return false;

        var location = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]);
        if (location.tileType == TileType.Dynamic)
        {
            if (location.entityBase is BoxBase box && box.CanShoot())
            {
                return true;
            }
        }
        return false;
    }

    protected virtual void ProcessLocalScale()
    {
        if (direction == DynamicEntityDirection.Right)
            skeletonAnimation.gameObject.transform.localScale = new Vector3(-animLocalScale.x, animLocalScale.y, animLocalScale.z);
        else skeletonAnimation.gameObject.transform.localScale = new Vector3(animLocalScale.x, animLocalScale.y, animLocalScale.z);
    }

    protected virtual string GetDirectAnimName()
    {
        var directAnimName = frontAnimName;
        if (direction == DynamicEntityDirection.Up)
            directAnimName = backAnimName;
        else if (direction == DynamicEntityDirection.Right)
            directAnimName = sideAnimName;
        else if (direction == DynamicEntityDirection.Left)
            directAnimName = sideAnimName;

        if (skeletonGhost.ghostingEnabled)
        {
            if (direction == DynamicEntityDirection.Right || direction == DynamicEntityDirection.Left)
                skeletonGhost.sortWithDistanceOnly = true;
            else skeletonGhost.sortWithDistanceOnly = false;
        }
        return directAnimName;
    }

    protected virtual void ProcessAnim()
    {
        string directAnimName = GetDirectAnimName();

        string nextAnimName = "";
        if (state == DynamicEntityState.Idle)
        {
            nextAnimName = string.Format("{0}_idle", directAnimName);
            //if (subState == DynamicEntityState.Angry) nextAnimName = string.Format("{0}_idle_angry", directAnimName);
            if (afterAnimName != "") nextAnimName += "_" + afterAnimName;
        }
        else if (state == DynamicEntityState.Walk)
        {
            nextAnimName = string.Format("{0}_walk", directAnimName);
            //if (subState == DynamicEntityState.Angry) nextAnimName = string.Format("{0}_walk_angry", directAnimName);
            if (afterAnimName != "") nextAnimName += "_" + afterAnimName;
        }
        else if (state == DynamicEntityState.Shoot)
        {
            nextAnimName = string.Format("{0}_shoot", directAnimName);
            //if (subState == DynamicEntityState.Angry) nextAnimName = string.Format("{0}_shoot_angry", directAnimName);
            if (afterAnimName != "") nextAnimName += "_" + afterAnimName;
            this.shootAnimName = nextAnimName;
        }
        else if (state == DynamicEntityState.Die)
        {
            nextAnimName = "die_start";
        }

        //Debug.Log(nextAnimName + " directAnimName:" + directAnimName);
        var currentAnim = skeletonAnimation.AnimationName;
        if (currentAnim == nextAnimName || nextAnimName == "") return;
        //Debug.Log("nextAnimName:" + nextAnimName + " directAnimName:" + directAnimName);
        skeletonAnimation.AnimationName = nextAnimName;
    }

    protected virtual void Idle(float time)
    {
        state = DynamicEntityState.Idle;
    }

    protected void Blink()
    {
        blinkCoroutine = StartCoroutine(blinkIE());
        IEnumerator blinkIE()
        {
            while (IsLive())
            {
                yield return new WaitForSeconds(0.2f);
                if (skeletonAnimation.gameObject.activeSelf)
                {
                    skeletonAnimation.gameObject.SetActive(false);
                }
                else
                {
                    skeletonAnimation.gameObject.SetActive(true);
                }
            }
        }
    }

    protected void StopBlink()
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
        skeletonAnimation.gameObject.SetActive(true);
    }

    public virtual void ProcessEatItem(GameObject go, int balloon)
    {
    }

    protected virtual IEnumerator DieIE()
    {
        yield return null;

        state = DynamicEntityState.Die;
        skeletonAnimation.loop = false;

        //yield return new WaitForSeconds(1f);       
    }

    #region action active
    protected void Inivisible(float timeLife)
    {
        if (!subStates.Contains(DynamicEntityState.Invisible))
            subStates.Add(DynamicEntityState.Invisible);
        invisibleEffects.Add(EffectController.instance.GenEffectInvisible(transform.position));

        //if (invisibleCoroutine == null)
        //{
        //    invisibleTimer = Time.time + timeLife;
        //    invisibleCoroutine = StartCoroutine(InvisibleIE());
        //}
        //else
        //{
        //    invisibleTimer += timeLife;
        //}
        //IEnumerator InvisibleIE()
        //{
        //    subState = DynamicEntityState.Invisible;
        //    skeletonAnimation.skeleton.A = 0.5f;
        //    while (!isDie && GameManager.instance.IsPlaying && invisibleTimer > Time.time)
        //    {
        //        yield return null;
        //    }
        //    skeletonAnimation.skeleton.A = 1f;
        //    subState = DynamicEntityState.None;
        //    invisibleCoroutine = null;
        //}
    }
    #endregion

    #region action passive
    public void Frozen(float timeLife, bool hasEffect)
    {
        if (iceCoroutine == null)
        {
            iceTimer = Time.time + timeLife;
            iceCoroutine = StartCoroutine(FronzenIE());
        }
        else
        {
            iceTimer += timeLife;
        }
        IEnumerator FronzenIE()
        {
            skeletonAnimation.timeScale = 0;
            state = DynamicEntityState.Ice;
            GameObject go = null;
            if (hasEffect)
            {
                go = ObjectPool.instance.GetGameObject(PrefabCache.instance.iceEffectPf, transform.position + new Vector3(0, -0.5f, 0), Quaternion.identity);
                go.GetComponent<IceEffect>().PlayAnim(1);
                effectGameObjects.Add(go);
            }
            while (!isDie && GameManager.instance.IsPlaying && iceTimer > Time.time)
            {
                AudioManager.instance.PlaySound(SoundName.freeze_loop);
                yield return null;
            }
            AudioManager.instance.PlaySound(SoundName.freeze_break);

            if (hasEffect && go != null)
            {
                effectGameObjects.Remove(go);
                go.GetComponent<IceEffect>().PlayAnim(3);
            }

            state = DynamicEntityState.Idle;
            skeletonAnimation.timeScale = 1;
            iceCoroutine = null;
        }
    }

    public virtual void Killed()
    {
        Die();
    }
    #endregion
}
