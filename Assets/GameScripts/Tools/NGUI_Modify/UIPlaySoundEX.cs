using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

namespace Softstar
{
    [RequireComponent(typeof(UIButton))]
    public class UIPlaySoundEX : MonoBehaviour
    {
        public enum Trigger
        {
            OnClick,
            OnMouseOver,
            OnMouseOut,
            OnPress,
            OnDragStart,
            OnRelease,
            Custom,
            OnEnable,
            OnDisable,
        }

        public int m_soundID = 0;
        public Trigger m_trigger = Trigger.OnClick;

        public bool m_loop = false;
        public bool m_forcePlay = false;
        public float m_delay = 0.0f;

        private bool mIsOver = false;
        private bool canPlay
        {
            get
            {
                if (!enabled) return false;
                UIButton btn = GetComponent<UIButton>();
                return (btn == null || btn.isEnabled);
            }
        }

        private SoundSystem m_soundSystem;
        //---------------------------------------------------------------------------------------------------
        private void GetSoundSystem()
        {
            MusicApplication app = GameObject.FindObjectOfType<MusicApplication>();
            m_soundSystem = app.APP.GetSystem<SoundSystem>();
        } 
        //---------------------------------------------------------------------------------------------------
        void OnEnable()
        {
            if (m_trigger == Trigger.OnEnable)
                Play();
        }
        //---------------------------------------------------------------------------------------------------
        void OnDisable()
        {
            if (m_trigger == Trigger.OnDisable)
                Play();
        }
        //---------------------------------------------------------------------------------------------------
        void OnHover(bool isOver)
        {
            if (m_trigger == Trigger.OnMouseOver)
            {
                if (mIsOver == isOver) return;
                mIsOver = isOver;
            }

            if (canPlay && ((isOver && m_trigger == Trigger.OnMouseOver) || (!isOver && m_trigger == Trigger.OnMouseOut)))
                Play();
        }
        //---------------------------------------------------------------------------------------------------
        void OnPress(bool isPressed)
        {
            if (m_trigger == Trigger.OnPress)
            {
                if (mIsOver == isPressed) return;
                mIsOver = isPressed;
            }

            if (canPlay && ((isPressed && m_trigger == Trigger.OnPress) || (!isPressed && m_trigger == Trigger.OnRelease)))
                Play();
        }
        //---------------------------------------------------------------------------------------------------
        void OnDragStart()
        {
            if (canPlay && m_trigger == Trigger.OnDragStart)
            {
                Play();
            }             
        }
        //---------------------------------------------------------------------------------------------------
        void OnClick()
        {
            if (canPlay && m_trigger == Trigger.OnClick)
                Play();
        }
        //---------------------------------------------------------------------------------------------------
        void OnSelect(bool isSelected)
        {
            if (canPlay && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
                OnHover(isSelected);
        }
        //---------------------------------------------------------------------------------------------------
        public void Play()
        {
            if (m_soundSystem == null)
            {
                try
                {
                    GetSoundSystem();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                    return;
                }
            }

            if (m_loop)
                m_soundSystem.PlayLoopSound(m_soundID, m_forcePlay, m_delay);
            else
                m_soundSystem.PlaySound(m_soundID, m_delay);
        }
    }

}

