#define HANDLE_CLOSE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using PCITC.MES.MM.Tapper.Engine.Broker;
using PCITC.MES.MM.Tapper.Engine.Configurations;
using PCITC.MES.MM.Tapper.Engine.Consumer;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Engine.Producer;
using PCITC.MES.MM.Tapper.Engine.SignalR;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Configurations;
using PCITC.MES.MM.Tapper.Framework.Dapper;
using PCITC.MES.MM.Tapper.Framework.Log4Net;
using PCITC.MES.MM.Tapper.Framework.Serializing;
using PCITC.MES.MM.Tapper.Framework.WcfParser;

namespace PCITC.MES.MM.Tapper.Console
{
    public partial class MainForm : Form
    {
        private bool _started = false;
        private Broker _broker;
        private Producer _producer;
        private List<Consumer> _consumers; 
        private ConsoleSetting Setting { get;}
        private IEnumerable<NotifyEntity> _logs;
        private IEnumerable<TopicModel> _topics; 

        public MainForm()
        {
            InitializeComponent();
            Setting = new ConsoleSetting();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //IList<string> addresses=new List<string>();
            //foreach (var interfaces in NetworkInterface.GetAllNetworkInterfaces())
            //{
            //    foreach (var address in interfaces.GetIPProperties().UnicastAddresses)
            //    {
            //        if (address.Address.AddressFamily == AddressFamily.InterNetwork)
            //        {
            //            addresses.Add(address.Address.ToString());
            //        }
            //    }
            //}

            using (var connection = new OracleConnection(Setting.ConnectionStr))
            {
                connection.Open();
                _topics = connection.QueryList<TopicModel>(null, Setting.TopicModelTable, "*");
                if (_topics == null)
                {
                    return;
                }
                cbTopic.Items.Add("ALL");
                cbTopic.Items.AddRange(_topics.Select(t => t.TopicName).ToArray());
                connection.Close();
            }
            cbTopic.SelectedIndex = 0;
            cbLevel.SelectedIndex = 0;
            WcfChannelFactory.CloseChannelFactory();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_started) return;
            if (_broker == null)
            {
                InitializeTapper();
                _broker = Broker.Create().Start();
            }
            else
                _broker.Start();
            if (_producer == null)
                _producer = new Producer("P1").Start();
            else
                _producer.Start();

#if !HANDLE_CLOSE
            if (_consumers != null && _consumers.Count > 0)
            {
                foreach (var consumer in _consumers)
                {
                    consumer.Start();
                }
            }
            else
            {
                _consumers = new List<Consumer>();
                if (_topics.Any())
                {
                    var count = 1;
                    foreach (var topic in _topics)
                    {
                        _consumers.Add(new Consumer("C"+count.ToString("N")).Subscribe(topic.TopicName).Start());
                        count++;
                    }
                }
            }
#endif
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            _started = true;
            tsslNotify.Text = _broker.NotifyUrl;
        }

        private void timerStatistical_Tick(object sender, EventArgs e)
        {
            lbSysTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (DateTime.Now.Second%10 != 0||!_started) return;
            var staticInfos = _broker.GetBrokerStatisticInfo();
            lbTopicCount.Text = staticInfos.TopicCount.ToString();
            lbQueueCount.Text = staticInfos.QueueCount.ToString();
            lbConsumerCount.Text = staticInfos.ConsumerCount.ToString();
            lbAllTaskCount.Text = staticInfos.InMemoryTaskCount.ToString();
            lbActiveTaskCount.Text = staticInfos.ActiveTaskCount.ToString();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            using (var connection = new OracleConnection(Setting.ConnectionStr))
            {
                connection.Open();
                var sql = @"select * from T_MV_NOTIFY_ENTITY where notifytime>=:BegTime and notifyTime<:EndTime";
                var condition =
                    new
                    {
                        BegTime = tpDate.Value.Date,
                        EndTime = tpDate.Value.Date.AddDays(1)
                    };
                _logs= connection.QueryList<NotifyEntity>(sql, condition, null, null);
                connection.Close();
            }
            if (_logs.Any())
            {
                cbTarget.Items.Clear();
                cbTarget.Items.Add("ALL");
                cbTarget.SelectedIndex = 0;
                cbTarget.Items.AddRange(_logs.Where(l=>!string.IsNullOrEmpty(l.NotifyTarget)).Select(l=>l.NotifyTarget).Distinct().ToArray());
            }
            ShowData();
        }

        private void ShowData()
        {
            if (_logs != null&&_logs.Any())
            {
                var showLogs = _logs;
                if (cbTopic.SelectedIndex > 0)
                    showLogs = showLogs.Where(l => l.Topic == cbTopic.SelectedItem.ToString()).ToList();
                if (cbLevel.SelectedIndex > 0)
                    showLogs = showLogs.Where(l => l.NotifyLevel == cbLevel.SelectedItem.ToString()).ToList();
                if (cbTarget.SelectedIndex > 0)
                    showLogs = showLogs.Where(l => l.NotifyTarget == cbTarget.SelectedItem.ToString()).ToList();
                dgvLog.DataSource = showLogs;
                dgvLog.Columns["taskcode"].Visible = false;
                dgvLog.Columns["notifycode"].Visible = false;
                dgvLog.Columns["taskid"].Visible = false;
                dgvLog.Columns["topic"].HeaderText = "TOPIC";
                dgvLog.Columns["topic"].Width = 60;
                dgvLog.Columns["notifylevel"].HeaderText = "LEVEL";
                dgvLog.Columns["notifylevel"].Width = 60;
                dgvLog.Columns["notifytime"].HeaderText = "TIME";
                dgvLog.Columns["notifytime"].Width = 100;
                dgvLog.Columns["notifycontent"].HeaderText = "CONTENT";
                dgvLog.Columns["notifycontent"].Width = 630;
                dgvLog.Columns["notifytarget"].HeaderText = "TARGET";
                dgvLog.Columns["notifytarget"].Width = 140;
                dgvLog.Columns["stackinfo"].HeaderText = "STACK";
                dgvLog.Columns["stackinfo"].Width = 170;
                dgvLog.Show();
            }
        }

        private void cbTopic_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowData();
        }

        private void cbLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowData();
        }

        private void cbTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowData();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_consumers.Count > 0)
            {
                foreach (var consumer in _consumers)
                {
                    consumer.Shutdown();
                }
            }
            _producer.Shutdown();
            _broker.Shutdown();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            _started = false;
            tsslNotify.Text = string.Empty;
        }

        static void InitializeTapper()
        {
            Configuration.Create()
                .UseAutofac()
                .RegisterCommonComponents()
                .UseLog4Net()
                .UseJsonNet()
                .RegisterTapperComponents()
                .RegisterNotification();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            WcfChannelFactory.CloseChannelFactory();
        }
    }
}
