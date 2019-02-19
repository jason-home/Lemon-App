﻿using LemonLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static LemonLibrary.InfoHelper;

namespace Lemon_App
{
    /// <summary>
    /// DataItem.xaml 的交互逻辑
    /// </summary>
    public partial class DataItem : UserControl
    {
        public delegate void MouseDownHandle(DataItem sender);
        public event MouseDownHandle Play;
        public event MouseDownHandle Add;
        public event MouseDownHandle Download;

        public Music music;
        private MainWindow Mainwindow = null;
        private bool needb = false;
        public DataItem(Music dat,bool needDeleteBtn=false,MainWindow mw=null)
        {
            try
            {
                InitializeComponent();
                Mainwindow = mw;
                music = dat;
                Loaded += delegate {
                    needb = needDeleteBtn;
                    name.Text = dat.MusicName;
                    ser.Text = dat.Singer;
                    mss.Text = dat.MusicName_Lyric;
                };
            }
            catch { }
        }
        bool ns = false;
        public bool isChecked = false;
        public void ShowDx() {
            if (He.LastItem != null)
            {
                mss.Opacity = 0.6;
                ser.Opacity = 0.8;
                He.LastItem.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                He.LastItem.namss.SetResourceReference(ForegroundProperty, "ResuColorBrush");
                He.LastItem.ser.SetResourceReference(ForegroundProperty, "ResuColorBrush");
                He.LastItem.color.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
            Background = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
            ser.SetResourceReference(ForegroundProperty, "ThemeColor");
            namss.SetResourceReference(ForegroundProperty, "ThemeColor");
            color.SetResourceReference(BackgroundProperty, "ThemeColor");
            mss.Opacity = 1;
            ser.Opacity = 1;

            He.LastItem = this;
        }
        public void NSDownload(bool ns) {
            this.ns = ns;
            if (ns)
            {
                namss.Margin = new System.Windows.Thickness(60, 0, 10, 0);
                CheckView.Visibility = System.Windows.Visibility.Visible;
                MouseDown += CheckView_MouseDown;
            }
            else {
                namss.Margin = new System.Windows.Thickness(10, 0, 10, 0);
                CheckView.Visibility = System.Windows.Visibility.Collapsed;
                MouseDown -= CheckView_MouseDown;
            }
        }

        private void CheckView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Check();
        }

        public void Check() {
            if (!isChecked)
            {
                GO.Visibility = Visibility.Visible;
                CheckView.SetResourceReference(BorderBrushProperty, "ThemeColor");
                isChecked = true;
            }
            else {
                GO.Visibility = Visibility.Collapsed;
                CheckView.SetResourceReference(BorderBrushProperty, "TextX1ColorBrush");
                isChecked = false;
            }
        }

        private void PlayBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Play(this);
            if (!ns)
                ShowDx();
        }
        private Dictionary<string, string> ListData = new Dictionary<string, string>();//name,id
        private async void AddBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Add_Gdlist.Children.Clear();
            ListData.Clear();
            JObject o = JObject.Parse(await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/splcloud/fcgi-bin/songlist_list.fcg?utf8=1&-=MusicJsonCallBack&uin={Settings.USettings.LemonAreeunIts}&rnd=0.693477705380313&g_tk=1803226462&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0", null,
                "yqq_stat=0; pgv_info=ssid=s9030869079; ts_last=y.qq.com/; pgv_pvid=6655025332; ts_uid=7057611058; pgv_pvi=8567083008; pgv_si=s9362499584; _qpsvr_localtk=0.4296063820147471; uin=o2728578956; skey=@uvgfbeYR4; ptisp=cm; RK=sKKMfg2M0M; ptcz=7b132d6e799e806b8e425c58966902006b53fc86e1ecb0d2678db33d44f7239d; luin=o2728578956; lskey=000100007afb4fb9a74df33c89945350f16ebce7ef0faca9f0da0aa18246d750b53e68077ae6f83ba3901761; p_uin=o2728578956; pt4_token=-HKvK3MM2TlBjBsPDdO*3Iw6shscAWOJEz-pf5eTH2g_; p_skey=rW8tk6zH9QhmEIOjVvvBXdIeyFQbGGi2xnYiT8f2Ioo_; p_luin=o2728578956; p_lskey=000400008417d0c8d018dff398f9512dcee49d8e75aeae7d975e77c39a169d1e17a25b0dfecfb63bebf56cba; ts_refer=xui.ptlogin2.qq.com/cgi-bin/xlogin"));
            foreach (var a in o["list"]) {
                string name = a["dirname"].ToString();
                ListData.Add(name, a["dirid"].ToString());
                var mdb = new MDButton(true) { TName = name,Margin=new System.Windows.Thickness(0, 10, 0, 0) };
                mdb.MouseDown += Mdb_MouseDown;
                Add_Gdlist.Children.Add(mdb);
            }
            var md = new MDButton(true) { TName = "取消", Margin = new System.Windows.Thickness(0, 10, 0, 0) };
            md.MouseDown += delegate { Gdpop.IsOpen = false; };
            Add_Gdlist.Children.Add(md);
            Gdpop.IsOpen = true;
            Add(this);
        }

        private void Mdb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Gdpop.IsOpen = false;
            string name = (sender as MDButton).TName;
            string id = ListData[name];
            string[] a = MusicLib.AddMusicToGD(music.MusicID,id);
            Toast.Send(a[1]+": "+a[0]);
        }

        private void DownloadBtn_MouseDown(object sender, MouseButtonEventArgs e) => Download(this);

        private async void DeleteBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("确定要删除此歌曲吗?") == System.Windows.Forms.DialogResult.OK)
            {
                int index = He.MGData_Now.Data.IndexOf(music);
                string dirid = await MusicLib.GetGDdiridByNameAsync(He.MGData_Now.name);
                string Musicid = He.MGData_Now.ids[index];
                Mainwindow.DataItemsList.Children.RemoveAt(index);
                Toast.Send(MusicLib.DeleteMusicFromGD(Musicid, He.MGData_Now.id, dirid));
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Buttons.Visibility = Visibility.Visible;
            if(needb)DeleteBtn.Visibility = Visibility.Visible;
            grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#07000000"));
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Buttons.Visibility = Visibility.Collapsed;
            DeleteBtn.Visibility = Visibility.Collapsed;
            grid.Background = new SolidColorBrush(Colors.Transparent);
        }
    }


    public class He
    {
        public static DataItem LastItem = null;
        public static MusicGData MGData_Now = null;
    }
}
