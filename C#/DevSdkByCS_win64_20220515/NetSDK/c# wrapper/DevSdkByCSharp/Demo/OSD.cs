using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Demo
{
    class OSD
    {

        static int lineDistance = 5;//行间距
        static Graphics gcs;
        static int iHeight = 0;
        static string[] nrLine;
        static string[] nrLinePos;
        static int searchPos = 0;
        static int section = 1;
        static int sectionHeight = 10;
        static DispMode dm = DispMode.None;
        static int iPanelNotPagerHeight = 0;
        static bool displayed = false;
        /// <summary>
        /// 分析要显示文本的内容，将文本进行分段，分行，并测算好行距，段距等
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="ft"></param>
        /// <param name="iWidth"></param>
        /// <param name="value"></param>
        public static void GetTextInfo(Panel pl, Font ft, int iWidth, string value)
        {
            try
            {
                iHeight = 0;
                if (value != "")
                {
                    if (gcs == null)
                    {
                        gcs = pl.CreateGraphics();
                        SizeF sf0 = gcs.MeasureString(new string('测', 20), ft);
                        searchPos = (int)(iWidth * 20 / sf0.Width);
                        if (searchPos > 2) searchPos -= 2;
                    }

                    nrLine = value.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);//记下每一段文本的信息
                    section = nrLine.Length;
                    nrLinePos = new string[section];//存放每行分割的Index数字
                    SizeF sf1, sf2;
                    string temps, tempt;
                    string drawstring;
                    int temPos;//临时Index
                    int ipos;//文字Index
                    //将每一段文字的分成句子，并记下每句的起始Idex
                    for (int i = 0; i < section; i++)
                    {
                        ipos = 0;
                        temPos = searchPos;
                        if (searchPos >= nrLine[i].Length)
                        {
                            ipos += nrLine[i].Length;
                            nrLinePos[i] += "," + ipos.ToString();
                            iHeight++;
                            continue;
                        }
                        drawstring = nrLine[i];
                        nrLinePos[i] = "";
                        while (drawstring.Length > searchPos)
                        {
                            bool isfind = false;
                            for (int j = searchPos; j < drawstring.Length; j++)
                            {
                                temps = drawstring.Substring(0, j);
                                tempt = drawstring.Substring(0, j + 1);
                                sf1 = gcs.MeasureString(temps, ft);
                                sf2 = gcs.MeasureString(tempt, ft);
                                if (sf1.Width < iWidth && sf2.Width > iWidth)
                                {
                                    iHeight++;
                                    ipos += j;
                                    nrLinePos[i] += "," + ipos.ToString();
                                    isfind = true;
                                    drawstring = drawstring.Substring(j);
                                    break;
                                }
                            }
                            if (!isfind)
                            {
                                break;
                            }
                        }
                        ipos += drawstring.Length;
                        nrLinePos[i] += "," + ipos.ToString();
                        iHeight++;

                    }
                }
                if (dm == DispMode.None)
                {
                    if (value == "")
                    {
                        iPanelNotPagerHeight = 0;
                        return;
                    }
                    else
                    {
                        iPanelNotPagerHeight = iHeight * (ft.Height + lineDistance) + (section - 1) * (sectionHeight - lineDistance);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show( e.Message);
                return;
            }
        }
        /// <summary>
        /// 根据GetTextInfo方法中测算好的信息来绘制文本，将文本显示到Panel上
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="solidbrushColor"></param>
        /// <param name="iWidth"></param>
        public static void PaintTextOnPanel(Panel pl, string text, Font font, Color solidbrushColor, int iWidth)
        {



           
                if (pl == null)
                    return;
                Graphics g = pl.CreateGraphics();
                g.DrawString(text, font, new SolidBrush(Color.Red), new Point(50, 50));


             
               
            
            //String drawString = text;
            //Font drawFont = font;
            //SolidBrush drawBrush = new SolidBrush(solidbrushColor);
            //SizeF textSize = g.MeasureString(text, font);//文本的矩形区域大小   
            //int lineCount = Convert.ToInt16(textSize.Width / iWidth) + 1;//计算行数   
            //int fHeight = font.Height;
            //int htHeight = 0;
            //bool isPageStart = false;
            //float x = 0.0F;
            //StringFormat drawFormat = new StringFormat();
            //lineCount = drawString.Length;//行数不超过总字符数目   
            //int i, idx, first;
            //string subStr, tmpStr = "", midStr = "";
            //string[] idxs;
            //int tmpPage = 1;
            //string preLineStr = "";
            //for (i = 0; i < section; i++)
            //{
            //    if (i == 10)
            //    {
            //        first = 0;
            //    }
            //    first = 0;
            //    subStr = nrLine[i];
            //    if (nrLinePos[i] != null) tmpStr = nrLinePos[i].TrimStart(',');
            //    midStr = subStr.Substring(first);
            //    if (tmpStr != "")
            //    {
            //        idxs = tmpStr.Split(',');
            //        for (int j = 0; j < idxs.Length; j++)
            //        {
            //            idx = int.Parse(idxs[j]);//每句的结束Index                       
            //            midStr = subStr.Substring(first, idx - first);//通过上句的结束Index和本句的结束Index计算本句的内容
            //            if (dm == DispMode.None)
            //            {

            //                g.DrawString(midStr, drawFont, drawBrush, x, Convert.ToInt16(htHeight), drawFormat);
            //            }

            //            if (j == idxs.Length - 1)
            //            {
            //                htHeight += (fHeight + sectionHeight);
            //            }
            //            else
            //            {
            //                htHeight += (fHeight + lineDistance);
            //            }
            //            first = idx;//记下本句结束的Index
            //        }
            //    }
            //}
        }





        /// <summary>
        /// 显示模式，分为全显示和分页显示两种
        /// </summary>
        public enum DispMode
        {
            None = 0,
            Page = 1

        }

     
    }
}
