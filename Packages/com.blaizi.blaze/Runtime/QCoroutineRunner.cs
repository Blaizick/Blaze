using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blaze.Runtime 
{
    public interface IQYieldInstruction
    {
        public bool KeepWaiting { get; }
        public void Step();
    }

    public class QWaitForSeconds : IQYieldInstruction
    {
        public float progress = 0.0f;
        public float duration = 0.0f;

        public QWaitForSeconds(float seconds)
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

    public class QCoroutine
    {
        private readonly Stack<IEnumerator> m_Stack = new();
        public object _object;

        public QCoroutine(IEnumerator root, object _object = null)
        {
            m_Stack.Push(root);
            this._object = _object;
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
                    case IQYieldInstruction wait:
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

    public class QCoroutineRunnerBase
    {
        private List<QCoroutine> m_Coroutines = new();
        private List<QCoroutine> m_PausedCoroutines = new();

        public virtual void QInit()
        {
            SceneManager.sceneUnloaded += scene =>
            {
                QStopAllCoroutines();
            };
        }

        public virtual void QUpdate()
        {
            for (int i = m_Coroutines.Count - 1; i >= 0; i--)
            {
                if (!m_Coroutines[i].MoveNext())
                {
                    m_Coroutines.RemoveAt(i);
                }
            }
        }

        #region BaseMethods

        public virtual QCoroutine QStartCoroutine(IEnumerator routine)
        {
            var coroutine = new QCoroutine(routine);
            m_Coroutines.Add(coroutine);
            return coroutine;
        }

        public virtual void QPauseCoroutine(QCoroutine coroutine)
        {
            int coroutineIndex = m_Coroutines.IndexOf(coroutine);
            if (!m_PausedCoroutines.Contains(coroutine) && 
                coroutineIndex > -1)
            {
                m_Coroutines.RemoveAt(coroutineIndex);
                m_PausedCoroutines.Add(coroutine);
            }
        }

        public virtual void QUnpauseCoroutine(QCoroutine coroutine)
        {
            var pausedIndex = m_PausedCoroutines.IndexOf(coroutine);
            if (pausedIndex > -1 && 
                !m_Coroutines.Contains(coroutine))
            {
                m_PausedCoroutines.RemoveAt(pausedIndex);
                m_Coroutines.Add(coroutine);
            }
        }
        public virtual void QStopCoroutine(QCoroutine coroutine)
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

        public virtual void QStopAllCoroutines()
        {
            m_Coroutines.Clear();
            m_PausedCoroutines.Clear();
        }

        #endregion
    }

    public class QCoroutineRunner : Singleton<QCoroutineRunner>
    {
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
        public static QCoroutine WaitOnCondition(System.Func<bool> condition)
        {
            return Instance.StartWaitOnCondition(condition);
        }

        public virtual QCoroutine StartWaitOnCondition(System.Func<bool> condition)
        {
            return QStartCoroutine(DoWaitOnCondition(condition));
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