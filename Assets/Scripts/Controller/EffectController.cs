using Assets.Scripts.Helper;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class EffectController : MonoBehaviour
    {
        public static EffectController instance;

        [SerializeField] private Transform pfEffectDie;
        [SerializeField] private Transform pfEffectHit;
        [SerializeField] private GameObject earGoldPf;
        [SerializeField] private GameObject effectAngryPf;
        [SerializeField] private GameObject effectInvisiblePf;
        [SerializeField] private GameObject effectPortalPf;
        [SerializeField] private GameObject effectPortalSidePf;
        [SerializeField] private GameObject effectRebornPf;

        private List<ParticleSystem> particles;
        private List<GameObject> cacheEffectDoors;

        private void Awake()
        {
            instance = this;
            cacheEffectDoors = new List<GameObject>();
            particles = new List<ParticleSystem>();
        }

        private void Start()
        {
            //GenEffectAngry(Vector3.zero);
            //Debug.Log("Start:" + DateTime.Now.ToString("HH:mm:ss.fff"));
        }

        public void GenDieEffect(Vector3 position)
        {
            var go = ObjectPool.instance.GetGameObject(pfEffectDie.gameObject, Vector3.zero, Quaternion.identity);
            go.transform.position = position;
            //particles.Add(go.GetComponent<ParticleSystem>());
        }

        public void GenHitEffect(Vector3 position)
        {
            var go = ObjectPool.instance.GetGameObject(pfEffectHit.gameObject, Vector3.zero, Quaternion.identity);
            go.transform.position = position;
            //particles.Add(go.GetComponent<ParticleSystem>());
        }

        public ParticleSystem GenEffectAngry(Vector3 position)
        {
            var go = ObjectPool.instance.GetGameObject(effectAngryPf, Vector3.zero, Quaternion.identity);
            go.transform.position = position;
            return go.GetComponent<ParticleSystem>();
        }
        public ParticleSystem GenEffectInvisible(Vector3 position)
        {
            var go = ObjectPool.instance.GetGameObject(effectInvisiblePf, Vector3.zero, Quaternion.identity);
            go.transform.position = position;
            return go.GetComponent<ParticleSystem>();
        }

        public ParticleSystem GenEffectPortal(Vector3 position)
        {
            var go = ObjectPool.instance.GetGameObject(effectPortalPf, Vector3.zero, Quaternion.identity);
            go.transform.position = position;
            cacheEffectDoors.Add(go);
            return go.GetComponent<ParticleSystem>();
        }

        public ParticleSystem GenEffectPortalSide(Vector3 position)
        {
            var go = ObjectPool.instance.GetGameObject(effectPortalSidePf, Vector3.zero, Quaternion.identity);
            go.transform.position = position;
            cacheEffectDoors.Add(go);
            return go.GetComponent<ParticleSystem>();
        }

        public ParticleSystem GenEffectReborn(Vector3 position)
        {
            var go = ObjectPool.instance.GetGameObject(effectRebornPf, Vector3.zero, Quaternion.identity);
            go.transform.position = position;
            var par = go.GetComponent<ParticleSystem>();
            particles.Add(par);
            return par;
        }

        public void DisableAllEffectDoor()
        {
            foreach (var c in cacheEffectDoors)
                ObjectPool.instance.Disable(c);
        }

        //public void GenGetGoldEffect(Vector3 startPosition, GameObject target, int gemAmount)
        //{
        //    if (gemAmount == 0) return;

        //    var distance = 1f;
        //    var n = gemAmount / 5;
        //    for (int k = 0; k < n; k++)
        //    {
        //        Vector3 pos = startPosition + new Vector3(Random.Range(-distance, 0), Random.Range(-distance, 0));
        //        Gen(pos);
        //        pos = startPosition + new Vector3(Random.Range(0, distance), Random.Range(-distance, 0));
        //        Gen(pos);
        //        pos = startPosition + new Vector3(Random.Range(-distance, 0), Random.Range(0, distance));
        //        Gen(pos);
        //        pos = startPosition + new Vector3(Random.Range(distance, 0), Random.Range(0, distance));
        //        Gen(pos);
        //        pos = startPosition + new Vector3(Random.Range(-distance / 2, distance / 2), Random.Range(-distance / 2, distance / 2));
        //        Gen(pos);
        //    }
        //    void Gen(Vector3 pos)
        //    {
        //        var go = ObjectPool.instance.GetGameObject(earGoldPf, startPosition, Quaternion.identity);
        //        go.GetComponent<BoxCollider2D>().isTrigger = false;

        //        go.transform.DOMove(pos, 0.3f).OnUpdate(() =>
        //        {
        //        }).OnComplete(() =>
        //        {
        //            StartCoroutine(Waitting(go));
        //        });
        //    }

        //    IEnumerator Waitting(GameObject go)
        //    {
        //        yield return new WaitForSeconds(0.4f);
        //        AudioManager.instance.PlayCollectCoin();

        //        if (target != null && target.activeInHierarchy)
        //        {
        //            bool disabled = false;
        //            while (target != null && target.activeInHierarchy)
        //            {
        //                yield return null;

        //                go.transform.position = Vector3.MoveTowards(go.transform.position, target.transform.position, 12 * Time.deltaTime);
        //                var distance = GeneralHelper.DistanceXY(go.transform.position, target.transform.position);
        //                if (distance <= 0.1f)
        //                {
        //                    ObjectPool.instance.Disable(go);
        //                    disabled = true;
        //                    yield break;
        //                }
        //            }
        //            if (!disabled)
        //                cacheGems.Add(go);
        //        }
        //        else
        //        {
        //            cacheGems.Add(go);
        //        }
        //    }
        //}

        //public void GenGetGoldEffect(GameObject target)
        //{
        //    if (cacheGems.Count <= 0) return;

        //    StartCoroutine(Waitting());
        //    IEnumerator Waitting()
        //    {
        //        yield return new WaitForSeconds(0.5f);
        //        var tmpGems = cacheGems.ToList();
        //        for (int k = 0; k < tmpGems.Count; k++)
        //        {
        //            var go = tmpGems[k];
        //            go.GetComponent<BoxCollider2D>().isTrigger = true;
        //            go.transform.DOMove(target.transform.position, 0.5f).OnComplete(() =>
        //            {
        //                ObjectPool.instance.Disable(go);
        //                cacheGems.Remove(go);
        //            });
        //        }
        //    }
        //}

        private void Update()
        {
            for (int k = 0; k < particles.Count; k++)
            {
                var par = particles[k];
                if (par.isStopped)
                {
                    //Debug.Log("Start 2:" + DateTime.Now.ToString("HH:mm:ss.fff"));
                    ObjectPool.instance.Disable(par.gameObject);
                    particles.RemoveAt(k);
                    k--;
                }
            }
        }
    }
}