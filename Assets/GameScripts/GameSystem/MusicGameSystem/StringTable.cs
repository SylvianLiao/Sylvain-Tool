using UnityEngine;

namespace Softstar
{
    public class StringTable
    {
        public ENUM_LanguageType Language { set; get; }
        public T_GameDB<S_StringTable_Tmp> StringDB { set; get; }

        public StringTable(ENUM_LanguageType lang, T_GameDB<S_StringTable_Tmp> strDB)
        {
            Language = lang;
            StringDB = strDB;
        }

        public string GetString(int id)
        {
            string strRet;
            try
            {
                S_StringTable_Tmp strTmp = StringDB.GetData(id);
                switch (Language)
                {
                    default:
                    case ENUM_LanguageType.English:
                        strRet = strTmp.strENG_US;
                        break;
                    case ENUM_LanguageType.TraditionalChinese:
                        strRet = strTmp.strZH_TW;
                        break;
                    case ENUM_LanguageType.SimplifiedChinese:
                        strRet = strTmp.strZH_CN;
                        break;
                }
            }
            catch(System.Exception e)
            {
                UnityDebugger.Debugger.LogError(e.Message);
                strRet = "Null Reference";
            }
            return strRet;
        }
    }
}
