using UnityEngine;
using Mono.Data.Sqlite;

namespace Softstar
{
    public class FileHashDatabase
    {
        private string m_strDbName;
        private string m_strUri;
        private SqliteConnection m_connection;

        public const string FILE_HASH_TABLE = "file_hash";
        public const string COL_SHA1 = "sha1_hash";
        public const string COL_PATH = "file_path";
        public const string COL_LEN = "file_length";

        public FileHashDatabase(string dbName = "filehash.db")
        {
            m_strDbName = dbName;
            m_strUri = Application.persistentDataPath + "/" + m_strDbName;
        }

        public void Connect()
        {
            UnityDebugger.Debugger.Log("Connect SQLite DB = " + m_strUri);
            m_connection = new SqliteConnection("data source=" + m_strUri);
            m_connection.Open();
        }

        public void CreateTable()
        {
            string strQuery = string.Format("CREATE TABLE IF NOT EXISTS {0}({1} VARCHAR NOT NULL, {2} VARCHAR NOT NULL, {3} INTEGER NOT NULL, CONSTRAINT key_constraint PRIMARY KEY({1}, {2}))",
                FileHashDatabase.FILE_HASH_TABLE,
                FileHashDatabase.COL_SHA1, FileHashDatabase.COL_PATH, FileHashDatabase.COL_LEN);

            int iResult = ExecuteNoneQuery(strQuery);
            UnityDebugger.Debugger.Log("Create Table result: " + iResult);

            CreateTableIndex();
        }

        public void CreateTableIndex()
        {
            string strQuery = string.Format("CREATE INDEX IF NOT EXISTS index_file ON {0}({1})", FileHashDatabase.FILE_HASH_TABLE, FileHashDatabase.COL_PATH);
            int iResult = ExecuteNoneQuery(strQuery);
            UnityDebugger.Debugger.Log("Create Table index result: " + iResult);
        }

        public void Truncate()
        {
            string strQuery = "DROP TABLE " + FileHashDatabase.FILE_HASH_TABLE;

            int iResult = ExecuteNoneQuery(strQuery);
            UnityDebugger.Debugger.Log("Truncate Table result: " + iResult);

            CreateTable();
        }

        public void Insert(string sha1, string path, int len)
        {
            string strQuery = string.Format("INSERT INTO {0}({1}, {2}, {3}) VALUES('{4}', '{5}', {6})",
                FileHashDatabase.FILE_HASH_TABLE,
                FileHashDatabase.COL_SHA1, FileHashDatabase.COL_PATH, FileHashDatabase.COL_LEN,
                sha1, path, len);

            int iResult = ExecuteNoneQuery(strQuery);
            UnityDebugger.Debugger.Log("Insert Table result: " + iResult);
        }

        public void RemoveData(string path)
        {
            string strQuery = string.Format("DELETE FROM {0} WHERE {1}='{2}'",
                FileHashDatabase.FILE_HASH_TABLE,
                FileHashDatabase.COL_PATH, path);
            int iResult = ExecuteNoneQuery(strQuery);
            UnityDebugger.Debugger.Log("RemoveData result: " + iResult);
        }

        public FileHash GetData(string sha1, string path)
        {
            string strQuery = string.Format("SELECT {0}, {1}, {2} FROM {3} WHERE {0}='{4}' AND {1}='{5}'",
                FileHashDatabase.COL_SHA1, FileHashDatabase.COL_PATH, FileHashDatabase.COL_LEN,
                FileHashDatabase.FILE_HASH_TABLE,
                sha1, path);

            SqliteDataReader reader = ExecuteReader(strQuery);
            FileHash fh = null;
            while (reader.Read())
            {
                fh = new FileHash();
                fh.SHA1 = reader.GetString(0);
                fh.Path = reader.GetString(1);
                fh.Length = reader.GetInt32(2);
                UnityDebugger.Debugger.Log(string.Format("SHA1[{0}] path[{1}] length[{2}]",
                    fh.SHA1, fh.Path, fh.Length));
            }
            reader.Close();

            if(fh != null)
            {
                return fh;
            }
            return null;
        }

        public void Close()
        {
            if (m_connection != null)
            {
                m_connection.Close();
            }
        }

        public int ExecuteNoneQuery(string query)
        {
            UnityDebugger.Debugger.Log("ExecuteNoneQuery : " + query);
            SqliteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = query;
            int iResult = cmd.ExecuteNonQuery();
            return iResult;
        }

        public SqliteDataReader ExecuteReader(string query)
        {
            UnityDebugger.Debugger.Log("ExecuteReader : " + query);
            SqliteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = query;
            SqliteDataReader reader = cmd.ExecuteReader();
            return reader;
        }
    }
}
