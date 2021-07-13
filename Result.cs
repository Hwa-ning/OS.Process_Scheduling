using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schd
{
    class Result
    {
        public int processID;
        public int startP;
        public int burstTime;
        public int waitingTime;
        public int count_exe;    // 프로세스가 몇번째 실행되는지 알려주는 int형 변수
        public int arriveTime;   // 프로세스의 arriveTime
        public Boolean isFinish; // 프로세스의 작업이 끝났는지 확인하는 Boolean 변수 (true면 끝, false면 아직 작업이 남음)
        public Result(int processID, int startP, int burstTime, int waitingTime, int arriveTime, int count_exe, Boolean isFinish_YorN)
        {
            this.processID = processID;
            this.startP = startP;
            this.burstTime = burstTime;
            this.waitingTime = waitingTime;
            this.arriveTime = arriveTime;
            this.count_exe = count_exe;
            this.isFinish = isFinish_YorN;
        }
    }
}
