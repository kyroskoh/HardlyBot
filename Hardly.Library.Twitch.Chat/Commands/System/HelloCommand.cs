﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hardly.Library.Twitch.Commands.System {
    class HelloCommand : TwitchCommandController {
        public HelloCommand(TwitchChatRoom room) : base(room) {
            ChatCommand.Create(room, "hi", HiCommand, "Says hello", null, false, null, false);
            ChatCommand.Create(room, "echo", EchoCommand, "Echos what you say", null, false, null, false);
            ChatCommand.Create(room, "timemeout", TimeMeOut, "Shhh...", null, false, null, false);
            ChatCommand.Create(room, "banme", BanYouBecauseYouAreStupidAndYouShouldntHaveDoneThat, "This will ban you. Dont do it unless you want to be benned.", null, false, null, false);
            ChatCommand.Create(room, "about", About, "About the bot or sober.", null, false, null, false);
        }

        private void EchoCommand(TwitchUser speaker, String additionalText) {
            string message = "You said ";
            if (additionalText == null) {
                message += "...nothing!";
            }
            else {
                if (additionalText.ToLower().Contains("fuck")) {
                    message = "Hey now.";
                }
                else {
                    message += additionalText;
                }
            }
            room.SendWhisper(speaker, message);
        }
        private void TimeMeOut(TwitchUser speaker, String time) {
            try {
                if (time.IsEmpty()) {
                    room.Timeout(speaker, TimeSpan.FromSeconds(Random.Uint.Between(1, 600)));
                }
                else {
                    room.Timeout(speaker, TimeSpan.FromSeconds(Int32.Parse(time)));
                }
            }
            catch (Exception) {
                //*do nothing ever i want this to be (somewhat)secret*//
            }
        }
        private void BanYouBecauseYouAreStupidAndYouShouldntHaveDoneThat(TwitchUser speaker, String lastWords) { //yay i changed my ide ti keep the brackets on this line!
            room.SendChatMessage(".ban " + speaker.userName); //if this line gets ran you deserve it.
            room.SendChatMessage(speaker.userName + " is now banned. Everyone in the chat should type 'F' or riPepperonis to pay respects."); //lol jk dont do this
            lastWords = null; //noone cares so im deleting your words
        }//Oh GOD do i feel dirty....

        private void About(TwitchUser speaker, String additionalText) {
            if (additionalText != null && additionalText.Equals("sober")) {
                room.SendChatMessage(" //TODO get quote from sober"); //I NEED SOMEETHING FROM YOU SOBER ~1am
            }
            else {
                room.SendChatMessage("Hi, I am HardlyBot! I am a chatbot designed to host card games. I am currently open source and made by HardlySober @ http://bit.ly/1LUViNe ");
            }
        }

        private void HiCommand(TwitchUser speaker, String additionalText) {
            room.SendWhisper(speaker, "Hello");
        }
    }
}
