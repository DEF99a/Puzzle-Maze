using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Helper
{
    public class GeneralHelper
    {
        public static Vector3 DefaultVector3 = new Vector3(10000, 10000);
        public static Vector3Int DefaultVector3Int = new Vector3Int(10000, 10000, 0);

        public static int moveCount;

        public static float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }

        public static Vector3 GetVectorFromAngle(float angle)
        {
            float angleRad = angle * (Mathf.PI / 180);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static Vector3 GetRandomDir()
        {
            return new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized;
        }

        public static float DistanceXY(Vector3 a, Vector3 b)
        {
            return Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x, b.y));
        }

        public static void FormatTime(int sec, out string hStr, out string mStr, out string sStr)
        {
            int hour = sec / 3600;
            int min = (sec - hour * 3600) / 60;
            int s = sec - hour * 3600 - min * 60;
            hStr = string.Format("{0}", hour);
            mStr = string.Format("{0}", min);
            sStr = string.Format("{0}", s);
            if (hour < 10) hStr = string.Format("0{0}", hour);
            if (min < 10) mStr = string.Format("0{0}", min);
            if (s < 10) sStr = string.Format("0{0}", s);
        }

        public static int FormatTime(int sec, out string hStr, out string mStr)
        {
            int hour = sec / 3600;
            int min = (sec - hour * 3600) / 60;
            int s = sec - hour * 3600 - min * 60;
            hStr = string.Format("{0}H", hour);
            if (s == 0)
                mStr = string.Format("{0}M", min);
            else mStr = string.Format("{0}M", min + 1);
            return s;
        }

        public static IEnumerator MoveFollowTarget(Transform source, Transform target, Action callback, float speed = 5)
        {
            moveCount++;
            while (target != null && target.gameObject.activeInHierarchy)
            {
                yield return null;

                source.position = Vector3.MoveTowards(source.position, target.position, speed * Time.deltaTime);
                var distance = DistanceXY(source.position, target.position);
                if (distance <= 0.02f)
                {
                    ObjectPool.instance.Disable(source.gameObject);
                    break;
                }
            }
            moveCount--;
            callback();
        }

        public static int Pow(int coso, int mu)
        {
            if (mu <= 0) return 1;
            var res = 1;
            for (int k = 0; k < mu; k++)
            {
                res *= coso;
            }
            return res;
        }
    }
}