using System.Collections;
using UnityEngine;

namespace Blaze.Runtime
{
    public class ManagedBehaviour : MonoBehaviour
    {
        private QCoroutineRunnerBase m_CoroutineRunnerBase = null;
        public QCoroutineRunnerBase CoroutineRunnerBase
        {
            get
            {
                if (m_CoroutineRunnerBase == null)
                {
                    m_CoroutineRunnerBase = new();
                    m_CoroutineRunnerBase.QInit();
                }
                return m_CoroutineRunnerBase;
            }
        }

        public virtual void Update()
        {
            CoroutineRunnerBase.QUpdate();
        }

        #region CoroutineMethods
        public QCoroutine QStartCoroutine(IEnumerator routine)
        {
            return CoroutineRunnerBase.QStartCoroutine(routine);
        }

        public void QPauseCoroutine(QCoroutine coroutine)
        {
            CoroutineRunnerBase.QPauseCoroutine(coroutine);
        }

        public void QUnpauseCoroutine(QCoroutine coroutine)
        {
            CoroutineRunnerBase.QUnpauseCoroutine(coroutine);
        }

        public void QStopCoroutine(QCoroutine coroutine)
        {
            CoroutineRunnerBase.QStopCoroutine(coroutine);
        }

        public void QStopAllCoroutines()
        {
            CoroutineRunnerBase.QStopAllCoroutines();
        }
        #endregion
    }
}