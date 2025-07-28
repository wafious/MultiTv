using LibVLCSharp.Shared;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MultiTv
{
    public interface IMainForm
    {
        void NotifyAllIsolated(bool isIsolated, int sourceInstanceId);
        bool TryRestoreControl(VideoControl control);
        void AddVideoControlToDynamicTable(TableLayoutPanel panel, VideoControl vc);
    }
    public interface IVideoControl
    {
        void NotifyIsolated(bool isFullscreen, int instance = 0);
        int InstanceId { get; set; }
        bool IsInUse { get; }
        void SetPlayer(IVLCPlayer player);
        ContextMenuStrip GetIsolatedMenu();
    }
    public interface IVLCPlayer
    {
        void Play(string url);
        void Stop();
        void Mute();
        void SetVolume(int volume);
        void AttachTo(IntPtr handle);
        bool Init(IVLCPlayer p, string url = "");
        VLCState State { get; }
        bool SetMedia(string url);
        bool SetHandle(IntPtr handle);
        bool IsInit();
    }

    #region IsolatedForm
    class IsolatedForm : Form, IVideoControl
    {
        private static IsolatedForm instance;
        private static VideoControl internalControl;
        private static IVLCPlayer player;
        private static IMainForm mainForm;

        public IsolatedForm(VideoControl sourceControl)
        {
            instance = this;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                       ControlStyles.UserPaint |
                       ControlStyles.DoubleBuffer, true);
            Size = new Size(400, 800);
            internalControl = sourceControl;
            internalControl.Dock = DockStyle.Fill;

            this.Controls.Add(internalControl);
            internalControl.NotifyIsolated(true, internalControl.InstanceId);
            // Replace this line in the constructor:
            // this.FormClosed += OnExitFullscreen();

            // With this:
            this.FormClosed += (s, e) => OnExitFullscreen();
        }

        public static void ShowFullscreen(VideoControl control, IVLCPlayer player)
        {
            if (instance != null) return;

            instance = new IsolatedForm(control);
            mainForm = control.FindForm() as IMainForm;

            var newControl = new VideoControl
            {
                InstanceId = control.InstanceId
            };
            newControl.SetPlayer(player);
            instance.Controls.Add(newControl);
            newControl.Dock = DockStyle.Fill;
            newControl.ContextMenuStrip = newControl.GetIsolatedMenu();

            mainForm?.NotifyAllIsolated(true, control.InstanceId);

            instance.FormClosed += (s, e) => OnExitFullscreen();
            instance.Show();
        }
        private static void OnExitFullscreen()
        {
            if (internalControl != null)
                internalControl.NotifyIsolated(false, internalControl.InstanceId);

            if (mainForm != null && !mainForm.TryRestoreControl(internalControl))
            {
                internalControl.Dispose();
            }
            instance = null;
        }

        public static void ExitFullscreen()
        {
            OnExitFullscreen();
        }

        public void NotifyIsolated(bool isFullscreen, int instance = 0)
        {
            internalControl.NotifyIsolated(isFullscreen, instance);
        }

        public void SetPlayer(IVLCPlayer player)
        {
            IsolatedForm.player = player;
            internalControl.SetPlayer(player);
        }

        public ContextMenuStrip GetIsolatedMenu()
        {
            return internalControl.GetIsolatedMenu();
        }
        public int InstanceId
        {
            get => internalControl.InstanceId;
            set => internalControl.InstanceId = value;
        }

        public bool IsInUse => internalControl.IsInUse;


    }
    #endregion

    #region VideoControl
    public class VideoControl : UserControl, IVideoControl
    {
        #region "Designer Code"
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.streamURLMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.RefreshMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.fullScrenMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.NurAudioMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.volumeUpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.upMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.downMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.volumeDownMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.MuteMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.PauseMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.RecordMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.CloseMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator8,
            this.streamURLMenu,
            this.toolStripSeparator3,
            this.RefreshMenu,
            this.toolStripMenuItem1,
            this.fullScrenMenu,
            this.toolStripSeparator5,
            this.NurAudioMenu,
            this.toolStripSeparator1,
            this.toolStripSeparator13,
            this.volumeUpMenu,
            this.toolStripSeparator11,
            this.volumeDownMenu,
            this.toolStripSeparator9,
            this.toolStripSeparator12,
            this.MuteMenu,
            this.toolStripSeparator6,
            this.PauseMenu,
            this.toolStripSeparator4,
            this.RecordMenu,
            this.toolStripSeparator7,
            this.CloseMenu,
            this.toolStripSeparator2,
            this.ExitMenu,
            this.toolStripSeparator10});
            this.contextMenu.Name = "contextMenuStrip1";
            this.contextMenu.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.contextMenu.Size = new System.Drawing.Size(181, 352);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(177, 6);
            // 
            // streamURLMenu
            // 
            this.streamURLMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.streamURLMenu.Name = "streamURLMenu";
            this.streamURLMenu.Size = new System.Drawing.Size(180, 22);
            this.streamURLMenu.Text = "Open Link";
            //this.streamURLMenu.Click += new System.EventHandler(this.streamURLMenu_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
            // 
            // RefreshMenu
            // 
            this.RefreshMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.RefreshMenu.Name = "RefreshMenu";
            this.RefreshMenu.Size = new System.Drawing.Size(180, 22);
            this.RefreshMenu.Text = "Refresh";
            //this.RefreshMenu.Click += new System.EventHandler(this.RefreshMenu_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // fullScrenMenu
            // 
            this.fullScrenMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.fullScrenMenu.Name = "fullScrenMenu";
            this.fullScrenMenu.Size = new System.Drawing.Size(180, 22);
            this.fullScrenMenu.Text = "Isolate";
            //this.fullScrenMenu.Click += new System.EventHandler(this.fullScrenMenu_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(177, 6);
            // 
            // NurAudioMenu
            // 
            this.NurAudioMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.NurAudioMenu.Name = "NurAudioMenu";
            this.NurAudioMenu.Size = new System.Drawing.Size(180, 22);
            this.NurAudioMenu.Text = "Audio/Video";
            //this.NurAudioMenu.Click += new System.EventHandler(this.NurAudioMenu_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(177, 6);
            // 
            // volumeUpMenu
            // 
            this.volumeUpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.upMenu,
            this.downMenu});
            this.volumeUpMenu.Name = "volumeUpMenu";
            this.volumeUpMenu.Size = new System.Drawing.Size(180, 22);
            this.volumeUpMenu.Text = "Volume Up";
            //this.volumeUpMenu.Click += new System.EventHandler(this.volumeUpMenu_Click);
            // 
            // upMenu
            // 
            this.upMenu.Name = "upMenu";
            this.upMenu.Size = new System.Drawing.Size(180, 22);
            // 
            // downMenu
            // 
            this.downMenu.Name = "downMenu";
            this.downMenu.Size = new System.Drawing.Size(180, 22);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(177, 6);
            // 
            // volumeDownMenu
            // 
            this.volumeDownMenu.Name = "volumeDownMenu";
            this.volumeDownMenu.Size = new System.Drawing.Size(180, 22);
            this.volumeDownMenu.Text = "Volume Down";
            //this.volumeDownMenu.Click += new System.EventHandler(this.volumeDownMenu_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(177, 6);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(177, 6);
            // 
            // MuteMenu
            // 
            this.MuteMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.MuteMenu.Name = "MuteMenu";
            this.MuteMenu.Size = new System.Drawing.Size(180, 22);
            this.MuteMenu.Text = "Mute";
            //this.MuteMenu.Click += new System.EventHandler(this.MuteMenu_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(177, 6);
            // 
            // PauseMenu
            // 
            this.PauseMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.PauseMenu.Name = "PauseMenu";
            this.PauseMenu.Size = new System.Drawing.Size(180, 22);
            this.PauseMenu.Text = "Pause";
            //this.PauseMenu.Click += new System.EventHandler(this.PauseMenu_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(177, 6);
            // 
            // RecordMenu
            // 
            this.RecordMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.RecordMenu.Name = "RecordMenu";
            this.RecordMenu.Size = new System.Drawing.Size(180, 22);
            this.RecordMenu.Text = "Record";
            //this.RecordMenu.Click += new System.EventHandler(this.RecordMenu_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(177, 6);
            // 
            // CloseMenu
            // 
            this.CloseMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.CloseMenu.Name = "CloseMenu";
            this.CloseMenu.Size = new System.Drawing.Size(180, 22);
            this.CloseMenu.Text = "Close";
            //this.CloseMenu.Click += new System.EventHandler(this.CloseMenu_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // ExitMenu
            // 
            this.ExitMenu.Name = "ExitMenu";
            this.ExitMenu.Size = new System.Drawing.Size(180, 22);
            this.ExitMenu.Text = "Exit";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(177, 6);
            // 
            // VideoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ContextMenuStrip = this.contextMenu;
            this.Name = "VideoControl";
            this.Size = new System.Drawing.Size(547, 545);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem streamURLMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fullScrenMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem MuteMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem PauseMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem CloseMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem RefreshMenu;
        private System.Windows.Forms.ToolStripMenuItem NurAudioMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem RecordMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem volumeUpMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem upMenu;
        private System.Windows.Forms.ToolStripMenuItem downMenu;
        private System.Windows.Forms.ToolStripMenuItem ExitMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem volumeDownMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        #endregion
        #endregion
        public int InstanceId { get; set; }
        public bool IsInUse { get; private set; }
        private IVLCPlayer player;
        private ContextMenuStrip normalMenu;
        private ContextMenuStrip fullscreenMenu;

        public VideoControl()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                       ControlStyles.UserPaint |
                       ControlStyles.DoubleBuffer, true);
            this.Dock = DockStyle.Fill;
            SetupMenus();
        }

        public void SetPlayer(IVLCPlayer player)
        {
            this.player = player;
            player.AttachTo(this.Handle);
            IsInUse = true;
        }
        private void SetupMenus()
        {
            normalMenu = new ContextMenuStrip();
            normalMenu.Items.Add("Open Link", null, OpenLink);
            normalMenu.Items.Add("Isolate", null, GoFullscreen);
            normalMenu.Items.Add("Mute", null, (s, e) => player?.Mute());
            normalMenu.Items.Add("Close", null, (s, e) => player?.Stop());

            fullscreenMenu = new ContextMenuStrip();
            fullscreenMenu.Items.Add("Exit Fullscreen", null, ExitFullscreen);
            fullscreenMenu.Items.Add("Mute", null, (s, e) => player?.Mute());

            this.ContextMenuStrip = normalMenu;
        }

        public ContextMenuStrip GetIsolatedMenu() => fullscreenMenu;

        public void NotifyIsolated(bool isFullscreen, int instance = 0)
        {
            if (instance != this.InstanceId)
                this.Enabled = !isFullscreen;
        }

        private void OpenLink(object sender, EventArgs e)
        {
            var input = new SelectURL()
            {
                Text = "Enter URL",
                StartPosition = FormStartPosition.CenterParent
            };
            if (input.ShowDialog() == DialogResult.OK)
            {
                player?.Play(input.URL);
                IsInUse = true;
            }
        }

        private void GoFullscreen(object sender, EventArgs e)
        {
            IsolatedForm.ShowFullscreen(this, player);
        }

        private void ExitFullscreen(object sender, EventArgs e)
        {
            IsolatedForm.ExitFullscreen();
        }
    }
    #endregion

    #region VlcPlayer
    public class VLCPlayer : IVLCPlayer
    {
        private LibVLC libVLC;
        private MediaPlayer mediaPlayer;
        private Media media;
        private IntPtr m_handle;

        public VLCPlayer()
        {
            Core.Initialize();

            libVLC = new LibVLC(enableDebugLogs: false);
            mediaPlayer = new MediaPlayer(libVLC)
            {
                EnableKeyInput = false,
                EnableMouseInput = false
            };
        }

        public void Play(string url)
        {
            Stop();
            media = new Media(libVLC, url, FromType.FromLocation);
            mediaPlayer.Play(media);
            mediaPlayer.EnableKeyInput = false;
            mediaPlayer.EnableMouseInput = false;

        }

        public void Stop()
        {
            if (mediaPlayer.IsPlaying)
            {
                mediaPlayer.Stop();
            }
            media?.Dispose();
        }

        public void Mute()
        {
            mediaPlayer.Mute = !mediaPlayer.Mute;
        }

        public void SetVolume(int volume)
        {
            mediaPlayer.Volume = volume;
        }

        public void AttachTo(IntPtr handle)
        {
            mediaPlayer.Hwnd = handle;
        }

        public VLCState State
        {
            get
            {
                if (mediaPlayer == null)
                    return VLCState.Error;
                return mediaPlayer.State;
            }
        }
        public bool SetMedia(string url)
        {
            try
            {
                media = new Media(libVLC, new Uri(url));
                mediaPlayer = new MediaPlayer(media);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SetMedia(string url, IntPtr handle)
        {
            try
            {
                media = new Media(libVLC, new Uri(url));
                mediaPlayer = new MediaPlayer(media);

                if (handle != IntPtr.Zero)
                {
                    mediaPlayer.Hwnd = handle;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SetMedia(string url, IntPtr handle, bool no_Video = false)
        {
            if (no_Video)
            {
                try
                {
                    media = new Media(libVLC, new Uri(url), ":no-video");

                    mediaPlayer = new MediaPlayer(media);

                    if (handle != IntPtr.Zero)
                    {
                        mediaPlayer.Hwnd = handle;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return SetMedia(url, this.m_handle);
            }
        }
        public IntPtr Handle
        {
            get
            {
                if (m_handle == IntPtr.Zero)
                {
                    m_handle = Marshal.GetIUnknownForObject(mediaPlayer);
                }
                return m_handle;
            }
        }
        public int InstanceId { get; set; }
        public bool IsInit()
        {
            return mediaPlayer == null ? false : true;
        }

        public bool Record(string _format, string qualityLevel)
        {
            string record = "record_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            if (mediaPlayer?.Media?.Mrl != null)
            {
                string path = $"_record_{DateTime.Now:yyyyMMdd_HHmmss}.{_format}";
                string outputOptions = GenerateSoutOptions(_format, qualityLevel, path);
                var media = new Media(libVLC, mediaPlayer.Media.Mrl, FromType.FromLocation);
                media.AddOption(outputOptions);
                media.AddOption(":sout-keep");
                mediaPlayer.Stop();
                mediaPlayer.Play(media);
            }
            return mediaPlayer.IsPlaying;
        }

        private string GenerateSoutOptions(string format, string quality, string path)
        {
            string mux;
            switch (format.ToLower())
            {
                case "mp4":
                    mux = "mp4";
                    break;
                case "flv":
                    mux = "flv";
                    break;
                case "ts":
                    mux = "ts";
                    break;
                default:
                    mux = "mp4";
                    break;
            }

            string videoSettings;
            switch (quality.ToLower())
            {
                case "high":
                    videoSettings = "vcodec=h264,vb=4096,fps=30";
                    break;
                case "medium":
                    videoSettings = "vcodec=h264,vb=2048,fps=25";
                    break;
                case "low":
                    videoSettings = "vcodec=h264,vb=1024,fps=20";
                    break;
                default:
                    videoSettings = "vcodec=h264,vb=2048,fps=25";
                    break;
            }

            string audioSettings;
            switch (quality.ToLower())
            {
                case "high":
                    audioSettings = "acodec=mp4a,ab=256";
                    break;
                case "medium":
                    audioSettings = "acodec=mp4a,ab=128";
                    break;
                case "low":
                    audioSettings = "acodec=mp4a,ab=96";
                    break;
                default:
                    audioSettings = "acodec=mp4a,ab=128";
                    break;
            }

            return ":sout=#transcode{" + videoSettings + "," + audioSettings + "}:duplicate{dst=display,dst=standard{access=file,mux=" + mux + ",dst=\"" + path + "\"}}";
        }

        internal void VolumeUp()
        {
            this.mediaPlayer.Volume += 10;
        }
        internal void VolumeDown()
        {
            this.mediaPlayer.Volume -= 10;
        }

        public bool Init(IVLCPlayer p, string url = "")
        {
            if (IsInit())
            {
                this.mediaPlayer = (MediaPlayer)p; return true;
            }
            else
            {
                SetMedia(url, this.m_handle);
                return false;
            }
        }
        bool IVLCPlayer.SetHandle(IntPtr handle)
        {
            if (mediaPlayer == null)
                return false;
            if (m_handle != IntPtr.Zero)
            {
                mediaPlayer.Hwnd = handle;
            }
            else
            {
                m_handle = handle;
            }
            return true;
        }
    }
    #endregion
    
    public partial class MultiTv : Form, IMainForm
    {
        private List<VideoControl> controls= new List<VideoControl>();

        public MultiTv()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                                   ControlStyles.UserPaint |
                                   ControlStyles.DoubleBuffer, true);

            this.BackColor = System.Drawing.Color.Black;
            this.Dock = DockStyle.Fill;

            SetupLayout();
        }

        private void SetupLayout()
        {
            for (int i = 0; i < 4; i++)
            {
                var vc = new VideoControl { InstanceId = i };
                vc.SetPlayer(new VLCPlayer());
                controls.Add(vc);
                pContainer.Controls.Add(vc, i % 2, i / 2);
            }
        }

        public void NotifyAllIsolated(bool isIsolated, int instanceId)
        {
            foreach (var vc in controls)
                vc.NotifyIsolated(isIsolated, instanceId);
        }

        public bool TryRestoreControl(VideoControl control)
        {
            foreach (Control c in pContainer.Controls)
            {
                if (c is Panel p && p.Controls.Count == 0)
                {
                    p.Controls.Add(control);
                    control.Dock = DockStyle.Fill;
                    return true;
                }
            }
            return false;
        }

        public void AddVideoControlToDynamicTable(TableLayoutPanel panel, VideoControl vc)
        {
            if (panel.ColumnCount == 2 && panel.RowCount == 2 && panel.Controls.Count >= 4)
            {
                var controls = panel.Controls.Cast<Control>().ToList();
                panel.Controls.Clear();

                panel.ColumnCount = 4;
                panel.RowCount = (int)Math.Ceiling(controls.Count / 4.0);

                for (int i = 0; i < controls.Count; i++)
                {
                    int row = i / 4;
                    int col = i % 4;
                    panel.Controls.Add(controls[i], col, row);
                }
            }

            int currentCapacity = panel.RowCount * panel.ColumnCount;
            if (panel.Controls.Count >= currentCapacity)
            {
                panel.RowCount++;
                panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            for (int row = 0; row < panel.RowCount; row++)
            {
                for (int col = 0; col < panel.ColumnCount; col++)
                {
                    if (panel.GetControlFromPosition(col, row) == null)
                    {
                        panel.Controls.Add(vc, col, row);
                        return;
                    }
                }
            }
        }
    }
}
