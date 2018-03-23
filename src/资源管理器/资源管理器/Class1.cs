using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ReportDemo
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
        public const uint SHGFI_SMALLICON = 0x1;    // 'Small icon
        public const uint SHGFI_TYPENAME = 0x400;

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbSizeFileInfo,
            uint uFlags);
    }


    public class MyFile
    {



        public static System.Drawing.Icon GetFileIcon(string name, bool LageIcon)
        {
            IntPtr hImgSmall;    //the handle to the system image list
            IntPtr hImgLarge;    //the handle to the system image list

            SHFILEINFO shinfo = new SHFILEINFO();


            //Use this to get the small Icon
            if (!LageIcon)
                hImgSmall = Win32.SHGetFileInfo(name, 0, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo),
                    Win32.SHGFI_ICON |
                    Win32.SHGFI_SMALLICON);
            else
                //Use this to get the large Icon
                hImgLarge = Win32.SHGetFileInfo(name, 0,
                    ref shinfo, (uint)Marshal.SizeOf(shinfo),
                    Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);

            //The icon is returned in the hIcon member of the shinfo
            //struct
            System.Drawing.Icon myIcon =
                System.Drawing.Icon.FromHandle(shinfo.hIcon);

            return myIcon;
        }

        public static string GetFileType(string name)
        {
            string s;
            SHFILEINFO shinfo = new SHFILEINFO();


            //Use this to get the small Icon

            Win32.SHGetFileInfo(name, 0, ref shinfo,
                (uint)Marshal.SizeOf(shinfo),
                Win32.SHGFI_TYPENAME);
            s = (shinfo.szTypeName);
            return s;
        }

    }

   
}
