using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using WpfApp1.model;

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

        private async  void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //HDD
            dataGrd.ItemsSource = (await LDLCExtractor(@"http://www.ldlc.com/informatique/pieces-informatique/disque-dur-interne/c4697/")).OrderBy(p=>p.PrixAuGO);
            
            //CG
            //dataGrd.ItemsSource = (await LDLCExtractor(@"http://www.ldlc.com/informatique/pieces-informatique/carte-graphique-interne/c4684/")).OrderBy(p => p.Prix);
        }

        public async Task<List<HDD>> LDLCExtractor(string baseUrl) 
        {
            List<HDD> LstProd = new List<HDD>(); 
            ScrapySharp.Network.ScrapingBrowser browser = new ScrapySharp.Network.ScrapingBrowser();
            var res = await browser.NavigateToPageAsync(new Uri(baseUrl));
            foreach (HtmlNode elem in res.Html.CssSelect(".pagerItem"))
            {
                var url = elem.Attributes.Where(a => a.Name == "href").First().Value;
                var rslt = await browser.NavigateToPageAsync(new Uri(url));
                foreach (HtmlAgilityPack.HtmlNode prod in rslt.Html.CssSelect(".cmp"))
                {
                    HDD tmp = new HDD();
                    var price = prod.CssSelect(".price").FirstOrDefault()?.InnerText;
                    float finalprice = 0.0f;
                    Regex regex = new Regex(@"(\d+)&euro;(\d\d)");
                    if (price != null)
                    {
                        Match match = regex.Match(price);
                        if (match.Success)
                        {
                            finalprice = float.Parse(match.Groups[1].ToString() + "." + match.Groups[2].ToString(), CultureInfo.InvariantCulture);
                            var resNom = prod.CssSelect(".nom").FirstOrDefault();
                            tmp.Nom = resNom?.InnerText;
                            tmp.Prix = finalprice;
                            var prodRes = await browser.NavigateToPageAsync(new Uri(resNom.Attributes.Where(a => a.Name == "href").First().Value));
                            
                            var tech = prodRes.Html.CssSelect(".param1192");
                            Regex reg2 = new Regex(@"(\d+)\s(To|Go)");
                            string spec = tech.First().InnerText;

                            Match match2 = reg2.Match(spec);
                            if (match2.Success)
                            {

                                if (match2.Groups[2].Value == "To") tmp.Capacite = int.Parse(match2.Groups[1].Value) * 1000;
                                else if (match2.Groups[2].Value == "Go") tmp.Capacite = int.Parse(match2.Groups[1].Value);

                                tmp.PrixAuGO = (float)tmp.Prix / (float)tmp.Capacite;
                            }
                            LstProd.Add(tmp);
                        }
                    }

                }
            }
            return LstProd;
        }

        
        
    }
}
