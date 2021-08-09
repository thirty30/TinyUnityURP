using System;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework
{
    public static class LogicUtilTool
    {
        public static long GetTimeStamp()
        {
            TimeSpan st = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(st.TotalSeconds);
        }

        public static int GetDaysBetweenTimeStamp(long st1, long st2)
        {
            int day1 = (int)(st1 / 86400);
            int day2 = (int)(st2 / 86400);
            return Math.Abs(day1 - day2);
        }

        public static void SetLayer(GameObject rObj, string strLayerName)
        {
            int nLayer = LayerMask.NameToLayer(strLayerName);
            SetLayer(rObj, nLayer);
        }

        public static void SetLayer(GameObject rObj, int nLayer)
        {
            rObj.layer = nLayer;
            int nChildNum = rObj.transform.childCount;
            for (int i = 0; i < nChildNum; i++)
            {
                var rChildObj = rObj.transform.GetChild(i).gameObject;
                SetLayer(rChildObj, nLayer);
            }
        }

        public static void SetTag(GameObject rObj, string strTagName)
        {
            rObj.tag = strTagName;
            int nChildNum = rObj.transform.childCount;
            for (int i = 0; i < nChildNum; i++)
            {
                var rChildObj = rObj.transform.GetChild(i).gameObject;
                SetTag(rChildObj, strTagName);
            }
        }

        public static GameObject FindChildByName(GameObject rObj, string strChildName)
        {
            Transform[] trans = rObj.GetComponentsInChildren<Transform>();
            foreach (var tran in trans)
            {
                if (tran.name == strChildName)
                {
                    return tran.gameObject;
                }
            }
            return null;
        }

        public static Vector2Int GetGridPosition(Vector3 a_pos)
        {
            int x = Mathf.CeilToInt(a_pos.x * 2);
            int z = Mathf.CeilToInt(a_pos.z * 2);
            return new Vector2Int(z, x);
        }

        public static Vector2Int[] GridSideArray = {
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(-1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(1, -1),
    };
        // 0 1 2
        // 3 * 4
        // 5 6 7
        public static Vector2Int GetGridSidePosition(Vector3 a_pos, int a_idx)
        {
            Vector2Int pos = LogicUtilTool.GetGridPosition(a_pos);
            return LogicUtilTool.GetGridSidePosition(pos, a_idx);
        }

        public static Vector2Int GetGridSidePosition(Vector2Int a_pos, int a_idx)
        {
            Vector2Int offset = LogicUtilTool.GridSideArray[a_idx];
            return (a_pos + offset);
        }

        static string[] units = { "", "K", "M", "B", "G", "T", "A", "C", "D", "E", "F", "H", "I", "J", "K", "L", "M", "N" };
        public static string FormatNumber(int a_value)
        {
            int idx = 0;
            int temp = a_value;
            for (int i = 0; i < 5; i++)
            {
                if (temp < 1000)
                {
                    idx = i;
                    break;
                }
                temp /= 1000;
            }
            float value = (float)a_value / Mathf.Pow(10, idx * 3);
            if (idx == 0)
            {
                return value.ToString();
            }
            return string.Format("{0:N2}", value) + units[idx];
        }

        public static string FormatNumber(double aValue)
        {
            int digit = (int)Math.Floor(Math.Log10(aValue));
            if (digit < 3)
            {
                return ((int)aValue).ToString();
            }

            int unit = digit / 3;
            double value = aValue / Math.Pow(10, unit * 3);
            return string.Format("{0:N2}", value) + units[unit];
        }

        public static string FormatTimeWithUnit(int aSecond)
        {
            int hour = aSecond / 3600;
            int min = (aSecond - (hour * 3600)) / 60;
            int sec = aSecond - (hour * 3600) - (min * 60);
            if (hour > 0)
            {
                return hour.ToString() + "h" + min.ToString() + "m";
            }
            else if (min > 0)
            {
                return min.ToString() + "m" + sec.ToString() + "s";
            }
            else
            {
                return sec.ToString() + "s";
            }
        }

        public static string FormatTimeMS(long aShowTime)
        {
            if (aShowTime < 0)
            {
                return "00:00";
            }
            var rDateTime = new DateTime(aShowTime * 1000 * 10000);
            if (aShowTime >= 60)
            {
                return rDateTime.ToString("mm:ss");
            }
            else
            {
                return rDateTime.ToString("ss:ff");
            }
        }

        public static string FormatTimeHM(long aShowTime)
        {
            var rDateTime = new DateTime(aShowTime * 1000 * 10000);
            if (aShowTime >= 60)
            {
                return rDateTime.ToString("HH:mm");
            }
            else
            {
                return rDateTime.ToString("mm:ss");
            }
        }

        public static string FormatTimeHMS(long aShowTime)
        {
            var rDateTime = new DateTime(aShowTime * 1000 * 10000);
            return rDateTime.ToString("HH:mm:ss");
        }

        public static List<Vector3> CalcBezierCurve(Vector3 aStartPoint, Vector3 aEndPoint)
        {
            List<Vector3> path = new List<Vector3>();

            Vector3 sP = aStartPoint;
            Vector3 tP = aEndPoint;
            Vector3 mP = new Vector3((sP.x + tP.x) / 2, 4.0f, (sP.z + tP.z) / 2);
            Vector3 vDir1 = (mP - sP).normalized;
            float fDis1 = Vector3.Distance(mP, sP);
            Vector3 vDir2 = (tP - mP).normalized;
            float fDis2 = Vector3.Distance(mP, tP);

            path.Add(sP);
            for (int i = 1; i <= 16; i++)
            {
                float fRate = i / 16.0f;
                float fDis10 = fDis1 * fRate;
                Vector3 vPos10 = vDir1 * fDis10 + sP;


                float fDis20 = fDis10 / fDis1 * fDis2;
                Vector3 vPos20 = vDir2 * fDis20 + mP;

                Vector3 vDir3 = (vPos20 - vPos10).normalized;
                float fDis3 = Vector3.Distance(vPos20, vPos10);

                float fDis30 = fDis10 / fDis1 * fDis3;
                Vector3 vPos30 = vDir3 * fDis30 + vPos10;
                path.Add(vPos30);
            }
            path.Add(tP);
            return path;
        }

        public static bool IsInRect(Vector3 aRectPoint1, Vector3 aRectPoint2, Vector3 aTargetPos)
        {
            if (aTargetPos.x > Mathf.Max(aRectPoint1.x, aRectPoint2.x)) { return false; }
            if (aTargetPos.x < Mathf.Min(aRectPoint1.x, aRectPoint2.x)) { return false; }

            if (aTargetPos.y > Mathf.Max(aRectPoint1.y, aRectPoint2.y)) { return false; }
            if (aTargetPos.y < Mathf.Min(aRectPoint1.y, aRectPoint2.y)) { return false; }

            if (aTargetPos.z > Mathf.Max(aRectPoint1.z, aRectPoint2.z)) { return false; }
            if (aTargetPos.z < Mathf.Min(aRectPoint1.z, aRectPoint2.z)) { return false; }

            return true;
        }
    }
}

