using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework
{
    public class GUIDGeneratorWithNumber
    {
        private long mGenNum;

        public GUIDGeneratorWithNumber()
        {
            this.mGenNum = 0;
        }

        public GUIDGeneratorWithNumber(long aStartNum)
        {
            this.mGenNum = aStartNum;
        }

        public long GetGUID()
        {
            return this.mGenNum++;
        }

        public void Clear()
        {
            this.mGenNum = 0;
        }
    }

    public class GUIDGeneratorWithTimeStamp
    {
        private long mLastTime;
        private int mGenNum;
        private int mIncreaseNum;

        public GUIDGeneratorWithTimeStamp(int aIncreaseNum = 100)
        {
            this.mGenNum = 0;
            this.mLastTime = 0;
            this.mIncreaseNum = 1;
            int temp = aIncreaseNum;
            while (temp > 0)
            {
                temp = temp / 10;
                this.mIncreaseNum *= 10;
            }
        }

        public long GetGUID()
        {
            long time = LogicUtilTool.GetTimeStamp();
            if (time > this.mLastTime)
            {
                this.mLastTime = time;
                this.mGenNum = 0;
            }
            this.mGenNum++;
            return this.mLastTime * this.mIncreaseNum + this.mGenNum;
        }
    }
}
