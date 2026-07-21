using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blaze.Runtime.Tweening
{
    public struct QTweenSettings<T> where T : unmanaged
    {
        public T startValue;
        public T endValue;
        public UnityAction<T> onValueChange;

        public QTweenSettings(T startValue, T endValue, UnityAction<T> onValueChange)
        {
            this.startValue = startValue;
            this.endValue = endValue;
            this.onValueChange = onValueChange;
        }
    }
    public struct ShakeSettings
    {
        public Vector3 startValue;
        public Vector3 strength;
        public int vibrato;
        public bool shake;
    
        public ShakeSettings(in Vector3 startValue, in Vector3 strength, int vibrato)
        {
            this.startValue = startValue;
            this.strength = strength;
            this.vibrato = vibrato;
            this.shake = true;
        }
    }
    public struct QTweenData
    {
        public object settings;
        public float progress;
        public float duration;
        public bool active;
        public UnityEvent onComplete;
        public DeltaTimeSource deltaTimeSource;
        public EaseType ease;
        public object target;
        public StructType structType;
        public AnimationType animationType;
        public ShakeSettings shakeSettings;
    }

    public enum EaseType : sbyte
	{
		Custom = -1,
		Default,
		Linear,
		InSine,
		OutSine,
		InOutSine,
		InQuad,
		OutQuad,
		InOutQuad,
		InCubic,
		OutCubic,
		InOutCubic,
		InQuart,
		OutQuart,
		InOutQuart,
		InQuint,
		OutQuint,
		InOutQuint,
		InExpo,
		OutExpo,
		InOutExpo,
		InCirc,
		OutCirc,
		InOutCirc,
		InElastic,
		OutElastic,
		InOutElastic,
		InBack,
		OutBack,
		InOutBack,
		InBounce,
		OutBounce,
		InOutBounce
	}
    internal static class StandardEasing
	{
		private static float InElastic(float t)
		{
			return 1f - StandardEasing.OutElastic(1f - t);
		}

		private static float OutElastic(float t)
		{
			float num = Mathf.Pow(2f, -10f * t * 1f);
			if (t <= 0.9999f)
			{
				return num * Mathf.Sin((t - 0.075f) * 6.2831855f / 0.3f) + 1f;
			}
			return 1f;
		}

		private static float OutBounce(float x)
		{
			if (x < 0.36363637f)
			{
				return 7.5625f * x * x;
			}
			if (x < 0.72727275f)
			{
				return 7.5625f * (x -= 0.54545456f) * x + 0.75f;
			}
			if ((double)x < 0.9090909090909091)
			{
				return 7.5625f * (x -= 0.8181818f) * x + 0.9375f;
			}
			return 7.5625f * (x -= 0.95454544f) * x + 0.984375f;
		}

		internal static float Evaluate(float t, EaseType ease)
		{
			switch (ease)
			{
			case EaseType.Linear:
				return t;
			case EaseType.InSine:
				return 1f - Mathf.Cos(t * 1.5707964f);
			case EaseType.OutSine:
				return Mathf.Sin(t * 1.5707964f);
			case EaseType.InOutSine:
				return -0.5f * (Mathf.Cos(3.1415927f * t) - 1f);
			case EaseType.InQuad:
				return t * t;
			case EaseType.OutQuad:
				return -t * (t - 2f);
			case EaseType.InOutQuad:
				t *= 2f;
				if (t < 1f)
				{
					return 0.5f * t * t;
				}
				return -0.5f * ((t -= 1f) * (t - 2f) - 1f);
			case EaseType.InCubic:
				return t * t * t;
			case EaseType.OutCubic:
				return (t -= 1f) * t * t + 1f;
			case EaseType.InOutCubic:
				t *= 2f;
				if (t < 1f)
				{
					return 0.5f * t * t * t;
				}
				return 0.5f * ((t -= 2f) * t * t + 2f);
			case EaseType.InQuart:
				return t * t * t * t;
			case EaseType.OutQuart:
				return -((t -= 1f) * t * t * t - 1f);
			case EaseType.InOutQuart:
				t *= 2f;
				if (t < 1f)
				{
					return 0.5f * t * t * t * t;
				}
				return -0.5f * ((t -= 2f) * t * t * t - 2f);
			case EaseType.InQuint:
				return t * t * t * t * t;
			case EaseType.OutQuint:
				return (t -= 1f) * t * t * t * t + 1f;
			case EaseType.InOutQuint:
				t *= 2f;
				if (t < 1f)
				{
					return 0.5f * t * t * t * t * t;
				}
				return 0.5f * ((t -= 2f) * t * t * t * t + 2f);
			case EaseType.InExpo:
				if (t != 0f)
				{
					return Mathf.Pow(2f, 10f * (t - 1f));
				}
				return 0f;
			case EaseType.OutExpo:
				if (t == 1f)
				{
					return 1f;
				}
				return -Mathf.Pow(2f, -10f * t) + 1f;
			case EaseType.InOutExpo:
				if (t == 0f)
				{
					return 0f;
				}
				if (t == 1f)
				{
					return 1f;
				}
				t *= 2f;
				if (t < 1f)
				{
					return 0.5f * Mathf.Pow(2f, 10f * (t - 1f));
				}
				return 0.5f * (-Mathf.Pow(2f, -10f * (t -= 1f)) + 2f);
			case EaseType.InCirc:
				return -(Mathf.Sqrt(1f - t * t) - 1f);
			case EaseType.OutCirc:
				return Mathf.Sqrt(1f - (t -= 1f) * t);
			case EaseType.InOutCirc:
				t *= 2f;
				if (t < 1f)
				{
					return -0.5f * (Mathf.Sqrt(1f - t * t) - 1f);
				}
				return 0.5f * (Mathf.Sqrt(1f - (t -= 2f) * t) + 1f);
			case EaseType.InElastic:
				return StandardEasing.InElastic(t);
			case EaseType.OutElastic:
				return StandardEasing.OutElastic(t);
			case EaseType.InOutElastic:
				if (t < 0.5f)
				{
					return StandardEasing.InElastic(t * 2f) * 0.5f;
				}
				return 0.5f + StandardEasing.OutElastic((t - 0.5f) * 2f) * 0.5f;
			case EaseType.InBack:
				return t * t * (2.70158f * t - 1.70158f);
			case EaseType.OutBack:
				return (t -= 1f) * t * (2.70158f * t + 1.70158f) + 1f;
			case EaseType.InOutBack:
				t *= 2f;
				if (t < 1f)
				{
					return 0.5f * (t * t * (3.5949094f * t - 2.5949094f));
				}
				return 0.5f * ((t -= 2f) * t * (3.5949094f * t + 2.5949094f) + 2f);
			case EaseType.InBounce:
				return 1f - StandardEasing.OutBounce(1f - t);
			case EaseType.OutBounce:
				return StandardEasing.OutBounce(t);
			case EaseType.InOutBounce:
				if ((double)t >= 0.5)
				{
					return (1f + StandardEasing.OutBounce(2f * t - 1f)) / 2f;
				}
				return (1f - StandardEasing.OutBounce(1f - 2f * t)) / 2f;
			}
            QDebugBase<InternalLogChannel>.Error(InternalLogChannel.System, $"Invalid ease type: {ease}.");
			return t;
		}

		private const float halfPi = 1.5707964f;

		internal const float backEaseConst = 1.70158f;

		internal const float defaultElasticEasePeriod = 0.3f;
	}
    public enum StructType
    {
        Default,
        Float,
        Vector2,
        Vector3,
        Vector4,
        Color,
    }
    public enum DeltaTimeSource
    {
        DeltaTime,
        UnscaledDeltaTime,
    }
    public enum AnimationType
    {
        Default,
        Custom,
        TransformPosition,
        TransformLocalPosition,
        TransformLocalScale,
        TransformEulerAngles,
        TransformPunchLocalPosition,
        TransformPunchLocalScale,
        TransformPunchLocalRotation,
        CanvasGroupAlpha,
    }

    public struct QTween
    {
        private uint m_Id;

        public uint Id
        {
            get
            {
                return m_Id;
            }
        }

        public QTween OnComplete(UnityAction onComplete)
        {
            if (QTweenManager.Instance)
            {
                ref var data = ref QTweenManager.Instance.tweens.buffer[Id];
                if (onComplete != null && data.active)
                {
                    data.onComplete.AddListener(onComplete);
                }
            }
            return this;
        }
        public QTween SetDeltaTimeSource(DeltaTimeSource deltaTimeSource)
        {
            if (QTweenManager.Instance)
            {
                ref var data = ref QTweenManager.Instance.tweens.buffer[Id];
                if (data.active)
                {
                    data.deltaTimeSource = deltaTimeSource;
                }
            }
            return this;
        }

        public bool Active
        {
            get
            {
                if (QTweenManager.Instance)
                {
                    return QTweenManager.Instance.tweens.buffer[Id].active;
                }
                return false;
            }
        }

        public QTween SetEase(EaseType ease)
        {
            ref var data = ref QTweenManager.Instance.tweens.buffer[Id];
            if (data.active && QTweenManager.Instance)
            {
                data.ease = ease;
            }
            return this;
        }

        public IEnumerator WaitForCompletion()
        {
            while (Active && QTweenManager.Instance)
            {
                yield return null;
            }
        }

        public void Complete()
        {
            if (Active && QTweenManager.Instance)
            {
                QTweenManager.Instance.CompleteTween(Id);
            }
        }

        public static void CompleteAll(object target)
        {
            if (target != null && QTweenManager.Instance)
            {
                QTweenManager.Instance.CompleteTweens(QTweenManager.Instance.GetAllTweensOnObject(target));
            }
        }

        public QTween(uint id)
        {
            m_Id = id;
        }

        public static QTween Animate<T>(ref QTweenSettings<T> settings, 
            float duration, 
            object target, 
            AnimationType animationType, 
            StructType structType,
            ref ShakeSettings shakeSettings) where T : unmanaged
        {
            QTweenManager.Instance.tweens.CreateItem(out var id);
            ref var data = ref QTweenManager.Instance.tweens.buffer[id];
            data.settings = settings;
            data.progress = 0.0f;
            data.duration = duration;
            data.active = true;
            data.onComplete = new();
            data.deltaTimeSource = DeltaTimeSource.DeltaTime;
            data.ease = EaseType.InOutSine;
            data.target = target;
            data.structType = structType;
            data.animationType = animationType;
            data.shakeSettings = shakeSettings;
            if (target != null)
            {
                QTweenManager.Instance.GetAllTweensOnObject(target).Add(id);
            }
            QTweenManager.Instance.createdTweens.Add(id);
            return new QTween(id);
        }
        public static QTween Animate<T>(T startValue, 
            T endValue, 
            float duration, 
            object target, 
            AnimationType animationType, 
            StructType structType) where T : unmanaged
        {
            QTweenSettings<T> settings = new(startValue, endValue, null);
            ShakeSettings shakeSettings = new();
            return Animate(ref settings, duration, target, animationType, structType, ref shakeSettings);
        }
        public static QTween AnimatePunch(Vector3 startValue,
            Vector3 streangth,
            float duration,
            int vibrato,
            object target,
            AnimationType animationType)
        {
            QTweenSettings<Vector3> settings = new(startValue, streangth, null);
            ShakeSettings shakeSettings = new(startValue, streangth, vibrato);
            return Animate(ref settings, duration, target, animationType, StructType.Vector3, ref shakeSettings);
        }

        public static QTween Custom<T>(T startValue, 
            T endValue, 
            float duration, 
            UnityAction<T> onValueChange, 
            object target, 
            StructType structType) where T : unmanaged
        {
            QTweenSettings<T> settings = new(startValue, endValue, onValueChange);
            ShakeSettings shakeSettings = new();
            return Animate(ref settings, duration, target, AnimationType.Custom, structType, ref shakeSettings);
        }
        
        public static QTween Custom(float startValue, float endValue, float duration, UnityAction<float> onValueChange, object target)
        {
            return Custom(startValue, endValue, duration, onValueChange, target, StructType.Float);
        }
        public static QTween Custom(Vector2 startValue, Vector2 endValue, float duration, UnityAction<Vector2> onValueChange, object target)
        {
            return Custom(startValue, endValue, duration, onValueChange, target, StructType.Vector2);
        }
        public static QTween Custom(Vector3 startValue, Vector3 endValue, float duration, UnityAction<Vector3> onValueChange, object target)
        {
            return Custom(startValue, endValue, duration, onValueChange, target, StructType.Vector3);
        }
        public static QTween Custom(Vector4 startValue, Vector4 endValue, float duration, UnityAction<Vector4> onValueChange, object target)
        {
            return Custom(startValue, endValue, duration, onValueChange, target, StructType.Vector4);
        }
        public static QTween Custom(Color startValue, Color endValue, float duration, UnityAction<Color> onValueChange, object target)
        {
            return Custom(startValue, endValue, duration, onValueChange, target, StructType.Color);
        }

        public static QTween Position(Transform transform, Vector3 startValue, Vector3 endValue, float duration)
        {
            return Animate(startValue, endValue, duration, transform, AnimationType.TransformPosition, StructType.Vector3);
        }
        public static QTween LocalPosition(Transform transform, Vector3 startValue, Vector3 endValue, float duration)
        {
            return Animate(startValue, endValue, duration, transform, AnimationType.TransformLocalPosition, StructType.Vector3);
        }
        public static QTween LocalScale(Transform transform, Vector3 startValue, Vector3 endValue, float duration)
        {
            return Animate(startValue, endValue, duration, transform, AnimationType.TransformLocalScale, StructType.Vector3);
        }
        public static QTween Alpha(CanvasGroup canvasGroup, float startValue, float endValue, float duration)
        {
            return Animate(startValue, endValue, duration, canvasGroup, AnimationType.CanvasGroupAlpha, StructType.Float);
        }
        public static QTween PunchLocalPosition(Transform transform, Vector3 punch, float duration, int vibrato = 3)
        {
            return AnimatePunch(transform.localPosition, punch, duration, vibrato, transform, AnimationType.TransformPunchLocalPosition);
        }
        public static QTween PunchLocalScale(Transform transform, Vector3 punch, float duration, int vibrato = 3)
        {
            return AnimatePunch(transform.localPosition, punch, duration, vibrato, transform, AnimationType.TransformLocalScale);
        }
        public static QTween PunchLocalRotation(Transform transform, Vector3 punch, float duration, int vibrato = 3)
        {
            return AnimatePunch(transform.localPosition, punch, duration, vibrato, transform, AnimationType.TransformPunchLocalRotation);
        }
    }

	[AddComponentMenu("")]
    public class QTweenManager : ManagedBehaviour
    {
        public Array32<QTweenData> tweens;
        [NonSerialized]
        public List<uint> createdTweens;
        public Dictionary<object, List<uint>> objectTweensDic;

        public static uint maxTweens = ushort.MaxValue;
        public static bool customUpdate;

        private List<uint> m_TweensToComplete = new();

        private static QTweenManager s_Instance;
        private static bool s_InstanceCreated;

        public static QTweenManager Instance => s_Instance;
        public static bool InstanceCreated => s_InstanceCreated;

        public Type floatType;
        public Type vector2Type;
        public Type vector3Type;
        public Type vector4Type;
        public Type colorType;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void BeforeSceneLoad()
        {
            var go = new GameObject(typeof(QTweenManager).Name);
            var ai = go.AddComponent<QTweenManager>();
            DontDestroyOnLoad(go);
            ai.Init();
            s_Instance = ai;
            s_InstanceCreated = true;
        }

        public virtual void OnDestroy()
        {
            s_InstanceCreated = false;
        }

        public virtual void Init()
        {
            tweens = new(maxTweens);
            tweens.CreateItem(out _);

            objectTweensDic = new();
            createdTweens = new();

            floatType = typeof(float);
            vector2Type = typeof(Vector2);
            vector3Type = typeof(Vector3);
            vector4Type = typeof(Vector4);
            colorType = typeof(Color);
        }

        public List<uint> GetAllTweensOnObject(object target)
        {
            if (!objectTweensDic.TryGetValue(target, out var list))
            {
                list = new();
                objectTweensDic[target] = list;
            }
            return list;
        }

        public virtual void QUpdate()
        {
            m_TweensToComplete.Clear();
            for (int i = createdTweens.Count - 1; i >= 0; i--)
            {
                var id = createdTweens[i];
                ref var data = ref tweens.buffer[id];
                if (data.duration > 0.0f)
                {
                    float dt = data.deltaTimeSource switch
                    {
                        DeltaTimeSource.DeltaTime => Time.deltaTime,
                        DeltaTimeSource.UnscaledDeltaTime => Time.unscaledDeltaTime,
                        _ => Time.deltaTime,
                    };
                    data.progress += dt / data.duration;
                }
                if (data.progress >= 1.0f)
                {
                    m_TweensToComplete.Add(id);
                }
                else
                {
                    ApplyValueForProgress(ref data, data.progress);
                }
            }
            CompleteTweens(m_TweensToComplete);
        }

        public virtual void ApplyValueForProgress(ref QTweenData data, float t)
        {
            switch (data.structType)
            {
                case StructType.Float:
                    var fSettings = (QTweenSettings<float>)data.settings;
                    var fVal = Mathf.LerpUnclamped(fSettings.startValue, fSettings.endValue, t);
                    switch (data.animationType)
                    {
                        case AnimationType.CanvasGroupAlpha:
                            ((CanvasGroup)data.target).alpha = fVal;
                            break;
                    }
                    if (fSettings.onValueChange != null)
                    {
                        fSettings.onValueChange(fVal);
                    }
                    break;
                case StructType.Vector2:
                    var v2Settings = (QTweenSettings<Vector2>)data.settings;
                    var v2Val = Vector2.LerpUnclamped(v2Settings.startValue, v2Settings.endValue, t);
                    switch (data.animationType)
                    {
                        case AnimationType.Default:
                            break;
                    }
                    if (v2Settings.onValueChange != null)
                    {
                        v2Settings.onValueChange(v2Val);
                    }
                    break;
                case StructType.Vector3:
                    var v3Settings = (QTweenSettings<Vector3>)data.settings;
                    Vector3 v3Val;
                    if (data.shakeSettings.shake)
                    {
                        float decay = 1f - t;
                        float wave = Mathf.Sin(t * data.shakeSettings.vibrato * Mathf.PI * 2f);
                        v3Val = data.shakeSettings.startValue + data.shakeSettings.strength * wave * decay;
                    }
                    else
                    {
                        v3Val = Vector3.LerpUnclamped(v3Settings.startValue, v3Settings.endValue, t);
                    }
                    switch (data.animationType)
                    {
                        case AnimationType.TransformPosition:
                            ((Transform)data.target).position = v3Val;
                            break;
                        case AnimationType.TransformLocalPosition:
                            ((Transform)data.target).localPosition = v3Val;
                            break;
                        case AnimationType.TransformLocalScale:
                            ((Transform)data.target).localScale = v3Val;
                            break;
                        case AnimationType.TransformEulerAngles:
                            ((Transform)data.target).eulerAngles = v3Val;
                            break;
                        case AnimationType.TransformPunchLocalPosition:
                            ((Transform)data.target).localPosition = v3Val;
                            break;
                        case AnimationType.TransformPunchLocalScale:
                            ((Transform)data.target).localScale = v3Val;
                            break;
                        case AnimationType.TransformPunchLocalRotation:
                            ((Transform)data.target).localRotation = Quaternion.Euler(v3Val);
                            break;
                    }
                    if (v3Settings.onValueChange != null)
                    {
                        v3Settings.onValueChange(v3Val);
                    }
                    break;
                case StructType.Vector4:
                    var v4Settings = (QTweenSettings<Vector4>)data.settings;
                    var v4Val = Vector3.LerpUnclamped(v4Settings.startValue, v4Settings.endValue, t);
                    switch (data.animationType)
                    {
                        case AnimationType.Default:
                            break;
                    }
                    if (v4Settings.onValueChange != null)
                    {
                        v4Settings.onValueChange(v4Val);
                    }
                    break;
                case StructType.Color:
                    var colSettings = (QTweenSettings<Color>)data.settings;
                    var colVal = Color.LerpUnclamped(colSettings.startValue, colSettings.endValue, t);
                    switch (data.animationType)
                    {
                        case AnimationType.Default:
                            break;
                    }
                    if (colSettings.onValueChange != null)
                    {
                        colSettings.onValueChange(colVal);
                    }
                    break;
            }
        }

        public virtual void CompleteTween(uint id)
        {
            CompleteTweens(new(){id});
        }

        public virtual void CompleteTweens(List<uint> ids)
        {
            for (int i = ids.Count - 1; i >= 0; i--)
            {
                uint id = ids[i];
                ref var data = ref tweens.buffer[id];
                ApplyValueForProgress(ref data, StandardEasing.Evaluate(1.0f, data.ease));
                data.onComplete?.Invoke();
                data.active = false;
                tweens.ReleaseItem(id);
                createdTweens.Remove(id);
                if (data.target != null)
                {
                    GetAllTweensOnObject(data.target).Remove(id);
                }
            }
        }

        public override void Update()
        {
            if (!customUpdate)
            {
                QUpdate();
            }
            base.Update();
        }
    }
}