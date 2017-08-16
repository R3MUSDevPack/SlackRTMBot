using R3MUS.Devpack.Core;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;

namespace R3MUS.Devpack.SlackRTMBot
{
    class Program
    {
        ResponseRoot start;
        string baseStartUrl = "https://slack.com/api/rtm.start/?token=";
        WebSocket connection;
        Channel denDeets;
        Group groupDeets;

        static void Main(string[] args)
        {
            try
            {
                var p = new Program();
                while (true)
                {
                    try
                    {
                        Task t = p.ConnectionRun();
                        t.Wait();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.InnerException.Message);
                        System.Threading.Thread.Sleep(10000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                Console.ReadLine();
            }            
        }

        public Program()
        {
            start = (ResponseRoot)Web.BaseRequest(string.Concat(baseStartUrl, Properties.Settings.Default.Token)).Deserialize(typeof(ResponseRoot));
        }

        public async Task ConnectionRun()
        {
			Start();
        }

		private void Start()
		{
			denDeets = start.channels.First(f => f.name.Equals("den"));
			groupDeets = start.groups.First(f => f.name.Equals("it_testing"));

			connection = new WebSocket(start.url);
			connection.Opened += new EventHandler(connection_Opened);
			connection.Error += new EventHandler<ErrorEventArgs>(connection_Error);
			connection.Closed += new EventHandler(connection_Closed);
			connection.MessageReceived += new EventHandler<MessageReceivedEventArgs>(connection_MessageReceived);
			connection.Open();
			Console.ReadLine();
		}

        private void connection_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Connection Established");
        }
        private void connection_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection Closed");
			Start();
        }
        private void connection_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var responseObj = (BaseType)e.Message.Deserialize(typeof(BaseType));
                if (responseObj.type == "message")
                {
                    HandleMessage(e.Message);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void connection_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
            connection.Close();
            ConnectionRun();
        }

        private void HandleMessage(string rcv)
        {
            var message = (MessageRx)rcv.Deserialize(typeof(MessageRx));

            if ((message.text != null) && (message.text.StartsWith("!")))
            {
                if (message.channel.StartsWith("G"))
                {
                    message.Channel = start.groups.First(f => f.id == message.channel).name;
                }
                else
                {
                    message.Channel = start.channels.First(f => f.id == message.channel).name;
                }

                Console.WriteLine(string.Concat(
                    DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"), ": ",
                    message.Channel, ": ",
                    message.User, ":  ",
                    message.text));

                var command = message.text.Split(' ').First().Replace("!", "");

                message.User = start.users.First(f => f.id == message.user).real_name;

                if (message.text.StartsWith("!") && BotResponse.GetCommands().Contains(command))
                {
                    switch (((Commands)Enum.Parse(typeof(Commands), command)))
                    {
                        case Commands.ops:
							connection.SendMessage("Moment please!", message.channel);
							var response = string.Empty;
							while(response == string.Empty && ((Commands)Enum.Parse(typeof(Commands), response)) == Commands.ops)
							{
								response = BotResponse.GetBoardEvents();
							}
							connection.SendMessage(BotResponse.GetBoardEvents(), message.channel);
						break;
						case Commands.calendar:
							connection.SendMessage(string.Concat("*Eve Time is now ", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "*\r\n", BotResponse.GetCalendar()), message.channel);
						break;
                        case Commands.deploymentops:
                            connection.SendMessage(string.Concat("*Eve Time is now ", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "*\r\n", BotResponse.GetCalendar("deployment")), message.channel);
                            break;
                        case Commands.evetime:
                            connection.SendMessage(string.Format("The time, at the 3rd beep, will be {0}. Beep. Beep. Beep.", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")), message.channel);
                            break;
                        case Commands.serverstatus:
                            connection.SendMessage(string.Format("TQ status: {0}", BotResponse.GetServerStatus()), message.channel);
                            break;
                        case Commands.status:
                            connection.SendMessage(string.Format("I'm fine thank you. How are you?"), message.channel);
                            break;
                        case Commands.winninglotterynumbers:
                            connection.SendMessage(string.Format("42 & 69"), message.channel);
                            break;
                        case Commands.towers:
                            connection.SendMessage(BotResponse.GetTowers(), message.channel);
                            break;
                        case Commands.commands:
                            connection.SendMessage(BotResponse.GetCommands(), message.channel);
                            break;
                        case Commands.help:
                            connection.SendMessage(string.Format("I don't think I'm qualified to give you the help you need.", command), message.channel);
                            break;
                        case Commands.unbugger:
                            connection.SendMessage(string.Format("I'm sorry Dave, but I can't let you do that."), message.channel);
                            break;
                        case Commands.moinlocation:
                            connection.SendMessage(string.Format("His head is up his arse, second shelf on the right."), message.channel);
                            break;
                        case Commands.vaslocation:
                            connection.SendMessage(string.Format("'Polishing his Erebus'."), message.channel);
                            break;
                        case Commands.ctacountdown:
                            connection.SendMessage(BotResponse.GetCTACountdown(), message.channel);
                            break;
                        case Commands.joellocation:
                            connection.SendMessage("'Looking for his banana.'", message.channel);
                            break;
                        case Commands.shouldiweartrouserstoday:
                            connection.SendMessage(string.Format("Yes {0}, the judge ordered you to do so.", message.User), message.channel);
                            break;
                        case Commands.skintassong:
                            connection.SendMessage(BotResponse.GetSkintasSong(), message.channel);
                            break;
                        default:
                            connection.SendMessage(string.Format("You're a '{0}'", command), message.channel);
                            break;
                    }
                }
                else
                {
                    connection.SendMessage(string.Format("I have no idea what '{0}' means", command), message.channel);
                }
            }
            else if (message.text.ToLower().Contains("slackbot") || message.text.ToLower().Split(' ').Contains("hal"))
            {
                message.User = start.users.First(f => f.id == message.user).real_name;

                if (true == false)// (message.User.ToLower().Equals("vasic"))
                {
                    connection.SendMessage(string.Format(BotResponse.GenerateInsult(), message.User), message.channel);
                }
                else
                {
                    if (message.channel.StartsWith("G"))
                    {
                        message.Channel = start.groups.First(f => f.id == message.channel).name;
                    }
                    else
                    {
                        message.Channel = start.channels.First(f => f.id == message.channel).name;
                    }
                    Console.WriteLine(string.Concat(
                        DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"), ": ",
                        message.Channel, ": ",
                        message.User, ":  ",
                        message.text));

                    if (message.text.IsShutUp())
                    {
                        connection.SendMessage(string.Concat("Come & make me, ", message.User), message.channel);
                    }
                    else if (message.text.IsLoveMessage())
                    {
                        connection.SendMessage(string.Format("I love you too, {0}, but I feel we should still see other people", message.User), message.channel);
                    }
                    else if (message.text.IsInsult())
                    {
                        connection.SendMessage(string.Format(BotResponse.GenerateInsult(), message.User), message.channel);
                    }
                    else if (message.text.IsThankYou())
                    {
                        connection.SendMessage(string.Format("You're very welcome, {0}!", message.User), message.channel);
                    }
                }
            }
        }
    }        
}