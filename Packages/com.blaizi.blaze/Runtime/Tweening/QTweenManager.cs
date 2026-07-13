using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Blaze.Runtime.Tweening
{
    public class QTweenManager : MonoBehaviour
    {
        private static bool s_TweenManagerInstanceCreated = false;
        private static QTweenManager s_TweenManagerInstance = null;
        
        public static QTweenManager Instance
        {
            get
            {
                if (s_TweenManagerInstance == null)
                {
                    if (!s_TweenManagerInstanceCreated)
                    {
                        var go = new GameObject("Q Tween Manager");
                        s_TweenManagerInstance = go.AddComponent<QTweenManager>();
                        DontDestroyOnLoad(go);
                        s_TweenManagerInstance.Init();
                        s_TweenManagerInstanceCreated = true;
                    }
                }
                return s_TweenManagerInstance;
            }
        }
        private List<IQTweenCore> m_Tweens = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetStatistics()
        {
            s_TweenManagerInstance = null;
            s_TweenManagerInstanceCreated = false;
        }

        public void Update()
        {
            for (int i = 0; i < m_Tweens.Count; i++)
            {
                var tween = m_Tweens[i];

                tween.Process(Time.deltaTime);
                
                if (tween.ShouldEnd())
                {
                    CompleteTween(tween);
                }
                else
                {
                    tween.ApplyValue();
                }
            }
        }

        public virtual void Init()
        {
            SceneManager.sceneUnloaded += scene =>
            {
                m_Tweens.Clear();
            };
        }

        public void StartTween(IQTweenCore tween)
        {
            m_Tweens.Add(tween);
            tween.Active = true;
        }
        public void KillTweensConnectedTo(object connectedObject)
        {
            for (int i = 0; i < m_Tweens.Count; i++)
            {
                var tween = m_Tweens[i];
                if (tween.ConnectedObject == connectedObject)
                {
                    tween.Active = false;
                    m_Tweens.RemoveAt(i--);
                }
            }
        }
        public void KillTween(IQTweenCore tween)
        {
            tween.Active = false;
            m_Tweens.Remove(tween);
        }
        public void CompleteTween(IQTweenCore tween)
        {
            tween.EndQuiet();
            tween.ApplyValue();
            m_Tweens.Remove(tween);
            tween.Active = false;
            tween.OnComplete?.Invoke();
        } 
    }

    public interface ILerper<T>
    {
        public T Lerp(T startValue, T endValue, float progress);
    }
    public class FloatLerper : ILerper<float>
    {
        public virtual float Lerp(float startValue, float endValue, float progress)
        {
            return Mathf.Lerp(startValue, endValue, progress);
        }
    }
    public class Vector3Lerper : ILerper<Vector3>
    {
        public virtual Vector3 Lerp(Vector3 startValue, Vector3 endValue, float progress)
        {
            return Vector3.Lerp(startValue, endValue, progress);
        }
    }

    public interface IQEaseFunction
    {
        public float Use(float t);
    }

    public class QLinearEaseFunction : IQEaseFunction
    {
        public float Use(float t) => t;
    }
    public class QInQuadEaseFunction : IQEaseFunction
    {
        public float Use(float t) => t * t;
    }
    public class QOutQuadEaseFunction : IQEaseFunction
    {
        public float Use(float t) => 1f - (1f - t) * (1f - t);
    }
    public class QInOutQuadEaseFunction : IQEaseFunction
    {
        public float Use(float t)
        {
            return t < 0.5f
                ? 2f * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        }
    }
    public class QInCubicEaseFunction : IQEaseFunction
    {
        public float Use(float t) => t * t * t;
    }
    public class QOutCubicEaseFunction : IQEaseFunction
    {
        public float Use(float t)
        {
            t -= 1f;
            return t * t * t + 1f;
        }
    }
    public class QInOutCubicEaseFunction : IQEaseFunction
    {
        public float Use(float t) => t;
    }
    public class QOutBackEaseFunction : IQEaseFunction
    {
        public float Use(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;

            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }
    }
    public class QOutBounceEaseFunction : IQEaseFunction
    {
        public float Use(float t)
        {                    
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1f / d1)
                return n1 * t * t;

            if (t < 2f / d1)
            {
                t -= 1.5f / d1;
                return n1 * t * t + 0.75f;
            }

            if (t < 2.5f / d1)
            {
                t -= 2.25f / d1;
                return n1 * t * t + 0.9375f;
            }

            t -= 2.625f / d1;
            return n1 * t * t + 0.984375f;
        }
    }
    
    public interface IQTweenCore
    {
        public bool Active { get; set; }
        public UnityEvent OnComplete { get; set; }
        public object ConnectedObject { get; set; }
        public IQEaseFunction EaseFunction { get; set; }

        public void ApplyValue();
        public bool ShouldEnd();
        public void Process(float t);
        public void EndQuiet();
    }
    public abstract class QBaseTweenCore<T> : IQTweenCore
    {
        public IQTweenValueController<T> valueController;
        public object connectedObject;
        
        public bool active;
        public UnityEvent onComplete = new();
        public IQEaseFunction easeFunction = new QInOutCubicEaseFunction();

        public bool Active
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => active;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => active = value;
        }
        public UnityEvent OnComplete
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => onComplete;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => onComplete = value;
        }
        public object ConnectedObject
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => connectedObject;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => connectedObject = value;
        }
        public IQEaseFunction EaseFunction
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => EaseFunction;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => EaseFunction = value;
        }

        private QTween m_Tween = null;
        public QTween Tween
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (m_Tween == null)
                {
                    m_Tween = new(this);
                }
                return m_Tween;
            }
        }

        public QBaseTweenCore(IQTweenValueController<T> valueController, 
            object connectedObject
            )
        {
            this.valueController = valueController;
            this.connectedObject = connectedObject;
        }

        public abstract void ApplyValue();
        public abstract bool ShouldEnd();
        public abstract void Process(float t);
        public abstract void EndQuiet();
    }
    public class QLerpTweenCore<T> : QBaseTweenCore<T>
    {
        public T startValue;
        public T endValue;
        public ILerper<T> lerper;
        public float duration;

        public float progress;

        public QLerpTweenCore(IQTweenValueController<T> valueController, 
            T startValue, 
            T endValue, 
            float duration, 
            ILerper<T> lerper, 
            object connectedObject
            ) : base(valueController, connectedObject)
        {
            this.startValue = startValue;
            this.endValue = endValue;
            this.lerper = lerper;
            this.duration = duration;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ApplyValue()
        {
            float t = easeFunction.Use(progress);
            valueController.Value = lerper.Lerp(startValue, endValue, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool ShouldEnd()
        {
            return progress >= 1.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Process(float t)
        {
            progress += t / duration;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EndQuiet()
        {
            progress = 1.0f;
        }
    }

    public interface IQPuncher<T>
    {
        public QPunchTweenCore<T> TweenCore { get; set; }

        public T Use();
    }

    public abstract class QBasePuncher<T> : IQPuncher<T>
    {
        protected QPunchTweenCore<T> m_TweenCore; 
        public QPunchTweenCore<T> TweenCore 
        { 
            get => m_TweenCore;
            set => m_TweenCore = value; 
        }

        public QBasePuncher() {} 
        public QBasePuncher(QPunchTweenCore<T> tweenCore) 
        {
            m_TweenCore = tweenCore;
        }

        public abstract T Use();
    }

    public class QVector3Puncher : QBasePuncher<Vector3>
    {
        public QVector3Puncher() {} 
        public QVector3Puncher(QPunchTweenCore<Vector3> tweenCore) : base(tweenCore) {}

        public override Vector3 Use()
        {
            float t = m_TweenCore.easeFunction.Use(m_TweenCore.progress);
            float decay = 1f - t;
            float wave = Mathf.Sin(t * Mathf.PI * m_TweenCore.vibrato);
            return m_TweenCore.startValue + m_TweenCore.punch * wave * decay;
        }
    }

    public class QPunchTweenCore<T> : QBaseTweenCore<T>
    {
        public T startValue;
        public T punch;

        public float duration;
        public float progress;

        public float vibrato = 4.0f;

        public IQPuncher<T> puncher;

        public QPunchTweenCore(IQTweenValueController<T> valueController, 
            T startValue, 
            T punch, 
            float duration,
            float vibrato, 
            IQPuncher<T> puncher, 
            object connectedObject) : base(valueController, connectedObject)
        {
            this.startValue = startValue;
            this.punch = punch;
            this.duration = duration;
            this.vibrato = vibrato;
            this.puncher = puncher;
        
            puncher.TweenCore = this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ApplyValue()
        {
            valueController.Value = puncher.Use();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Process(float t)
        {
            progress += t / duration;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EndQuiet()
        {
            progress = 1.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool ShouldEnd()
        {
            return progress >= 1.0f;
        }
    }

    public class QTween
    {
        public IQTweenCore tweenCore;

        public bool Active => tweenCore.Active;

        public QTween(IQTweenCore tweenCore)
        {
            this.tweenCore = tweenCore;
        }

        public IEnumerator WaitForCompletion()
        {
            while (tweenCore.Active)
            {
                yield return null;
            }
        }

        public QTween ApplyValue()
        {
            tweenCore.ApplyValue();
            return this;
        }

        public QTween Complete()
        {
            QTweenManager.Instance?.CompleteTween(tweenCore);
            return this;
        }

        public QTween OnComplete(UnityAction action)
        {
            tweenCore.OnComplete?.AddListener(action);
            return this;
        }

        public QTween Kill()
        {
            QTweenManager.Instance?.KillTween(tweenCore);
            return this;
        }
        
        public QTween SetEaseFunction(IQEaseFunction easeFunction)
        {
            tweenCore.EaseFunction = easeFunction;
            return this;
        } 
    }

    public class QTweenUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTweenActive(QTween tween)
        {
            return tween != null && tween.Active;
        }
    }

    public interface IQTweenValueController<T>
    {
        public T Value { get; set; }
    }

    public class QDelegateTweenValueController<T> : IQTweenValueController<T>
    {
        private Func<T> m_Getter;
        private Action<T> m_Setter;

        public T Value 
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Getter.Invoke(); 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_Setter?.Invoke(value); 
        }

        public QDelegateTweenValueController(Func<T> getter, Action<T> setter)
        {
            m_Getter = getter;
            m_Setter = setter;
        }
    }

    public static class QTweenCanvasGroupExtensions
    {
        public static QTween QFade(this CanvasGroup target, float endValue, float duration)
        {
            var tweenCore = new QLerpTweenCore<float>(
                new QDelegateTweenValueController<float>(() => target.alpha, value => target.alpha = value),
                target.alpha, 
                endValue, 
                duration, 
                new FloatLerper(),
                target
                );
            QTweenManager.Instance?.StartTween(tweenCore);
            return tweenCore.Tween;
        }
    }

    public static class QTweenTransformExtensions
    {
        public static QTween QLocalScale(this Transform target, Vector3 endValue, float duration)
        {
            var tweenCore = new QLerpTweenCore<Vector3>(
                new QDelegateTweenValueController<Vector3>(() => target.localScale, value => target.localScale = value),
                target.localScale, 
                endValue, 
                duration, 
                new Vector3Lerper(),
                target
                );
            QTweenManager.Instance?.StartTween(tweenCore);
            return tweenCore.Tween;
        }

        public static QTween QPunchLocalScale(this Transform target, Vector3 punch, float duration, float vibrato = 1.0f)
        {
            var tweenCore = new QPunchTweenCore<Vector3>(
                new QDelegateTweenValueController<Vector3>(() => target.localScale, value => target.localScale = value),
                target.localScale, 
                punch, 
                duration,
                vibrato, 
                new QVector3Puncher(),
                target
                );
            QTweenManager.Instance?.StartTween(tweenCore);
            return tweenCore.Tween;
        }
    }

    public static class QTweenObjectExtensions
    {
        public static void QKill(this object _object)
        {
            QTweenManager.Instance?.KillTweensConnectedTo(_object);
        }
    }
}