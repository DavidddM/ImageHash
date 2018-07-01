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
        public ImageHash()
        {
            InitializeComponent();
        }

        private void ChooseImage(object sender, EventArgs e)
        {
            bool binary = radioButton1.Checked;
            LoadImage(binary);
        }
        private void SaveAs(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            MessageBox.Show(saveFileDialog1.FileName + " has been saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void LoadImage(bool binary)
        {
            if (chooseImageDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Load(chooseImageDialog.FileName);
                richTextBox1.Text = "Loading..";
                Thread th = new Thread(() => { UpdateGUI(GetBArr(chooseImageDialog.FileName), binary); });
                th.Start();
            }
        }

        private void UpdateGUI(byte[] bArr, bool binary)
        {
            string richtext = "";
            richtext = binary ? String.Join(null, bArr) : Convert.ToBase64String(bArr);
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

        public void SaveRichText()
        {
            using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
            {
                sw.Write(richTextBox1.Text);
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            SaveRichText();
        }
    }
}
