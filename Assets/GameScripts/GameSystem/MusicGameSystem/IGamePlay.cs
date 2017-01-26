using UnityEngine;

namespace Softstar
{
    public interface IGamePlayParticleCreater
    {
        GameObject CreateParticleObject(EParticleType type);
        void ReleaseParticleObject(EParticleType type, GameObject go);
        void ShowParticle(EParticleType type, Transform tran);
        void ShowHitJudge(HitJudgeType type, Transform tran);
        void PlayNoteSoundEffect(EParticleType type);
        void CreateDoubleFadeOutObj(Vector3 vecA, Vector3 vecB, int depth);
    }

    public interface IGamePlayNoteCreater
    {
        GameObject CreateNote(NoteType type);
        void ReleaseNote(NoteType type, GameObject go);
    }
}
