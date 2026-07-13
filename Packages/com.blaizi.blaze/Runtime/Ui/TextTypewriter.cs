using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blaze.Runtime.Ui
{
    public class TextTypewriter : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
    {
        public TMP_Text targetText;

        public float charsPerSec = 120f;

        public Coroutine writeCoroutine;
        public Coroutine coroutine;
        private string m_WritingText;
        private bool m_InterruptRequested;

        public bool skipOnClick;

        private bool m_Clicked;
        private int m_CurCharId;
        private bool m_Writing;
        private bool m_WaitingForClick;
        private float m_WaitForClickStartTime;
        
        public bool Writing => m_Writing;
        public bool WaitingForClick => m_WaitingForClick;
        public float WaitingForClickTime => Time.time - m_WaitForClickStartTime;

        public string Text
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (m_Writing)
                {
                    return m_WritingText.Substring(m_CurCharId);
                }
                return m_WritingText;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (m_Writing)
                {
                    Interrupt();
                }
                targetText.text = value;
            }
        }

        public IEnumerator WaitForClick()
        {
            m_WaitForClickStartTime = Time.time;
            m_WaitingForClick = true;
            m_Clicked = false;
            while (!m_Clicked)
            {
                yield return null;
            }
            m_WaitingForClick = false;
        }

        public Coroutine WriteCoroutine(string text)
        {
            if (writeCoroutine != null)
            {
                StopCoroutine(writeCoroutine);
            }

            writeCoroutine = StartCoroutine(_WriteCoroutine(text));
            return writeCoroutine;
        }

        private IEnumerator _WriteCoroutine(string text)
        {
            m_InterruptRequested = false;
            m_WritingText = text;
            targetText.text = text;
            int c = 0;
            while (c < text.Length)
            {
                if (m_InterruptRequested)
                {
                    m_InterruptRequested = false;
                    break;
                }

                c = (targetText.maxVisibleCharacters = c + 1);
                yield return new WaitForSeconds(1f / charsPerSec);
            }
        }

        public void Skip()
        {
            Interrupt();
        }


        public void Interrupt()
        {
            if (writeCoroutine != null)
            {
                if (m_WaitingForClick)
                {
                    m_Clicked = true;
                }
                m_Writing = false;
                coroutine = null;
                m_WaitingForClick = false;
                targetText.text = m_WritingText;
                targetText.maxVisibleCharacters = int.MaxValue;
                m_InterruptRequested = true;
                writeCoroutine = null;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            m_Clicked = true;
            if (skipOnClick)
            {
                Skip();
            }
        }
    }
}