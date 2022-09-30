using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using Microsoft.Win32;
using TestProfKholod.Model;
using Brushes = System.Drawing.Brushes;

namespace TestProfKholod.ViewModel
{
    public class LoadFile : INotifyPropertyChanged
    {
        private string PathFile;
        private string TextFromFile;
        public ObservableCollection<string> UrlList { get; set; }
        public ObservableCollection<LogInfo> ListLogInfos { get; set; }
        public ParseCommand WriteListBox { get; set; }
        public ParseCommand StartParse { get; set; }
        public ParseCommand StartParseSingle { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public ParseCommand OpenLink { get; set; }
        public ParseCommand OpenLinkSingle { get; set; }
        public ParseCommand CancelCommand { get; set; }
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();


        public LoadFile()
        {
            UrlList = new ObservableCollection<string>();
            WriteListBox = new ParseCommand(OpenFile);
            StartParse = new ParseCommand(ParseStart, CanLoad);
            StartParseSingle = new ParseCommand(ParseStartSingle, CanLoadSingle);
            ListLogInfos = new ObservableCollection<LogInfo>();
            OpenLink = new ParseCommand(LoadAll, CanLoad);
            OpenLinkSingle = new ParseCommand(LoadSingle, CanLoadSingle);
            CancelCommand = new ParseCommand(CancelProcess);
        }

        

        public async void OpenFile(object check)
        {
            await Task.Run(() =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    if (openFileDialog.ShowDialog() == true)
                    {
                        PathFile = openFileDialog.FileName;
                       
                        LoadingFile((bool)check);
                    }

                }
            );
        }

        private async void LoadingFile(bool check)
        {
            await Task.Run(() =>
            {
                using (FileStream fstream = File.OpenRead(PathFile))
                {
                    TextFromFile = File.ReadAllText(PathFile);

                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    
                    string[] ar = (TextFromFile.Trim().Split('\n'));
                    foreach (var item in ar)
                    {
                        UrlList.Add(item);
                    }
                    
                });
                if (check)
                    LoadAll(null);

            });

        }

        public async void ParseStart(object str)
        {
            await Task.Run(() =>
                {
                    _cancellationToken = new CancellationTokenSource();
                    ParseSite ps = new ParseSite();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ListLogInfos.Clear();
                    });
                    int maxCount = 0;
                    int idMax = -1;
                    int id = 0;
                    foreach (var item in UrlList)
                    {
                        if (_cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        LogInfo li = new LogInfo();
                        li.Url = item;
                        li.Count = ps.SearchTag("a", item);
                        if (li.Count > maxCount)
                        {
                            if (idMax > -1)
                                ListLogInfos[idMax].Color = "White";
                            li.Color = "Green";
                            idMax = id;
                            maxCount = li.Count;
                        }
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ListLogInfos.Add(li);
                        });
                        id++;
                    }

                }
            );
            
        }

        private void CancelProcess(object obj)
        {
            _cancellationToken.Cancel();
        }

        private bool CanCancelProcess(object Url)
        {
            if (ListLogInfos.Count == 0)
                return false;
            return true;
        }
        private async void ParseStartSingle(object Url)
        {
            await Task.Run(() =>
                {
                    ParseSite ps = new ParseSite();

                    LogInfo li = new LogInfo();
                    li.Url = (string)Url;
                    li.Count = ps.SearchTag("a", (string)Url);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ListLogInfos.Clear();
                        ListLogInfos.Add(li);
                    });
                    

                }
            );
           
            
        }
        public void LoadSingle(object Url)
        {
            if (Url is null)
                return;
            Process.Start((string)Url);
        }

        public bool CanLoad(object obj)
        {
            if (UrlList.Count == 0)
                return false;
            return true;
        }
        public bool CanLoadSingle(object obj)
        {
            if (obj == null)
                return false;
            if (UrlList.Count == 0)
                return false;
            return true;
        }
        public void LoadAll(object obj)
        {
            try
            {
                foreach (var url in UrlList)
                {
                    Process.Start(url);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
