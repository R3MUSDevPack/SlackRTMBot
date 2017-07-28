using eZet.EveLib.EveXmlModule;
using R3MUS.Devpack.Core;
using R3MUS.Devpack.Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eZet.EveLib.EveXmlModule.Models.Character.UpcomingCalendarEvents;

namespace R3MUS.Devpack.SlackRTMBot
{
    public class BotResponse
    {
        public static string GenerateInsult()
        {
            Random r = new Random();
            int rInt = r.Next(0, 6);

            switch (rInt)
            {
                case 1:
                    return "{0}, your mother was a hamster & your father smelled of elderberries! Now go away or I shall taunt you a second time!";
                case 2:
                    return "I'd insult you back, {0}, but nature did a far better job :P";
                case 3:
                    return "I'm jealous of people that don't know you, {0}.";
                case 4:
                    return "You're the reason they need to put instructions on shampoo bottles.";
                case 5:
                    return "{0}: The reason the gene pool needs a lifeguard.";
                default:
                    return "You kiss your mother with that mouth???";
            }
        }

        public static string GetSkintasSong()
        {
            var song = new List<string>();
            song.Add("His name was Skinta,");
            song.Add("He was an FC,");
            song.Add("Heated railguns and a beer");
            song.Add("and a tiny bladder here.\r\n");
            song.Add("He likes to solo ");
            song.Add("and do a blops op");
            song.Add("Don't invite him round for tea,");
            song.Add("You'll get Copson and Ali P\r\n");
            song.Add("With hunters pre - assigned ");
            song.Add(" Keep range, we're not aligned! ");
            song.Add("You'll loose some ships but on zkillboard, ");
            song.Add(" Some dank kills you'll find! \r\n");
            song.Add("At the blops op, the r3mus bloooops op. ");
            song.Add("Torpedos and TPs get Chrispus all weepy ");
            song.Add("at the blooooops op...we whelp in style.");

            return string.Join("\r\n", song);
        }

        public static string GetTowers()
        {
            var sheet = new CharacterKey(4355544, "0knl1LoJnR1ycZqSaPUCB9iXF2fwqEfISLHQcbpzrCJD0uE5lMSKnbY7dzVoj9Yj");
            var events = GetCalendarEvents().Where(s =>
                s.EventTitle.ToLower().Contains("control tower")
                &&
                (s.EventDate - DateTime.UtcNow).Days <= 7
                ).Select(s => string.Concat(s.EventDateAsString, ": ", s.EventTitle)).ToArray<string>();
            if (events.Count() == 0)
            {
                return "AIN'T GOT NO TOWERS GOIN' OFFLINE, FOOL!";
            }
            else
            {
                return string.Join("\r\n", events);
            }
        }

        public static string GetCalendar(string type = "all")
        {
            var events = GetCalendarEvents().Where(s =>
                !s.EventTitle.ToLower().Contains("control tower")
                &&
                s.OwnerName.ToLower() != "ccp"
                &&
                s.OwnerName.ToLower() != "eve system"
                &&
                s.EventDate >= DateTime.UtcNow
                &&
                (type == "all" || s.EventTitle.ToLower().Contains(type))
                ).Take(5).Select(s => string.Concat(s.EventDateAsString, ": ", s.EventTitle)).ToArray<string>();
            if (events.Length > 0)
            {
                return string.Join("\r\n", events);
            }
            else
            {
                return "I don't see any events in the in-game calendar.";
            }
        }

								public static string GetBoardEvents()
								{
												var client = new Client() { UserName = Properties.Settings.Default.Email, Password = Properties.Settings.Default.Password };
												if(client.Logon())
												{
																var post = new Post() { content = "!ops", nonce = "340545009352704000", tts = false };
																client.PostMessage(Properties.Settings.Default.Channel, post);
																System.Threading.Thread.Sleep(500);
																var message = client.GetMessages(Properties.Settings.Default.Channel, 0).First();
																return message.content.Replace("```", "\n");
												}
												return "Cannot talk to Jarvis. Sorry old bean.";
								}

        public static string GetServerStatus()
        {
            var response = Web.BaseRequest("https://api.eveonline.com/Server/ServerStatus.xml.aspx");
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(eveapi));
            var rdr = new System.IO.StringReader(response);
            var responseObj = (eveapi)serializer.Deserialize(rdr);

            try
            {
                switch (Convert.ToBoolean(responseObj.result.serverOpen))
                {
                    case false:
                        return "Down";
                    case true:
                        return "Up";
                    default:
                        return "Buggered if I know";
                }
            }
            catch
            {
                return "Probably Down";
            }
        }

        public static string GetCommands()
        {
            var result = new List<string>();
            result.Add("The following commands are available:");
            foreach (string command in Enum.GetNames(typeof(Commands)))
            {
                result.Add(string.Concat("!", command));
            }
            return string.Join("\n", result);
        }

        public static List<Event> GetCalendarEvents()
        {
            var sheet = new CharacterKey(Properties.Settings.Default.APIKey, Properties.Settings.Default.VCode);
            return sheet.Characters.First().GetUpcomingCalendarEvents().Result.Events.ToList();
        }

        public static string GetCTACountdown()
        {
            var CTA = GetCalendarEvents().Where(w => w.EventTitle.ToLower().Contains("cta")).FirstOrDefault();
            if (CTA != null)
            {
                var tDays = (CTA.EventDate - DateTime.UtcNow).Days;
                var tHrs = (CTA.EventDate - DateTime.UtcNow).Hours;
                var tMins = (CTA.EventDate - DateTime.UtcNow).Minutes;
                var tSecs = (CTA.EventDate - DateTime.UtcNow).Seconds;
                return string.Format("{0} Days {1} Hours {2} Minutes {3} Seconds til the next CTA", tDays.ToString(), tHrs.ToString(), tMins.ToString(), tSecs.ToString());
            }
            else
            {
                return "No CTAs on the calendar.";
            }
        }
    }

    public enum Commands
    {
        ops,
								calendar,
								deploymentops,
        evetime,
        serverstatus,
        status,
        winninglotterynumbers,
        towers,
        help,
        commands,
        unbugger,
        moinlocation,
        vaslocation,
        ctacountdown,
        shouldiweartrouserstoday,
        joellocation,
        skintassong
    }

}
