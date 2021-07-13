using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schd
{
    class Process
    {
        public int processID;
        public int arriveTime;
        public int burstTime;
        public int priority;
        public Process(int processID, int arriveTime, int burstTime, int priority)
        {
            this.processID = processID;
            this.arriveTime = arriveTime;
            this.burstTime = burstTime;
            this.priority = priority;
        } 
    }
}
