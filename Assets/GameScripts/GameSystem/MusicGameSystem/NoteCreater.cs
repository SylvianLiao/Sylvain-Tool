using UnityEngine;
using System.Collections;
using Softstar;

public class NoteCreater
{
    private MainApplication m_mainapp;
    public MainApplication mainapp { get { return m_mainapp; } }

    delegate GameObject CreateNote(NoteType type);
    CreateNote m_CreateNote;

    delegate void ReleaseNote(NoteType type, GameObject go);
    ReleaseNote m_ReleaseNote;

    public NoteCreater(MainApplication mainapp)
    {
        m_mainapp = mainapp;
    }

    public GameObject createNote(NoteType type)
    {
        return m_CreateNote(type);
    }

    public void releaseNote(NoteType type, GameObject go)
    {
        m_ReleaseNote(type, go);
    }

    public void SetState(IGamePlayNoteCreater obj)
    {
        m_CreateNote = obj.CreateNote;
        m_ReleaseNote = obj.ReleaseNote;
    }
}
