#region License
//
//****************************************************************************
// *
// * Copyright (c) 1999-2010 Willey Network Technology, Inc. All Rights Reserved.
// *
// * This software is the confidential and proprietary information of Willey
// * Network Technology, Inc. ("Confidential Information").  You shall not
// * disclose such Confidential Information and shall use it only in
// * accordance with the terms of the license agreement you entered into
// * with Willey.
// *
// * WILLEY MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY OF THE
// * SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// * IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// * PURPOSE, OR NON-INFRINGEMENT. CRELAB SHALL NOT BE LIABLE FOR ANY DAMAGES
// * SUFFERED BY LICENSEE AS A RESULT OF USING, MODIFYING OR DISTRIBUTING
// * THIS SOFTWARE OR ITS DERIVATIVES.
// *
// * Original Author: Henry
// * Last checked in by $Author$
// * $Id$
// */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

using B1POS.Models;

namespace B1POS.Util
{
    public class CashDrawer
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;

            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;

            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true,
        CharSet = CharSet.Ansi, ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] 
        string szPrinter, out IntPtr hPrinter, Int32 pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);
        
        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true,
        CharSet = CharSet.Ansi, ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level,
        [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32
        dwCount, out Int32 dwWritten);

        /*ESC p M n1 n2
        M =0 代表一个钱箱 n代表脉冲宽度 n1 =40--50 之间
        M =1 代表两个钱箱 n2 =120--150之间*/
        static byte[] commandOpenDrawer = new byte[] { 0x1B, 0x70, 0x0, 0x32, 0xC8};

        public static void Open()
        {
            Thread thread = new Thread(new ThreadStart(OpenDrawer));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private static void OpenDrawer()
        {
            Settings settings = new Settings();
            settings.getValuesFromRegistry();

            IntPtr hPrinter = new IntPtr(0);
            if (OpenPrinter(settings.PrinterName, out hPrinter, 0))
            {
                Int32 dwWritten = 0;

                DOCINFOA docinfo = new DOCINFOA();
                docinfo.pDocName = "CashPop";

                if (StartDocPrinter(hPrinter, 1, docinfo))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        IntPtr pBytesCommandOpenDrawer = Marshal.UnsafeAddrOfPinnedArrayElement(commandOpenDrawer, 0);
                        WritePrinter(hPrinter, pBytesCommandOpenDrawer, commandOpenDrawer.Length, out dwWritten);
                        Marshal.FreeCoTaskMem(pBytesCommandOpenDrawer);

                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
        }
    }
}