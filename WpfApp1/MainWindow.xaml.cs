using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WpfApp1
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ScrapySharp.Network.ScrapingBrowser browser = new ScrapySharp.Network.ScrapingBrowser();
            var res = await browser.NavigateToPageAsync(new Uri("http://www.ldlc.com/informatique/pieces-informatique/disque-dur-interne/c4697/"));
           

           foreach (HtmlAgilityPack.HtmlNode elem in res.Html.CssSelect(".cmp"))
            {

                Debug.WriteLine(  "nom : " + elem.CssSelect(".nom").FirstOrDefault()?.InnerText+ " prix  : " + elem.CssSelect(".price").FirstOrDefault()?.InnerText);
            }
        }
    }
}
