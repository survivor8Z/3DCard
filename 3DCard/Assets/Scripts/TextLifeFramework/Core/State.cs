/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace EnjoyGameClub.TextLifeFramework.Core
{
    [Serializable]
    public class State
    {
        public StateMachine StateMachine;
        public UnityEvent onStateEnter = new UnityEvent();
        public UnityEvent onStateExit = new UnityEvent();
        public UnityEvent onStateUpdate = new UnityEvent();
        public UnityEvent onStateLateUpdate = new UnityEvent();
        public Dictionary<State, Func<bool>> StateDictionary = new Dictionary<State, Func<bool>>();
        public string Name;

        public State(StateMachine stateMachine, string name)
        {
            Name = name;
            StateMachine = stateMachine;
        }

        public void AddTranslation(State targetState, Func<bool> f)
        {
            StateDictionary ??= new Dictionary<State, Func<bool>>();
            StateDictionary[targetState] = f;
        }

        public void Enter()
        {
            onStateEnter?.Invoke();
        }

        public void Exit()
        {
            onStateExit?.Invoke();
        }

        public void Update()
        {
            onStateUpdate?.Invoke();
        }

        public void LateUpdate()
        {
            onStateLateUpdate?.Invoke();
            foreach (var singleTrans in StateDictionary.Where(singleTrans => singleTrans.Value.Invoke()))
            {
                StateMachine.ChangeState(singleTrans.Key);
                return;
            }
        }
    }
}