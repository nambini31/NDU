using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SAIM.Core.Utilities
{
    public static class Helper
    {

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            //MemoryStream ms = new MemoryStream();
            //imageIn.Save(ms, ImageFormat.Bmp);
            //return ms.ToArray();
            byte[] arr;
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Png);
                ms.Close();
                arr =  ms.ToArray();
            }

            return arr;
        }

        public static System.Drawing.Image GetImageFromBytes(byte[] bytes, bool makeTransparent)
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
                //MessageBox.Show(e.Message);
                return null;
            }
        }
        public static System.Drawing.Image GetImageFromBytes(byte[] bytes)
        {
            return GetImageFromBytes(bytes, false);
        }
    }
}
