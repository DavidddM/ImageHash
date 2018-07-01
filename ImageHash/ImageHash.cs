using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public bool binary = false;
        public bool AutoType = false;
        public readonly string bArrDelim = " ";

        public ImageHash()
        {
            InitializeComponent();
        }

        #region button-clicks
        private void ChooseImage(object sender, EventArgs e)
        {
            chooseImageDialog.ShowDialog();
        }
        private void SaveAs(object sender, EventArgs e)
        {
            saveAsDialog.ShowDialog();
        }
        private void OpenFile(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }
        #endregion

        #region load-image
        public void LoadImage()
        {
            try
            {
                pictureBox1.Load(chooseImageDialog.FileName);
                richTextBox1.Text = "Loading..";
                Thread th = new Thread(() => { UpdateGUI(GetBArr(chooseImageDialog.FileName), binary); });
                th.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occured. Invalid image file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void LoadImage(string str)
        {
            if (AutoType)
            {
                if (IsByteArray(str))
                {
                    binary = true;
                    radioButton1.Checked = true;
                }
                else
                {
                    binary = false;
                    radioButton2.Checked = true;
                }
            }
            try
            {
                byte[] bArr = binary ? FromStringToBARR(str) : Convert.FromBase64String(str);
                Image image;
                using (MemoryStream ms = new MemoryStream(bArr))
                {
                    image = Image.FromStream(ms);
                }

                pictureBox1.Image = image;
            }

            catch (Exception)
            {
                MessageBox.Show("Given file isn't in correct format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                richTextBox1.Text = "";
                pictureBox1.Image = null;
                return;
            }
            
        }
        #endregion

        #region save/load richtext
        public void SaveRichText(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(richTextBox1.Text);
            }
        }

        public void LoadRichText(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string str = sr.ReadToEnd();
                richTextBox1.Text = str;
                LoadImage(str);
            }
        }
        #endregion

        #region file-dialogs
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            SaveRichText(saveAsDialog.FileName);
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            LoadRichText(openFileDialog.FileName);
        }

        private void chooseImageDialog_FileOk(object sender, CancelEventArgs e)
        {
            LoadImage();
        }
        #endregion

        #region helper-methods
        private void UpdateGUI(byte[] bArr, bool binary)
        {
            string richtext = "";
            richtext = binary ? String.Join(bArrDelim, bArr) : Convert.ToBase64String(bArr);
            richTextBox1.Text = richtext;
        }

        private byte[] GetBArr(object fileName)
        {
            Image img = Image.FromFile(fileName.ToString());
            MemoryStream memoryStream = new MemoryStream();
            img.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            byte[] byteArr = memoryStream.ToArray();

            return byteArr;
        }

        public byte[] FromStringToBARR(string str)
        {
            string[] sArr = str.Split(bArrDelim.ToCharArray());
            byte[] bArr = new byte[sArr.Length];

            for (int i = 0; i < sArr.Length; i++)
            {
                bArr[i] = Convert.ToByte(sArr[i]);
            }

            return bArr;
        }

        public bool IsByteArray(string str)
        {
            foreach(var c in str.Replace(" ", String.Empty))
            {
                if (!Int32.TryParse(c.ToString(), out int n)) return false;
            }
            return true;
        }
        #endregion

        #region radio-buttons
        private void radioButton1_Click(object sender, EventArgs e)
        {
            if (binary) return;
            binary = true;
            if (richTextBox1.Text.Equals("")) return;
            richTextBox1.Text = String.Join(bArrDelim, Convert.FromBase64String(richTextBox1.Text));
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            if (!binary) return;
            binary = false;
            if (richTextBox1.Text.Equals("")) return;
            richTextBox1.Text = Convert.ToBase64String(FromStringToBARR(richTextBox1.Text));
        }
        #endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            AutoType = !AutoType;
        }  
    }
}
