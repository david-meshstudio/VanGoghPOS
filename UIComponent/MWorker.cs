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
                BackgroundWorker cw = getAvailableWorker();
                if(cw == null)
                {
                    JobQueue.Enqueue((MJob)job);
                }
                else
                {
                    cw.RunWorkerAsync((MJob)job);
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
            if(JobQueue.Count > 0)
            {
                MJob jobNext = JobQueue.Dequeue();
                ((BackgroundWorker)sender).RunWorkerAsync(jobNext);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            long ts = BasicTool.GetCurrentTimestampLong();
        }

        public class MJob
        {
            public string id = "", type = "", task = "", parameter = "";
        }
    }
}
