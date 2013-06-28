using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Excel;
using DepDot;

namespace DBlast
{
    public partial class Main : Form
    {
        private string _inputFile;
        private BackgroundWorker _bw;
        private Dictionary<string, string> _owners = new Dictionary<string,string>();
        private List<DepDot.LineItem> _lines = new List<DepDot.LineItem>();
        private List<string> _uniqueOwners = new List<string>();
        public bool _allChecked = true;

        private bool _automaticallySendEmails;

        public Main()
        {
            _automaticallySendEmails = false;
           
            InitializeComponent();
        }

        private void btnBrowseInput_Click(object sender, EventArgs e)
        {
            ResetLoadedFile();

            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "Excel file|*.xlsx;*.xls";
            o.Title = "Choose an Excel file to convert...";
            o.Multiselect = false;
            o.RestoreDirectory = true;

            if (o.ShowDialog() == DialogResult.OK)
            {
                _inputFile = o.FileName;
                txtInput.Text = _inputFile;
                txtInput.Select(txtInput.Text.Length - 1, 1);
            }

            if (string.IsNullOrEmpty(_inputFile)) return;

            _bw = new BackgroundWorker();
            _bw.DoWork += new DoWorkEventHandler(PopulateOwnersList);
            _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BrowseLoadComplete);
            //_bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            _bw.WorkerReportsProgress = true;
            _bw.WorkerSupportsCancellation = true;
            btnGenerate.Enabled = false;
            _bw.RunWorkerAsync();
            this.UseWaitCursor = true;
        }

        private void BrowseLoadComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            this.UseWaitCursor = false;

            if (_owners == null || _owners.Count == 0)
            {
                ResetLoadedFile();
                MessageBox.Show("Couldn't read email addresses from the file. Check the format and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                btnGenerate.Enabled = true;
            }
        }

        private void ResetLoadedFile()
        {
            _inputFile = "";
            txtInput.Text = "";
            _owners.Clear();
            _uniqueOwners.Clear();
            _lines.Clear();
            clstOwners.Items.Clear();
            _allChecked = true;
            btnGenerate.Enabled = false;
        }

        private void PopulateOwnersList(object sender, DoWorkEventArgs e)
        {
            GetOwnersEmailLookup();

            foreach (KeyValuePair<string, string> owner in _owners)
            {
                AddOwnerToList(owner.Key);
            }
        }

