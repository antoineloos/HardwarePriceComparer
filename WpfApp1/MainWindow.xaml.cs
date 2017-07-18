﻿using HtmlAgilityPack;
using Nager.AmazonProductAdvertising;
using Nager.AmazonProductAdvertising.Model;
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //HDD
            var lstProd =  (await LDLCExtractor(@"http://www.ldlc.com/informatique/pieces-informatique/disque-dur-interne/c4697/")).OrderBy(p=>p.PrixAuGO);
            dataGrdldlc.ItemsSource = lstProd;
            var lstProd2 = (await TopAchatExtractor(@"https://www.topachat.com/pages/produits_cat_est_micro_puis_rubrique_est_wdi_sata.html")).OrderBy(p=>p.PrixAuGO);
            dataGrdtopachat.ItemsSource = lstProd2;
            //https://www.grosbill.com/3-disque_dur-3.5-type-informatique
            //http://www.materiel.net/disque-dur/

            var auth = new AmazonAuthentication();
            auth.AccessKey = "";
            auth.SecretKey = "";
            var wrapper = new AmazonWrapper(auth, AmazonEndpoint.FR);
            foreach(HDD di in lstProd2)
            {
                var lstReslt = wrapper.Search(di.Nom, AmazonSearchIndex.Electronics);
                if (lstReslt != null && lstReslt.Items!=null && lstReslt.Items.Item!=null)
                {
                    foreach (Item item in lstReslt.Items.Item)
                    {
                       
                            Debug.WriteLine("modèle : " + item.ItemAttributes?.Model + " marque : " + item.ItemAttributes?.Brand + " price : " + item.ItemAttributes?.ListPrice?.FormattedPrice);
                            
                    }
                }
            }
           
            
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

        

        public async Task<List<HDD>> TopAchatExtractor(string rootUrl)
        {
            string UrlBase = "https://www.topachat.com";
            List<HDD> LstProd = new List<HDD>();
            List<string> LstPage = new List<string>();
            ScrapySharp.Network.ScrapingBrowser browser = new ScrapySharp.Network.ScrapingBrowser();
            var res = await browser.NavigateToPageAsync(new Uri(rootUrl));
            LstPage.Add(rootUrl);
            var pagin = res.Html.CssSelect(".pagination");
            foreach (HtmlNode elem in pagin.Last().ChildNodes.Where(n => n.Attributes.Any(a => a.Name == "href")))
            {

                LstPage.Add(elem.Attributes.Where(at => at.Name == "href").First().Value);

            }

            LstPage.Remove(LstPage.Last());
            LstPage[0] = LstPage.First().Replace(UrlBase, "");

            foreach (string elem in LstPage)
            {
                
                var rslt = await browser.NavigateToPageAsync(new Uri(UrlBase+elem));
                foreach (HtmlAgilityPack.HtmlNode prod in rslt.Html.CssSelect(".grille-produit"))
                {
                    HDD tmp = new HDD();
                    var price = prod.CssSelect(".price").FirstOrDefault()?.InnerText;
                    float finalprice = 0.0f;
                    Regex regex = new Regex(@"(\d+.\d\d)&nbsp;&euro;");
                    if (price != null)
                    {
                        Match match = regex.Match(price);
                        if (match.Success)
                        {
                            finalprice = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                         
                            tmp.Prix = finalprice;
                           
                            
                            Regex reg2 = new Regex(@"(.+),\s(\d+)\s(To|Go)");
                            string spec = prod.CssSelect(".libelle").First().InnerText;

                            Match match2 = reg2.Match(spec);
                            if (match2.Success)
                            {
                                tmp.Nom = match2.Groups[1].Value.Replace("\t","");
                                
                                if (match2.Groups[3].Value == "To") tmp.Capacite = int.Parse(match2.Groups[2].Value) * 1000;
                                else if (match2.Groups[3].Value == "Go") tmp.Capacite = int.Parse(match2.Groups[2].Value);

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
