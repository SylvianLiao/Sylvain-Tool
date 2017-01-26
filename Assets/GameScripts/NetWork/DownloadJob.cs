using System.Text;
using System.IO;
using UnityEngine.Networking;

using Softstar.GamePacket;


namespace Softstar
{
    public enum EDownloadState
    {
        Start,
        Downloading,
        Done,
        Error
    }

    public class DownloadJob : DownloadHandlerScript
    {
        public string FileName { set; get; }
        public string URL { set; get; }
        public string FilePath { set; get; }
        public EDownloadState State { set; get; }

        private UnityWebRequest m_webReq;
        private BinaryWriter m_writer;

        public DownloadJob():base()
        {
            URL = "";
            FilePath = "";
            State = EDownloadState.Start;
            m_webReq = null;
            m_writer = null;
        }

        public void Start()
        {
            m_webReq = new UnityWebRequest(URL);
            m_webReq.downloadHandler = this;
            m_webReq.Send();
            State = EDownloadState.Downloading;
        }

        /// <summary>
        /// DownloadHandlerScript的isDone即使download結束仍回傳false，因此用new改寫其isDone的行為
        /// </summary>
        public new bool isDone
        {
            get
            {
                if(m_webReq == null)
                {
                    return true;
                }
                return m_webReq.isDone;
            }
        }

        public bool isError
        {
            get
            {
                if(m_webReq == null)
                {
                    return true;
                }
                return m_webReq.isError;
            }
        }

        public long responseCode
        {
            get
            {
                if (m_webReq == null)
                {
                    return -1;
                }
                return m_webReq.responseCode;
            }
        }

        public void SetError()
        {
            State = EDownloadState.Error;
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
            }
            catch(IOException e)
            {
                UnityDebugger.Debugger.LogError("File delete failed");
                UnityDebugger.Debugger.LogError(e.Message);
            }
        }

        protected override void ReceiveContentLength(int contentLength)
        {
            try
            {
                string[] strPathAry = FilePath.Split(new char[] { '/', '\\' });
                string tempPath = "";
                for (int i = 0; i < strPathAry.Length - 1; i++)
                {
                    tempPath = Path.Combine(tempPath, strPathAry[i]);
                }
                UnityDebugger.Debugger.Log("DownloadJob Combined path: " + tempPath);
                if (!string.IsNullOrEmpty(tempPath))
                {
                    if (!Directory.Exists(tempPath))
                    {
                        UnityDebugger.Debugger.Log("Create directory: " + tempPath);
                        Directory.CreateDirectory(tempPath);
                    }
                }
                Stream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                m_writer = new BinaryWriter(fs);
            }
            catch (IOException e)
            {
                UnityDebugger.Debugger.LogError(e.Message);
            }

            UnityDebugger.Debugger.Log("DownloadJob, responseCode = " + m_webReq.responseCode.ToString());
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if(m_writer == null)
            {
                return false;
            }
            m_writer.Write(data, 0, dataLength);
            return true;
        }

        protected override void CompleteContent()
        {
            if (m_writer != null)
            {
                m_writer.Flush();
                m_writer.Close();
                State = EDownloadState.Done;
            }
        }
    }
}
