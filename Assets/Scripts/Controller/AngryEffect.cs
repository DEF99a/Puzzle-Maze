//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AngryEffect : MonoBehaviour
//{
//    public static AngryEffect instance;

//    [SerializeField] private List<ParticleSystem> particleSystems;

//    public bool play { get; private set; }

//    ParticleSystem parent;

//    private void Awake()
//    {
//        instance = this;
//        parent = GetComponent<ParticleSystem>();
//        gameObject.SetActive(false);
//    }

//    public void Setup(float duration)
//    {
//        gameObject.SetActive(true);
//        play = true;
//        foreach (var ps in particleSystems)
//        {
//            ParticleSystemStopBehavior.StopEmittingAndClear(ps);
//            ps.stop();
//        }
//        //parent.Stop();
//        //var child = parent.transform.GetChild(0).GetComponent<ParticleSystem>();
//        //child.Stop();
//        foreach (var ps in particleSystems)
//        {
//            var m = ps.main;
//            m.duration = duration;
//            ps.Play();
//        }
//        child.Play();
//        parent.Play();
//        //parent.Play();
//    }

//    private void Update()
//    {
//        if (play)
//        {
//            if (parent.isStopped)
//            {
//                PlayerController.instance.EndAngry();
//                play = false;
//                gameObject.SetActive(false);
//            }
//            else if (PlayerController.instance.IsLive())
//            {
//                transform.position = PlayerController.instance.transform.position;
//            }
//            else parent.Stop();
//        }
//    }
//}
