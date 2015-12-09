using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using COM.MeshStudio.Lib.BasicComponent;
using System.Windows.Forms;

namespace COM.MeshStudio.Lib.UIComponent
{
    public class MWorker
    {
        public List<BackgroundWorker> workers = new List<BackgroundWorker>();
        public delegate void JobDoneMethod(string message);
        public JobDoneMethod WorkerJobDone;
        public Queue<MJob> JobQueue = new Queue<MJob>();
        public Dictionary<long, List<MJob>> TimerJobQueue = new Dictionary<long, List<MJob>>();
        private Timer timer;

        public MWorker(int number)
        {
            timer = new Timer();
            timer.Interval = 60 * 1000;
            timer.Tick += timer_Tick;
            timer.Start();
            for (int i = 0; i < number; i++)
            {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += worker_DoWork;
                bw.RunWorkerCompleted += worker_DoCompleted;
                workers.Add(bw);
            }
        }

        public void AddJob(string jobstr)
        {
            List<object> jobList = JsonTool.JSON_Decode_Object(jobstr, new List<object>() { new MJob() });
            foreach(var job in jobList)
            {
                MJob mjob = (MJob)job;
                AddJob(mjob);
            }
        }

        private void AddJob(MJob mjob)
        {
            if (mjob.startTime == 0)
            {
                BackgroundWorker cw = getAvailableWorker();
                if (cw == null)
                {
                    JobQueue.Enqueue(mjob);
                }
                else
                {
                    cw.RunWorkerAsync(mjob);
                }
            }
            else
            {
                long startTime = mjob.startTime;
                if (TimerJobQueue.Keys.Contains<long>(startTime))
                {
                    TimerJobQueue[startTime].Add(mjob);
                }
                else
                {
                    TimerJobQueue.Add(startTime, new List<MJob>() { mjob });
                }
            }
        }

        private BackgroundWorker getAvailableWorker()
        {
            BackgroundWorker result = null;
            foreach (BackgroundWorker bw in workers)
            {
                if(!bw.IsBusy)
                {
                    result = bw;
                    break;
                }
            }
            return result;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                MJob mjob = (MJob)e.Argument;
                switch(mjob.type)
                {
                    case "API":
                        e.Result = APIAdaptor.CallAPIByFunction(mjob.task, mjob.parameter);
                        break;
                    case "FILE":
                        if (mjob.task == "read")
                        {
                            e.Result = FileAdaptor.ReadFile(mjob.parameter);
                        }
                        else if (mjob.task == "write")
                        {
                            string[] paras = mjob.parameter.Split(new char[] { ',' });
                            FileAdaptor.WriteFile(paras[0], paras[1]);
                            e.Result = "OK";
                        }
                        break;
                    case "DEVICE":
                        break;
                    default:
                        string jobString = JsonTool.JSON_Encode_Object(new List<object>() { mjob });
                        e.Result = "NoMatchJobType:" + jobString;
                        break;
                }
            }
            catch(Exception exp)
            {
                e.Result = exp.ToString();

            }
        }

        private void worker_DoCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerJobDone(e.Result.ToString());
            if (JobQueue.Count > 0)
            {
                MJob jobNext = JobQueue.Dequeue();
                ((BackgroundWorker)sender).RunWorkerAsync(jobNext);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            long ts = BasicTool.GetCurrentTimestampLong();
            List<long> keyList = TimerJobQueue.Keys.ToList<long>();
            keyList.Sort();
            foreach(long startTime in keyList)
            {
                if(startTime <= ts)
                {
                    List<MJob> mjobList = TimerJobQueue[startTime];
                    TimerJobQueue.Remove(startTime);
                    foreach(MJob mjob in mjobList)
                    {
                        mjob.startTime = 0;
                        AddJob(mjob);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public class MJob
        {
            public string id = "", type = "", task = "", parameter = "";
            public long startTime = 0;
        }
    }
}
