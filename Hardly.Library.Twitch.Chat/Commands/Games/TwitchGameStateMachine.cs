﻿using System;
using Hardly.Games;

namespace Hardly.Library.Twitch {
    public abstract class TwitchGameStateMachine<GameLogicController> : TwitchGame<GameLogicController> where GameLogicController : Game, new() {
        public GameState currentState {
            get;
            private set;
        }
        object myLock = new object();

        public TwitchGameStateMachine(TwitchChatRoom room, Type startingStateType) : base(room) {
            SetState(null, startingStateType);
        }

        public bool SetState(GameState theStateIThinkWeAreIn, Type theStateToChangeToIfInTheStateIExpect) {
           // currentState?.Close();
            theStateIThinkWeAreIn?.Close();
            Action openAction = null;

            lock (myLock) {
                if(theStateIThinkWeAreIn?.GetType() != theStateToChangeToIfInTheStateIExpect) {
                    if(theStateIThinkWeAreIn == null || currentState == null || currentState.Equals(theStateIThinkWeAreIn)) {
                        Debug.Assert(!theStateToChangeToIfInTheStateIExpect.Equals(currentState?.GetType()));
                        Log.debug("Twitch Game: Setting next state to " + theStateToChangeToIfInTheStateIExpect.ToString());
                        currentState = (GameState)theStateToChangeToIfInTheStateIExpect.GetConstructor(new Type[] { this.GetType() }).Invoke(new object[] { this });
                        openAction = currentState.Open;
                    }
                }
            }

            if(openAction != null) {
                openAction();
                return true;
            }
                   
            return false;
        }
    }
}