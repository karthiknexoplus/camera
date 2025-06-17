using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Web.UI;

namespace Demo
{
    class HtmlGeneratorHelp
    {
        //public static void CreateMain(string path)
        //{
        //    StringWriter swriter = new StringWriter();
        //    HtmlTextWriter writer = new HtmlTextWriter(swriter);
        //    writer.RenderBeginTag(HtmlTextWriterTag.Html);
        //    writer.AddAttribute("cols", "180, *");
        //    writer.AddAttribute("border", "0");
        //    writer.RenderBeginTag(HtmlTextWriterTag.Frameset);
        //    writer.AddAttribute("src", "index.html");
        //    writer.RenderBeginTag(HtmlTextWriterTag.Frame);

        //    writer.AddAttribute("src", "config.html");
        //    writer.AddAttribute("name", "config");
        //    writer.RenderBeginTag(HtmlTextWriterTag.Frame);

        //    writer.RenderEndTag();
        //    writer.RenderEndTag();
        //    writer.RenderEndTag();
        //    writer.RenderEndTag();

        //    FileStream fs = new FileStream(path, FileMode.Create);
        //    byte[] data = Encoding.Default.GetBytes(swriter.ToString());
        //    fs.Write(data, 0, data.Length);
        //    fs.Flush();
        //    fs.Close();
        //}
        //public static void CreateIndex(string path,string name)
        //{
        //    StringWriter swriter = new StringWriter();
        //    HtmlTextWriter writer = new HtmlTextWriter(swriter);
        //    writer.RenderBeginTag(HtmlTextWriterTag.Html);
        //    writer.RenderBeginTag(HtmlTextWriterTag.Head);
        //    writer.RenderEndTag();
        //    writer.RenderBeginTag(HtmlTextWriterTag.Body);
        //    writer.AddAttribute("type", "text/css");
        //    writer.RenderBeginTag(HtmlTextWriterTag.Style);
        //    writer.Write("body {font: normal 15px auto &apos;Trebuchet MS&apos;, Verdana, Arial, Helvetica, sans-serif;color: #4f6b72;background: #E6EAE9;}a {color: #c75f3e;}");
        //    writer.RenderEndTag();//style
        //    writer.RenderBeginTag(HtmlTextWriterTag.Ul);
        //    writer.RenderBeginTag(HtmlTextWriterTag.Li);
        //    writer.AddAttribute("target", "config");
        //    writer.AddAttribute("href", "config.html#anchor_1");
        //    writer.RenderBeginTag(HtmlTextWriterTag.A);
        //    writer.Write(name);
        //    writer.RenderEndTag();//a
        //    writer.RenderEndTag();//li

        //    writer.RenderEndTag();//ul
        //    writer.RenderEndTag();//body
        //    writer.RenderEndTag();//html

        //    FileStream fs = new FileStream(path, FileMode.Create);
        //    byte[] data = Encoding.Default.GetBytes(swriter.ToString());
        //    fs.Write(data, 0, data.Length);
        //    fs.Flush();
        //    fs.Close();
        //}
        //public static string CreateConfigHeader(string name)
        //{
        //    StringWriter swriter = new StringWriter();
        //    HtmlTextWriter writer = new HtmlTextWriter(swriter);
        //    writer.RenderBeginTag(HtmlTextWriterTag.Html);
        //    writer.RenderBeginTag(HtmlTextWriterTag.Head);
        //    writer.RenderEndTag();
        //    writer.RenderBeginTag(HtmlTextWriterTag.Body);
        //    writer.AddAttribute("type", "text/css");
        //    writer.RenderBeginTag(HtmlTextWriterTag.Style);
        //    writer.Write(@"body {font: normal 11px auto &apos;Trebuchet MS&apos;, Verdana, Arial, Helvetica, sans-serif;color: #4f6b72;
        //    background: #E6EAE9;}a {color: #c75f3e;}table {width: 800px;padding: 0;margin: 0;}caption {padding: 0 0 5px 0;width: 700px;
        //    font: italic 11px &apos;Trebuchet MS&apos;, Verdana, Arial, Helvetica, sans-serif;text-align: right;}th {font: bold 11px &apos;
        //    Trebuchet MS&apos;, Verdana, Arial, Helvetica, sans-serif;color: #4f6b72;border-right: 1px solid #C1DAD7;border-bottom: 1px solid #C1DAD7;
        //    border-top: 1px solid #C1DAD7;letter-spacing: 2px;text-transform: uppercase;text-align: left;padding: 6px 6px 6px 12px;
        //    background: #CAE8EA no-repeat;}th.nobg {border-top: 0;border-left: 0;border-right: 1px solid #C1DAD7;
        //    background: none;}td {border-right: 1px solid #C1DAD7;border-bottom: 1px solid #C1DAD7;background: #fff;font-size:11px;
        //    padding: 6px 6px 6px 12px;color: #4f6b72;}td.alt {background: #F5FAFA;color: #797268;}th.spec {border-left: 1px solid #C1DAD7;
        //    border-top: 0;background: #fff no-repeat;font: bold 10px &apos;Trebuchet MS&apos;, Verdana, Arial, Helvetica, sans-serif;}th.specalt {border-left: 1px solid #C1DAD7;
        //    border-top: 0;background: #f5fafa no-repeat;font: bold 10px &apos;Trebuchet MS&apos;, Verdana, Arial, Helvetica, sans-serif;
        //    color: #797268;}html&gt;body td{ font-size:11px;}body,td,th {font-family: 宋体, Arial;font-size: 12px;}");
        //    writer.RenderEndTag();//style

