using System.Collections;
using UnityEngine;

namespace Blaze.Runtime 
{
    public class BlazeCoroutineRunner : MonoBehaviour
    {
        private static BlazeCoroutineRunner s_Instance = null;

        public static BlazeCoroutineRunner Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new GameObject().AddComponent<BlazeCoroutineRunner>();
                }
                return s_Instance;
            }
        }

	    #region WaitOnConditionThenExecute
	    public static void WaitOnConditionThenExecute(System.Func<bool> condition, System.Action action)
	    {
	    	Instance.StartWaitOnConditionThenExecute(condition, action);
	    }

	    public void StartWaitOnConditionThenExecute(System.Func<bool> condition, System.Action action)
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

	    public void StartWaitThenExecute(float wait, System.Action action, bool unscaledTime = false)
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
        public static Coroutine WaitOnCondition(System.Func<bool> condition)
        {
            return Instance.StartWaitOnCondition(condition);
        }

        public Coroutine StartWaitOnCondition(System.Func<bool> condition)
        {
            return StartCoroutine(DoWaitOnCondition(condition));
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
