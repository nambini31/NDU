using System.Runtime.InteropServices;
using System.Text;

namespace SAIM.Core.Utilities
{
    public class IniFile 
    {
        public string Path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <param name="iniPath"></param>
        public IniFile(string iniPath)
        {
            Path = iniPath;
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <param name="section"></param>
        /// Section name
        /// <param name="key"></param>
        /// Key Name
        /// <param name="value"></param>
        /// Value Name
        public void IniWriteValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.Path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public string IniReadValue(string section, string key)
        {
            var temp = new StringBuilder(255);
            var i = GetPrivateProfileString(section, key, "", temp, 255, this.Path);
            return temp.ToString();

        }
    }
}
