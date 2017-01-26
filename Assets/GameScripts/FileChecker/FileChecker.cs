using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Softstar
{
    public class FileChecker
    {
        private string m_strFile;
        private FileHashList m_updateList;
        private FileHashDatabase m_fileHashDatabase;
        private List<FileHash> m_downloadList;

        public List<FileHash> DownloadList
        {
            get
            {
                return m_downloadList;
            }
        }

        public FileChecker(string updateFile = "update.json")
        {
            m_strFile = updateFile;
            m_updateList = null;
            m_fileHashDatabase = null;
            m_downloadList = new List<FileHash>();
        }

        public void ConnectDatabase()
        {
            m_fileHashDatabase = new FileHashDatabase();
            m_fileHashDatabase.Connect();
            m_fileHashDatabase.CreateTable();
        }

        public void CloseDatabase()
        {
            m_fileHashDatabase.Close();
        }

        public FileHash GetFileHash(string strPath)
        {
            for (int i = 0; i < m_downloadList.Count; i++)
            {
                if(m_downloadList[i].Path == strPath)
                {
                    return m_downloadList[i];
                }
            }
            return null;
        }

        public void LoadUpdateFile()
        {
            string updateFilePath = Application.persistentDataPath + "/" + m_strFile;
            try
            {
                using (FileStream fs = new FileStream(updateFilePath, FileMode.Open, FileAccess.Read))
                {
                    StreamReader reader = new StreamReader(fs);
                    m_updateList = LitJson.JsonMapper.ToObject<FileHashList>(reader);
                    reader.Close();
                }
            }
            catch(Exception e)
            {
                UnityDebugger.Debugger.LogError(string.Format("LoadUpdateFile [{0}] failed", updateFilePath));
            }

            /*
            for(int i=0;i< m_updateList.List.Length;i++)
            {
                UnityDebugger.Debugger.Log(string.Format("SHA1[{0}] file[{1}] len[{2}]",
                    m_updateList.List[i].SHA1,
                    m_updateList.List[i].Path,
                    m_updateList.List[i].Length));
            }
            */
        }

        public int CheckFile()
        {
            if (m_updateList != null)
            {
                m_downloadList.Clear();

                FileHash[] fileHashList = m_updateList.List;
                for (int i = 0; i < fileHashList.Length; i++)
                {
                    FileHash fh = fileHashList[i];
                    FileHash fhCompare = m_fileHashDatabase.GetData(fh.SHA1, fh.Path);
                    if(fhCompare == null)
                    {
                        m_downloadList.Add(fh); // 加入下載清單
                    }
                }

                for(int i=0;i<m_downloadList.Count;i++)
                {
                    UnityDebugger.Debugger.Log(string.Format("Download List: SHA1[{0}] file[{1}] len[{2}]",
                        m_downloadList[i].SHA1,
                        m_downloadList[i].Path,
                        m_downloadList[i].Length));
                }

                return m_downloadList.Count;
            }
            return 0;
        }

        public void UpdateFile(string strSha1, string strFile, int iLength)
        {
            m_fileHashDatabase.RemoveData(strFile);
            m_fileHashDatabase.Insert(strSha1, strFile, iLength);
        }
    }
}
