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

namespace Account_Manager
{
    public partial class MainWindow : Form
    {
        public static string pathrequestOBJ = string.Empty;
        public static string data = string.Empty;
        public static string balance = string.Empty;
        public static string auth = string.Empty;
        public static string reauth = string.Empty;
        public static string UID = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            comboBox_ALL_COPY_FILTER.Text = "spreaker.com/episode";
            
        }
        private static DateTime ConvertToTime(string time)
        {
            return DateTime.Parse(time.Replace(" - ", "/"));
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            int accs_amount = 0;
            int accs_amount_dead = 0;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.WindowState = FormWindowState.Maximized;
            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory() + @"\DATA");
            var di2 = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\DATA");
            var ordered = di2.EnumerateDirectories().OrderBy(x => x.CreationTime).Select(d => d.Name);
            foreach (string dir in ordered)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                var base64EncodedBytes = System.Convert.FromBase64String(di.Name);
                TreeNode node = new TreeNode(System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split(':')[0], 0, 1);
                if (!File.Exists(@"DATA\" + di.Name + @"\STATUS.txt"))
                {
                    using (StreamWriter sw = (File.CreateText(@"DATA\" + di.Name + @"\STATUS.txt")))
                    {
                        sw.Write("ENABLED:");
                        sw.Close();
                    }
                }
                if (File.ReadAllText(@"DATA\" + di.Name + @"\STATUS.txt").Contains("DISABLED"))
                {
                    node.ForeColor = Color.Red;
                    accs_amount_dead++;
                }
                else
                {
                    node.ForeColor = Color.GreenYellow;
                    accs_amount++;
                }
                try
                {
                    node.Tag = di.Name;
                }
                catch (UnauthorizedAccessException)
                {
                    node.ImageIndex = 12;
                    node.SelectedImageIndex = 12;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DirectoryLister",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    treeView_DATA.Nodes.Add(node);
                }
            }
            label_ACCS_AMOUNT.Text = accs_amount.ToString();
            label_ACCS_AMOUNT_DEAD.Text = accs_amount_dead.ToString();
        }

        private void treeView_DATA_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            
            pathrequestOBJ = e.Node.Tag.ToString();

            label_PAYPAL_TEXT.Text = "...";

            chart_Earnings.Series["Dollar"].Points.Clear();
            chart_Downloads.Series["Downloads"].Points.Clear();
            chart_Impressions.Series["Impressions"].Points.Clear();

            chart_Earnings.Series["Dollar"].SetCustomProperty("LabelStyle", "Top");
            chart_Downloads.Series["Downloads"].SetCustomProperty("LabelStyle", "Top");
            chart_Impressions.Series["Impressions"].SetCustomProperty("LabelStyle", "Top");

            chart_Earnings.Series["Dollar"].SetCustomProperty("LabelStyle", "Top");
            chart_Downloads.Series["Downloads"].SetCustomProperty("LabelStyle", "Top");
            chart_Impressions.Series["Impressions"].SetCustomProperty("LabelStyle", "Top");

            chart_Earnings.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Orange;
            chart_Downloads.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Orange;
            chart_Impressions.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Orange;
            chart_Earnings.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Orange;
            chart_Downloads.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Orange;
            chart_Impressions.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Orange;

            chart_Earnings.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            chart_Downloads.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            chart_Impressions.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            chart_Earnings.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            chart_Downloads.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            chart_Impressions.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            label_STATUS.Text = File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\STATUS.txt").Split(':')[0];
            label_CASE.Text = File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\STATUS.txt").Split(':')[1];
            if (!File.Exists(@"DATA\" + pathrequestOBJ + @"\STATUS.txt"))
            {
                using (StreamWriter sw = (File.CreateText(@"DATA\" + pathrequestOBJ + @"\STATUS.txt")))
                {
                    sw.Write("ENABLED:");
                    sw.Close();
                }
            }
            if (!File.Exists(@"DATA\" + pathrequestOBJ + @"\stats_earnings.txt"))
            {
                using (StreamWriter sw = (File.CreateText(@"DATA\" + pathrequestOBJ + @"\stats_earnings.txt")))
                {
                    sw.Write("");
                    sw.Close();
                }
            }
            if (!File.Exists(@"DATA\" + pathrequestOBJ + @"\stats_downloads.txt"))
            {
                using (StreamWriter sw = (File.CreateText(@"DATA\" + pathrequestOBJ + @"\stats_downloads.txt")))
                {
                    sw.Write("");
                    sw.Close();
                }
            }
            if (!File.Exists(@"DATA\" + pathrequestOBJ + @"\episodes.txt"))
            {
                using (StreamWriter sw = (File.CreateText(@"DATA\" + pathrequestOBJ + @"\episodes.txt")))
                {
                    sw.Write("0");
                    sw.Close();
                }
            }
            if (!File.Exists(@"DATA\" + pathrequestOBJ + @"\Genre.txt"))
            {
                using (StreamWriter sw = (File.CreateText(@"DATA\" + pathrequestOBJ + @"\Genre.txt")))
                {
                    sw.Write("NONE");
                    sw.Close();
                }
            }
            MatchCollection matches = Regex.Matches(File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\stats_earnings.txt"), @"impressions_sold"":(.+?)}");
            int index = 1;
            int cpm_balance = 0;
            foreach (Match match in matches)
            {
                string cash = "0";
                if (Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value.ToString() != "null")
                {
                    cash = Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value;
                }
                chart_Earnings.Series["Dollar"].Points.AddXY(index, cash);
                chart_Earnings.Series["Dollar"].Points[index - 1].LabelForeColor = Color.YellowGreen;
                chart_Earnings.Series["Dollar"].Points[index - 1].Font = new System.Drawing.Font("Arial", 12);
                cpm_balance = cpm_balance + Int32.Parse(Math.Round(double.Parse(Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture), 0, MidpointRounding.AwayFromZero).ToString());
                if (Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value.ToString() != "null")
                {
                    chart_Earnings.Series["Dollar"].Points[index - 1].Label = Math.Round(double.Parse(Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture), 2, MidpointRounding.AwayFromZero).ToString();

                }
                else
                {
                    chart_Earnings.Series["Dollar"].Points[index - 1].Label = "0";

                }
                chart_Impressions.Series["Impressions"].Points.AddXY(index, Regex.Match(match.Value, @"_sold"":(.+?),").Groups[1].Value.ToString());
                chart_Impressions.Series["Impressions"].Points[index - 1].LabelForeColor = Color.YellowGreen;
                chart_Impressions.Series["Impressions"].Points[index - 1].Font = new System.Drawing.Font("Arial", 12);
                chart_Impressions.Series["Impressions"].Points[index - 1].Label = Regex.Match(match.Value, @"_sold"":(.+?),").Groups[1].Value.ToString();

                index++;
            }

            MatchCollection matches2 = Regex.Matches(File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\stats_downloads.txt"), @"plays_count"":(.+?)}");
            int index2 = 1;
            foreach (Match match2 in matches2)
            {
                int value = Int32.Parse(Regex.Match(match2.Value, @"plays_count"":(.+?),").Groups[1].Value.ToString()) + Int32.Parse(Regex.Match(match2.Value, @"downloads_count"":(.+?)}").Groups[1].Value.ToString());
                chart_Downloads.Series["Downloads"].Points.AddXY(index2, value);
                chart_Downloads.Series["Downloads"].Points[index2 - 1].LabelForeColor = Color.YellowGreen;
                chart_Downloads.Series["Downloads"].Points[index2 - 1].Font = new System.Drawing.Font("Arial", 12);
                chart_Downloads.Series["Downloads"].Points[index2 - 1].Label = value.ToString();

                index2++;
            }

            label_EPISODES_VALUE.Text = File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\episodes.txt");


            label_DEBUG.Text = "DEBUG";

            auth = File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\auth.txt").Split(':')[0];
            reauth = File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\auth.txt").Split(':')[1];
            UID = File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\UID.txt");
            data = File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\DATA.txt");
            balance = File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\Balance.txt");
            var base64EncodedBytes = System.Convert.FromBase64String(pathrequestOBJ);
            richTextBox_SPREAKER.Clear();
            richTextBox_Spotify.Clear();
            richTextBox_Podchaser.Clear();
            richTextBox_Podcast_Addict.Clear();
            richTextBox_JioSaavn.Clear();
            richTextBox_IHeart_Radio.Clear();
            richTextBox_Google.Clear();
            richTextBox_Deezer.Clear();
            richTextBox_Castbox.Clear();
            richTextBox_Apple.Clear();
            richTextBox_Amazon.Clear();
            label_GENRE.Text = File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\Genre.txt");
            label_Email.Text = e.Node.Text;
            label_Password.Text = System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split(':')[1];

            label_Balance.Text = balance + "$";
            int impressions = 0;
            MatchCollection matchess = Regex.Matches(File.ReadAllText(@"DATA\" + pathrequestOBJ + @"\stats_earnings.txt"), @"impressions_sold"":(.+?)}");
            foreach (Match match in matchess)
            {
                impressions += Int32.Parse(Regex.Match(match.Value, @"_sold"":(.+?),").Groups[1].Value.ToString());
            }

            label_CPM.Text = Math.Round((Convert.ToDouble(cpm_balance) * 1000 / impressions), 6, MidpointRounding.AwayFromZero).ToString() + "$/k";

            List<string> lines = File.ReadAllLines(@"DATA\" + pathrequestOBJ + @"\Links.txt").ToList<string>();
            for (int i = 0; i != lines.Count(); i++)
            {
                if (lines[i].Contains("spreaker.com/episode"))
                {
                    richTextBox_SPREAKER.AppendText(lines[i]);
                    if (i < lines.Count())
                    {
                        richTextBox_SPREAKER.AppendText("\n");
                    }
                }
                else if (lines[i].Contains("spotify.com"))
                {
                    richTextBox_Spotify.AppendText(lines[i]);
                    if (i < lines.Count())
                    {
                        richTextBox_Spotify.AppendText("\n");
                    }
                }
                else if (lines[i].Contains("podcastaddict.com"))
                {
                    richTextBox_Podcast_Addict.AppendText(lines[i]);
                    if (i < lines.Count())
                    {
                        richTextBox_Podcast_Addict.AppendText("\n");
                    }
                }
                else if (lines[i].Contains("podchaser.com"))
                {
                    richTextBox_Podchaser.AppendText(lines[i]);
                    if (i < lines.Count())
                    {
                        richTextBox_Podchaser.AppendText("\n");
                    }
                }
                else if (lines[i].Contains(""))
                {
                    richTextBox_JioSaavn.AppendText(lines[i]);
                    if (i < lines.Count())
                    {
                        richTextBox_JioSaavn.AppendText("\n");
                    }
                }
                else if (lines[i].Contains(""))
                {
                    richTextBox_IHeart_Radio.AppendText(lines[i]);
                    if (i < lines.Count())
                    {
                        richTextBox_IHeart_Radio.AppendText("\n");
                    }
                }
                else if (lines[i].Contains(""))
                {
                    richTextBox_Google.AppendText(lines[i]);
                    if (i < lines.Count())
                    {
                        richTextBox_Google.AppendText("\n");
                    }
                }
                else if (lines[i].Contains(""))
                {
                    richTextBox_Deezer.AppendText(lines[i]);
                    if (i < lines.Count())
                    {
                        richTextBox_Deezer.AppendText("\n");
                    }
                }
                else if (lines[i].Contains(""))
                {
                    richTextBox_Castbox.AppendText(lines[i]);
                    if (i < lines.Count())
                    {
                        richTextBox_Castbox.AppendText("\n");
                    }
                }
                else if (lines[i].Contains(""))
                {
                    richTextBox_Apple.AppendText(lines[i]);
                    if (i < lines.Count())
                    {
                        richTextBox_Apple.AppendText("\n");
                    }
                }
                else
                {
                    richTextBox_Amazon.AppendText(lines[i]);
                    if (i < lines.Count())
                    {
                        richTextBox_Amazon.AppendText("\n");
                    }
                }
            }
        }

        private void label_Email_MouseClick(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(label_Email.Text);
        }

        private void label_Password_MouseClick(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(label_Password.Text);
        }

        // API UPDATE //
        private void button_API_UPDATE_Click(object sender, EventArgs e)
        {
            if (auth == "")
            {

            }
            else
            {
                while (true)
                {
                    try
                    {
                        Leaf.xNet.HttpRequest requestOBJ = new Leaf.xNet.HttpRequest();

                        requestOBJ.AddHeader("authorization", "Bearer " + reauth);
                        string response_auth = requestOBJ.Post("https://api.spreaker.com/oauth2/token", "grant_type=refresh_token&client_id=10&client_secret=QD4cn2JWnWK3sMswh59j6Bg5Gty43wRG&refresh_token=" + reauth, "application/x-www-form-urlencoded").ToString();
                        auth = Regex.Match(response_auth, @"access_token"":""(.+?)""").Groups[1].Value;
                        reauth = Regex.Match(response_auth, @"refresh_token"":""(.+?)""").Groups[1].Value;
                        File.WriteAllText(@"DATA\" + pathrequestOBJ + @"\auth.txt", auth + ":" + reauth);

                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string revenue_data = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/revenuead?c=en_US").ToString();
                        if (revenue_data.Contains("balance_amount"))
                        {
                            label_Balance.Text = Regex.Match(revenue_data, @"balance_amount"":(.+?),").Groups[1].Value + "$";
                            File.WriteAllText(@"DATA\" + pathrequestOBJ + @"\Balance.txt", Regex.Match(revenue_data, @"balance_amount"":(.+?),").Groups[1].Value);
                        }

                        chart_Earnings.Series["Dollar"].Points.Clear();
                        chart_Downloads.Series["Downloads"].Points.Clear();
                        chart_Impressions.Series["Impressions"].Points.Clear();
                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string earnings_stats = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/revenuead/statistics?c=en_US&start_date=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-01&end_date=" + DateTime.Now.Year + "-"+ DateTime.Now.Month + "-"+ DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString();
                        File.WriteAllText(@"DATA\" + pathrequestOBJ + @"\stats_earnings.txt", earnings_stats);
                        MatchCollection matches = Regex.Matches(earnings_stats, @"impressions_sold"":(.+?)}");
                        int index = 1;
                        foreach (Match match in matches)
                        {
                            try
                            {
                                string cash = "0";
                                if (Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value.ToString() != "null")
                                {
                                    cash = Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value;
                                }
                                chart_Earnings.Series["Dollar"].Points.AddXY(index, cash);
                                chart_Earnings.Series["Dollar"].Points[index - 1].LabelForeColor = Color.YellowGreen;
                                chart_Earnings.Series["Dollar"].Points[index - 1].Font = new System.Drawing.Font("Arial", 12);
                                if (Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value.ToString() != "null")
                                {
                                    chart_Earnings.Series["Dollar"].Points[index - 1].Label = Math.Round(double.Parse(Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture), 2, MidpointRounding.AwayFromZero).ToString();

                                }
                                else
                                {
                                    chart_Earnings.Series["Dollar"].Points[index - 1].Label = "0";

                                }
                                chart_Impressions.Series["Impressions"].Points.AddXY(index, Regex.Match(match.Value, @"_sold"":(.+?),").Groups[1].Value.ToString());
                                chart_Impressions.Series["Impressions"].Points[index - 1].LabelForeColor = Color.YellowGreen;
                                chart_Impressions.Series["Impressions"].Points[index - 1].Font = new System.Drawing.Font("Arial", 12);
                                chart_Impressions.Series["Impressions"].Points[index - 1].Label = Regex.Match(match.Value, @"_sold"":(.+?),").Groups[1].Value.ToString();

                                index++;
                            }
                            catch 
                            {

                            }
                        }

                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string download_stats = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/statistics/plays?c=en_US&from=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-01&to=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + "&group=day").ToString();
                        File.WriteAllText(@"DATA\" + pathrequestOBJ + @"\stats_downloads.txt", download_stats);
                        MatchCollection matches2 = Regex.Matches(download_stats, @"plays_count"":(.+?)}");
                        int index2 = 1;
                        foreach (Match match2 in matches2)
                        {
                            try
                            {
                                int value = Int32.Parse(Regex.Match(match2.Value, @"plays_count"":(.+?),").Groups[1].Value.ToString()) + Int32.Parse(Regex.Match(match2.Value, @"downloads_count"":(.+?)}").Groups[1].Value.ToString());
                                chart_Downloads.Series["Downloads"].Points.AddXY(index2, value);
                                chart_Downloads.Series["Downloads"].Points[index2 - 1].LabelForeColor = Color.YellowGreen;
                                chart_Downloads.Series["Downloads"].Points[index2 - 1].Font = new System.Drawing.Font("Arial", 12);
                                chart_Downloads.Series["Downloads"].Points[index2 - 1].Label = value.ToString();

                                index2++;
                            }
                            catch
                            {

                            }
                        }

                        try
                        {
                            requestOBJ.AddHeader("authorization", "Bearer " + auth);
                            string bank_info = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/revenuead/bankinfo?c=en_US").ToString();
                            label_PAYPAL_TEXT.ForeColor = Color.LimeGreen;
                            label_PAYPAL_TEXT.Text = Regex.Match(bank_info, @"paypal_email"":""(.+?)""").Groups[1].Value;
                        }
                        catch (Exception ex)
                        {
                            if (ex.ToString().Contains("404"))
                            {
                                Chilkat.HttpRequest multiPart = new Chilkat.HttpRequest();
                                multiPart.HttpVerb = "POST";
                                multiPart.Path = "v2/users/" + UID + "/revenuead/bankinfo?c=en_US";
                                multiPart.ContentType = "multipart/form-data";
                                multiPart.AddHeader("authorization", "Bearer " + auth);
                                multiPart.AddParam("paypal_email", @"pwaspotify@gmail.com");
                                multiPart.AddParam("paypal_email_confirm", @"pwaspotify@gmail.com");
                                Chilkat.Http http = new Chilkat.Http();
                                Chilkat.HttpResponse fafafafa = http.SynchronousRequest("api.spreaker.com", 443, true, multiPart);
                            }
                        }

                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string episodes = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/shows?c=en_US&filter=editable&limit=500&export=show_stats,show_private,show_visibility,show_last_episode_at,show_author&access_level=AUTHOR").ToString();
                        MatchCollection matches3 = Regex.Matches(episodes, @"listenable_episodes_count"":(.+?),");
                        int episodes_num = 0;
                        foreach (Match match3 in matches3)
                        {
                            episodes_num += Int32.Parse(Regex.Match(match3.Value, @"listenable_episodes_count"":(.+?),").Groups[1].Value);
                        }
                        File.WriteAllText(@"DATA\" + pathrequestOBJ + @"\episodes.txt", episodes_num.ToString());
                        label_EPISODES_VALUE.Text = episodes_num.ToString();


                        label_DEBUG.Text = "UPDATED";
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (ex.ToString().Contains("429"))
                        {
                            label_DEBUG.Text = "RATE LIMIT";
                            Thread.Sleep(2500);
                        }
                        else if (ex.ToString().Contains("401"))
                        {
                            label_DEBUG.Text = "INVALID ACCOUNT";
                            break;
                        }
                        else
                        {
                            richTextBox_SPREAKER.AppendText(ex.ToString());
                        }
                    }
                }
            }
        }

        private void button_COPY_LINKS_Click(object sender, EventArgs e)
        {
            System.Text.StringBuilder mongo = new System.Text.StringBuilder();
            int cc_index = 1;
            foreach(Control cc in tabControl1.SelectedTab.Controls)
            {
                if (cc_index != tabControl1.SelectedTab.Controls.Count)
                {
                    mongo.Append(cc.Text + "\n");
                    cc_index++;
                }
                else
                {
                    mongo.Append(cc.Text);
                }
            }

            Clipboard.SetText(mongo.ToString());
        }

        private void button_OPEN_FOLDER_Click(object sender, EventArgs e)
        {
            string path_acc = String.Concat(Directory.GetCurrentDirectory(), @"\DATA\", pathrequestOBJ);
            Process.Start(path_acc);
        }

        private void button_DELETE_Click(object sender, EventArgs e)
        {
            Directory.Delete(String.Concat(Directory.GetCurrentDirectory(), @"\DATA\", pathrequestOBJ), true);
            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory() + @"\DATA");
            foreach (string dir in dirs)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                var base64EncodedBytes = System.Convert.FromBase64String(di.Name);
                TreeNode node = new TreeNode(System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split(':')[0], 0, 1);
                try
                {
                    node.Tag = di.Name;
                }
                catch (UnauthorizedAccessException)
                {
                    node.ImageIndex = 12;
                    node.SelectedImageIndex = 12;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DirectoryLister",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    treeView_DATA.Nodes.Add(node);
                }
            }
        }

        private void button_ADD_Click(object sender, EventArgs e)
        {
            Form_ADD ff = new Form_ADD();
            ff.Show();
        }

        private void button_REFRESH_Click(object sender, EventArgs e)
        {
            int accs_amount = 0;
            int accs_amount_dead = 0;
            treeView_DATA.Nodes.Clear();
            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory() + @"\DATA");
            var di2 = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\DATA");
            var ordered = di2.EnumerateDirectories().OrderBy(x => x.CreationTime).Select(d => d.Name);
            foreach (string dir in ordered)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                var base64EncodedBytes = System.Convert.FromBase64String(di.Name);
                if (!File.Exists(@"DATA\" + di.Name + @"\STATUS.txt"))
                {
                    using (StreamWriter sw = (File.CreateText(@"DATA\" + di.Name + @"\STATUS.txt")))
                    {
                        sw.Write("ENABLED:");
                        sw.Close();
                    }
                }
                TreeNode node = new TreeNode(System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Split(':')[0], 0, 1);
                if (File.ReadAllText(@"DATA\" + di.Name + @"\STATUS.txt").Contains("DISABLED"))
                {
                    node.ForeColor = Color.Red;
                    accs_amount_dead++;
                }
                else if (File.ReadAllText(@"DATA\" + di.Name + @"\episodes.txt") == "0")
                {
                    node.ForeColor = Color.Orange;
                }
                else
                {
                    node.ForeColor = Color.GreenYellow;
                    accs_amount++;
                }
                try
                {
                    node.Tag = di.Name;
                }
                catch (UnauthorizedAccessException)
                {
                    node.ImageIndex = 12;
                    node.SelectedImageIndex = 12;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DirectoryLister",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    treeView_DATA.Nodes.Add(node);
                }
            }
            label_ACCS_AMOUNT.Text = accs_amount.ToString();
            label_ACCS_AMOUNT_DEAD.Text = accs_amount_dead.ToString();
        }

        private void label_PAYPAL_TEXT_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(label_PAYPAL_TEXT.Text);
        }

        private void button_UPDATE_TOTAL_STATS_Click(object sender, EventArgs e)
        {
            int earnings = 0;
            int impressions = 0;
            int downloads = 0;
            int episodes = 0;
            try
            {
                foreach (string path in Directory.GetDirectories(Directory.GetCurrentDirectory() + @"\DATA\"))
                {
                    if (File.ReadAllText(path + @"\STATUS.txt").Contains("ENABLED"))
                    {
                        MatchCollection matches = Regex.Matches(File.ReadAllText(path + @"\stats_earnings.txt"), @"impressions_sold"":(.+?)}");
                        foreach (Match match in matches)
                        {
                            try
                            {
                                impressions += Int32.Parse(Regex.Match(match.Value, @"_sold"":(.+?),").Groups[1].Value.ToString());

                            }
                            catch
                            {

                            }
                        }

                        MatchCollection matches2 = Regex.Matches(File.ReadAllText(path + @"\stats_downloads.txt"), @"plays_count"":(.+?)}");
                        foreach (Match match2 in matches2)
                        {
                            try
                            {
                                downloads += Int32.Parse(Regex.Match(match2.Value, @"plays_count"":(.+?),").Groups[1].Value.ToString()) + Int32.Parse(Regex.Match(match2.Value, @"downloads_count"":(.+?)}").Groups[1].Value.ToString());
                            }
                            catch
                            {

                            }
                        }

                        earnings += Int32.Parse((File.ReadAllText(path + @"\Balance.txt")).Split('.')[0]);
                        episodes += Int32.Parse((File.ReadAllText(path + @"\episodes.txt")).Split('.')[0]);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            label_TOTAL_DOLLAR.Text = earnings.ToString() + "$";
            label_TOTAL_IMPRESSIONS.Text = impressions.ToString();
            label_TOTAL_DOWNLOADS.Text = downloads.ToString();
            label_TOTAL_EPISODES.Text = episodes.ToString();
        }

        private void button_EDIT_Click(object sender, EventArgs e)
        {
            try
            {
                Form_EDIT ff = new Form_EDIT(pathrequestOBJ, UID);
                ff.Show();
            }
            catch
            {

            }
        }

        private void button_UPDATE_ALL_API_Click(object sender, EventArgs e)
        {
            var di2 = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\DATA");
            var ordered = di2.EnumerateDirectories().OrderBy(x => x.CreationTime).Select(d => d.Name);
            foreach (string dir in ordered)
            {
                
                
                Thread.Sleep(250);
                DirectoryInfo di = new DirectoryInfo(dir);
                var base64EncodedBytes = System.Convert.FromBase64String(di.Name);
                string folder = di.Name.ToString();
                if (File.ReadAllText(Directory.GetCurrentDirectory() + @"\DATA\" + folder + @"\STATUS.txt").Contains("ENABLED"))
                {
                    Leaf.xNet.HttpRequest requestOBJ = new Leaf.xNet.HttpRequest();
                    string reauth2 = File.ReadAllText(Directory.GetCurrentDirectory() + @"\DATA\" + folder + @"\auth.txt").Split(':')[1];
                    string auth2 = File.ReadAllText(Directory.GetCurrentDirectory() + @"\DATA\" + folder + @"\auth.txt").Split(':')[0];
                    string UID2 = File.ReadAllText(Directory.GetCurrentDirectory() + @"\DATA\" + folder + @"\UID.txt");

                    while (true)
                    {
                        try
                        {


                            requestOBJ.AddHeader("authorization", "Bearer " + reauth2);
                            string response_auth = requestOBJ.Post("https://api.spreaker.com/oauth2/token", "grant_type=refresh_token&client_id=10&client_secret=QD4cn2JWnWK3sMswh59j6Bg5Gty43wRG&refresh_token=" + reauth2, "application/x-www-form-urlencoded").ToString();
                            auth2 = Regex.Match(response_auth, @"access_token"":""(.+?)""").Groups[1].Value;
                            reauth2 = Regex.Match(response_auth, @"refresh_token"":""(.+?)""").Groups[1].Value;
                            File.WriteAllText(@"DATA\" + folder + @"\auth.txt", auth2 + ":" + reauth2);

                            if (!File.Exists(@"DATA\" + folder + @"\STATUS.txt"))
                            {
                                using (StreamWriter sw = (File.CreateText(@"DATA\" + folder + @"\STATUS.txt")))
                                {
                                    sw.Write("NONE");
                                    sw.Close();
                                }
                            }
                            try
                            {
                                requestOBJ.AddHeader("authorization", "Bearer " + auth2);
                                string revenue_data = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID2 + "/revenuead?c=en_US").ToString();
                                if (revenue_data.Contains("balance_amount"))
                                {
                                    label_Balance.Text = Regex.Match(revenue_data, @"balance_amount"":(.+?),").Groups[1].Value + "$";
                                    File.WriteAllText(@"DATA\" + folder + @"\Balance.txt", Regex.Match(revenue_data, @"balance_amount"":(.+?),").Groups[1].Value);
                                }
                                File.WriteAllText(@"DATA\" + folder + @"\STATUS.txt", String.Concat(Regex.Match(revenue_data, @"adrevenue_status"":""(.+?)""").Groups[1].Value, ":", Regex.Match(revenue_data, @"The Monetization Program has been blocked for the following reason: (.+?)""").Groups[1].Value));
                            }
                            catch
                            {

                            }
                            chart_Earnings.Series["Dollar"].Points.Clear();
                            chart_Downloads.Series["Downloads"].Points.Clear();
                            chart_Impressions.Series["Impressions"].Points.Clear();
                            requestOBJ.AddHeader("authorization", "Bearer " + auth2);
                            string earnings_stats = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID2 + "/revenuead/statistics?c=en_US&start_date=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-01&end_date=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString();
                            File.WriteAllText(@"DATA\" + folder + @"\stats_earnings.txt", earnings_stats);
                            MatchCollection matches = Regex.Matches(earnings_stats, @"impressions_sold"":(.+?)}");
                            int index = 1;
                            foreach (Match match in matches)
                            {
                                try
                                {
                                    string cash = "0";
                                    if (Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value.ToString() != "null")
                                    {
                                        cash = Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value;
                                    }
                                    chart_Earnings.Series["Dollar"].Points.AddXY(index, cash);
                                    chart_Earnings.Series["Dollar"].Points[index - 1].LabelForeColor = Color.YellowGreen;
                                    chart_Earnings.Series["Dollar"].Points[index - 1].Font = new System.Drawing.Font("Arial", 12);
                                    if (Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value.ToString() != "null")
                                    {
                                        chart_Earnings.Series["Dollar"].Points[index - 1].Label = Math.Round(double.Parse(Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture), 2, MidpointRounding.AwayFromZero).ToString();

                                    }
                                    else
                                    {
                                        chart_Earnings.Series["Dollar"].Points[index - 1].Label = "0";

                                    }
                                    chart_Impressions.Series["Impressions"].Points.AddXY(index, Regex.Match(match.Value, @"_sold"":(.+?),").Groups[1].Value.ToString());
                                    chart_Impressions.Series["Impressions"].Points[index - 1].LabelForeColor = Color.YellowGreen;
                                    chart_Impressions.Series["Impressions"].Points[index - 1].Font = new System.Drawing.Font("Arial", 12);
                                    chart_Impressions.Series["Impressions"].Points[index - 1].Label = Regex.Match(match.Value, @"_sold"":(.+?),").Groups[1].Value.ToString();

                                    index++;
                                }
                                catch
                                {

                                }
                            }
                            Thread.Sleep(250);
                            requestOBJ.AddHeader("authorization", "Bearer " + auth2);
                            string download_stats = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID2 + "/statistics/plays?c=en_US&from=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-01&to=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + "&group=day").ToString();
                            File.WriteAllText(@"DATA\" + folder + @"\stats_downloads.txt", download_stats);
                            MatchCollection matches2 = Regex.Matches(download_stats, @"plays_count"":(.+?)}");
                            int index2 = 1;
                            foreach (Match match2 in matches2)
                            {
                                try
                                {
                                    int value = Int32.Parse(Regex.Match(match2.Value, @"plays_count"":(.+?),").Groups[1].Value.ToString()) + Int32.Parse(Regex.Match(match2.Value, @"downloads_count"":(.+?)}").Groups[1].Value.ToString());
                                    chart_Downloads.Series["Downloads"].Points.AddXY(index2, value);
                                    chart_Downloads.Series["Downloads"].Points[index2 - 1].LabelForeColor = Color.YellowGreen;
                                    chart_Downloads.Series["Downloads"].Points[index2 - 1].Font = new System.Drawing.Font("Arial", 12);
                                    chart_Downloads.Series["Downloads"].Points[index2 - 1].Label = value.ToString();
                                    index2++;
                                }
                                catch
                                {

                                }
                            }

                            try
                            {
                                requestOBJ.AddHeader("authorization", "Bearer " + auth2);
                                string bank_info = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID2 + "/revenuead/bankinfo?c=en_US").ToString();
                                label_PAYPAL_TEXT.ForeColor = Color.LimeGreen;
                                label_PAYPAL_TEXT.Text = Regex.Match(bank_info, @"paypal_email"":""(.+?)""").Groups[1].Value;
                            }
                            catch (Exception ex)
                            {
                                if (ex.ToString().Contains("404"))
                                {
                                    Chilkat.HttpRequest multiPart = new Chilkat.HttpRequest();
                                    multiPart.HttpVerb = "POST";
                                    multiPart.Path = "v2/users/" + UID2 + "/revenuead/bankinfo?c=en_US";
                                    multiPart.ContentType = "multipart/form-data";
                                    multiPart.AddHeader("authorization", "Bearer " + auth2);
                                    multiPart.AddParam("paypal_email", @"pwaspotify@gmail.com");
                                    multiPart.AddParam("paypal_email_confirm", @"pwaspotify@gmail.com");
                                    Chilkat.Http http = new Chilkat.Http();
                                    Chilkat.HttpResponse fafafafa = http.SynchronousRequest("api.spreaker.com", 443, true, multiPart);
                                }
                            }

                            requestOBJ.AddHeader("authorization", "Bearer " + auth2);
                            string episodes = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID2 + "/shows?c=en_US&filter=editable&limit=500&export=show_stats,show_private,show_visibility,show_last_episode_at,show_author&access_level=AUTHOR").ToString();
                            MatchCollection matches3 = Regex.Matches(episodes, @"listenable_episodes_count"":(.+?),");
                            int episodes_num = 0;
                            foreach (Match match3 in matches3)
                            {
                                episodes_num += Int32.Parse(Regex.Match(match3.Value, @"listenable_episodes_count"":(.+?),").Groups[1].Value);
                            }
                            File.WriteAllText(@"DATA\" + folder + @"\episodes.txt", episodes_num.ToString());
                            label_EPISODES_VALUE.Text = episodes_num.ToString();


                            break;
                        }
                        catch (Exception ex)
                        {
                            if (ex.ToString().Contains("429"))
                            {
                                label_DEBUG.Text = "RATE LIMIT";
                                Thread.Sleep(2500);
                            }
                            else if (ex.ToString().Contains("401"))
                            {
                                label_DEBUG.Text = "INVALID ACCOUNT";
                                break;
                            }
                            else
                            {
                                richTextBox_SPREAKER.AppendText(ex.ToString());
                            }
                        }
                    }
                }
            }
            label_DEBUG.Text = "UPDATED";
        }

        private void button_ALL_LINKS_Click(object sender, EventArgs e)
        {
            List<string> links = new List<string>();
            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory() + @"\DATA");
            var di2 = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\DATA");
            var ordered = di2.EnumerateDirectories().OrderBy(x => x.CreationTime).Select(d => d.Name);
            foreach (string dir in ordered)
            {
                if(!File.ReadAllText(@"DATA\" + dir + @"\STATUS.txt").Contains("DISABLED"))
                {
                    List<string> requestOBJ = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\DATA\" + dir + @"\Links.txt").ToList();
                    foreach (string requestOBJ2 in requestOBJ)
                    {
                        if (requestOBJ2.Contains(comboBox_ALL_COPY_FILTER.Text))
                        {
                            links.Add(requestOBJ2);
                        }
                    }
                } 
            }
            Clipboard.SetText(string.Join("\n", links));
        }

        private void button_ALL_ADS_Click(object sender, EventArgs e)
        {
            string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory() + @"\DATA");
            var di2 = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\DATA");
            var ordered = di2.EnumerateDirectories().OrderBy(x => x.CreationTime).Select(d => d.Name);
            List<string> ads = richTextBox1.Text.Split(',').ToList();
            List<string> ads_data = new List<string>();
            foreach(string ad in ads)
            {
                ads_data.Add(String.Concat(@"{""timecode"":", ad.Split(':')[0], @",""ads_max_count"":", ad.Split(':')[1], @"}"));
            }
            string ads_multi_data = String.Join(",", ads_data);
            Chilkat.HttpRequest multiPart = new Chilkat.HttpRequest();
            foreach (string dir in ordered)
            {
                Thread.Sleep(100);
                DirectoryInfo di = new DirectoryInfo(dir);
                string authy = File.ReadAllText(@"DATA\" + di.Name + @"\auth.txt");
                List<string> lines = File.ReadAllLines(@"DATA\" + di.Name + @"\Links.txt").ToList<string>();
                for (int i = 0; i != lines.Count(); i++)
                {
                    if (lines[i].Contains("spreaker.com/episode"))
                    {
                        multiPart.HttpVerb = "POST";
                        multiPart.Path = "v2/" + lines[i].Split('/')[3] + "s/" + lines[i].Split('/')[4] + "/cuepoints?c=en_US";
                        multiPart.ContentType = "multipart/form-data";
                        multiPart.AddHeader("authorization", "Bearer " + authy.Split(':')[0]);
                        multiPart.AddParam("cuepoints", @"[" + ads_multi_data + "]");
                        Chilkat.Http http = new Chilkat.Http();
                        Chilkat.HttpResponse fafafafa = http.SynchronousRequest("api.spreaker.com", 443, true, multiPart);
                        http.CloseAllConnections();
                    }
                }
            }
            label_DEBUG.Text = "DONE";
        }

        private void button_IMPORT_Click(object sender, EventArgs e)
        {
            int idexxx = 0;
            List<string> import = File.ReadAllLines("import.txt").ToList();
            foreach(string line in import)
            {
                Thread.Sleep(1000);
                try
                {
                    label_DEBUG.Text = idexxx.ToString();
                    Leaf.xNet.HttpRequest requestOBJ = new Leaf.xNet.HttpRequest();
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(line.Split(':')[1] + ":" + line.Split(':')[2]);
                    string dir = String.Concat(Directory.GetCurrentDirectory(), @"\DATA\", System.Convert.ToBase64String(plainTextBytes));

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        Directory.CreateDirectory(dir + @"\Podcasts");
                        string response_requestOBJ = requestOBJ.Post("https://api.spreaker.com/oauth2/token", String.Concat("client_id=10&client_secret=QD4cn2JWnWK3sMswh59j6Bg5Gty43wRG&grant_type=password&password=", line.Split(':')[2], "&scope=basic%20spreaker&username=", line.Split(':')[1]), "application/x-www-form-urlencoded").ToString();
                        auth = Regex.Match(response_requestOBJ, @"access_token"":""(.+?)""").Groups[1].Value;
                        reauth = Regex.Match(response_requestOBJ, @"refresh_token"":""(.+?)""").Groups[1].Value;
                        Thread.Sleep(2500);
                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        UID = Regex.Match(requestOBJ.Get("https://api.spreaker.com/v2/me").ToString(), @"user_id"":(.+?),").Groups[1].Value;
                        Thread.Sleep(2500);
                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        requestOBJ.Put("https://api.spreaker.com/v2/users/" + UID + "/revenuead?c=en_US&terms_accepted=true").ToString();
                        Thread.Sleep(2500);
                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string revenue_data = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/revenuead?c=en_US").ToString();
                        balance = "0";
                        if (revenue_data.Contains("balance_amount"))
                        {
                            balance = Regex.Match(revenue_data, @"balance_amount"":(.+?),").Groups[1].Value;
                        }
                        Thread.Sleep(2500);
                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string earnings_stats = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/revenuead/statistics?c=en_US&start_date=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-01&end_date=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString();
                        Thread.Sleep(2500);
                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string download_stats = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/statistics/plays?c=en_US&from=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-01&to=" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + "&group=day").ToString();
                        Thread.Sleep(2500);
                        requestOBJ.AddHeader("authorization", "Bearer " + auth);
                        string episodes = requestOBJ.Get("https://api.spreaker.com/v2/users/" + UID + "/shows?c=en_US&filter=editable&limit=500&export=show_stats,show_private,show_visibility,show_last_episode_at,show_author&access_level=AUTHOR").ToString();
                        MatchCollection matches = Regex.Matches(episodes, @"listenable_episodes_count"":(.+?),");
                        int episodes_num = 0;
                        foreach (Match match in matches)
                        {
                            episodes_num += Int32.Parse(Regex.Match(match.Value, @"listenable_episodes_count"":(.+?),").Groups[1].Value);
                        }
                        Thread.Sleep(2500);

                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\auth.txt")))
                        {
                            sw.Write(auth + ":" + reauth);
                            sw.Close();
                        }
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\DATA.txt")))
                        {
                            sw.Write(line.Split(':')[0] + ":" + line.Split(':')[1] + ":" + line.Split(':')[2] + ":" + line.Split(':')[3] + ":" + line.Split(':')[4] + ":" + line.Split(':')[5]);
                            sw.Close();
                        }
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\UID.txt")))
                        {
                            sw.Write(UID);
                            sw.Close();
                        }
                        using (StreamWriter sw = (File.CreateText(@"DATA\" + System.Convert.ToBase64String(plainTextBytes) + @"\Links.txt")))
                        {
                            sw.Write("");
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
                            sw.Write("Improv");
                            sw.Close();
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = (File.CreateText(@"Debug.txt")))
                    {
                        sw.Write(ex.ToString());
                        sw.Close();
                    }
                }
                idexxx++;
            }
        }

        private void button_cmonth_Click(object sender, EventArgs e)
        {
            int cmonth_cash = 0;
            try
            {
                foreach (string path in Directory.GetDirectories(Directory.GetCurrentDirectory() + @"\DATA\"))
                {
                    if (File.ReadAllText(path + @"\STATUS.txt").Contains("ENABLED"))
                    {
                        MatchCollection matches = Regex.Matches(File.ReadAllText(path + @"\stats_earnings.txt"), @"impressions_sold"":(.+?)}");
                        foreach (Match match in matches)
                        {
                            string cash = "0";
                            if (Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value.ToString() != "null")
                            {
                                cash = Regex.Match(match.Value, @"revenue_amount"":(.+?)}").Groups[1].Value;
                            }
                            cmonth_cash = cmonth_cash + Int32.Parse(cash.Split('.')[0]);
                        }
                    }
                }
                label_cmonth_cash.Text = cmonth_cash.ToString() + " $";
            }
            catch (Exception ex)
            {
                richTextBox1.Text = ex.ToString();
            }
        }
    }
}
