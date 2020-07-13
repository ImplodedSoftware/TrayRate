using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ImGeneralPluginEngine;
using ImGeneralPluginEngine.Abstractions;
using NeonScripting;
using NeonScripting.Models;

namespace TrayRate
{
    public class TrayRatePlugin : IImGeneralPlugin
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;
        private INeonScriptHost _host;
        public void InitializePlugin(INeonScriptHost host, Action<IImGeneralPlugin> pluginCloseAction)
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.MainIcon,
                Visible = true,
                Text = "Tray Rate"
            };
            _host = host;
            BuildMenu();
        }

        public void OnEvent(NeonScriptEventTypes eventType)
        {
        }

        public void ClosePlugin()
        {
            _notifyIcon.Visible = false;
            Application.Exit();
        }

        public string Name => "Tray Rate";
        public string Author => "Mikael Stalvik";

        private readonly List<string> _ratingNames = new List<string>
        {
            "No rating",
            "0.5 stars",
            "1 star",
            "1.5 stars",
            "2 stars",
            "2.5 stars",
            "3 stars",
            "3.5 stars",
            "4 stars",
            "4.5 stars",
            "5 stars",
        };
        private ToolStripMenuItem CreateMenuItem(int ratingId)
        {
            var item = new ToolStripMenuItem
            {
                Tag = ratingId,
                Text = _ratingNames[ratingId],
                ToolTipText = _ratingNames[ratingId]
            };
            item.Click += RatingItem_Click;
            return item;
        }

        private void RatingItem_Click(object sender, EventArgs e)
        {
            var tms = (ToolStripMenuItem) sender;
            var tag = (int) tms.Tag;
            if (_host.RemoteCalls.ActiveTrack != null)
            {
                _host.RemoteCalls.RateCurrentTrack(tag);
            }
        }

        private void BuildMenu()
        {
            _contextMenu = new ContextMenuStrip();
            for (var i = 10; i >= 0; i--)
            {
                _contextMenu.Items.Add(CreateMenuItem(i));
            }
            _notifyIcon.ContextMenuStrip = _contextMenu;
            _contextMenu.Opening += (sender, args) =>
            {
                var workRating = 0;
                if (_host.RemoteCalls.ActiveTrack != null)
                {
                    workRating = _host.RemoteCalls.ActiveTrack.Rating;
                    var currentRating = _host.RemoteCalls.DownsizeRating(workRating);
                    foreach (var item in _contextMenu.Items)
                    {
                        var cmi = (ToolStripMenuItem) item;
                        var tag = (int) cmi.Tag;
                        cmi.Checked = tag == currentRating;
                    }
                }
            };
        }
    }
}
