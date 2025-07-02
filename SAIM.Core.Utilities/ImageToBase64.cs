using System;
using System.Drawing;
using System.IO;

namespace SAIM.Core.Utilities
{
    /// <summary>
    /// Summary description for ImageToBase64.
    /// </summary>
    public class ImageToBase64
    {
        public ImageToBase64()
        {
        }

        /// <summary>
        /// Transforme une chaîne de caractères
        /// en base 64 en un objet <see cref="System.Drawing.Image"/>
        /// </summary>
        /// <param name="str">Chaîne de caractères</param>
        /// <returns><see cref="System.Drawing.Image"/> résultante</returns>
        public Image GetImageFromBase64String(string str)
        {
            return GetImageFromBase64String(str, false);
        }

        /// <summary>
        /// Transforme une chaîne de caractères
        /// en base 64 en un objet <see cref="System.Drawing.Image"/>
        /// </summary>
        /// <param name="str">Chaîne de caractères</param>
        /// <param name="makeTransparent">Si vrai, rend la couleur de fond transparent</param>
        /// <returns><see cref="System.Drawing.Image"/> résultante</returns>
        public Image GetImageFromBase64String(string str, bool makeTransparent)
        {
            return GetImageFromBytes(Convert.FromBase64String(str), makeTransparent);
        }

        /// <summary>
        /// Transforme un tableau de <see cref="byte"/>
        /// en base 64 en un objet <see cref="System.Drawing.Image"/>
        /// </summary>
        /// <param name="bytes">tableau de <see cref="byte"/></param>
        /// <returns><see cref="System.Drawing.Image"/> résultante</returns>
        public Image GetImageFromBytes(byte[] bytes)
        {
            return GetImageFromBytes(bytes, false);
        }

        /// <summary>
        /// Transforme un tableau de <see cref="byte"/>
        /// en base 64 en un objet <see cref="System.Drawing.Image"/>
        /// </summary>
        /// <param name="bytes">tableau de <see cref="byte"/></param>
        /// <param name="makeTransparent">Si vrai, rend la couleur de fond transparent</param>
        /// <returns><see cref="System.Drawing.Image"/> résultante</returns>
        public Image GetImageFromBytes(byte[] bytes, bool makeTransparent)
        {
            try
            {
                var ms = new MemoryStream(bytes);
                var img = (Bitmap)Bitmap.FromStream(ms);
                if (makeTransparent)
                    img.MakeTransparent();

                ms.Close();
                return img;
            }
            catch
            {
               // MessageBox.Show(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Transforme un objet <see cref="System.Drawing.Image"/>
        /// en un tableau de <see cref="byte"/>.
        /// </summary>
        /// <param name="img">Objet <see cref="System.Drawing.Image"/></param>
        /// <returns>tableau de <see cref="byte"/></returns>
        public byte[] GetBytesFromImage(Image img)
        {
            var ms = new MemoryStream();
            img.Save(ms, img.RawFormat);
            return ms.ToArray();
        }

        /// <summary>
        /// Transforme un objet <see cref="System.Drawing.Image"/>
        /// en une chaîne en base 64.
        /// </summary>
        /// <param name="img">Objet <see cref="System.Drawing.Image"/></param>
        /// <returns>Chaîne de caractères</returns>
        public string GetBase64StringFromImage(Image img)
        {
            return Convert.ToBase64String(GetBytesFromImage(img));
        }

        public String GetStringFromBase64String(string str)
        {
            return GetStringFromBase64String(str, false);
        }

        public String GetStringFromBase64String(string str, bool makeTransparent)
        {
            return str;
        }


    }
}
