using Assets.Scripts.Define;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main1 : PlayerController
{
    [SerializeField] private GameObject bulletPf;

    public override void StartAction()
    {
        //Debug.Log("isDie:" + isDie + " GameManager.instance.IsGameOver:" + GameManager.instance.IsGameOver + " InGuard:" + InGuard
        //    + " state:" + state);
        if (isDie || GameManager.instance.IsGameLoose || InGuard) return;
        if (state == DynamicEntityState.Shoot || state == DynamicEntityState.Ice) return;
        if (subStates.Contains(DynamicEntityState.Invisible)) return;
        AudioManager.instance.PlaySound(SoundName.skill_bomberman);

        var cell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
        var pos = cell + new Vector3(0.5f, 0.5f);

        var go = ObjectPool.instance.GetGameObject(bulletPf, pos, Quaternion.identity);
        go.GetComponent<Mario2Bomb>().FirstInit(new Assets.Scripts.Bullet.ItemParam()
        {
            characterBase = this,
            characterId = this.EntityId,
            direct = direction,
            speed = shootSpeed * 0.5f
            //speed = 0.5f
        });
    }
}
