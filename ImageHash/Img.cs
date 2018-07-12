using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageHash
{
    class Img
    {
        // Properties
        public Image Image { get; set; }
        public byte[] ByteArray { get; set; } = null;
        public string Base64String { get; set; } = null;


        // Class constants
        public const string BDELIM = " ";


        // Class constructors
        public Img(Image Image)
        {
            this.Image = Image;
            InitializeByteArray();
            InitializeBase64String();
        }

        public Img(Image Image, string Base64String)
        {
            this.Image = Image;
            this.Base64String = Base64String;
            InitializeByteArray();
        }

        public Img(Image Image, byte[] ByteArray)
        {
            this.Image = Image;
            this.ByteArray = ByteArray;
            InitializeBase64String();
        }

        // Methods
        public void SaveByteArray(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(String.Join(Img.BDELIM, ByteArray));
            }
        }

        public void SaveBase64String(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(Base64String);
            }
        }

        private void InitializeByteArray()
        {
            ImageConverter imgCon = new ImageConverter();
            ByteArray = (byte[])imgCon.ConvertTo(Image, typeof(byte[]));
        }

        private void InitializeBase64String()
        {
            if(ByteArray is null)
            {
                InitializeByteArray();
            }

            Base64String = Convert.ToBase64String(ByteArray);
        }

        // Static methods
        // Loading from byte array while the array itself is passed
        public static Img FromByteArray(byte[] bArr)
        {
            ImageConverter imgCon = new ImageConverter();
            Image image = (Image)imgCon.ConvertFrom(bArr);

            return new Img(image, bArr);
        }

        // Loading from byte array using file path
        public static Img FromByteArray(string filePath)
        {
            string tempStr = null;
            using (StreamReader sr = new StreamReader(filePath))
            {
                tempStr = sr.ReadToEnd();
            }
            byte[] bArr = Img.FromStringToByteArray(tempStr);
            return FromByteArray(bArr);
        }

        // Loading from Base64 string. Since Base64 string and file path are both the same type, method cannot be overloaded
        public static Img FromBase64String(string str, bool filePath)
        {
            byte[] bArr = null;
            switch (filePath)
            {
                case false:
                    {
                        bArr = Convert.FromBase64String(str);
                        break;
                    }
                case true:
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            string tempStr = sr.ReadToEnd();
                            bArr = Convert.FromBase64String(tempStr);
                        }
                        break;
                    }
            }
            return Img.FromByteArray(bArr);
        }

        // Split string by delimeter and convert it to byte array
        public static byte[] FromStringToByteArray(string str)
        {
            string[] sArr = str.Split(BDELIM.ToCharArray());
            byte[] bArr = new byte[sArr.Length];

            for (int i = 0; i < sArr.Length; i++)
            {
                bArr[i] = Convert.ToByte(sArr[i]);
            }

            return bArr;
        }

        // Check is given string is byte array or not
        public static bool IsByteArray(string str)
        {
            foreach (var c in str.Replace(" ", String.Empty))
            {
                if (!Char.IsDigit(c)) return false;
            }
            return true;
        }
    }
}
