using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Schd
{
    class ReadyQueueElement
    {
        public int processID;
        public int burstTime;
        public int waitingTime;
        public int arriveTime;
        public int priority; 
        public int count_exe; // 프로세스가 실행된 횟수를 저장하는 변수

        public ReadyQueueElement(int PID, int burstT, int waitingT, int arriveT, int priority, int count_exe)
        {
            this.processID = PID;
            this.burstTime = burstT;
            this.waitingTime = waitingT;
            this.arriveTime = arriveT;
            this.priority = priority;
            this.count_exe = count_exe;
        }
    }

    class Non_Preemptive_Priority
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;

            int runTime = 0;
            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();
            do
            {
                while (jobList.Count != 0)                      // jobList에 하나 이상 있다면
                {
                    Process frontJob = jobList.ElementAt(0);    // (Process)frontJob에 jobList의 0번을 꺼내온다

                    if (frontJob.arriveTime == runTime)         // frontJob이 arrive한 상태라면
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0,frontJob.arriveTime, frontJob.priority,0)); // frontJob을 readyQueue에 추가
                        jobList.RemoveAt(0);                    // readyQueue에 추가했으므로 jobList에선 삭제
                    }
                    else                                        // frontJob이 아직 arrive하지 않았다면
                    {
                        break;                                  // while문 탈출
                    }
                }
                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0) // 현재 실행중인 프로세스가 없고 readyQueue에 프로세스가 하나 이상 존재한다면
                    {
                        int min = readyQueue.ElementAt(0).priority; // min을 readyQueue의 0번째 우선순위로 초기화
                        int min_idx = 0;
                        for (int i = 1; i < readyQueue.Count; i++)  // readyQueue의 프로세스들 중 우선순위가 가장 높은 것을 찾는다
                        {                                           // !!! priority의 수가 작을수록 우선순위가 높다고 가정 !!!
                            if (min > readyQueue.ElementAt(i).priority)
                            {
                                min = readyQueue.ElementAt(i).priority;
                                min_idx = i;
                            }
                        }
                        ReadyQueueElement rq = readyQueue.ElementAt(min_idx); // rq = readyQueue의 min_idx번째
                        readyQueue.RemoveAt(min_idx);   // readyQueue에서 제거
                        rq.count_exe++;
                        resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime,rq.arriveTime,rq.count_exe,true)); // resultList에 rq의 Result를 추가
                        cpuDone = rq.burstTime;         // cpuDone은 현재의 프로세스 burstTime으로 설정
                        cpuTime = 0;                    // cpuTime을 다시 0으로 초기화
                        currentProcess = rq.processID;  // 현재 실행되고 있는 프로세스의 PID
                    }
                }
                else                            // 현재 실행중인 프로세스가 있다면
                {
                    if (cpuTime == cpuDone)     // 현재 실행중인 프로세스가 끝났다면
                    {
                        currentProcess = 0;     // 현재 실행되고 있는 프로세스가 끝났으므로 currentProcess는 0으로 설정
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++)  //readyQueue의 모든 프로세스들 waitingTime++
                    readyQueue.ElementAt(i).waitingTime++;

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0);//jobList 또는 readyQueue 또는 currentProcess중에 하나라도 있다면 계속 실행

            return resultList;
        }
    }
    class Preemptive_Priority
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;

            int runTime = 0;

            ReadyQueueElement current = null;                // 현재 실행중인 프로세스를 저장해둘 current
            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();
            do
            {
                while (jobList.Count != 0)                   // jobList에 하나 이상 있다면
                {
                    Process frontJob = jobList.ElementAt(0); // (Process)frontJob에 jobList의 0번을 꺼내온다

                    if (frontJob.arriveTime == runTime)      // frontJob이 arrive한 상태라면
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0,frontJob.arriveTime,frontJob.priority,0)); // frontJob을 readyQueue에 추가
                        jobList.RemoveAt(0);                 // readyQueue에 추가했으므로 jobList에선 삭제
                        if (current != null)
                        {
                            if (current.priority > frontJob.priority) // 새로 들어온 프로세스의 우선순위가 현재 프로세스의 우선순위보다 높다면
                            {                                         // !!! priority의 수가 작을수록 우선순위가 높다고 가정 !!!
                                ReadyQueueElement np = readyQueue.ElementAt(readyQueue.Count - 1); // np = readyqueue의 마지막(방금 추가 됐으므로 마지막에 존재)
                                readyQueue.RemoveAt(readyQueue.Count - 1);       // 새롭게 선점될 프로세스이므로 readyQueue에서 삭제
                                np.count_exe++;
                                current.burstTime -= cpuTime; // 현재 프로세스의 남은 burstTime 계산
                                resultList.Add(new Result(current.processID, runTime - cpuTime, cpuTime, current.waitingTime,current.arriveTime,current.count_exe,false));
                                // 현재까지 진행한 프로세스의 결과값을 resultList에 추가 
                                readyQueue.Add(new ReadyQueueElement(current.processID, current.burstTime, current.waitingTime,current.arriveTime, current.priority,current.count_exe));
                                // 선점으로 인해 종료되는 프로세스를 readyQueue에 추가

                                current = np;                                   // 현재 실행중인 프로세스에 선점된 프로세스 저장
                                cpuDone = np.burstTime;                         // cpuDone은 선점된 프로세스 burstTime으로 설정
                                cpuTime = 0;                                    // cpuTime은 0으로 초기화
                                currentProcess = np.processID;                  // 선점된 프로세스의 PID
                            }
                        }
                    }
                    else            // frontJob이 아직 arrive하지 않았다면
                    {
                        break;      // while문 탈출
                    }
                }
                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0)                      // 현재 실행중인 프로세스가 없고 readyQueue에 프로세스가 하나 이상 존재한다면
                    {
                        int min = readyQueue.ElementAt(0).priority; //min을 readyQueue의 0번째 우선순위로 초기화
                        int min_idx = 0;
                        for (int i = 1; i < readyQueue.Count; i++)  // readyQueue의 프로세스들 중 우선순위가 가장 높은 것을 찾는다
                        {                                           // !!! priority의 수가 작을수록 우선순위가 높다고 가정 !!!
                            if (min > readyQueue.ElementAt(i).priority)
                            {
                                min = readyQueue.ElementAt(i).priority;
                                min_idx = i;
                            }
                        }
                        ReadyQueueElement rq = readyQueue.ElementAt(min_idx);// rq = readyQueue의 min_idx번째
                        readyQueue.RemoveAt(min_idx);   // readyQueue에서 제거
                        rq.count_exe++;
                        cpuDone = rq.burstTime;         // cpuDone은 현재의 프로세스 burstTime으로 설정
                        cpuTime = 0;                    // cpuTime을 다시 0으로 초기화
                        current = rq;
                        currentProcess = rq.processID;  //현재 실행되고 있는 프로세스는 rq의 PID
                    }
                }
                else                        // 현재 실행중인 프로세스가 있다면
                {
                    if (cpuTime == cpuDone) // 현재 실행중인 프로세스가 끝났다면
                    {
                        resultList.Add(new Result(current.processID, runTime - cpuTime, cpuTime, current.waitingTime,current.arriveTime,current.count_exe,true));
                        // 현재까지 실행한 프로세스의 결과값을 resultList에 추가
                        currentProcess = 0; // 현재 실행되고 있는 프로세스가 끝났으므로 currentProcess는 0으로 설정
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++)  //readyQueue의 모든 프로세스들 waitingTime++
                    readyQueue.ElementAt(i).waitingTime++;

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0); //jobList 또는 readyQueue 또는 currentProcess중에 하나라도 있다면 계속 실행

            return resultList;
        }
    }
    class SJF
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;

            int runTime = 0;
            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();
            do
            {
                while (jobList.Count != 0)                      // jobList에 하나 이상 있다면
                {
                    Process frontJob = jobList.ElementAt(0);    // (Process)frontJob에 jobList의 0번을 꺼내온다

                    if (frontJob.arriveTime == runTime)         // frontJob이 arrive한 상태라면
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0,frontJob.arriveTime, frontJob.priority,0)); // frontJob을 readyQueue에 추가
                        jobList.RemoveAt(0);                    // readyQueue에 추가했으므로 jobList에선 삭제
                    }
                    else                                        // frontJob이 아직 arrive하지 않았다면
                    {

                        break;                                  // while문 탈출
                    }
                }
                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0)                         // 현재 실행중인 프로세스가 없고 readyQueue에 프로세스가 하나 이상 존재한다면
                    {
                        int min = readyQueue.ElementAt(0).burstTime;   //min을 readyQueue의 0번째 burstTime으로 초기화
                        int min_idx = 0;
                        for (int i = 1; i < readyQueue.Count; i++)     //readyQueue의 프로세스들 중 burstTime이 가장 짧은 것을 찾는다
                        {
                            if (min > readyQueue.ElementAt(i).burstTime)
                            {
                                min = readyQueue.ElementAt(i).burstTime;
                                min_idx = i;
                            }
                        }
                        ReadyQueueElement rq = readyQueue.ElementAt(min_idx); // rq = readyQueue의 min_idx번째
                        readyQueue.RemoveAt(min_idx);   // readyQueue에서 제거
                        rq.count_exe++;
                        resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime,rq.arriveTime,rq.count_exe,true)); // resultList에 rq의 Result를 추가
                        cpuDone = rq.burstTime;         // cpuDone은 현재의 프로세스 burstTime으로 설정
                        cpuTime = 0;                    // cpuTime을 다시 0으로 초기화
                        currentProcess = rq.processID;  // 현재 실행되고 있는 프로세스의 PID
                    }
                }
                else                        // 현재 실행중인 프로세스가 있다면
                {
                    if (cpuTime == cpuDone) // 현재 실행중인 프로세스가 끝났다면
                    {
                        currentProcess = 0; // 현재 실행되고 있는 프로세스가 끝났으므로 currentProcess는 0으로 설정
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++) // readyQueue의 모든 프로세스들 waitingTime++ 
                    readyQueue.ElementAt(i).waitingTime++;

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0); // jobList 또는 readyQueue 또는 currentProcess중에 하나라도 있다면 계속 실행

            return resultList;
        }
    }
    class SRTF
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;

            int runTime = 0;

            ReadyQueueElement current = null;                // 현재 실행중인 프로세스를 저장해둘 current
            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();
            do
            {
                while (jobList.Count != 0)                   // jobList에 하나 이상 있다면
                {
                    Process frontJob = jobList.ElementAt(0); // (Process)frontJob에 jobList의 0번을 꺼내온다

                    if (frontJob.arriveTime == runTime)      // frontJob이 arrive한 상태라면
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0,frontJob.arriveTime, frontJob.priority,0)); // frontJob을 readyQueue에 추가
                        jobList.RemoveAt(0);                 // readyQueue에 추가했으므로 jobList에선 삭제
                        if (current != null)
                        {
                            if ((current.burstTime - cpuTime) > frontJob.burstTime)
                            {
                                // 새로 들어온 프로세스의 burstTime이 현재 프로세스의 burstTime보다 작다면
                                // = (새로 들어온 프로세스가 선점되어야 한다면)
                                ReadyQueueElement np = readyQueue.ElementAt(readyQueue.Count - 1); // np = readyqueue의 마지막(방금 추가 됐으므로 마지막에 존재)
                                readyQueue.RemoveAt(readyQueue.Count - 1);       // 새롭게 선점될 프로세스이므로 readyQueue에서 삭제
                                np.count_exe++;

                                current.burstTime -= cpuTime; // 현재 프로세스의 남은 burstTime 계산
                                resultList.Add(new Result(current.processID, runTime - cpuTime, cpuTime, current.waitingTime,current.arriveTime,current.count_exe,false));
                                // 현재까지 진행한 프로세스의 결과값을 resultList에 추가 
                                readyQueue.Add(new ReadyQueueElement(current.processID, current.burstTime, current.waitingTime,current.arriveTime, current.priority,current.count_exe));
                                // 선점으로 인해 종료되는 프로세스를 readyQueue에 추가

                                current = np;                                   // 현재 실행중인 프로세스에 선점된 프로세스 저장
                                cpuDone = np.burstTime;                         // cpuDone은 선점된 프로세스 burstTime으로 설정
                                cpuTime = 0;                                    // cpuTime은 0으로 초기화
                                currentProcess = np.processID;                  // 선점된 프로세스의 PID
                            }
                        }
                    }
                    else            // frontJob이 아직 arrive하지 않았다면
                    {
                        break;      // while문 탈출
                    }
                }
                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0)                       // 현재 실행중인 프로세스가 없고 readyQueue에 프로세스가 하나 이상 존재한다면
                    {
                        int min = readyQueue.ElementAt(0).burstTime; //min을 readyQueue의 0번째 burstTime으로 초기화
                        int min_idx = 0;
                        for (int i = 1; i < readyQueue.Count; i++)   // readyQueue의 프로세스들 중 burstTime이 가장 짧은 것을 찾는다
                        {
                            if (min > readyQueue.ElementAt(i).burstTime)
                            {
                                min = readyQueue.ElementAt(i).burstTime;
                                min_idx = i;
                            }
                        }
                        ReadyQueueElement rq = readyQueue.ElementAt(min_idx);// rq = readyQueue의 min_idx번째
                        readyQueue.RemoveAt(min_idx);   // readyQueue에서 제거
                        rq.count_exe++;
                        cpuDone = rq.burstTime;         // cpuDone은 현재의 프로세스 burstTime으로 설정
                        cpuTime = 0;                    // cpuTime을 다시 0으로 초기화
                        current = rq;
                        currentProcess = rq.processID;  //현재 실행되고 있는 프로세스는 rq의 PID
                    }
                }
                else                        // 현재 실행중인 프로세스가 있다면
                {
                    if (cpuTime == cpuDone) // 현재 실행중인 프로세스가 끝났다면
                    {
                        resultList.Add(new Result(current.processID, runTime - cpuTime, cpuTime, current.waitingTime,current.arriveTime,current.count_exe,true));
                        // 현재까지 실행한 프로세스의 결과값을 resultList에 추가
                        currentProcess = 0; // 현재 실행되고 있는 프로세스가 끝났으므로 currentProcess는 0으로 설정
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++)  //readyQueue의 모든 프로세스들 waitingTime++
                    readyQueue.ElementAt(i).waitingTime++;

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0); //jobList 또는 readyQueue 또는 currentProcess중에 하나라도 있다면 계속 실행

            return resultList;
        }
    }
    class FCFS
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;

            int runTime = 0;

            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();
            do
            {
                while (jobList.Count != 0)                   // jobList에 하나 이상 있다면
                {
                    Process frontJob = jobList.ElementAt(0); // (Process)frontJob에 jobList의 0번을 꺼내온다
                    if (frontJob.arriveTime == runTime)      // frontJob이 arrive한 상태라면
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0,frontJob.arriveTime, frontJob.priority,0)); // frontJob을 readyQueue에 추가
                        jobList.RemoveAt(0);                 // readyQueue에 추가했으므로 jobList에선 삭제
                    }
                    else                                     // frontJob이 아직 arrive하지 않았다면
                    {
                        break;                               // while문 탈출
                    }
                }

                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0) // 현재 실행중인 프로세스가 없고 readyQueue에 프로세스가 하나 이상 존재한다면
                    {
                        ReadyQueueElement rq = readyQueue.ElementAt(0); // rq = readyQueue의 0번째
                        readyQueue.RemoveAt(0);        // readyQueue에서 제거
                        rq.count_exe++;
                        resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime,rq.arriveTime,rq.count_exe,true)); //resultList에 rq의 Result를 추가
                        cpuDone = rq.burstTime;        // cpuDone은 현재의 프로세스 burstTime으로 설정
                        cpuTime = 0;                   // cpuTime을 다시 0으로 초기화
                        currentProcess = rq.processID; // 현재 실행되고 있는 프로세스의 PID
                    }
                }
                else                                   //현재 실행중인 프로세스가 있다면
                {
                    if (cpuTime == cpuDone)            // 현재 실행중인 프로세스가 끝났다면
                    {
                        currentProcess = 0;            // 현재 실행되고 있는 프로세스가 끝났으므로 currentProcess는 0으로 설정
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++) //readyQueue의 모든 프로세스들 waitingTime++ 
                    readyQueue.ElementAt(i).waitingTime++;

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0); //jobList 또는 readyQueue 또는 currentProcess중에 하나라도 있다면 계속 실행

            return resultList;
        }
    }
    class RoundRobin
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList, int timequantum)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;

            int runTime = 0;

            Boolean isBiggerQuantum = false;    // 현재 프로세스의 burstTime이 timequantum보다 큰지 확인하기 위한 boolean 변수 (크다면 true,작다면 false)
            ReadyQueueElement temp = null;      // 현재 프로세스를 다시 Round Robin의 맨 뒤로 넣어줄때 사용하는 temp 변수 

            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();
            do
            {
                while (jobList.Count != 0)      // jobList에 하나 이상 있다면
                {
                    Process frontJob = jobList.ElementAt(0);  // (Process)frontJob에 jobList의 0번을 꺼내온다
                    if (frontJob.arriveTime == runTime)       // frontJob이 arrive한 상태라면
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0,frontJob.arriveTime, frontJob.priority, 0)); // frontJob을 readyQueue에 추가
                        jobList.RemoveAt(0);    // readyQueue에 추가했으므로 jobList에선 삭제
                    }
                    else                        // frontJob이 아직 arrive하지 않았다면
                    {
                        break;                  // while문 탈출
                    }
                }

                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0)  // 현재 실행중인 프로세스가 없고 readyQueue에 프로세스가 하나 이상 존재한다면
                    {
                        ReadyQueueElement rq = readyQueue.ElementAt(0); // rq = readyQueue의 0번째
                        readyQueue.RemoveAt(0);                         // readyQueue에서 제거
                        cpuTime = 0;                                    // cpuTime을 다시 0으로 초기화
                        currentProcess = rq.processID;
                        rq.count_exe++;                                 
                        if (rq.burstTime > timequantum)                 // burstTime이 timequantum보다 큰지 확인    
                            isBiggerQuantum = true;
                        else
                            isBiggerQuantum = false;

                        if (isBiggerQuantum)                            // burstTime이 timeQuantum보다 클 때
                        {
                            resultList.Add(new Result(rq.processID, runTime, timequantum, rq.waitingTime,rq.arriveTime, rq.count_exe,false)); // resultList에 rq의 Result를 추가
                            cpuDone = timequantum;                      // cpuDone은 우선 timequautum으로 설정
                            rq.burstTime -= timequantum;                // burstTime을 timequantum만큼 빼줌
                            temp = rq;                                  // 남은 부분을 readyQueue에 넣어줄때를 위해 temp로 저장
                        }
                        else                                            // burstTime이 timeQuantum보다 작거나 같을 때
                        {
                            resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime, rq.arriveTime, rq.count_exe,true));
                            cpuDone = rq.burstTime;                     // cpuDone은 burstTime
                        }
                    }
                }
                else                        // 현재 실행중인 프로세스가 있다면
                {
                    if (cpuTime == cpuDone) // 현재 실행중인 프로세스가 끝났다면
                    {
                        currentProcess = 0; // 현재 실행되고 있는 프로세스가 끝났으므로 currentProcess는 0으로 설정
                        if (isBiggerQuantum) 
                        {
                            readyQueue.Add(new ReadyQueueElement(temp.processID, temp.burstTime, 0,temp.arriveTime, temp.priority, temp.count_exe));
                            // temp로 저장해 둔 남은 프로세스부분을 레디큐에 넣어주기
                            isBiggerQuantum = false; // isBiggerQuantum 초기화
                        }
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++) //readyQueue의 모든 프로세스들 waitingTime++
                    readyQueue.ElementAt(i).waitingTime++;

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0); //jobList 또는 readyQueue 또는 currentProcess중에 하나라도 있다면 계속 실행

            return resultList;
        }
    }
    class HRRN
    {
        public static List<Result> Run(List<Process> jobList, List<Result> resultList)
        {
            int currentProcess = 0;
            int cpuTime = 0;
            int cpuDone = 0;
            int runTime = 0;
            // !!! HRRN에선 priority를 Response Ration로 사용 !!!

            List<ReadyQueueElement> readyQueue = new List<ReadyQueueElement>();
            do
            {
                while (jobList.Count != 0)                      // jobList에 하나 이상 있다면
                {
                    Process frontJob = jobList.ElementAt(0);    // (Process)frontJob에 jobList의 0번을 꺼내온다

                    if (frontJob.arriveTime == runTime)         // frontJob의 arriveTime이랑 runTime이랑 같으면 = frontJob이 arrive했다면
                    {
                        readyQueue.Add(new ReadyQueueElement(frontJob.processID, frontJob.burstTime, 0,frontJob.arriveTime, 1,0));
                        //readyQueue에 frontJob으로 ReadyQueueElement를 생성하여 추가
                        //여기서 Priority는 (Waiting Time + Burst Time) / Burst Time인데 Waiting Time = 0 이므로 1을 설정한다. 
                        jobList.RemoveAt(0);                    // readyQueue에 추가했으므로 jobList에선 삭제
                    }
                    else                                        // frontJob이 아직 arrive하지 않았다면
                    {
                        break;                                  // while문 탈출
                    }
                }
                if (currentProcess == 0)
                {
                    if (readyQueue.Count != 0)                      // 실행중인 프로세스가 없고 readyQueue에 프로세스가 하나 이상 존재한다면
                    {
                        int max = readyQueue.ElementAt(0).priority; // 우선 max를 readyQueue의 0번째 우선순위(Response Ratio)로 초기화
                        int max_idx = 0;
                        for (int i = 1; i < readyQueue.Count; i++)  // readyQueue의 프로세스들 중 우선순위(Response Ratio)가 가장 높은 것을 찾는다
                        {
                            if (max < readyQueue.ElementAt(i).priority)
                            {
                                max = readyQueue.ElementAt(i).priority;
                                max_idx = i;
                            }
                        }
                        ReadyQueueElement rq = readyQueue.ElementAt(max_idx); // resultList에 rq의 Result를 추가
                        readyQueue.RemoveAt(max_idx);   // readyQueue에서 max번째 삭제
                        rq.count_exe++;
                        resultList.Add(new Result(rq.processID, runTime, rq.burstTime, rq.waitingTime,rq.arriveTime,rq.count_exe,true));
                        cpuDone = rq.burstTime;         // cpuDone은 현재의 프로세스 burstTime으로 설정
                        cpuTime = 0;                    // cpuTime을 다시 0으로 초기화
                        currentProcess = rq.processID;  // 현재 실행되고 있는 프로세스는 rq의 PID
                    }
                }
                else                                    // 현재 실행중인 프로세스가 있다면
                {
                    if (cpuTime == cpuDone)             // 현재 실행중인 프로세스가 끝났다면
                    {
                        currentProcess = 0;             // 현재 실행되고 있는 프로세스가 끝났으므로 currentProcess는 0으로 설정

                        for (int i = 0; i < readyQueue.Count; i++)  //readyQueue의 모든 프로세스 Response Ratio를 재설정
                            readyQueue.ElementAt(i).priority = (readyQueue.ElementAt(i).waitingTime + readyQueue.ElementAt(i).burstTime)*100 / readyQueue.ElementAt(i).burstTime;
                        // !!! HRRN에선 priority를 Response Ratio로 사용 !!!
                        // ReadyQueueElement가 priority를 int형으로 가지기 때문에 분모에 100을 곱한 값 사용
                        continue;
                    }
                }

                cpuTime++;
                runTime++;

                for (int i = 0; i < readyQueue.Count; i++)  //readyQueue의 모든 프로세스 waitingTime 1씩 증가
                {
                    readyQueue.ElementAt(i).waitingTime++;
                }

            } while (jobList.Count != 0 || readyQueue.Count != 0 || currentProcess != 0);//jobList, readyQueue, currentProcess 중 1개라도 있다면 반복

            return resultList;
        }
    }

}