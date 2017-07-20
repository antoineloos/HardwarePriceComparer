using HtmlAgilityPack;
using Prism.Mvvm;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WpfApp1.model;

namespace WpfApp1
{
    public class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<HDD> lstLDLC;
        public ObservableCollection<HDD> LstLDLC
        {
            get { return lstLDLC; }
            set { SetProperty(ref lstLDLC, value); }
        }

        private ObservableCollection<HDD> lstTopAchat;
        public ObservableCollection<HDD> LstTopAchat
        {
            get { return lstTopAchat; }
            set { SetProperty(ref lstTopAchat, value); }
        }

        private ObservableCollection<HDD> lstAmazon;
        public ObservableCollection<HDD> LstAmazon
        {
            get { return lstAmazon; }
            set { SetProperty(ref lstAmazon, value); }
        }

        private ObservableCollection<HDD> lstGrosbill;
        public ObservableCollection<HDD> LstGrosbill
        {
            get { return lstGrosbill; }
            set { SetProperty(ref lstGrosbill, value); }
        }

        private ObservableCollection<HDD> lstMaterielNet;
        public ObservableCollection<HDD> LstMaterielNet
        {
            get { return lstMaterielNet; }
            set { SetProperty(ref lstMaterielNet, value); }
        }

        public MainWindowViewModel()
        {
            LstAmazon = new ObservableCollection<HDD>();
            LstGrosbill = new ObservableCollection<HDD>();
            LstLDLC = new ObservableCollection<HDD>();
            LstTopAchat = new ObservableCollection<HDD>();
            LstMaterielNet = new ObservableCollection<HDD>();

            Task.Factory.StartNew(() =>
            {
                ExtractorHelper.LDLCExtractor(@"http://www.ldlc.com/informatique/pieces-informatique/disque-dur-interne/c4697/", LstLDLC);
            });

            Task.Factory.StartNew(() =>
            {
                ExtractorHelper.TopAchatExtractor(@"https://www.topachat.com/pages/produits_cat_est_micro_puis_rubrique_est_wdi_sata.html", LstTopAchat);
            });

            Task.Factory.StartNew(() =>
            {
                ExtractorHelper.GrosbillExtractor(@"https://www.grosbill.com/3-disque_dur-3.5-type-informatique", LstGrosbill);
            });
        }
       
    }
}