        private void AddOwnerToList(string owner)
        {
            if (clstOwners.InvokeRequired)
            {
                clstOwners.Invoke(new Action<string>(AddOwnerToList), owner);
                return;
            }

            clstOwners.Items.Add(owner, true);
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text))
            {
                MessageBox.Show("You must choose an Excel file to process.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (chkAutoSendEmails.Checked)
            {
                if (MessageBox.Show("Are you sure you want to send emails automatically once processing finishes?", "Are you sure?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    ProcessFile();
                }
            }
            else
            {
                ProcessFile();
            }
        }

        private void ProcessFile()
        {
            _bw = new BackgroundWorker();
            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            _bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            _bw.WorkerReportsProgress = true;
            _bw.WorkerSupportsCancellation = true;
            btnGenerate.Enabled = false;
            _bw.RunWorkerAsync();
            this.UseWaitCursor = true;
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.UserState as string))
            {
                Console.WriteLine(e.UserState.ToString());
            }
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.UseWaitCursor = false;
            MessageBox.Show("Email notifications created successfully.", "All done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetLoadedFile();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            GetDependencies();
            GetOwnersEmailLookup();
            GetListOfOwners();
            CreateDependencyTables();
        }

        private void GetDependencies()
        {
            _bw.ReportProgress(0, "Reading Excel file (dependencies)...");

            try
            {
                _lines = DepDot.ExcelReader.Read(_inputFile, false, false);
                _bw.ReportProgress(0, _lines.Count + " rows read");
            }
            catch (Exception ex)
            {
                _bw.ReportProgress(0, ex.Message);
                _bw.CancelAsync();
                return;
            }
        }

        private void GetOwnersEmailLookup()
        {
            _bw.ReportProgress(0, "Reading Excel file (email addresses)...");

            if(string.IsNullOrEmpty(_inputFile)) return;

            try
            {
                FileStream testStream = File.Open(_inputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                testStream.Close();
                testStream.Dispose();
            }
            catch (Exception e)
            {
                throw new ApplicationException("ERROR: Couldn't open " + _inputFile + " - " + e.Message);
                _bw.CancelAsync();
            }

            FileStream stream = File.Open(_inputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader excelReader;
            int row = 0;

            if (_inputFile.ToLower().Contains("xlsx"))
            {
                //Reading from a OpenXml Excel file (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                //Reading from a binary Excel file ('97-2003 format; *.xls)
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }

            try
            {
                excelReader.IsFirstRowAsColumnNames = false;
                DataSet result = excelReader.AsDataSet(new List<string> { "Owners" });

                while (excelReader.Read())
                {
                    if (!string.IsNullOrEmpty(excelReader.GetString(0)))
                    {
                        if (!_owners.ContainsKey(excelReader.GetString(0)))
                        {
                            _owners.Add(excelReader.GetString(0), excelReader.GetString(1));
                            _bw.ReportProgress(0, "Added: " + excelReader.GetString(0) + " <" + excelReader.GetString(1) + ">");
                        }
                    }
                }

                _bw.ReportProgress(0, _owners.Count + " owners read");
            }
            catch (Exception e)
            {
                _bw.ReportProgress(0, "ERROR: Could not process Excel file (must contain Tab called 'Owners' and be well formed)");
                //throw new ApplicationException("ERROR: Could not process Excel file (must contain Tab called 'Owners' and be well formed)"); 
            }
            finally
            {
                excelReader.Close();
                _bw.CancelAsync();
            }
        }

        private void CreateDependencyTables()
        {
            StringBuilder sb = new StringBuilder();
            string ownerEmail;

            foreach (string uniqueOwner in _uniqueOwners)
            {
                bool updateThisOwner = true;

                _bw.ReportProgress(0, "Building table for owner: " + uniqueOwner);

                //Check if the owner has an email address for notifications, if not, move to the next owner
                if (_owners.ContainsKey(uniqueOwner))
                {
                    ownerEmail = _owners[uniqueOwner];
                }
                else
                {
                    _bw.ReportProgress(0, "Owner does not have an email address: " + uniqueOwner);
                    continue;
                }

                //Check if the owner is selected in the checkboxlist control
                for (int i = 0; i < clstOwners.Items.Count; i++)
                {
                    if (clstOwners.Items[i].ToString() == uniqueOwner)
                    {
                        if (!clstOwners.GetItemChecked(i))
                        {
                            _bw.ReportProgress(0, "Owner not selected for an update: " + uniqueOwner);
                            updateThisOwner = false;
                        }
                    }
                }
                if (!updateThisOwner) continue;

                sb.AppendLine("<style> body { font-family:tahoma,arial;font-size:9pt; } table { border:1px solid #c0c0c0; border-collapse:collapse; } table th { text-align:left; } table th, table td{ border:1px solid #c0c0c0; padding: 3px 5px 3px 5px; } .nowrap { white-space: nowrap; }</style>");
                sb.AppendLine("<table");
                sb.AppendLine("<tr><th>Schedule</th><th>Version</th><th>Due date</th><th>Project name</th><th>Description</th><th>ID</th><th>Direction</th><th>Related</th><th>Dependency</th><th>Impact</th><th>Control</th><th>Comments</th></tr>");              

                foreach (LineItem line in _lines)
                {
                    if (line.Owner == uniqueOwner)
                    {
                        _bw.ReportProgress(0, " - Dependency ID " + line.Id.ToString() + " found");
                        sb.AppendLine("<tr>");
                        sb.AppendFormat("<td>{0}</td>", line.Schedule);
                        sb.AppendFormat("<td>{0}</td>", line.Version);
                        sb.AppendFormat("<td class=\"nowrap\">{0}</td>", line.ToProduction.ToString("yyyy-MM-dd"));
                        sb.AppendFormat("<td>{0}</td>", line.ProjectName);
                        sb.AppendFormat("<td>{0}</td>", line.Description);
                        sb.AppendFormat("<td>{0}</td>", line.Id);
                        sb.AppendFormat("<td>{0}</td>", line.Direction);
                        sb.AppendFormat("<td>{0}</td>", line.Reference);
                        sb.AppendFormat("<td>{0}</td>", line.Dependency);
                        sb.AppendFormat("<td>{0}</td>", line.Impact);
                        sb.AppendFormat("<td><span style=\"color:{1};\">{0}</span></td>", line.Control, line.Control.ToString().Replace("AMBER","orange"));
                        sb.AppendFormat("<td><small>{0}</small></td>", line.Comments.Replace("/","<br/>"));
                        sb.AppendLine("</tr>");
                    }
                }

                sb.AppendLine("</table>");
                
                try
                {
                    Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                    Microsoft.Office.Interop.Outlook.MailItem mailItem = (Microsoft.Office.Interop.Outlook.MailItem)outlookApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                    mailItem.Subject = "Your project dependencies " + DateTime.Now.ToString("yyyy-MM-dd");
                    mailItem.To = ownerEmail;
                    mailItem.HTMLBody = sb.ToString();
                    mailItem.Display(false);
                    if (_automaticallySendEmails) mailItem.Send();
                }
                catch (Exception eX)
                {
                    _bw.ReportProgress(0, "Error occurred trying to create an Outlook email: " + eX.Message);
                    _bw.CancelAsync();
                }
                sb.Clear();

            }
            _bw.ReportProgress(0, sb.ToString());
        }

        private void GetListOfOwners()
        {
            foreach (LineItem line in _lines)
            {
                if (!_uniqueOwners.Contains(line.Owner))
                {
                    _uniqueOwners.Add(line.Owner);
                }
            }
        }

        private void chkAutoSendEmails_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoSendEmails.Checked)
            {
                _automaticallySendEmails = true;
            }
            else { 
                _automaticallySendEmails = false;
            }
        }

        private void clstOwners_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < clstOwners.Items.Count; i++)
                {
                    if (_allChecked)
                    {
                        clstOwners.SetItemChecked(i, false);
                    }
                    else
                    {
                        clstOwners.SetItemChecked(i, true);
                    }
                }

                _allChecked = !_allChecked;
            }
        }

    }
}
