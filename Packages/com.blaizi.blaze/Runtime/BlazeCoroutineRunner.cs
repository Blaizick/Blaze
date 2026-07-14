using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blaze.Runtime 
{
    public interface IBlazeYieldInstruction
    {
        public bool KeepWaiting { get; }
        public void Step();
    }

    public class BlazeWaitForSeconds : IBlazeYieldInstruction
    {
        public float progress = 0.0f;
        public float duration = 0.0f;

        public BlazeWaitForSeconds(float seconds)
        {
            this.duration = seconds;
        }
        
        public bool KeepWaiting
        {
            get
            {
                return progress < 1.0f;
            }
        }

        public void Step()
        {
            progress += Time.deltaTime / duration;
        }
    }

    public class BlazeCoroutine
    {
        private readonly Stack<IEnumerator> m_Stack = new();

        public BlazeCoroutine(IEnumerator root)
        {
            m_Stack.Push(root);
        }

        public bool MoveNext()
        {
            while (m_Stack.Count > 0)
            {
                var current = m_Stack.Peek();

                if (!current.MoveNext())
                {
                    m_Stack.Pop();
                    continue;
                }

                switch (current.Current)
                {
                    case IEnumerator nested:
                    {
                        m_Stack.Push(nested);
                        continue;
                    }
                    case IBlazeYieldInstruction wait:
                    {
                        wait.Step();

                        if (wait.KeepWaiting)
                        {
                            return true;
                        }

                        continue;
                    }
                    default:
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class BlazeCoroutineRunner : MonoBehaviour
    {
        private static BlazeCoroutineRunner s_Instance = null;
        public static BlazeCoroutineRunner Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    if (!s_InstanceCreated)
                    {
                        var go = new GameObject("Blaze Coroutine Runner");
                        s_Instance = go.AddComponent<BlazeCoroutineRunner>();
                        s_Instance.Init();
                        DontDestroyOnLoad(go);
                    }
                }
                return s_Instance;
            }
        }
        private static bool s_InstanceCreated = false;

        private List<BlazeCoroutine> m_Coroutines = new();
        private List<BlazeCoroutine> m_PausedCoroutines = new();

        public static bool customUpdate = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetStatistics()
        {
            s_Instance = null;
            s_InstanceCreated = false;
        }

        public virtual void Init()
        {
            SceneManager.sceneUnloaded += scene =>
            {
                BStopAllCoroutines();
            };
        }

        public virtual void BUpdate()
        {
            for (int i = m_Coroutines.Count - 1; i >= 0; i--)
            {
                if (!m_Coroutines[i].MoveNext())
                {
                    m_Coroutines.RemoveAt(i);
                }
            }
        }

        public virtual void Update()
        {
            if (!customUpdate)
            {
                BUpdate();
            }
        }

        #region BaseMethods

        public virtual BlazeCoroutine BRunCoroutine(IEnumerator routine)
        {
            var coroutine = new BlazeCoroutine(routine);
            m_Coroutines.Add(coroutine);
            return coroutine;
        }
        
        public virtual void BPauseCoroutine(BlazeCoroutine coroutine)
        {
            int coroutineIndex = m_Coroutines.IndexOf(coroutine);
            if (!m_PausedCoroutines.Contains(coroutine) && 
                coroutineIndex > -1)
            {
                m_Coroutines.RemoveAt(coroutineIndex);
                m_PausedCoroutines.Add(coroutine);
            }
        }

        public virtual void BUnpauseCoroutine(BlazeCoroutine coroutine)
        {
            var pausedIndex = m_PausedCoroutines.IndexOf(coroutine);
            if (pausedIndex > -1 && 
                !m_Coroutines.Contains(coroutine))
            {
                m_PausedCoroutines.RemoveAt(pausedIndex);
                m_Coroutines.Add(coroutine);
            }
        }

        public virtual void BStopCoroutine(BlazeCoroutine coroutine)
        {
            int coroutineIndex = m_Coroutines.IndexOf(coroutine);
            if (coroutineIndex > -1)
            {
                m_Coroutines.RemoveAt(coroutineIndex);
            }
            else
            {
                int pausedIndex = m_PausedCoroutines.IndexOf(coroutine);
                if (pausedIndex > -1)
                {
                    m_PausedCoroutines.RemoveAt(pausedIndex);
                }
            }
        }

        public virtual void BStopAllCoroutines()
        {
            m_Coroutines.Clear();
            m_PausedCoroutines.Clear();
        }

        #endregion

	    #region WaitOnConditionThenExecute
	    public static void WaitOnConditionThenExecute(System.Func<bool> condition, System.Action action)
	    {
	    	Instance.StartWaitOnConditionThenExecute(condition, action);
	    }

	    public virtual void StartWaitOnConditionThenExecute(System.Func<bool> condition, System.Action action)
	    {
	    	StartCoroutine(DoWaitOnConditionThenExecute(condition, action));
	    }

	    private IEnumerator DoWaitOnConditionThenExecute(System.Func<bool> condition, System.Action action)
	    {
	    	yield return new WaitUntil (() => condition() == true);
	    	action();
	    }
	    #endregion

        #region WaitThenExecute
	    public static void WaitThenExecute(float wait, System.Action action, bool unscaledTime = false)
        {
	    	Instance.StartWaitThenExecute(wait, action, unscaledTime);
        }

	    public virtual void StartWaitThenExecute(float wait, System.Action action, bool unscaledTime = false)
        {
	    	StartCoroutine(DoWaitThenExecute(wait, action, unscaledTime));
        }

	    private IEnumerator DoWaitThenExecute(float wait, System.Action action, bool unscaledTime = false)
        {
            if (wait <= 0f)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
	    		if (unscaledTime)
	    		{
	    			yield return new WaitForSecondsRealtime (wait);
	    		}
	    		else
	    		{
	    			yield return new WaitForSeconds (wait);
	    		}
            }
            action();
        }
        #endregion

        #region WaitOnCondition
        public static BlazeCoroutine WaitOnCondition(System.Func<bool> condition)
        {
            return Instance.StartWaitOnCondition(condition);
        }

        public virtual BlazeCoroutine StartWaitOnCondition(System.Func<bool> condition)
        {
            return BRunCoroutine(DoWaitOnCondition(condition));
        }

        IEnumerator DoWaitOnCondition(System.Func<bool> condition)
        {
            while (condition())
            {
               yield return new WaitForEndOfFrame();
            }
        }
        #endregion
    }
}