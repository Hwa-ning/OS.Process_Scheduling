# Process Scheduling

## 작성자 : Hwa-ning

2020.05.27 제작
<br/>

- OS에서 Process를 선택하는 Scheduling 기법에 대해 이해하고 알고리즘을 작성
- C# / .NET Framework 이용<br>

### 구현 스케쥴링 목록

1. FCFS Scheduling (First Come First Served)<br>
   도착한 순서대로 프로세스를 처리하는 스케줄링 방식, 다음 실행할 프로세스를 Dispatch 할 때 Ready Queue에서 가장 먼저 도착한 프로세스를 Dispatch

2. SJF Scheduling (Shortest Job First)<br>
   Burst Time이 가장 짧은 프로세스를 가장 먼저 처리하는 스케줄링 방식, 다음 실행할 프로세스를 Dispatch 할 때 Ready Queue에서 실행시간이 가장 짧은 프로세스를 Dispatch

3. Non-Preemptive Priority Scheduling<br>
   Priority가 가장 높은 프로세스를 가장 먼저 처리하는 스케줄링 방식, 다음 실행할 프로세스를 Dispatch 할 때 Ready Queue에서 우선순위가 가장 높은 프로세스를 Dispatch

4. Round-Robin Scheduling<br>
   FCFS 기반에 Time Quantum이 존재하여 각각의 프로세스는 한 번의 실행 동안 최대 Time Quantum만큼만 실행하는 스케줄링 방식, Time Quantum의 시간 안에 프로세스를 완료하지 못한다면 남은 실행시간을 계산하여 Ready Queue의 맨 끝으로 삽입하고 Ready Queue에서 가장 먼저 도착한 프로세스를 Dispatch

5. SRTF Scheduling (Shortest Remaining Time First)<br>
   SJF 기반의 선점형 스케줄링 방식, 선점형이므로 프로세스가 도착할 때마다 현재 실행 중인 프로세스의 남은 실행시간과 도착한 프로세스의 실행시간을 비교하여서 더 짧은 것을 고름. 선점이 발생해 쫓겨난 프로세스의 남은 실행시간을 계산하여 다시 Ready Queue의 맨 끝에 삽입

6. Preemptive Priority Scheduling<br>
   Priority가 가장 높은 프로세스를 가장 먼저 처리하며 선점형이 적용된 스케줄링 방식, 선점형이므로 프로세스가 도착할 때마다 현재 실행 중인 프로세스의 우선순위와 도착한 프로세스의 우선순위를 비교하여 더 높은 우선순위의 프로세스를 실행하도록 한다. 선점이 발생해 쫓겨난 프로세스의 남은 실행시간을 계산해서 Ready Queue의 맨 끝에 삽입

7. HRRN Scheduilng (Highest Response Ration Next)<br>
   SJF 스케줄링의 문제점인 실행시간이 긴 프로세스의 Starvation을 보완한 스케줄링 방식, 다음 실행할 프로세스를 Dispatch 하는 과정에서 실행시간과 대기시간을 고려한 Response Ratio가 가장 높은 프로세스를 Dispatch (Response Ratio = (Waiting Time + Burst Time) / Burst Time)

### 사용 방법

![캡처1](https://user-images.githubusercontent.com/69469529/125495895-00f59e67-43bc-4e76-875a-f947094236b6.JPG)

스케줄러를 실행하려면 ❻ OpenFile에서 실행할 입력 파일을 선택하고 ❽에서 실행할 스케줄링 알고리즘을 선택하고 ❼ Run을 하면 값이 출력된다. 샘플의 스케줄러에서 ❸에서 전체 프로세스의 평균 응답시간과 평균 턴어라운드 시간을 추가 구현했으며 ❹에서 간트 차트를 출력할 때 해당 프로세스의 번호와 해당 프로세스의 시작하는 시간을 출력하도록 수정했다. ❺에서 DataGridView를 이용하여 프로세스 각각의 Response Time, Waiting Time, Turnaround Time을 출력하도록 했다. ❽에서는 ComboBox 기능을 이용하여 실행하려는 스케줄링 알고리즘을 선택하도록 하였으며, ❾에서는 Round Robin 스케줄링의 Time Quantum을 입력받도록 했다.

### 파일 입력

![캡처2](https://user-images.githubusercontent.com/69469529/125496191-9becc247-f262-498d-b767-f28f5aaf78fc.JPG)

본 프로그램은 실행 전 txt 파일을 불러와서 입력값을 받으며 txt 파일은
process / process# / Arrive Time / Burst Time / Priority
과 같은 내용을 가져야 하며 이는 process/프로세스의 번호/도착 시간/실행 시간/우선순위를 의미한다. 우선순위는 숫자가 작을수록 더 높은 우선순위를 가진다고 가정하고 구현했다. (우선순위 1이 우선순위 3보다 우선 순위가 높음.)
