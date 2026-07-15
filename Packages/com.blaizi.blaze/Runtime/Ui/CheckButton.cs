using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Blaze.Runtime.Ui
{
    public class CheckButton : MonoBehaviour
    {
        public GameObject root;

        public State activeState;
        public State notActiveState;

        public List<State> AllStates => new List<State>()
        {
            activeState, notActiveState,
        };

        [Serializable]
        public class State
        {
            public GameObject root;
            public TMP_Text text;
            public Button button;
        }

        public UnityEvent onClick = new();
        public bool active;

        public virtual void InitView(string text)
        {
            foreach (var state in AllStates)
            {
                state.text.text = text;
                state.button.onClick.AddListener(() => onClick?.Invoke());
            }
            
            Update();
        }

        public virtual void Update()
        {
            activeState.root.SetActive(active);
            notActiveState.root.SetActive(!active);
        }
    }
}