using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Schd
{
    public partial class Scheduling : Form
    {
        int count = 0;
        string[] readText;
        private bool readFile = false;
        List<Process> pList, pView;
        List<Result> resultList;

        public Scheduling()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            pView.Clear();
            pList.Clear();

            //파일 오픈
            string path =  SelectFilePath();
            if (path == null) return;
    
            readText = File.ReadAllLines(path);
            
            //토큰 분리
            for (int i = 0; i < readText.Length; i++)
            {
                string[] token = readText[i].Split(' ');
                Process p = new Process(int.Parse(token[1]), int.Parse(token[2]), int.Parse(token[3]), int.Parse(token[4]));
                pList.Add(p);
            }

            //Grid에 process 출력
            dataGridView1.Rows.Clear();
            string[] row = { "", "", "", "" };
            foreach (Process p in pList)
            {
                row[0] = p.processID.ToString();
                row[1] = p.arriveTime.ToString();
                row[2] = p.burstTime.ToString();
                row[3] = p.priority.ToString();
                count++;
                dataGridView1.Rows.Add(row);
            }

            //arriveTime으로 정렬
            pList.Sort(delegate(Process x, Process y)
            {
                if (x.arriveTime > y.arriveTime) return 1;
                else if (x.arriveTime < y.arriveTime) return -1;
                else
                {
                    return x.processID.CompareTo(y.processID);
                }
                //return x.arriveTime.CompareTo(y.arriveTime);
            });

            readFile = true;
        }

        private string SelectFilePath()
        {
            openFileDialog1.Filter = "텍스트파일|*.txt";
            return (openFileDialog1.ShowDialog() == DialogResult.OK) ? openFileDialog1.FileName : null;
        }

        private void Run_Click(object sender, EventArgs e)
        {
            if (!readFile) return;
            //스케쥴러 실행

            if (comboBox1.Text == "Non_Preemptive_Priority")
                resultList = Non_Preemptive_Priority.Run(pList, resultList);
            else if (comboBox1.Text == "Preemptive_Priority")
                resultList = Preemptive_Priority.Run(pList, resultList);
            else if (comboBox1.Text == "HRRN")
                resultList = HRRN.Run(pList, resultList);
            else if (comboBox1.Text == "FCFS")
                resultList = FCFS.Run(pList, resultList);
            else if (comboBox1.Text == "SRTF")
                resultList = SRTF.Run(pList, resultList);
            else if (comboBox1.Text == "SJF")
                resultList = SJF.Run(pList, resultList);
            else if (comboBox1.Text == "Round Robin")
            {
                if (textBox1.Text == null)
                {
                    MessageBox.Show("Time Quantum을 입력해주세요.");
                    return;
                }
                resultList = RoundRobin.Run(pList, resultList, int.Parse(textBox1.Text));
            }
            else
            {
                MessageBox.Show("스케줄링 방식을 선택하세요.");
                return;
            }

            //결과출력
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            string[] row = { "", "", "", "","" };
            string[] view_process = { "", "", "","",""};    // 프로세스마다의 결과를 넘겨주기 위한 배열 view_process

            double[] responseTime = new double[count+1];    // 프로세스마다의 결과를 저장하기위한 배열들 선언
            double[] waitingTime = new double[count+1]; 
            double[] turnaroundTime = new double[count+1];
            string[] pid = new string[count + 1];   

            for (int i = 0; i < count + 1; i++)     //모두 0으로 초기화
            {
                responseTime[i] = 0;
                waitingTime[i] = 0;
                turnaroundTime[i] = 0;
            }
            foreach (Result r in resultList)
            {
                row[0] = r.processID.ToString();
                row[1] = r.burstTime.ToString();
                row[2] = r.waitingTime.ToString();
                row[3] = r.startP.ToString();
                waitingTime[r.processID] += r.waitingTime; // waitingTime을 누적시킨다.
                pid[r.processID] =r.processID.ToString();
                if (r.count_exe == 1)               // count_exe가 1이라면 첫 실행이라는 의미
                    responseTime[r.processID] = (r.startP - r.arriveTime); // 첫 실행이므로 responseTime을 계산해서 저장
                   
                if (r.isFinish)                     // isFinish가 true라면 프로세스가 종료됨을 의미
                    turnaroundTime[r.processID] = r.startP + r.burstTime - r.arriveTime;  // 작업을 마쳤으므로 turnaroundTime을 계산해서 저장
                dataGridView2.Rows.Add(row);
            }
            for(int i = 1; i<count+1;i++)
            {
                responseTime[0] += responseTime[i];     //배열의 0번째는 평균 시간을 구하기 위한 배열의 합을 계산하는 index로 사용
                waitingTime[0] += waitingTime[i];
                turnaroundTime[0] += turnaroundTime[i];
            }
            for(int i = 1; i < count + 1; i++)
            {
                view_process[0] = pid[i];
                view_process[1] = responseTime[i].ToString();
                view_process[2] = waitingTime[i].ToString();
                view_process[3] = turnaroundTime[i].ToString();
                dataGridView3.Rows.Add(view_process);
            }
            TRTime.Text = "Total Execution Time : " + (resultList[resultList.Count - 1].startP + resultList[resultList.Count - 1].burstTime).ToString();
            avgRT.Text = "Average Waiting Time : " + (waitingTime[0] / count).ToString();
            resT.Text = "Average Response Time : " + (responseTime[0] / count).ToString();
            tardT.Text = "Average Turnaround Time : " + (turnaroundTime[0] / count).ToString();
            panel1.Invalidate();
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            int startPosition = 10;
            //double waitingTime = 0.0;

            int resultListPosition = 0;
            foreach (Result r in resultList)
            {
                e.Graphics.DrawString("p" + r.processID.ToString(), Font, Brushes.Black, startPosition + (r.startP * 10), resultListPosition);
                e.Graphics.DrawRectangle(Pens.Red, startPosition + (r.startP * 10), resultListPosition + 20, r.burstTime * 10, 30);
                //e.Graphics.DrawString(r.burstTime.ToString(), Font, Brushes.Black, startPosition + (r.startP * 10), resultListPosition + 60);
                //e.Graphics.DrawString(r.waitingTime.ToString(), Font, Brushes.Black, startPosition + (r.startP * 10), resultListPosition + 80);
                e.Graphics.DrawString(r.startP.ToString(), Font, Brushes.Black, startPosition + (r.startP * 10), resultListPosition + 60);
                //waitingTime += (double)r.waitingTime;
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void TRTime_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click_1(object sender, EventArgs e)
        {

        }

        private void resT_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pList = new List<Process>();
            pView = new List<Process>();
            resultList = new List<Result>();
           

            //입력창
            DataGridViewTextBoxColumn processColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn arriveTimeColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn burstTimeColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn priorityColumn = new DataGridViewTextBoxColumn();

            processColumn.HeaderText = "프로세스";
            processColumn.Name = "process";
            arriveTimeColumn.HeaderText = "도착시간";
            arriveTimeColumn.Name = "arriveTime";
            burstTimeColumn.HeaderText = "실행시간";
            burstTimeColumn.Name = "burstTime";
            priorityColumn.HeaderText = "우선순위";
            priorityColumn.Name = "priority";

            dataGridView1.Columns.Add(processColumn);
            dataGridView1.Columns.Add(arriveTimeColumn);
            dataGridView1.Columns.Add(burstTimeColumn);
            dataGridView1.Columns.Add(priorityColumn);



            //결과창
            DataGridViewTextBoxColumn resultProcessColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn resultBurstTimeColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn resultWaitingTimeColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn resultStartTiemColumn = new DataGridViewTextBoxColumn();

            resultProcessColumn.HeaderText = "프로세스";
            resultProcessColumn.Name = "process";
            resultBurstTimeColumn.HeaderText = "실행시간";
            resultBurstTimeColumn.Name = "resultBurstTimeColumn";
            resultWaitingTimeColumn.HeaderText = "대기시간";
            resultWaitingTimeColumn.Name = "waitingTime";
            resultStartTiemColumn.HeaderText = "시작시간";
            resultStartTiemColumn.Name = "startTime";

            dataGridView2.Columns.Add(resultProcessColumn);
            dataGridView2.Columns.Add(resultBurstTimeColumn);
            dataGridView2.Columns.Add(resultWaitingTimeColumn);
            dataGridView2.Columns.Add(resultStartTiemColumn);

            //프로세스별 출력
            DataGridViewTextBoxColumn view_ProcessColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn view_ResponseTimeColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn view_WaitingTimeColumn = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn view_TurnaroundTimeColumn = new DataGridViewTextBoxColumn();

            view_ProcessColumn.HeaderText = "프로세스";
            view_ProcessColumn.Name = "V_process";
            view_ResponseTimeColumn.HeaderText = "Response Time";
            view_ResponseTimeColumn.Name = "V_resT";
            view_WaitingTimeColumn.HeaderText = "Waiting Time";
            view_WaitingTimeColumn.Name = "V_avgRT";
            view_TurnaroundTimeColumn.HeaderText = "Turnaround Time";
            view_TurnaroundTimeColumn.Name = "V_tardT";

            dataGridView3.Columns.Add(view_ProcessColumn);
            dataGridView3.Columns.Add(view_ResponseTimeColumn);
            dataGridView3.Columns.Add(view_WaitingTimeColumn);
            dataGridView3.Columns.Add(view_TurnaroundTimeColumn);
        }
    }
}
