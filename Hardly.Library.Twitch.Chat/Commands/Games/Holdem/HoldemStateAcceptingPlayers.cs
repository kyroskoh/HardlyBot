﻿using Hardly.Games;
using System;

namespace Hardly.Library.Twitch {
	internal class HoldemStateAcceptingPlayers : GameStateAcceptingPlayers<TwitchHoldem> {
		public HoldemStateAcceptingPlayers(TwitchHoldem controller) : base(controller) {
			AddCommand(controller.room, "playholdem", PlayCommand, "Joins a game of Holdem.", null, false, null, false);
			AddCommand(controller.room, "startholdem", StartCommand, "Starts a game of Holdem if there is at least two players", null, true, null, false);
			AddCommand(controller.room, "cancelplayholdem", CancelPlayCommand, "Cancels a play, if it's not too late.", null, false, null, false);
        }

        private void CancelPlayCommand(SqlTwitchUser speaker, string additionalText) {
			var player = controller.game.Get(speaker);
			if(player != null) {
				TwitchUserPointManager userPoints = controller.room.pointManager.ForUser(speaker);
				userPoints.Award(player.bet, 0);
				controller.room.SendWhisper(speaker, "You're out, later dude.");
				if(!controller.game.CanStart()) {
					StopTimers();
				}
			} else {
				controller.room.SendWhisper(speaker, "You weren't in the game, soooo...");
			}
		}

		private void StartCommand(SqlTwitchUser speaker, string additionalText) {
			if(controller.game.CanStart()) {
                StartGame();
			}
		}

		private void PlayCommand(SqlTwitchUser speaker, string additionalText) {
			TwitchUserPointManager userPoints = controller.room.pointManager.ForUser(speaker);

            if(userPoints.Points > controller.game.bigBlind) {
				controller.game.Join(speaker, userPoints);
				if(controller.game.CanStart()) {
					MinHit_StartWaitingForAdditionalPlayers();
				}
				SendJoinMessage(speaker);
				StartIfReady();
			} else {
				controller.room.SendWhisper(speaker, "You flat broke, come back later.");
			}
		}

		void SendJoinMessage(SqlTwitchUser speaker) {
			string chatMessage = "You're in, sit tight we start ";
			chatMessage += GetStartingInMessage();
			controller.room.SendWhisper(speaker, chatMessage);
		}

		private void StartIfReady() {
			if(controller.game.IsFull()) {
                StartGame();
            }
        }

        void StartGame() {
            if(controller.game.StartGame()) {
                controller.SetState(this, typeof(HoldemStatePlayPreFlop));
            }
        }

        protected override void AnnounceGame() {
			controller.room.SendChatMessage("Holdem: !playholdem to join in.");
            base.AnnounceGame();
		}

		string GetStartingInMessage() {
			TimeSpan timeRemaining = roundTimer.TimeRemaining();
			string chatMessage;
            if(controller.game.CanStart()) {
                if(timeRemaining > TimeSpan.FromSeconds(5)) {
                    chatMessage = "in " + timeRemaining.ToSimpleString();

                    chatMessage += " or when " + controller.game.NumberOfOpenSpots() + " more people !play.";
                } else {
                    chatMessage = "sooon.";
                }
            } else {
                chatMessage = "once more people join.";
            }

			return chatMessage;
		}

		internal override void FinalTimeUp() {
            StartGame();
		}

		internal override void TimeUp() {
			controller.room.SendChatMessage("Holdem: !playholdem to join in, we start " + GetStartingInMessage());
		}
	}
}