/*
# This Sample Code is provided for the purpose of illustration only and is not intended to be used 
# in a production environment. THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" 
# WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
# WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. We grant You a nonexclusive, 
# royalty-free right to use and modify the Sample Code and to reproduce and distribute the object code 
# form of the Sample Code, provided that You agree: (i) to not use Our name, logo, or trademarks to 
# market Your software product in which the Sample Code is embedded; (ii) to include a valid copyright 
# notice on Your software product in which the Sample Code is embedded; and (iii) to indemnify, hold 
# harmless, and defend Us and Our suppliers from and against any claims or lawsuits, including attorneys’ 
# fees, that arise or result from the use or distribution of the Sample Code.

# This sample script is not supported under any Microsoft standard support program or service. 
# The sample script is provided AS IS without warranty of any kind. Microsoft further disclaims 
# all implied warranties including, without limitation, any implied warranties of merchantability 
# or of fitness for a particular purpose. The entire risk arising out of the use or performance of 
# the sample scripts and documentation remains with you. In no event shall Microsoft, its authors, 
# or anyone else involved in the creation, production, or delivery of the scripts be liable for any 
# damages whatsoever (including, without limitation, damages for loss of business profits, business 
# interruption, loss of business information, or other pecuniary loss) arising out of the use of or 
# inability to use the sample scripts or documentation, even if Microsoft has been advised of the 
# possibility of such damages 
*/

using System;
using System.Windows.Forms;

namespace Pinger
{
    public partial class frmHiddenMain : Form
    {
        public Networking.PingerStatuses PingerStatus = Networking.PingerStatuses.Initializing;
        public Logger FileLogger = new Logger(AppSettingsManager.LogFileLocation, AppSettingsManager.LogEventsToFile);
                
        public frmHiddenMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            FileLogger.LogWithoutDuplicate("New Pinger Session Started...");
            pingTimer.Interval = AppSettingsManager.PingIntervalInSeconds * 1000;
            pingTimer.Enabled = true;
            pingTimer.Start();
            startToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = true;
            pingNowToolStripMenuItem.Enabled = true;
            AttemptToPing();


        }

        private void pingTimer_Tick(object sender, EventArgs e)
        {
            AttemptToPing();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileLogger.LogWithoutDuplicate("Pinger closing by user...");
            Application.Exit();
        }

        private void AttemptToPing()
        {
            if (Networking.PingHost(AppSettingsManager.IpAddressOrHostnameToPing))
            {
                if (AppSettingsManager.ShowNotificationWhenStatusChanges)
                {
                    if (PingerStatus != Networking.PingerStatuses.Alive)
                    {
                        notifyIconStatus.BalloonTipIcon = ToolTipIcon.Info;
                        notifyIconStatus.BalloonTipTitle = $"{AppSettingsManager.IpAddressOrHostnameToPing} is alive...";
                        notifyIconStatus.BalloonTipText = "Ping was successful...";
                        notifyIconStatus.ShowBalloonTip(AppSettingsManager.NotificationTimeoutInSeconds * 1000);
                        FileLogger.LogWithoutDuplicate($"{AppSettingsManager.IpAddressOrHostnameToPing} is alive...");
                    }
                }
                notifyIconStatus.Icon = Properties.Resources.ConnectToRemoteServer;
                notifyIconStatus.Text = $"Pinger (Alive) | {DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt")}";
                PingerStatus = Networking.PingerStatuses.Alive;
            }
            else
            {
                if (AppSettingsManager.ShowNotificationWhenStatusChanges)
                {
                    if (PingerStatus == Networking.PingerStatuses.Alive)
                    {
                        notifyIconStatus.BalloonTipIcon = ToolTipIcon.Warning;
                        notifyIconStatus.BalloonTipTitle = $"{AppSettingsManager.IpAddressOrHostnameToPing} is unreachable!";
                        notifyIconStatus.BalloonTipText = AppSettingsManager.NotificationFailureMessage;
                        notifyIconStatus.ShowBalloonTip(AppSettingsManager.NotificationTimeoutInSeconds * 1000);
                    }
                }
                notifyIconStatus.Icon = Properties.Resources.Disconnected;
                notifyIconStatus.Text = $"Pinger (Unreachable) | {DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt")}";
                PingerStatus = Networking.PingerStatuses.Unreachable;
                FileLogger.LogWithoutDuplicate($"{AppSettingsManager.IpAddressOrHostnameToPing} is unreachable!");
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pingTimer.Start();
            startToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = true;
            pingNowToolStripMenuItem.Enabled = true;
            notifyIconStatus.Text = "Pinger | [Started]";
            FileLogger.LogWithoutDuplicate("Pinger resumed by user...");
            AttemptToPing();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pingTimer.Stop();
            startToolStripMenuItem.Enabled = true;
            stopToolStripMenuItem.Enabled = false;
            pingNowToolStripMenuItem.Enabled = false;
            notifyIconStatus.Icon = Properties.Resources.Disconnected;
            notifyIconStatus.Text = "Pinger | [Stopped]";
            PingerStatus = Networking.PingerStatuses.Stopped;
            FileLogger.LogWithoutDuplicate("Pinger paused by user...");
        }

        private void pingNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AttemptToPing();
        }
    }
}
