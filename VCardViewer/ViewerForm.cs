﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MsgViewer.Helpers;
using VCardViewer.Properties;

namespace VCardViewer
{
    public partial class ViewerForm : Form
    {
        private readonly List<string> _tempFolders = new List<string>();

        public ViewerForm()
        {
            InitializeComponent();
        }

        private void ViewerForm_Load(object sender, EventArgs e)
        {
            WindowPlacement.SetPlacement(Handle, Settings.Default.Placement);
            Closing += ViewerForm_Closing;
        }

        private void ViewerForm_Closing(object sender, EventArgs e)
        {
            Settings.Default.Placement = WindowPlacement.GetPlacement(Handle);
            Settings.Default.Save();
            foreach (var tempFolder in _tempFolders)
            {
                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);
            }
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            var openFileDialog1 = new OpenFileDialog
            {
                // ReSharper disable once LocalizableElement
                Filter = "VCard|*.vcf",
                FilterIndex = 1,
                Multiselect = false
            };

            if (Directory.Exists(Settings.Default.InitialDirectory))
                openFileDialog1.InitialDirectory = Settings.Default.InitialDirectory;

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.InitialDirectory = Path.GetDirectoryName(openFileDialog1.FileName);

                // Open the selected file to read.
                string tempFolder = null;

                try
                {
                    tempFolder = GetTemporaryFolder();
                    _tempFolders.Add(tempFolder);

                    //var msgReader = new Reader();
                    ////msgReader.SetCulture("nl-NL");
                    ////msgReader.SetCulture("de-DE");
                    //var files = msgReader.ExtractToFolder(openFileDialog1.FileName, tempFolder, HyperLinkCheckBox.Checked);

                    //var error = msgReader.GetErrorMessage();

                    //if (!string.IsNullOrEmpty(error))
                    //    throw new Exception(error);

                    //if (!string.IsNullOrEmpty(files[0]))
                    //    webBrowser1.Navigate(files[0]);

                    //FilesListBox.Items.Clear();

                    //foreach (var file in files)
                    //    FilesListBox.Items.Add(file);
                }
                catch (Exception ex)
                {
                    if (tempFolder != null && Directory.Exists(tempFolder))
                        Directory.Delete(tempFolder, true);

                    MessageBox.Show(ex.Message);
                }
            }
        }

        public string GetTemporaryFolder()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        private void BackButton_Click_1(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void ForwardButton_Click_1(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintDialog();
        }

        private void webBrowser1_Navigated_1(object sender, WebBrowserNavigatedEventArgs e)
        {
            StatusLabel.Text = e.Url.ToString();
        }
    }
}