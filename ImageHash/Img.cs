using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ImageHash
{
    class Img
    {
        #region Delegates
        public delegate void CallBackDelegate(int i);
        public delegate void UpdateGUIDelegate(string str, bool append = true);
        #endregion

        #region Properties
        // Properties
        public Image Image { get; set; }
        public string Base64String { get; set; } = null;
        public string HashedBase64String { get; set; } = null;
        public bool IsHashed { get; set; }
        #endregion

        #region Fields
        // Fields
        private List<int> Key;
        private byte[] ByteArray;
        #endregion

        #region Class constants
        // Class constants
        public const string BDELIM = " ";
        #endregion

        #region Class constructors
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
        }
        #endregion

        #region Methods
        // Methods
        public void SaveKey(StreamWriter sw)
        {
            sw.Write(String.Join(Img.BDELIM, Key));
            sw.Close();
        }

        public void SaveBase64String(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(Base64String);
            }
        }
        public void HashBase64String(CallBackDelegate callBack, UpdateGUIDelegate updateGUI)
        {
            if (IsHashed)
            {
                updateGUI("Base64 string is already hashed..");
                return;
            }
            IsHashed = true;
            HashedBase64String = HashString(Base64String, ref Key, callBack, updateGUI);
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
        #endregion

        #region Static Methods
        // Static methods
        // Loading from byte array while the array itself is passed
        private static Img FromByteArray(byte[] bArr)
        {
            ImageConverter imgCon = new ImageConverter();
            Image image = (Image)imgCon.ConvertFrom(bArr);

            return new Img(image);
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

        public static Img FromHashedFile(string str, string stringKey, CallBackDelegate callBack, UpdateGUIDelegate updateGUI)
        {
            string newStr = null;
            updateGUI("Parsing key..");
            List<int> key = Img.ParseKey(stringKey, callBack);
            updateGUI("Key parsed successfully..\nUnhashing the file using the key..");
            newStr = Img.Unhash(str, key, callBack);
            updateGUI("Done");
            return Img.FromBase64String(newStr, false);
        }

        // Shuffle list; Not completely random, but fastest way
        public static List<T> ShuffleList<T>(List<T> list, CallBackDelegate callBack)
        {
            Random random = new Random();
            int cb = 0;
            for (int i = 0, j = 0; i < list.Count; i++, j++)
            {
                var index = random.Next(i, list.Count);
                var temp = list[index];
                list[index] = list[i];
                list[i] = temp;

                if (list.Count / 100 == j)
                {
                    j = 0;
                    cb++;
                    callBack(cb);
                }
            }
            return list;
        }

        // Hash string and generate it's key
        static string HashString(string str, ref List<int> key, CallBackDelegate callBack, UpdateGUIDelegate updateGUI)
        {
            updateGUI("Generating key..");
            key = ShuffleList(Enumerable.Range(0, str.Length).ToList(), callBack);
            updateGUI("Key generated..\nHashing image..");
            char[] tempCArr = new char[str.Length];
            int cb = 0;
            callBack(cb);
            for (int i = 0, j = 0; i < str.Length; i++, j++)
            {
                tempCArr[key[i]] = str[i];
                if (str.Length / 100 == j)
                {
                    j = 0;
                    cb++;
                    callBack(cb);
                }
            }
            updateGUI("Image hashed sucessfully..");
            return new string(tempCArr); ;
        }

        public static List<int> ParseKey(string stringKey, CallBackDelegate callBack)
        {
            string[] parsedKeyString = stringKey.Split(Img.BDELIM.ToCharArray());
            List<int> key = new List<int>();

            // Parse key
            int cb = 0;
            for (int i = 0, j = 0; i < parsedKeyString.Length; i++, j++)
            {
                key.Add(Convert.ToInt32(parsedKeyString[i]));

                if (parsedKeyString.Length / 100 == j)
                {
                    j = 0;
                    cb++;
                    callBack(cb);
                }
            }
            return key;
        }
        public static string Unhash(string str, List<int> key, CallBackDelegate callBack)
        {
            char[] tempCArr = new char[str.Length];

            int cb = 0;
            for (int i = 0, j = 0; i < key.Count; i++, j++)
            {
                tempCArr[i] = str[key[i]];

                if (key.Count / 100 == j)
                {
                    j = 0;
                    cb++;
                    callBack(cb);
                }
            }
            return new string(tempCArr);
        }
        #endregion
    }
}
