using Assets.Scripts.Define;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaunterGastly : EnemyController
{
    [SerializeField] private SkeletonDataAsset skin1;
    [SerializeField] private SkeletonDataAsset skin2;

    private int curSkin;
    public override void EnemyInit(EnemyType enemyType, BoxBase normalBox)
    {
        Debug.Log("wtf:" + skin1.name);

        skeletonAnimation.skeletonDataAsset = skin1;
        skeletonAnimation.Initialize(true);
        base.EnemyInit(enemyType, normalBox);        
        curSkin = 1;
    }

    protected override void Die()
    {
        if (tmpDie) return;
        tmpDie = true;

        if (invisibleEffects.Count > 0)
        {
            for (int k = 0; k < invisibleEffects.Count; k++)
            {
                var ang = invisibleEffects[k];
                invisibleEffects.RemoveAt(k);
                ObjectPool.instance.Disable(ang.gameObject);
                k--;
            }
        }
        subStates.Clear();

        this.life--;
        Debug.Log("life:" + life);

        if (this.life <= 0)
        {
            Debug.Log("curSkin:" + curSkin);

            if (curSkin == 1)
            {
                AudioManager.instance?.PlaySound(SoundName.skill_haunter);

                curSkin = 2;
                this.life = 1;

                skeletonAnimation.skeletonDataAsset = skin2;
                skeletonAnimation.Initialize(true);

                if(bornCoroutine != null) StopCoroutine(bornCoroutine);
                if (!subStates.Contains(DynamicEntityState.PrepareReborn))
                    subStates.Add(DynamicEntityState.PrepareReborn);
                StartCoroutine(ReBorn());
            }
            else
            {
                base.Die();

                MyGraph.instance.UpdateLocation(currentCell, null);
                StartCoroutine(DieIE());
            }
        }
        else
        {
            if (!subStates.Contains(DynamicEntityState.PrepareReborn))
                subStates.Add(DynamicEntityState.PrepareReborn);
            StartCoroutine(ReBorn());
        }
    }
}
