using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageHash
{
    public partial class ImageHash : Form
    {
        Img Img = null;
        public delegate void LogDelegate(string str, bool append = true);
        public delegate void ProgressBarDelegate(int i);

        public ImageHash()
        {
            InitializeComponent();
        }

        #region button-clicks
        private void ChooseImage(object sender, EventArgs e)
        {
            if (chooseImageDialog.ShowDialog() == DialogResult.OK)
            {
                Log("Loading file..");
                Thread InitClassThread = new Thread(() =>
                {
                    try
                    {
                        Img = new Img(Image.FromFile(chooseImageDialog.FileName));
                        pictureBox1.Image = Img.Image;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("An error has occured. Make sure that you've selected valid file and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
                InitClassThread.Start();
                Log("File loaded successfully..");
            }
        }

        private void SaveAs(object sender, EventArgs e)
        {
            if (saveAsDialog.ShowDialog() == DialogResult.OK)
            {
                Thread SaveFileThread = new Thread(() =>
                {
                    try
                    {
                        Img.SaveBase64String(saveAsDialog.FileName);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("An error has occured. Make sure that the image has successfully been loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
                SaveFileThread.Start();
            }
        }
        private void OpenFile(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Log("Loading file..");
                Thread LoadFileThread = new Thread(() =>
                {
                    try
                    {

                        Img = Img.FromBase64String(openFileDialog.FileName, true);

                        Log("File loaded successfully..");
                        pictureBox1.Image = Img.Image;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("An error has occured. Make sure that you've selected valid file type and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log("", false);
                    }
                });
                LoadFileThread.Start();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Log("", false);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string str = null;
            string key = null;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                Log("Loading file..");
                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    str = sr.ReadToEnd();
                }

                Log("File loaded..\nLoading key..");

                if (openKeyDialog.ShowDialog() == DialogResult.OK)
                {
                    Thread th = new Thread(() =>
                    {
                        try
                        {
                            using (StreamReader sr = new StreamReader(openKeyDialog.FileName))
                            {
                                key = sr.ReadToEnd();
                            }
                            Img = Img.FromHashedFile(str, key, UpdateProgressBar, Log);
                            pictureBox1.Image = Img.Image;
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("An error has occured. Make sure that you've selected valid file type and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log("", false);
                        }
                    });
                    th.Start();
                    return;
                }
                Log("Something went wrong..\nAborting..");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (Img is null)
            {
                Log("Nothing to hash..");
                return;
            }
            Thread HashingThread = new Thread(() => { Img.HashBase64String(UpdateProgressBar, Log); });
            HashingThread.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (saveAsDialog.ShowDialog() == DialogResult.OK)
            {
                Thread SaveFileThread = new Thread(() =>
                {
                    using (StreamWriter sw = new StreamWriter(saveAsDialog.FileName))
                    {
                        sw.Write(Img.HashedBase64String);
                    }
                    Img.SaveKey(new StreamWriter(saveAsDialog.FileName + "k"));
                });
                SaveFileThread.Start();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (Img is null) return;
            Img = null;
            pictureBox1.Image = null;
            Log("Cleared..");
        }
        #endregion

        #region GUI
        public void Log(string str, bool append = true)
        {
            if (!richTextBox1.InvokeRequired)
            {
                if (!append)
                {
                    richTextBox1.Text = str;
                }
                if (append)
                {
                    if (!richTextBox1.Text.Equals("")) richTextBox1.Text += "\n" + str;
                    else Log(str, false);
                }
            }
            else
            {
                LogDelegate LogDeleg = new LogDelegate(Log);
                richTextBox1.Invoke(LogDeleg, new object [] { str, append });
            }
        }

        public void UpdateProgressBar(int i)
        {
            if (!progressBar1.InvokeRequired)
            {
                progressBar1.Value = i;
            }
            else
            {
                ProgressBarDelegate progressBarDeleg = new ProgressBarDelegate(UpdateProgressBar);
                progressBar1.Invoke(progressBarDeleg, new object[] { i });
            }
        }

        public void ResetProgressBar()
        {
            UpdateProgressBar(0);
        }
        #endregion
    }
}