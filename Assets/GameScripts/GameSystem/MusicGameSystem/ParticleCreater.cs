using UnityEngine;
using System.Collections;
using Softstar;

public class ParticleCreater
{
    //public GamePlayState gamePlayState { get; set; }
    delegate GameObject create(EParticleType type);
    create m_Create;

    delegate void release(EParticleType type, GameObject go);
    release m_Release;

    delegate void showParticle(EParticleType type, Transform tran);
    showParticle m_ShowParticle;

    delegate void showMissEffect(HitJudgeType hitJudgeType, Transform tran);
    showMissEffect m_ShowMissEffect;

    delegate void playSoundEffect(EParticleType type);
    playSoundEffect m_PlaySoundEffect;

    delegate void createDoubleFadeOut(Vector3 vecA, Vector3 vecB, int depth);
    createDoubleFadeOut m_CreateDoubleFadeOut;

    public ParticleCreater()
    {
    }

    public GameObject Create(EParticleType type)
    {
        return m_Create(type);
    }

    public void Release(EParticleType type, GameObject go)
    {
        m_Release(type, go);
    }

    public void ShowParticle(EParticleType type, Transform tran)
    {
        m_ShowParticle(type, tran);
    }

    public void ShowHitJudge(HitJudgeType type, Transform tran)
    {
        m_ShowMissEffect(type, tran);
    }

    public void ShowMissEffect(Transform tran)
    {
        m_ShowMissEffect(HitJudgeType.Miss, tran);
    }

    public void PlayNoteSoundEffect(EParticleType type)
    {
        m_PlaySoundEffect(type);
    }

    public void CreateDoubleFadeOut(Vector3 vecA, Vector3 vecB, int depth)
    {
        m_CreateDoubleFadeOut(vecA, vecB,depth);
    }

    public void SetState(IGamePlayParticleCreater obj)
    {
        m_Create = obj.CreateParticleObject;
        m_Release = obj.ReleaseParticleObject;
        m_ShowParticle = obj.ShowParticle;
        m_ShowMissEffect = obj.ShowHitJudge;
        m_PlaySoundEffect = obj.PlayNoteSoundEffect;
        m_CreateDoubleFadeOut = obj.CreateDoubleFadeOutObj;
    }
}
