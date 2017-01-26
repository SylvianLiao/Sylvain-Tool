using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar;

namespace Softstar
{
    public interface IGamePlayGUI
    {
        Transform transform { get;}
        float DragLineScaleOffset { get; }
        Dictionary<NoteType, GameObject> NoteBaseObjectMap { get; }
        Dictionary<EParticleType, GameObject> ParticleBaseObjectMap { get;}
        GameObject ComboBulletShot { get; set; }
        GameObject ComboBulletEffect { get; set; }
        GameObject ComboFinishShot { get; set; }
        Dictionary<ComboEffectLevel, GameObject> ComboEffectMap { get;}
        Dictionary<HitJudgeType, GameObject> HitJudgeBaseObjectMap { get;}
        Dictionary<NoteLineType, Material> NoteLineMaterialMap { get;}
        List<GameLine> GameLineList { get;}

        void InitializeHitPoint();
        void InitializeArcPosition();
        void InitializeNoteObject();
        void InitializeParticleObject();
    }
}
