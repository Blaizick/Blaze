using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Blaze.Runtime
{
    public class AnimatedStateSequence : ManagedBehaviour
    {
        [SerializeField]
        private State[] m_States = Array.Empty<State>();
        public virtual State[] States
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_States; 
            }
        }

        protected int m_CurStateId = 0;
        protected virtual int CurStateId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_CurStateId;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_CurStateId = value;
            }
        }

        [Serializable]
        public class State
        {
            [SerializeField]
            protected GameObject m_Root;
            public virtual GameObject Root
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return m_Root;
                }
            }
        }

        [NonSerialized]
        protected bool m_CustomUpdate;
        public virtual bool CustomUpdate
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_CustomUpdate;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_CustomUpdate = value;
            }
        }

        [SerializeField]
        protected float m_DelayBtwStateChange = 0.1f;
        public virtual float DelayBtwStateChange
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_DelayBtwStateChange;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_DelayBtwStateChange = value;
            }
        }

        protected float m_StateChangeProgress = 0.0f;
        public virtual float StateChangeProgress
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_StateChangeProgress;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_StateChangeProgress = value;
            }
        }

        [SerializeField]
        protected bool m_UseUnscaledDeltaTime = false;
        public bool UseUnscaledDeltaTime
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_UseUnscaledDeltaTime;
            }
        }

        protected bool m_CustomInit = false;
        public bool CustomInit
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_CustomInit;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_CustomInit = value;
            }
        }

        public virtual void Init()
        {
            for (int i = 0; i < States.Length; i++)
            {
                States[i].Root.SetActive(i == CurStateId);
            }
        }

        public virtual void Awake()
        {
            if (!CustomInit)
            {
                Init();
            }
        }

        public override void Update()
        {
            if (!CustomUpdate)
            {
                _Update();
            }
            base.Update();
        }

        public virtual void _Update()
        {
            if (States.Length > 0)
            {
                float dt = UseUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
                StateChangeProgress += dt / DelayBtwStateChange;

                if (StateChangeProgress >= 1.0f)
                {
                    States[CurStateId].Root.SetActive(false);

                    StateChangeProgress--;
                    
                    if (++CurStateId >= m_States.Length)
                    {
                        CurStateId = 0;                        
                    }
                
                    States[CurStateId].Root.SetActive(true);
                }
            }
        }
    }
}