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
using System.IO.Ports;

namespace B1POS.Util
{
    public enum PoleDisplayType : uint
    {
        Dark = 0,
        Price = 1,
        Total = 2,
        Payment = 3,
        Change = 4
    }

    public class PoleDisplayer
    {
        private static string text;
        private static PoleDisplayType type;
        private static SerialPort serialPort = null;

        private const string PORT_NAME = "COM1";
        private const int BAUD_RATE = 2400;

        //static byte[] commandClear = new byte[] { 0x0C };
        //static byte[] commandInitialize = new byte[] { 0x1B, 0x40 };
        //static byte[] commandLight = new byte[] { 0x1B, 0x73 };

        public static void Display(string _text, PoleDisplayType _type)
        {
            text = _text;
            type = _type;

            Thread thread = new Thread(new ThreadStart(Display));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private static void Display()
        {
            try
            {
                if (serialPort == null)
                {
                    serialPort = new SerialPort(PORT_NAME, BAUD_RATE, Parity.None, 8);
                    serialPort.Open();
                }

                if (serialPort.IsOpen)
                {
                    //serialPort.Write(commandInitialize, 0, commandInitialize.Length);
                    //serialPort.Write(commandLight, 0, commandLight.Length);
                    //serialPort.Write(Convert.ToInt32(type).ToString());
                    //serialPort.WriteLine(FillStr(text));

                    char esc = (char)27;

                    serialPort.Write(esc + @"@");
                    serialPort.Write(esc + @"s" + Convert.ToInt32(type).ToString());
                    //serialPort.WriteLine(ASCIIString("ESC  Q  A  " + FillString(text) + "  CR"));
                    serialPort.WriteLine(FillString(text));
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 填充text,不含小数点的8位
        /// </summary>
        /// <param name="text"></param>
        private static string FillString(string text)
        {
            string temp = text.Replace(".", string.Empty);

            for (int i = 0; i < 8 - temp.Length; i++)
            {
                text += "0";
            }

            return text;
        }
    }
}