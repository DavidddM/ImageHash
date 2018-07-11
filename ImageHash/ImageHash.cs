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
        Img Img = null;

        public ImageHash()
        {
            InitializeComponent();
        }

        #region button-clicks
        private void ChooseImage(object sender, EventArgs e)
        {
            if(chooseImageDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Img = new Img(Image.FromFile(chooseImageDialog.FileName));
                    richTextBox1.Text = binary ? String.Join(Img.BDELIM, Img.ByteArray) : Img.Base64String;
                    pictureBox1.Image = Img.Image;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error has occured. Make sure that you've selected valid file and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void SaveAs(object sender, EventArgs e)
        {
            if (saveAsDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (binary) Img.SaveByteArray(saveAsDialog.FileName);
                    else Img.SaveBase64String(saveAsDialog.FileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("An error has occured. Make sure that the image has successfully been loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void OpenFile(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (!AutoType)
                    {
                        if (binary) { Img = Img.FromByteArray(openFileDialog.FileName); }
                        else { Img = Img.FromBase64String(openFileDialog.FileName, true); }
                    }
                    else
                    {
                        using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                        {
                            string tempStr = sr.ReadToEnd();
                            if (Img.IsByteArray(tempStr))
                            {
                                Img = Img.FromByteArray(Img.FromStringToByteArray(tempStr));
                                radioButton1.Checked = true;
                                binary = true;
                            }
                            else
                            {
                                Img = Img.FromBase64String(tempStr, false);
                                radioButton2.Checked = true;
                                binary = false;
                            }
                        }
                    }
                    pictureBox1.Image = Img.Image;
                    richTextBox1.Text = binary ? String.Join(Img.BDELIM, Img.ByteArray) : Img.Base64String;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("An error has occured. Make sure that you've selected valid file type and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region radio-buttons
        private void radioButton1_Click(object sender, EventArgs e)
        {
            if (binary) return;
            binary = true;
            if (richTextBox1.Text.Equals("")) return;
            richTextBox1.Text = String.Join(Img.BDELIM, Img.ByteArray);
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            if (!binary) return;
            binary = false;
            if (richTextBox1.Text.Equals("")) return;
            richTextBox1.Text = Img.Base64String;
        }
        #endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            AutoType = !AutoType;
        }  
    }
}
