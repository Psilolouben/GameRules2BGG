﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Xml;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var url = @"https://www.thegamerules.com/el/arxiki/by,%60p%60.product_availability/dirAsc/results,1-150?language=el-GR&categorylayout=default";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = null;
            readStream = new StreamReader(receiveStream, Encoding.UTF8);

            string data = readStream.ReadToEnd();

            var splitData =data.Split(new string[] { "catProductTitle" }, StringSplitOptions.None);
            var bgTitles = new List<string>();
            var bgIds = new List<int>();
            var allProducts = new List<DB.Product>();
            var titlesToRetrieve = new List<string>();

            foreach(var line in splitData)
            {
                var l = line.Split('>')[2].Split('<')[0];
                if (l.Contains("Sleeves") ||l.Contains("Dice Set") || l.Contains("Organizer") || l.Contains("D6") || l.Contains("Tokens") || l.Contains("&")) 
                    continue;
                l = l.Replace("(Exp.)","").Replace("(Exp)","").Replace("(", "").Replace(")", "");
                var existing = RetrieveFromDB(l);
                if (existing != null)
                    allProducts.Add(existing);
                else
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(l);
                    l = Encoding.UTF8.GetString(bytes);

                    if(l != "\n")
                      titlesToRetrieve.Add(l.TrimEnd());
                }
             }

            foreach(var title in titlesToRetrieve)
            {
                System.Threading.Thread.Sleep(2000);
                var idUrl = @"https://www.boardgamegeek.com/xmlapi2/search?query=" + title + "&exact=1&type=boardgame,boardgameexpansion";
                HttpWebRequest idrequest = (HttpWebRequest)WebRequest.Create(idUrl);
                HttpWebResponse idresponse = (HttpWebResponse)idrequest.GetResponse();

                Stream idreceiveStream = idresponse.GetResponseStream();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(idreceiveStream);

                var bgg_id = default(int);
                if (xmlDoc.DocumentElement.SelectNodes("item").Count > 0)
                {
                  bgg_id = Convert.ToInt32(xmlDoc.DocumentElement.SelectNodes("item")[0].Attributes[1].Value);
                  bgIds.Add(bgg_id);
                }
            }

            var query = string.Join(",", bgIds);

            var toFindUrl = @"https://www.boardgamegeek.com/xmlapi2/thing?id=" + query + "&exact=1&type=boardgame,boardgameexpansion";
            HttpWebRequest toFindrequest = (HttpWebRequest)WebRequest.Create(toFindUrl);
            HttpWebResponse toFindresponse = (HttpWebResponse)toFindrequest.GetResponse();

            Stream toFindreceiveStream = toFindresponse.GetResponseStream();

            XmlDocument toFindxmlDoc = new XmlDocument();
            toFindxmlDoc.Load(toFindreceiveStream);
        }


        private DB.Product RetrieveFromDB(string gameTitle)
        {
            var dbe = new DB.GameRules2BGGEntities();

            return dbe.Products.FirstOrDefault(sd => sd.Bgg_Name == gameTitle);


        }
    }
}