        //    writer.AddAttribute("name", "anchor_1");
        //    writer.RenderBeginTag(HtmlTextWriterTag.A);
        //    writer.Write(name);
        //    writer.RenderEndTag();//a

        //    writer.RenderBeginTag(HtmlTextWriterTag.Br);
        //    writer.AddAttribute("border", "1");
        //    writer.AddAttribute("name", "config_1");
        //    writer.AddAttribute("class", "table");
        //    writer.AddAttribute("cellspacing", "0");
        //    writer.AddAttribute("type", "0");
        //    writer.RenderBeginTag(HtmlTextWriterTag.Table);
        //    return swriter.ToString();


        //}
        //public static string CreateConfigEnder()
        //{
        //    return @"</table><br /></body></html>";           
        //}
        //public static string CreateNode(HtmlTextWriterTag tag, string attName, string attValue, string value)
        //{
        //    StringWriter swriter = new StringWriter();
        //    HtmlTextWriter writer = new HtmlTextWriter(swriter);
        //    if(attName.Length > 0 && attValue.Length > 0)
        //        writer.AddAttribute(attName, attValue);
        //    writer.RenderBeginTag(tag);
        //    if(value.Length > 0)
        //        writer.Write(value);
        //    writer.RenderEndTag();//th

        //    return swriter.ToString();
        //}
        //public static string CreateTr(object[] ths)
        //{
        //    StringWriter swriter = new StringWriter();
        //    HtmlTextWriter writer = new HtmlTextWriter(swriter);
        //    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        //    foreach (object a in ths)
        //    {
        //        writer.Write(a.ToString());
        //    }
        //    writer.RenderEndTag();
        //    return swriter.ToString();
        //}
        //public static string CreateTr(object ths)
        //{
        //    StringWriter swriter = new StringWriter();
        //    HtmlTextWriter writer = new HtmlTextWriter(swriter);
        //    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        //    writer.Write(ths.ToString());
        //    writer.RenderEndTag();
        //    return swriter.ToString();
        //}
        //public static void CreateConfig(string path, string name, string value, string header)
        //{
        //    string aa = CreateConfigHeader(name);
        //    StringBuilder tr = new StringBuilder();
            
        //    tr.Append(CreateNode(HtmlTextWriterTag.Th, "style", "width:50px", ""));
        //    string[] temp = header.Split('|');
        //    foreach (string s in temp)
        //    {
        //        tr.Append(CreateNode(HtmlTextWriterTag.Th, "", "", s));
        //    }           
        //    string bb = CreateTr(tr.ToString());

        //    StringBuilder td = new StringBuilder();
        //    td.Append(CreateNode(HtmlTextWriterTag.Td, "", "", ""));
        //    string[] temp1 = value.Split('|');
        //    foreach (string s in temp1)
        //    {
        //        td.Append(CreateNode(HtmlTextWriterTag.Td, "", "", s));
        //    } 
        //    string cc = CreateTr(td.ToString());
        //    string dd = CreateConfigEnder();

        //    FileStream fs = new FileStream(path, FileMode.Create);
        //    byte[] data = Encoding.Default.GetBytes(aa + bb + cc + dd);
        //    fs.Write(data, 0, data.Length);
        //    fs.Flush();
        //    fs.Close();
        //}
    }
}
