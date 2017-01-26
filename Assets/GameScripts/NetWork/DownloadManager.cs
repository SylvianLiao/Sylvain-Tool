using System.Collections.Generic;
using UnityEngine;

using Softstar.GamePacket;

namespace Softstar
{
    public class DownloadManager
    {
        public string BaseURL { set; get; }

        public delegate void DownloadFinishCallback(DownloadJob downloadJob);
        public event DownloadFinishCallback DownloadFinishEvent = null;

        private List<DownloadJob> m_jobList;
        private List<DownloadJob> m_downloadList;
        private List<DownloadJob> m_finishList;
        private int m_iMinJob;

        public DownloadManager()
        {
            BaseURL = "http://localhost/";

            m_jobList = new List<DownloadJob>();
            m_downloadList = new List<DownloadJob>();
            m_finishList = new List<DownloadJob>();
            m_iMinJob = 1; // 一次一個下載
        }

        public void AddJob(string strPath, string strDstPath="")
        {
            DownloadJob dj = new DownloadJob();
            dj.URL = BaseURL + strPath;
            if (string.IsNullOrEmpty(strDstPath))
            {
                dj.FilePath = Application.persistentDataPath + "/" + strPath;
                dj.FileName = strPath;
            }
            else
            {
                dj.FilePath = Application.persistentDataPath + "/" + strDstPath;
                dj.FileName = strDstPath;
            }
            m_jobList.Add(dj);
        }

        public int TotalJob()
        {
            return m_jobList.Count + m_finishList.Count;
        }

        public int NowJob()
        {
            return m_jobList.Count;
        }

        public int FinishJob()
        {
            return m_finishList.Count;
        }

        public float CompletePercent()
        {
            int total = TotalJob();
            if(total > 0)
            {
                return (float)m_finishList.Count / (float)total;
            }
            return 1.0f;
        }

        public void Clear()
        {
            m_jobList.Clear();
            m_downloadList.Clear();
            m_finishList.Clear();
        }

        public void Update()
        {
            //==============================================================
            // 檢查下載是否完成
            List<DownloadJob> doneList = null;
            if (m_downloadList.Count > 0)
            {
                for (int i = 0; i < m_downloadList.Count; i++)
                {
                    //if(m_downloadList[i].State == EDownloadState.Done)
                    if (m_downloadList[i].isDone)
                    {
                        UnityDebugger.Debugger.Log(string.Format("{0} download finish", m_downloadList[i].FilePath));
                        if (doneList == null)
                        {
                            doneList = new List<DownloadJob>();
                        }
                        doneList.Add(m_downloadList[i]);
                    }
                }
            }

            //==============================================================
            // 將完成的下載從清單移除
            if(doneList != null)
            {
                for(int i=0;i<doneList.Count;i++)
                {
                    UnityDebugger.Debugger.Log(string.Format("Download from [{0}] [{1}] to [{2}]", doneList[i].responseCode, doneList[i].URL, doneList[i].FilePath));
                    if (doneList[i].isError || doneList[i].responseCode != HttpStatus.OK)
                    {
                        doneList[i].SetError();
                        UnityDebugger.Debugger.LogError(string.Format("Download FAILED: http status:[{0}] [{1}]", doneList[i].responseCode, doneList[i].URL));
                    }
                    else
                    {
                        if(DownloadFinishEvent != null)
                        {
                            DownloadFinishEvent(doneList[i]);
                        }
                    }
                    m_jobList.Remove(doneList[i]);
                    m_downloadList.Remove(doneList[i]);
                    m_finishList.Add(doneList[i]);
                }
            }

            //==================================================================
            // 檢查並啟動下載
            if ((m_jobList.Count > 0) && (m_downloadList.Count < m_iMinJob))
            {
                for (int i = 0; i < m_jobList.Count; i++)
                {
                    if (m_jobList[i].State == EDownloadState.Start)
                    {
                        m_jobList[i].Start();
                        m_downloadList.Add(m_jobList[i]);
                    }

                    if (m_downloadList.Count >= m_iMinJob)
                    {
                        break;
                    }
                }
            }
        }
    }
}
