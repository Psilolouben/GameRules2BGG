using System;
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
            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

            string data = readStream.ReadToEnd();

            var splitData =data.Split(new string[] { "catProductTitle" }, StringSplitOptions.None);
            foreach(var line in splitData)
            {
                var l = line.Split('>')[2].Split('<')[0];
                if (l.Contains("Sleeves") ||l.Contains("Dice Set") || l.Contains("Organizer") || l.Contains("D6"))
                    continue;
            }
        }
    }
}
