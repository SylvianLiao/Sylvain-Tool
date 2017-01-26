using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum Enum_ClosedPos
{
    Myself = 0,
    Right,
    Left
}
public class Slot_Song : MonoBehaviour
{
    private UI_3D_SongMenu m_ui3DSongMenu;
    private MeshRenderer m_mesh;
    public int m_iSongGroup;

    [Header("Swipe Value")]
    public int m_iCurrentIndex;
    [HideInInspector]
    public Vector3 m_CatchedPos;
    [HideInInspector]
    public Vector3 m_CatchedRotate;
    private Enum_ClosedPos m_ClosedPos;
    private float m_ResetTime;
    private float m_SwipeSpeed;

    [Header("Lock Material")]
    public GameObject m_loadingMaterial;
    public GameObject m_lockMaterial;
    public GameObject m_tempLockMaterial;
    public MeshRenderer m_lockBtnMesh;

    //Runtime Data
    private AsyncLoadOperation m_songMaterialLoader;
    //-------------------------------------------------------------------------------------------------
    public void Initialize(UI_3D_SongMenu uiSongMenu, SwipeController controller, int index, int group)
    {
        m_ui3DSongMenu = uiSongMenu;
        m_iSongGroup = group;
        m_mesh = this.GetComponent<MeshRenderer>();

        m_iCurrentIndex = index;
        m_CatchedPos = this.transform.localPosition;
        m_CatchedRotate = this.transform.localEulerAngles;
        m_ResetTime = uiSongMenu.m_ResetTime;
        m_SwipeSpeed = uiSongMenu.m_SwipeSpeed;

        controller.RegisterNotifyCatched(this.gameObject.name, ActionCatched);
        controller.RegisterNotifyMoving(this.gameObject.name, CatchedMoving);

        m_loadingMaterial.gameObject.SetActive(false);
    }
    //-------------------------------------------------------------------------------------------------
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    //-------------------------------------------------------------------------------------------------
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    //-------------------------------------------------------------------------------------------------
    private void ActionCatched(bool isCatched)
    {
        //如果抓取目標不是HighLight目標則停止
        if (m_iCurrentIndex != 0)
        {
            return;
        }
        //釋放時物件彈回相對應的位置
        if (isCatched)
        {
            m_CatchedPos = this.transform.localPosition;
            m_CatchedRotate = this.transform.localEulerAngles;
        }
        else
        {
            CatchedMovedEnd();
        }
        //UnityDebugger.Debugger.Log("Song["+ m_iCurrentIndex + "] Catch = "+ isCatched);
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>做為抓取對象進行移動</summary>
    private void CatchedMoving(Vector3 subtractVec)
    {
        if (m_iCurrentIndex != 0)
        {
            //UnityDebugger.Debugger.Log("CatchedMoving Fail! m_iCurrentIndex = "+ m_iCurrentIndex);
            return;
        }

        Vector3 targetPos;
        bool getPos = subtractVec.x < 0 ?
           m_ui3DSongMenu.GetNextLeftPos(m_iCurrentIndex, out targetPos) :
           m_ui3DSongMenu.GetNextRightPos(m_iCurrentIndex, out targetPos);

        Vector3 targetRotate;
        bool getRotate = subtractVec.x < 0 ?
           m_ui3DSongMenu.GetNextLeftRotate(m_iCurrentIndex, out targetRotate) :
           m_ui3DSongMenu.GetNextRightRotate(m_iCurrentIndex, out targetRotate);

        if (getPos && getRotate)
        {
            //計算X方向
            float shouldDis = Mathf.Abs(targetPos.x - m_CatchedPos.x);
            float mayBeMovedToDis = Mathf.Abs(subtractVec.x) * m_SwipeSpeed;
            //計算Z方向
            float ratio = mayBeMovedToDis / shouldDis;
            float zDis = ratio * Mathf.Abs(targetPos.z - m_CatchedPos.z);

            float shouldRotate = targetRotate.y - m_CatchedRotate.y;
            if (shouldRotate > 180) shouldRotate -= 180;
            if (shouldRotate < -180) shouldRotate += 180;
            float yRotate = ratio * shouldRotate;

            if (mayBeMovedToDis > shouldDis)
            {
                this.transform.localPosition = new Vector3(targetPos.x, 0, targetPos.z);
                this.transform.localEulerAngles = targetRotate;
            }
            else
            {
                //計算應彈回的目標
                m_ClosedPos = subtractVec.x < 0 ? Enum_ClosedPos.Left : Enum_ClosedPos.Right;
                /*
                if (mayBeMovedToDis >= shouldDis / 2)
                {
                    m_ClosedPos = subtractVec.x < 0 ? Enum_ClosedPos.Left : Enum_ClosedPos.Right;
                }
                else
                {
                    m_ClosedPos = Enum_ClosedPos.Myself;
                }
                */
                subtractVec.x *= m_SwipeSpeed;

                //UnityDebugger.Debugger.Log("Swiping: mayBeMovedTo =" + new Vector3((subtractVec.x + m_CatchedPos.x), 0, (zDis + m_CatchedPos.z)));
                this.transform.localPosition = new Vector3((subtractVec.x + m_CatchedPos.x), 0, (zDis + m_CatchedPos.z));
                this.transform.localEulerAngles = new Vector3(m_CatchedRotate.x, m_CatchedRotate.y + yRotate, m_CatchedRotate.z);
                //UnityDebugger.Debugger.Log("Rotate ratio = " + ratio + ", mayBeRotate =" + yRotate + ", shouldRotate = " + shouldRotate);
            }

            //通知其他物件跟著移動
            float vaildRatio = ratio > 1 ? 1 : ratio;
            m_ui3DSongMenu.NotifyMoving(this, vaildRatio, subtractVec.x < 0);
        }
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>不是抓取對象時的移動</summary>
    public void NotCatchedMoving(float mvRatio, bool isLeft)
    {
        Vector3 targetPos;
        bool getPos = isLeft ? m_ui3DSongMenu.GetNextLeftPos(m_iCurrentIndex, out targetPos) :
            m_ui3DSongMenu.GetNextRightPos(m_iCurrentIndex, out targetPos);
        Vector3 myPos;
        getPos = m_ui3DSongMenu.GetSpecifiedIndexPos(m_iCurrentIndex, out myPos);

        Vector3 targetRotate;
        bool getRotate = isLeft ? m_ui3DSongMenu.GetNextLeftRotate(m_iCurrentIndex, out targetRotate) :
            m_ui3DSongMenu.GetNextRightRotate(m_iCurrentIndex, out targetRotate);
        Vector3 myRotate;
        getPos = m_ui3DSongMenu.GetSpecifiedIndexRotate(m_iCurrentIndex, out myRotate);

        if (!getPos || !getRotate)
        {
            return;
        }

        float xDis = targetPos.x - myPos.x;
        float zDis = targetPos.z - myPos.z;
        this.transform.localPosition = new Vector3(myPos.x + xDis * mvRatio, 0, myPos.z + zDis * mvRatio);

        float yRotate = targetRotate.y - myRotate.y;
        this.transform.localEulerAngles = new Vector3(myRotate.x, myRotate.y + (yRotate * mvRatio), myRotate.z);
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>作為抓取對象時的釋放</summary>
    private void CatchedMovedEnd()
    {
        Vector3 rightPos;
        bool getPos = m_ui3DSongMenu.GetNextRightPos(m_iCurrentIndex, out rightPos);
        Vector3 leftPos;
        getPos = m_ui3DSongMenu.GetNextLeftPos(m_iCurrentIndex, out leftPos);

        Vector3 rightRotate;
        bool getRotate = m_ui3DSongMenu.GetNextRightRotate(m_iCurrentIndex, out rightRotate);
        Vector3 leftRotate;
        getPos = m_ui3DSongMenu.GetNextLeftRotate(m_iCurrentIndex, out leftRotate);

        if (getPos && getRotate)
        {
            switch (m_ClosedPos)
            {
                case Enum_ClosedPos.Myself:
                    this.transform.DOLocalMove(m_CatchedPos, m_ResetTime);
                    this.transform.DOLocalRotate(m_CatchedRotate, m_ResetTime);
                    m_ui3DSongMenu.NotifyMovedEnd(this, Enum_ClosedPos.Myself);
                    break;
                case Enum_ClosedPos.Right:
                    this.transform.DOLocalMove(rightPos, m_ResetTime);
                    this.transform.DOLocalRotate(rightRotate, m_ResetTime);
                    m_iCurrentIndex++;
                    m_ui3DSongMenu.NotifyMovedEnd(this, Enum_ClosedPos.Right);
                    break;
                case Enum_ClosedPos.Left:
                    this.transform.DOLocalMove(leftPos, m_ResetTime);
                    this.transform.DOLocalRotate(leftRotate, m_ResetTime);
                    m_iCurrentIndex--;
                    m_ui3DSongMenu.NotifyMovedEnd(this, Enum_ClosedPos.Left);
                    break;
                default:
                    break;
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>不是抓取對象時的釋放</summary>
    public void NotCatchedMovedEnd(Enum_ClosedPos endPos)
    {
        Vector3 rightPos;
        bool getPos = m_ui3DSongMenu.GetNextRightPos(m_iCurrentIndex, out rightPos);
        Vector3 leftPos;
        getPos = m_ui3DSongMenu.GetNextLeftPos(m_iCurrentIndex, out leftPos);
        Vector3 myPos;
        getPos = m_ui3DSongMenu.GetSpecifiedIndexPos(m_iCurrentIndex, out myPos);

        Vector3 rightRotate;
        bool getRotate = m_ui3DSongMenu.GetNextRightRotate(m_iCurrentIndex, out rightRotate);
        Vector3 leftRotate;
        getPos = m_ui3DSongMenu.GetNextLeftRotate(m_iCurrentIndex, out leftRotate);
        Vector3 myRotate;
        getPos = m_ui3DSongMenu.GetSpecifiedIndexRotate(m_iCurrentIndex, out myRotate);
        if (getPos && getRotate)
        {
            switch (endPos)
            {
                case Enum_ClosedPos.Myself:
                    this.transform.DOLocalMove(myPos, m_ResetTime);
                    this.transform.DOLocalRotate(myRotate, m_ResetTime);
                    break;
                case Enum_ClosedPos.Right:
                    this.transform.DOLocalMove(rightPos, m_ResetTime);
                    this.transform.DOLocalRotate(rightRotate, m_ResetTime);
                    m_iCurrentIndex = m_ui3DSongMenu.GetNextRightIndex(m_iCurrentIndex);
                    break;
                case Enum_ClosedPos.Left:
                    this.transform.DOLocalMove(leftPos, m_ResetTime);
                    this.transform.DOLocalRotate(leftRotate, m_ResetTime);
                    m_iCurrentIndex = m_ui3DSongMenu.GetNextLeftIndex(m_iCurrentIndex);
                    break;
                default:
                    break;
            }
        }
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>抓取結束後更新資料(包含歌曲選單材質貼圖、歌曲GroupID)</summary>
    public void UpdateDataAfterCathed(PlayerDataSystem dataSystem, ResourceManager rm)
    {
        //只有第一個和最後一個歌曲選單頁面需要更新
        if (m_iCurrentIndex == m_ui3DSongMenu.m_firstSongIndex)
        {
            Slot_Song nextSong;
            if (m_ui3DSongMenu.GetSpecifiedSlotSong(m_iCurrentIndex + 1, out nextSong))
            {
                SongData songData = dataSystem.GetPreSongData(nextSong.m_iSongGroup);
                //若需更新資料
                if (songData.SongGroupID != m_iSongGroup)
                {
                    UpdateData(rm, dataSystem, songData);
                }
            }

            Hide();
        }
        else if (m_iCurrentIndex == m_ui3DSongMenu.m_lastSongIndex)
        {
            Slot_Song preSong;
            if (m_ui3DSongMenu.GetSpecifiedSlotSong(m_iCurrentIndex - 1, out preSong))
            {
                SongData songData = dataSystem.GetNextSongData(preSong.m_iSongGroup);
                //若需更新資料
                if (songData.SongGroupID != m_iSongGroup)
                {
                    UpdateData(rm, dataSystem, songData);
                }
            }

            Hide();
        }
        else
            Show();
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>根據給予的歌曲資料更新頁面(非同步方式讀取資源)</summary>
    public void UpdateData(ResourceManager rm, PlayerDataSystem dataSystem, SongData songData)
    {
        m_loadingMaterial.gameObject.SetActive(true);

        if (m_songMaterialLoader != null)
            m_songMaterialLoader.CancelLoad();


        string matPath = (songData.LockStatus == Enum_SongLockStatus.WaitForUnlock)? dataSystem.GetSongBgResourcePath(songData) : dataSystem.GetSongMaterialResourcePath(songData);
        AsyncLoadOperation loader = rm.GetResourceASync(    Enum_ResourcesType.Material,
                                                            matPath,
                                                            typeof(Material),
                                                            WaitForLoadingSongMaterial);
        m_songMaterialLoader = loader;

        SwitchLockMaterial(songData.LockStatus);

        m_iSongGroup = songData.SongGroupID;
    }
    //-------------------------------------------------------------------------------------------------
    private void WaitForLoadingSongMaterial(AsyncLoadOperation loader)
    {
        if (this == null)
            return;

        if (loader.m_assetObject != null)
        {
            //Set Material
            Material mat = loader.m_assetObject as Material;
            m_mesh.material = mat;
            m_loadingMaterial.gameObject.SetActive(false);
        }
        else
        {
            UnityDebugger.Debugger.LogError("Load Song Material [" + loader.m_strAssetName + "] Failed !");
        }

        this.gameObject.SetActive(true);
        m_songMaterialLoader = null;
    }
    //-------------------------------------------------------------------------------------------------
    public void SwitchLockMaterial(Enum_SongLockStatus lockStatus)
    {
        switch (lockStatus)
        {
            case Enum_SongLockStatus.Lock:
                m_lockMaterial.SetActive(true);
                m_tempLockMaterial.SetActive(false);
                Softstar.Utility.ChangeMaterial(m_lockBtnMesh, 72);
                break;
            case Enum_SongLockStatus.WaitForUnlock:
                m_lockMaterial.SetActive(true);
                m_tempLockMaterial.SetActive(false);
                Softstar.Utility.ChangeMaterial(m_lockBtnMesh, 71);
                break;
            case Enum_SongLockStatus.Unlock:
                m_lockMaterial.SetActive(false);
                m_tempLockMaterial.SetActive(false);
                break;
            default:
                m_lockMaterial.SetActive(true);
                m_tempLockMaterial.SetActive(false);
                Softstar.Utility.ChangeMaterial(m_lockBtnMesh, 72);
                break;
        }
     
    }
}
