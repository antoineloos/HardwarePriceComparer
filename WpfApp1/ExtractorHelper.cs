using HtmlAgilityPack;
using ScrapySharp.Extensions;
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
using System.Windows.Threading;
using WpfApp1.model;


namespace WpfApp1
{
    public class ExtractorHelper
    {
        ////https://www.grosbill.com/3-disque_dur-3.5-type-informatique
        ////http://www.materiel.net/disque-dur/

        //var auth = new AmazonAuthentication();
        //auth.AccessKey = "";
        //auth.SecretKey = "";
        //var wrapper = new AmazonWrapper(auth, AmazonEndpoint.FR);
        //foreach(HDD di in lstProd2)
        //{
        //    var lstReslt = wrapper.Search(di.Nom, AmazonSearchIndex.Electronics);
        //    if (lstReslt != null && lstReslt.Items!=null && lstReslt.Items.Item!=null)
        //    {
        //        foreach (Item item in lstReslt.Items.Item)
        //        {

        //                Debug.WriteLine("modèle : " + item.ItemAttributes?.Model + " marque : " + item.ItemAttributes?.Brand + " price : " + item.ItemAttributes?.ListPrice?.FormattedPrice);

        //        }
        //    }
        //}

        public static async void LDLCExtractor(string baseUrl, ObservableCollection<HDD> LstProd)
        {

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

                            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                LstProd.Add(tmp);
                            }));
                        }
                    }

                }
            }

            LstProd = new ObservableCollection<HDD>( LstProd.OrderBy(p => p.PrixAuGO));

        }

       


        public static async void TopAchatExtractor(string rootUrl, ObservableCollection<HDD> LstProd)
        {
            string UrlBase = "https://www.topachat.com";

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

                var rslt = await browser.NavigateToPageAsync(new Uri(UrlBase + elem));
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
                                tmp.Nom = match2.Groups[1].Value.Replace("\t", "");

                                if (match2.Groups[3].Value == "To") tmp.Capacite = int.Parse(match2.Groups[2].Value) * 1000;
                                else if (match2.Groups[3].Value == "Go") tmp.Capacite = int.Parse(match2.Groups[2].Value);

                                tmp.PrixAuGO = (float)tmp.Prix / (float)tmp.Capacite;
                            }
                            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                LstProd.Add(tmp);
                            }));
                        }
                    }

                }
            }

            LstProd = new ObservableCollection<HDD>(LstProd.OrderBy(p => p.PrixAuGO));

        }

        public static async void GrosbillExtractor(string rootUrl, ObservableCollection<HDD> LstProd)
        {
            string UrlBase = "https://www.grosbill.com/3-disque_dur-3.5-type-informatique?page=";
            string compl = "&tri=w&filtre_page=100&mode=listing&filtre_type_produit=disque_dur#";
            List<string> LstPage = new List<string>();
            ScrapySharp.Network.ScrapingBrowser browser = new ScrapySharp.Network.ScrapingBrowser();
            var res = await browser.NavigateToPageAsync(new Uri(rootUrl));
            LstPage.Add(rootUrl+compl+"1");

            //foreach (HtmlNode nod in res.Html.CssSelect(".pagination").First().CssSelect(".page_number"))
            //{
            //    LstPage.Add(UrlBase + nod.InnerText+compl+ nod.InnerText);
            //}





            foreach (string elem in LstPage)
            {

                var rslt = await browser.NavigateToPageAsync(new Uri(elem));
                
                foreach (HtmlNode nd in rslt.Html.CssSelect(".listing_product").First().SelectNodes("//tr"))
                {
                    HDD tmp = new HDD();
                    var price = nd.CssSelect(".btn_price_wrapper").FirstOrDefault()?.InnerText;
                   
                    float finalprice = 0.0f;
                    Regex regex = new Regex(@"(\d+)&euro;(\d\d)");
                    if (price != null)
                    {
                        Match match = regex.Match(price);
                        if (match.Success)
                        {
                            finalprice = float.Parse(match.Groups[1].ToString() + "." + match.Groups[2].ToString(), CultureInfo.InvariantCulture);

                            tmp.Prix = finalprice;
                        }
                    }
                    if (nd.CssSelect(".product_description").Count() == 1)
                    {
                        string spec = nd.CssSelect(".product_description").First().ChildNodes[1].InnerText;
                        Regex reg2 = new Regex(@"(.+)\s+(\d+)\s*(To|Go)(.*)");
                        

                        Match match2 = reg2.Match(spec);
                        if (match2.Success)
                        {
                            tmp.Nom = match2.Groups[1].Value.Replace("\t", "");

                            if (match2.Groups[3].Value == "To") tmp.Capacite = int.Parse(match2.Groups[2].Value) * 1000;
                            else if (match2.Groups[3].Value == "Go") tmp.Capacite = int.Parse(match2.Groups[2].Value);

                            tmp.PrixAuGO = (float)tmp.Prix / (float)tmp.Capacite;
                        }
                    }
                    
                    await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        if (tmp.Capacite != 0) LstProd.Add(tmp);
                    }));
                    
                }



            }

            Debug.WriteLine(LstProd.Count);
        }
    }
}
