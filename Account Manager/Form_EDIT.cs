using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using Leaf.xNet;
using System.Text.RegularExpressions;
using System.Threading;
using Account_Manager.Properties;
using System.Diagnostics;
using Chilkat;
using System.Net;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Html5;

namespace Account_Manager
{
    public partial class Form_EDIT : Form
    {
        public string pathrequestOBJ = string.Empty;
        public string auth = string.Empty;
        public string UID = string.Empty;
        public ChromeDriver driver;

        public Form_EDIT(string pathy, string uid)
        {
            InitializeComponent();
            pathrequestOBJ = pathy;
            auth = File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\auth.txt");
            UID = uid;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox_DEBUG.Clear();
            List<string> lines = File.ReadAllLines(@"DATA\" + pathrequestOBJ + @"\Links.txt").ToList<string>();
            for (int i = 0; i != lines.Count(); i++)
            {
                if (lines[i].Contains("spreaker.com/episode"))
                {
                    try
                    {
                        Chilkat.HttpRequest multiPart = new Chilkat.HttpRequest();
                        multiPart.HttpVerb = "POST";
                        multiPart.Path = "v2/" + lines[i].Split('/')[3] + "s/" + lines[i].Split('/')[4] + "/cuepoints?c=en_US";
                        multiPart.ContentType = "multipart/form-data";
                        multiPart.AddHeader("authorization", "Bearer " + auth.Split(':')[0]);
                        multiPart.AddParam("cuepoints", @"[{""timecode"":60000,""ads_max_count"":3},{""timecode"":120000,""ads_max_count"":3}]");
                        Chilkat.Http http = new Chilkat.Http();
                        Chilkat.HttpResponse fafafafa = http.SynchronousRequest("api.spreaker.com", 443, true, multiPart);
                        richTextBox_DEBUG.AppendText(fafafafa.BodyStr + "\n");
                    }
                    catch (Exception ex)
                    {
                        richTextBox_DEBUG.AppendText(ex.ToString() + "\n");
                    }
                }
            }
            richTextBox_DEBUG.AppendText("ADDED ADS");
        }

        private void button_GETLINKS_Click(object sender, EventArgs e)
        {
            richTextBox_DEBUG.Clear();
            try
            {
                List<string> links = new List<string>();
                Leaf.xNet.HttpRequest requestOBJ = new Leaf.xNet.HttpRequest();
                requestOBJ.AddHeader("authorization", "Bearer " + auth.Split(':')[0]);
                string response_shows = requestOBJ.Get("https://api.spreaker.com/v2/users/"+UID+"/shows?c=en_US&filter=editable&limit=50&access_level=AUTHOR").ToString();
                MatchCollection matches = Regex.Matches(response_shows, @"show_id"":(.+?),");
                foreach (Match match3 in matches)
                {
                    requestOBJ.AddHeader("authorization", "Bearer " + auth.Split(':')[0]);
                    string response = requestOBJ.Get("https://api.spreaker.com/v2/shows/" + Regex.Match(match3.Value, @"show_id"":(.+?),").Groups[1].Value + "/episodes?c=en_US&filter=editable&limit=50").ToString();
                    MatchCollection matches3 = Regex.Matches(response, @"site_url"":""(.+?)""");
                    foreach (Match match4 in matches3)
                    {
                        links.Add(Regex.Match(match4.Value, @"site_url"":""(.+?)""").Groups[1].Value.Replace(@"\/", "/"));
                        richTextBox_DEBUG.AppendText(Regex.Match(match4.Value, @"site_url"":""(.+?)""").Groups[1].Value.Replace(@"\/", "/") + "\n");
                    }
                }
                File.WriteAllLines(@"DATA\" + pathrequestOBJ + @"\Links.txt", links);
            }
            catch (Exception ex)
            {
                richTextBox_DEBUG.AppendText(ex.ToString() + "\n");
            }
        }

        private void button_CHROME_Click(object sender, EventArgs e)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--mute-audio");
            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);
            var chromeDriverService = ChromeDriverService.CreateDefaultService("Chrome");
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            driver = new ChromeDriver(chromeDriverService, chromeOptions);
            driver.Navigate().GoToUrl("https://www.spreaker.com/cms/shows");
            while (true)
            {
                try
                {
                    if (driver.FindElement(By.Id("login-form-submit")).Displayed)
                    {
                        Thread.Sleep(500);
                        break;
                    }
                }
                catch
                {

                }
            }
            var base64EncodedBytes = System.Convert.FromBase64String(pathrequestOBJ);
            driver.FindElement(By.Id("identity")).SendKeys(System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split(':')[0]);
            driver.FindElement(By.Id("password")).SendKeys(System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split(':')[1]);
            Thread.Sleep(100);
            driver.FindElement(By.Id("login-form-submit")).Click();
        }

        private void button_Kill_Click(object sender, EventArgs e)
        {
            try
            {
                driver.Quit();
            }
            catch
            {

            }
        }
    }
}
