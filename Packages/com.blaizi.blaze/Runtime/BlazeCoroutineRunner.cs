using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                return progress >= 1.0f;
            }
        }

        public void Step()
        {
            progress += Time.deltaTime / duration;
        }
    }

    public class CoroutineState
    {
        private readonly Stack<IEnumerator> m_Stack = new();

        public CoroutineState(IEnumerator root)
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
                        m_Stack.Push(nested);
                        break;

                    case IBlazeYieldInstruction wait:
                        if (wait.KeepWaiting)
                        {
                            wait.Step();
                            return true;
                        }
                        break;

                    case null:
                        return true;

                    default:
                        return true;
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
                    s_Instance = new GameObject("Blaze Coroutine Runner").AddComponent<BlazeCoroutineRunner>();
                }
                return s_Instance;
            }
        }

        private List<CoroutineState> m_Coroutines = new();

        public bool customUpdate = false;

        public void _Update()
        {
            for (int i = m_Coroutines.Count - 1; i >= 0; i--)
            {
                if (!m_Coroutines[i].MoveNext())
                {
                    m_Coroutines.RemoveAt(i);
                }
            }
        }

        public void Update()
        {
            if (!customUpdate)
            {
                _Update();
            }
        }

        public void Run(IEnumerator routine)
        {
            m_Coroutines.Add(new CoroutineState(routine));
        }

	    // #region WaitOnConditionThenExecute
	    // public static void WaitOnConditionThenExecute(System.Func<bool> condition, System.Action action)
	    // {
	    // 	Instance.StartWaitOnConditionThenExecute(condition, action);
	    // }

	    // public void StartWaitOnConditionThenExecute(System.Func<bool> condition, System.Action action)
	    // {
	    // 	StartCoroutine(DoWaitOnConditionThenExecute(condition, action));
	    // }

	    // private IEnumerator DoWaitOnConditionThenExecute(System.Func<bool> condition, System.Action action)
	    // {
	    // 	yield return new WaitUntil (() => condition() == true);
	    // 	action();
	    // }
	    // #endregion

        // #region WaitThenExecute
	    // public static void WaitThenExecute(float wait, System.Action action, bool unscaledTime = false)
        // {
	    // 	Instance.StartWaitThenExecute(wait, action, unscaledTime);
        // }

	    // public void StartWaitThenExecute(float wait, System.Action action, bool unscaledTime = false)
        // {
	    // 	StartCoroutine(DoWaitThenExecute(wait, action, unscaledTime));
        // }

	    // private IEnumerator DoWaitThenExecute(float wait, System.Action action, bool unscaledTime = false)
        // {
        //     if (wait <= 0f)
        //     {
        //         yield return new WaitForEndOfFrame();
        //     }
        //     else
        //     {
	    // 		if (unscaledTime)
	    // 		{
	    // 			yield return new WaitForSecondsRealtime (wait);
	    // 		}
	    // 		else
	    // 		{
	    // 			yield return new WaitForSeconds (wait);
	    // 		}
        //     }
        //     action();
        // }
        // #endregion

        // #region WaitOnCondition
        // public static Coroutine WaitOnCondition(System.Func<bool> condition)
        // {
        //     return Instance.StartWaitOnCondition(condition);
        // }

        // public Coroutine StartWaitOnCondition(System.Func<bool> condition)
        // {
        //     return StartCoroutine(DoWaitOnCondition(condition));
        // }

        // IEnumerator DoWaitOnCondition(System.Func<bool> condition)
        // {
        //     while (condition())
        //     {
        //        yield return new WaitForEndOfFrame();
        //     }
        // }
        // #endregion
    }
}
