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
using Leaf.xNet;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Account_Manager
{
    public partial class Form_ADD : Form
    {
        public static string data = string.Empty;
        public static string balance = string.Empty;
        public static string auth = string.Empty;
        public static string reauth = string.Empty;
        public static string UID = string.Empty;
        public static string dir = string.Empty;
        public Form_ADD()
        {
            InitializeComponent();
            comboBox1.Text = "NONE";
        }

        private void Form_ADD_Load(object sender, EventArgs e)
        {
            label_DEBUG_ADD.Text = "DEBUG";
        }

        private void button_ADD_ACC_Click(object sender, EventArgs e)
        {
            if (textBox_CC.Text != "" && textBox_CCV.Text != "" && textBox_EXP.Text != ""  && textBox_MAIL.Text != "" && textBox_NAME.Text != "" && textBox_PW.Text != "")
            {
                try
                {
                    HttpRequest requestOBJ = new HttpRequest();
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(textBox_MAIL.Text + ":" + textBox_PW.Text);
                    dir = String.Concat(Directory.GetCurrentDirectory(), @"\DATA\", System.Convert.ToBase64String(plainTextBytes));

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        Directory.CreateDirectory(dir + @"\Podcasts");
                        string response_requestOBJ = requestOBJ.Post("https://api.spreaker.com/oauth2/token", String.Concat("client_id=10&client_secret=QD4cn2JWnWK3sMswh59j6Bg5Gty43wRG&grant_type=password&password=", textBox_PW.Text, "&scope=basic%20spreaker&username=", textBox_MAIL.Text), "application/x-www-form-urlencoded").ToString();
                        auth = Regex.Match(response_requestOBJ, @"access_token"":""(.+?)""").Groups[1].Value;
                        reauth = Regex.Match(response_requestOBJ, @"refresh_token"":""(.+?)""").Groups[1].Value;

                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        UID = Regex.Match(requestOBJ.Get("https://api.spreaker.com/v2/me").ToString(), @"user_id"":(.+?),").Groups[1].Value;

                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        requestOBJ.Put("https://api.spreaker.com/v2/users/" + UID + "/revenuead?c=en_US&terms_accepted=true").ToString();

                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string revenue_data = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/revenuead?c=en_US").ToString();
                        balance = "0";
                        if (revenue_data.Contains("balance_amount"))
                        {
                            balance = Regex.Match(revenue_data, @"balance_amount"":(.+?),").Groups[1].Value;
                        }

                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string earnings_stats = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/revenuead/statistics?c=en_US&start_date=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-01&end_date=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString();

                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string download_stats = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/statistics/plays?c=en_US&from=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-01&to=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + "&group=day").ToString();

                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string episodes = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/shows?c=en_US&filter=editable&limit=500&export=show_stats,show_private,show_visibility,show_last_episode_at,show_author&access_level=AUTHOR").ToString();
                        MatchCollection matches = Regex.Matches(episodes, @"listenable_episodes_count"":(.+?),");
                        int episodes_num = 0;
                        foreach (Match match in matches)
                        {
                            episodes_num += Int32.Parse(Regex.Match(match.Value, @"listenable_episodes_count"":(.+?),").Groups[1].Value);
                        }

                        
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\auth.txt")))
                        {
                            sw.Write(auth + ":" + reauth);
                            sw.Close();
                        }
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\DATA.txt")))
                        {
                            sw.Write(textBox_NAME.Text + ":" + textBox_MAIL.Text + ":" + textBox_PW.Text + ":" + textBox_CC.Text + ":" + textBox_EXP.Text + ":" + textBox_CCV.Text);
                            sw.Close();
                        }
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\UID.txt")))
                        {
                            sw.Write(UID);
                            sw.Close();
                        }
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\Links.txt")))
                        {
                            sw.Write(textBox_LINKS.Text);
                            sw.Close();
                        }
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\Balance.txt")))
                        {
                            sw.Write(balance);
                            sw.Close();
                        }
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\stats_earnings.txt")))
                        {
                            sw.Write(earnings_stats);
                            sw.Close();
                        }
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\stats_downloads.txt")))
                        {
                            sw.Write(download_stats);
                            sw.Close();
                        }
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\episodes.txt")))
                        {
                            sw.Write(episodes_num);
                            sw.Close();
                        }
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\Genre.txt")))
                        {
                            sw.Write(comboBox1.Text);
                            sw.Close();
                        }
                        label_DEBUG_ADD.Text = "Added!";
                    }
                    else
                    {
                        label_DEBUG_ADD.Text = "Already in DB!";
                    }
                    



                }
                catch(Exception ex)
                {
                    if (ex.ToString().Contains("401"))
                    {
                        label_DEBUG_ADD.Text = "Invalid Login!";
                    }
                    else if (ex.ToString().Contains("429"))
                    {
                        label_DEBUG_ADD.Text = "Rate Limit!";
                    }
                    else
                    {
                        textBox_LINKS.AppendText(ex.ToString());
                        label_DEBUG_ADD.Text = "ERROR!";
                    }
                    if (!Directory.Exists(dir))
                    {
                        Directory.Delete(dir, true);
                    }
                }
            }
            else
            {
                label_DEBUG_ADD.Text = "Empty fields detected!";
            }
        }
    }
}
