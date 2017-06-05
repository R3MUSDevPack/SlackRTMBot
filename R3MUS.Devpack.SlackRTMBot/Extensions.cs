using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;

namespace R3MUS.Devpack.SlackRTMBot
{
    public static class Extensions
    {
        public static void SendMessage(this WebSocket ws, string msgText, string channelId)
        {
            Console.WriteLine(string.Concat(msgText, "\r\n"));
            var m = new MessageTx() { id = 999999, type = "message", channel = channelId, text = msgText };
            var msg = JsonConvert.SerializeObject(m);
            
            ws.Send(msg);
        }

        public static bool IsInsult(this string message)
        {
            var slagWords = new List<string>();
            slagWords.Add("ass");
            slagWords.Add("arse");
            slagWords.Add("fuck");
            slagWords.Add("bastard");
            slagWords.Add("shit");
            slagWords.Add("cunt");
            slagWords.Add("wanker");
            slagWords.Add("screw you");

            return slagWords.Any(a => message.ToLower().Contains(a));
        }

        public static bool IsLoveMessage(this string message)
        {
            return message.ToLower().Contains("i love you");
        }

        public static bool IsShutUp(this string message)
        {
            return message.ToLower().Contains("shut up");
        }
        public static bool IsThankYou(this string message)
        {
            return (message.ToLower().Contains("thanks") || message.ToLower().Contains("thank you"));
        }
    }

}
