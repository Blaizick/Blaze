using System;
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

    // public class BlazeWaitForCoroutine : IBlazeYieldInstruction
    // {
    //     public BlazeCoroutine coroutine;

    //     public BlazeWaitForCoroutine(BlazeCoroutine coroutine)
    //     {
    //         this.coroutine = coroutine;
    //     }

    //     public bool KeepWaiting
    //     {
    //         get
    //         {
    //             return coroutine.MoveNext();
    //         }
    //     }

    //     public void Step()
    //     {
            
    //     }
    // }

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

    public class QCoroutineRunner : Singleton<QCoroutineRunner>
    {
        private List<QCoroutine> m_Coroutines = new();
        private List<QCoroutine> m_PausedCoroutines = new();

        private Dictionary<object, List<QCoroutine>> m_CoroutinesDic = new();

        public static bool customUpdate = false;

        public virtual void Awake()
        {
            SceneManager.sceneUnloaded += scene =>
            {
                QStopAllCoroutines();
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

        protected static List<QCoroutine> GetAttachedCoroutines(object _object)
        {
            if (!Instance.m_CoroutinesDic.TryGetValue(_object, out var list))
            {
                list = new();
                Instance.m_CoroutinesDic[_object] = list;
            }
            return list;
        }

        public static QCoroutine QStartCoroutine(IEnumerator routine)
        {
            var coroutine = new QCoroutine(routine);
            Instance.m_Coroutines.Add(coroutine);
            return coroutine;
        }

        public static QCoroutine QStartCoroutine(object _object, IEnumerator routine)
        {
            var coroutine = QStartCoroutine(routine);
            GetAttachedCoroutines(_object).Add(coroutine);
            return coroutine;
        }
        
        public static void QPauseCoroutine(QCoroutine coroutine)
        {
            int coroutineIndex = Instance.m_Coroutines.IndexOf(coroutine);
            if (!Instance.m_PausedCoroutines.Contains(coroutine) && 
                coroutineIndex > -1)
            {
                Instance.m_Coroutines.RemoveAt(coroutineIndex);
                Instance.m_PausedCoroutines.Add(coroutine);
            }
        }

        public static void QPauseCoroutine(object _object, QCoroutine coroutine)
        {
            QPauseCoroutine(coroutine);
        }

        public static void QUnpauseCoroutine(QCoroutine coroutine)
        {
            var pausedIndex = Instance.m_PausedCoroutines.IndexOf(coroutine);
            if (pausedIndex > -1 && 
                !Instance.m_Coroutines.Contains(coroutine))
            {
                Instance.m_PausedCoroutines.RemoveAt(pausedIndex);
                Instance.m_Coroutines.Add(coroutine);
            }
        }

        public static void QUnpauseCoroutine(object _object, QCoroutine coroutine)
        {
            QUnpauseCoroutine(coroutine);
        }

        public static void QStopCoroutine(QCoroutine coroutine)
        {
            int coroutineIndex = Instance.m_Coroutines.IndexOf(coroutine);
            if (coroutineIndex > -1)
            {
                Instance.m_Coroutines.RemoveAt(coroutineIndex);
            }
            else
            {
                int pausedIndex = Instance.m_PausedCoroutines.IndexOf(coroutine);
                if (pausedIndex > -1)
                {
                    Instance.m_PausedCoroutines.RemoveAt(pausedIndex);
                }
            }
        }

        public static void QStopCoroutine(object _object, QCoroutine coroutine)
        {
            QStopCoroutine(coroutine);
            GetAttachedCoroutines(_object).Remove(coroutine);
        }

        public static void QStopAllCoroutines()
        {
            Instance.m_Coroutines.Clear();
            Instance.m_PausedCoroutines.Clear();
            Instance.m_CoroutinesDic.Clear();
        }

        public static void QStopAllCoroutines(object _object)
        {
            foreach (var coroutine in GetAttachedCoroutines(_object))
            {
                QStopCoroutine(coroutine);
            }
            Instance.m_CoroutinesDic[_object].Clear();
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

    public static class ObjectExtensions
    {
        public static QCoroutine QStartCoroutine(this object _object, IEnumerator routine)
        {
            return QCoroutineRunner.QStartCoroutine(_object, routine);
        }
        
        public static void QPauseCoroutine(this object _object, QCoroutine coroutine)
        {
            QCoroutineRunner.QPauseCoroutine(_object, coroutine);
        }

        public static void QUnpauseCoroutine(this object _object, QCoroutine coroutine)
        {
            QCoroutineRunner.QUnpauseCoroutine(_object, coroutine);
        }

        public static void QStopCoroutine(this object _object, QCoroutine coroutine)
        {
            QCoroutineRunner.QStopCoroutine(_object, coroutine);
        }

        public static void QStopAllCoroutines(this object _object)
        {
            QCoroutineRunner.QStopAllCoroutines(_object);
        }
    }
}