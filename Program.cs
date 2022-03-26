using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Registry
{
	public class Program
	{
		[DllImport("user32.dll")]
		static extern void MessageBox(int hwnd, string caption, string title, int flag);
		private static void Main()
		{
			Console.Title = "Registry";
			Console.SetWindowSize(80, 20);
			ServicePointManager.UseNagleAlgorithm = false;
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.CheckCertificateRevocationList = false;
			ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
			Program.LoadFile("Proxies", "Proxy.txt", ref Program.Proxies);
			Program.LoadFile("Usernames", "User.txt", ref Program.Targets);
			MakeEmailsForEachUser();
			
			try
			{
				Program.r = new StreamReader("Settings.json");
				Program.jsonString = Program.r.ReadToEnd();
				Program.settingsModel = JsonConvert.DeserializeObject<SettingsModel>(Program.jsonString);
			}
			catch
			{
				File.WriteAllText("Settings.json", "\r\n{\r\n\"Title\":\"brave.\",\r\n\"Message\":\"barve catch it ..\",\r\n\"Threads\":10,\r\n\"WebHook\":\"https://discord.com/api/webhooks/866406324137099274/XBtzr81E2VIOa4onJHJB9_OEZDluTC9K4a4dU-xDCBnfV-Sd3ihv6i3ZXUaVXxJvQPAL\",\r\n}");
				Program.r = new StreamReader("Settings.json");
				Program.jsonString = Program.r.ReadToEnd();
				Program.settingsModel = JsonConvert.DeserializeObject<SettingsModel>(Program.jsonString);
			}
			Program.Success("Settings Loaded Successfully");
			Console.Title = Program.settingsModel.Title;
			Program.Shuffle<string>(ref Program.Proxies);
			Program.session = Program.Input("Enter Session : ");
			new Thread(() => ConfirmAllEmails()).Start();
			Program.Password = Program.Input("Enter Password : ");
			string text = "";
			try
			{
				text = Program.settingsModel.Threads.ToString();
				bool flag = text == "0";
				if (flag)
				{
					text = Program.Input("Enter Threads : ");
				}
			}
			catch
			{
				text = Program.Input("Enter Threads : ");
			}
			bool flag2 = !string.IsNullOrEmpty(text);
			if (flag2)
			{
				Program.Threads = int.Parse(text);
			}
			while (Program.Emails.Count < 1)
            {
				Console.Write("\r[+] Please wait... Emails Processing...\r");
            }
			Thread thread = new Thread(new ThreadStart(Program.Threading));
			thread.Start();
			Task[] array = new Task[1];
			array[0] = Task.Run(delegate ()
			{
				Program.Title();
			});
			Task.WaitAny(array);
		}

		private static void Threading()
		{
			for (int i = 0; i <= Program.Threads; i++)
			{
				new Thread(delegate ()
				{
					Program.JustLoop();
				})
				{
					Priority = ThreadPriority.Highest
				}.Start();
				Thread.Sleep(10);
			}
		}

		private static void JustLoop()
		{
			while (Program.Stop)
			{
				try
				{
					Program.ProxiesManager++;
					bool flag = Program.ProxiesManager >= Enumerable.Count<string>(Program.Proxies);
					if (flag)
					{
						Program.ProxiesManager = 0;
					}
					Program.CheckUsers(Program.Proxies[Program.ProxiesManager]);
				}
				catch (Exception)
				{
				}
				Thread.Sleep(10);
			}
		}

		public static string encodebase64(string id, string session)
		{
			string s = string.Concat(new string[]
			{
				"{\"ds_user_id\":\"",
				id,
				"\",\"sessionid\":\"",
				session,
				"\",\"should_use_header_over_cookies\":true}"
			});
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			return Convert.ToBase64String(bytes);
		}

		public static void MakeEmailsForEachUser()
        {
			string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
			for (int i = 0; i < Program.Targets.Count; i++)
            {
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("registry");
				for (int j = 0; j < 8; j++)
                {
					stringBuilder.Append(chars[new Random().Next(chars.Length - 1)]);
                }
				stringBuilder.Append("@gmail.com");
				Program.Emails.Add(stringBuilder.ToString());
            }
        }

		public static void ConfirmAllEmails()
		{
			foreach (string email in Program.Emails)
            {
				try
				{
					string value = Regex.Match(Program.session, "(.*?)%").Groups[1].Value;
					byte[] bytes = new UTF8Encoding().GetBytes("email=" + email);
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://i.instagram.com/api/v1/accounts/send_confirm_email/");
					httpWebRequest.Method = "POST";
					httpWebRequest.Proxy = new WebProxy(Program.Proxies[new Random().Next(Program.Proxies.Count - 1)]);
					httpWebRequest.Headers.Add("Cookie", "sessionid=" + Program.session);
					httpWebRequest.UserAgent = "Instagram 207.0.0.39.120 Android (25/7.1.2; 240dpi; 1280x720; samsung; SM-N975F; SM-N975F; samsungexynos9825; en_US; 321039115)";
					httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
					httpWebRequest.ContentLength = (long)bytes.Length;
					Stream requestStream = httpWebRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Dispose();
					requestStream.Close();
					HttpWebResponse httpWebResponse;
					try
					{
						httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					}
					catch (WebException ex)
					{
						httpWebResponse = (HttpWebResponse)ex.Response;
					}
					bool flag = httpWebResponse != null;
					if (flag)
					{
						StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
						string text = streamReader.ReadToEnd().ToString();
						bool flag2 = text.Contains(email);
						if (flag2)
						{
							Program.Success("Everything right .");
						}
						streamReader.Dispose();
						streamReader.Close();
					}
				}
				catch (Exception)
				{
				}
				Thread.Sleep(10);
			}
			Program.Success("Emails Loaded Successfully");
		}

		private static void CheckUsers(string proxy)
		{
			while (true)
			{
				try
				{
					string value = Regex.Match(Program.session, "(.*?)%").Groups[1].Value;
					string target = Program.Targets[new Random().Next(Targets.Count - 1)];
					string email = Program.Emails[new Random().Next(Emails.Count - 1)];
					byte[] bytes = new UTF8Encoding().GetBytes(string.Format("suggestedUsername=&should_copy_consent_and_birthday_from_main=true&main_user_authorization_token=Bearer IGT:2:{0}&phone_id={1}&enc_password=#PWD_INSTAGRAM:0:0:{2}&username={3}&first_name=brave.&adid={4}&guid={5}&device_id=android-{6}&main_user_id={7}&force_sign_up_code=&waterfall_id={8}&one_tap_opt_in=true&should_link_to_main=false", new object[]
					{
						Program.encodebase64(value, Program.session),
						Guid.NewGuid(),
						Program.Password,
						target,
						Guid.NewGuid(),
						Guid.NewGuid(),
						Program.RandomString(16),
						value,
						Guid.NewGuid()
					}));
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://i.instagram.com/api/v1/multiple_accounts/create_secondary_account/");
					httpWebRequest.Method = "POST";
					httpWebRequest.Proxy = new WebProxy(proxy);
					httpWebRequest.Headers.Add("Cookie", "ds_user_id=" + Program.RandomString(15) + ";");
					httpWebRequest.UserAgent = "Instagram 207.0.0.39.120 Android (25/7.1.2; 240dpi; 1280x720; samsung; SM-N975F; SM-N975F; samsungexynos9825; en_US; 321039115)";
					httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
					httpWebRequest.ContentLength = (long)bytes.Length;
					Stream requestStream = httpWebRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Dispose();
					requestStream.Close();
					HttpWebResponse httpWebResponse;
					try
					{
						httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					}
					catch (WebException ex)
					{
						httpWebResponse = (HttpWebResponse)ex.Response;
					}
					bool flag = httpWebResponse != null;
					if (flag)
					{
						StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
						string text = streamReader.ReadToEnd().ToString();
						string text2 = text;
						bool flag2 = text2.Contains("This username");
						if (flag2)
						{
							Program.Counter++;
							continue;
						}
						bool flag3 = text.Contains("wait") | text.Contains("fail") | httpWebResponse.StatusCode == (HttpStatusCode)429;
						if (flag3)
						{
							Program.Failed++;
						}
						else
						{
							bool flag4 = text.Contains("challenge_required") | text.Contains("pk");
							if (flag4)
							{
								Program.Success("@" + target + " " + Program.settingsModel.Message);
								using (StreamWriter streamWriter = new StreamWriter("@" + Program.Target + ".txt", false))
								{
									streamWriter.WriteLine("Username: " + target);
									streamWriter.WriteLine("Email: " + email);
									streamWriter.WriteLine("Password: " + Program.Password);
									streamWriter.WriteLine("Session: " + Program.session);
									streamWriter.Dispose();
									streamWriter.Close();
								}
								Program.Targets.Remove(target);
								Program.WebHook(target);
								//Program.Stop = false;
								new Thread(() =>
								{
									MessageBox(0, "@" + target + " " + Program.settingsModel.Message, Program.settingsModel.Title, 0);
								});
							}
						}
						streamReader.Dispose();
						streamReader.Close();
					}
				}
				catch (Exception)
				{
				}
				Thread.Sleep(10);
				//break;
			}
		}

		private static void WebHook(string username)
		{
			bool flag = username.Length <= 6;
			if (flag)
			{
				byte[] bytes = new UTF8Encoding().GetBytes(string.Concat(new string[]
				{
					"{\"content\": null,\"embeds\": [{\"title\": \"Notify\",\"description\": \"@",
					username,
					" ",
					Program.settingsModel.Message,
					"\",\"footer\": { \"text\": \"",
					Program.settingsModel.Title,
					"\"},\"color\": 4460573}]}"
				}));
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Program.settingsModel.WebHook);
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.ContentLength = (long)bytes.Length;
				Stream requestStream = httpWebRequest.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Dispose();
				requestStream.Close();
			}
		}

		private static void Title()
		{
			while (Program.Stop)
			{
				int counter = Program.Counter;
				Thread.Sleep(1000);
				Program.RS = Program.Counter - counter;
				Console.Title = string.Format("{0} / Right'R [{1}] | RL [{2}] | RS [{3}]", new object[]
				{
					Program.settingsModel.Title,
					Program.Counter,
					Program.Failed,
					Program.RS
				});
				GC.Collect();
			}
		}

		private static string RandomString(int length)
		{
			return new string(Enumerable.ToArray<char>(Enumerable.Select<string, char>(Enumerable.Repeat<string>("abcdefghijkmnopqrstuvwxyz0123456789".ToLower(), length), (string c) => c[Program.rn.Next(c.Length)])));
		}

		private static void LoadFile(string name, string path, ref List<string> list)
		{
			try
			{
				list = Enumerable.ToList<string>(File.ReadAllLines(path));
			}
			catch
			{
				list = Enumerable.ToList<string>(File.ReadAllLines(Program.Input(name + " Path :")));
			}
			bool flag = list.Count <= 0;
			if (flag)
			{
				Program.Fail(name + " empty file");
				Thread.Sleep(2500);
				Environment.Exit(1);
			}
			else
			{
				Program.Success(name + " Loaded Successfully");
			}
		}

		public static void Shuffle<T>(ref List<T> list)
		{
			int i = list.Count;
			while (i > 1)
			{
				i--;
				int index = Program.rn.Next(i + 1);
				T value = list[index];
				list[index] = list[i];
				list[i] = value;
			}
		}

		private static void Print(string text, string ic, ConsoleColor first, ConsoleColor sec, bool input = false)
		{
			Console.ResetColor();
			Console.ForegroundColor = first;
			Console.Write("[");
			Console.ForegroundColor = sec;
			Console.Write(ic);
			Console.ForegroundColor = first;
			Console.Write("] ");
			Console.Write(text);
			Console.ForegroundColor = sec;
			bool flag = !input;
			if (flag)
			{
				Console.WriteLine();
			}
		}

		private static string Input(string text)
		{
			Program.Print(text, "+", ConsoleColor.Cyan, ConsoleColor.White, true);
			return Console.ReadLine();
		}

		private static void Fail(string text)
		{
			Program.Print(text, "!", ConsoleColor.Red, ConsoleColor.White, false);
		}

		private static void Warn(string text)
		{
			Program.Print(text, "!", ConsoleColor.Yellow, ConsoleColor.White, false);
		}

		private static void Success(string text)
		{
			Program.Print(text, "+", ConsoleColor.Green, ConsoleColor.White, false);
		}

		private static void DefaultPrint(string text)
		{
			Program.Print(text, "+", ConsoleColor.Cyan, ConsoleColor.White, false);
		}

		public static string jsonString = "";

		public static string Target = "";

		public static string Email = "";

		public static string Password = "";

		public static string session = "";

		public static Random rn = new Random();

		public static List<string> Proxies = new List<string>();

		public static List<string> Targets = new List<string>();

		public static List<string> Emails = new List<string>();

		public static int ProxiesManager = 0;

		public static int Counter = 0;

		public static int Failed = 0;

		public static int RS = 0;

		public static int Threads = 0;

		public static StreamReader r;

		public static SettingsModel settingsModel;

		public static bool Stop = true;
	}

}

