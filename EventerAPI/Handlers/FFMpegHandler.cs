using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using EventerAPI.General;
using WMPLib;

namespace EventerAPI.Handlers
{
    public class FFMpegHandler
    {
        private const string template_headline_string = "wwwwwwwwwwwwwwwwwwwwwwwww";
        private const string template_body_string = "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww";

        ///        string video_cmd = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \" [1:v]fade=in:st=0:d=0.1,fade=out:st=5:d=0.1:alpha=1[Text1]; [Text1] scale=w=iw/1:h=ih/1 [Text1e]; [0:v][Text1e] overlay=x='if(lte(t,{3}.5),0.07*(t-{3}.5)*main_h-52,''if(gte(t,{4}),nan, 0)'')':y=main_h-{7}+{6}:shortest=1[video1]; [2:v]fade=in:st=4.8:d=0.01,fade=out:st=15:d=0.5[Text2]; [Text2] scale=w=iw/1:h=ih/1 [Text2e]; [video1][Text2e]overlay=x='if(lte(t,{4}),nan,''if(gte(t,{5}),-40*(t-{4}.5),0)'')':y=main_h-{7}+{6}:shortest=1 [video2]; [3:v] scale=w=iw/1:h=ih/1 [bannere]; [video2][bannere] overlay=x=0 :y=main_h-{7}:shortest=1\" -y {1}{2}.mp4";

        string video_cmd = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \" [1:v]fade=in:st=0:d=0.1,fade=out:st=6:d=0.1:alpha=1[Text1]; [Text1] scale=w=iw/1:h=ih/1 [Text1e]; [0:v][Text1e] overlay=x='if(lte(t,{3}.5),0.07*(t-{3}.5)*main_h-52,''if(gte(t,{4}),nan, 0)'')':y=main_h-{7}+{6}:shortest=1[video1]; [2:v]fade=in:st=4:d=0.01,fade=out:st=15:d=0.5[Text2]; [Text2] scale=w=iw/1:h=ih/1 [Text2e]; [video1][Text2e]overlay=x='if(lte(t,{4}),nan,''if(gte(t,{5}),-40*(t-{4}.5),0)'')':y=main_h-{7}+{6}:shortest=1 [video2]; [3:v] scale=w=iw/1:h=ih/1 [bannere]; [video2][bannere] overlay=x=0 :y=main_h-{7}:shortest=1\" -y {1}{2}.mp4 ";
        string ios_video_cmd = "-i {0}{2} -y {1}{2}.mp4 ";

        string video_cmd_without_name = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \" [1:v]fade=in:st=0:d=0.1,fade=out:st=500:d=0.1:alpha=1[Text1]; [Text1] scale=w=iw/1:h=ih/1[Text1e]; [0:v][Text1e] overlay=x='if(lte(t,{3}.5),0.07*(t-{3}.5)*main_h-52,''if(gte(t,{4}),nan, 0)'')':y=main_h-{7}+{6}:shortest=1[video1]; [2:v]fade=in:st=0:d=0.5,fade=out:st=515:d=0.5[Text2]; [Text2] scale=w=iw/1:h=ih/1[Text2e]; [video1][Text2e]overlay=x='if(lte(t,{4}),nan,''if(gte(t,{5}),-40*(t-{4}.5),0)'')':y=main_h-{7}+{6}:shortest=1 [video2]; [3:v]fade=in:st=0:d=0.1,fade=out:st=515:d=0.5[banner]; [banner] scale=w=iw/1:h=ih/1[bannere]; [video2][bannere] overlay=x='if(lt(t,{3}.5),0.07*(t-{3}.5)*main_h-52,''if(gte(t,{5}),-40*(t-{6}), 0)'')':y=main_h-{7}:shortest=1\" -y {1}{2}.mp4";

        string video_cmd30 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=30:d=0.1,fade=out:st=35:d=0.0:alpha=1[Text1];[Text1]scale=w=iw/1:h=ih/1[Text1e];[0:v][Text1e]overlay=x=0:y=main_h-{4}+{3}:shortest=1[video1];[2:v]fade=in:st=35:d=0.1,fade=out:st=40:d=0.5[Text2];[Text2]scale=w=iw/1:h=ih/1[Text2e];[video1][Text2e]overlay=x=0 :y=main_h-{4}+{3}:shortest=1 [video2];[3:v]fade=in:st=0:d=0.01,fade=out:st=540:d=0.5[banner];[banner]scale=w=iw/1:h=ih/1[bannere];[video2][bannere]overlay=x=0 :y=main_h-{4}:shortest=1\" -y {1}{2}.mp4";
        string video_cmd60 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=60:d=0.1,fade=out:st=65:d=0.0:alpha=1[Text1];[Text1]scale=w=iw/1:h=ih/1[Text1e];[0:v][Text1e]overlay=x=0:y=main_h-{4}+{3}:shortest=1[video1];[2:v]fade=in:st=65:d=0.1,fade=out:st=70:d=0.5[Text2];[Text2]scale=w=iw/1:h=ih/1[Text2e];[video1][Text2e]overlay=x=0 :y=main_h-{4}+{3}:shortest=1 [video2];[3:v]fade=in:st=0:d=0.01,fade=out:st=540:d=0.5[banner];[banner]scale=w=iw/1:h=ih/1[bannere];[video2][bannere]overlay=x=0 :y=main_h-{4}:shortest=1\" -y {1}{2}.mp4";
        string video_cmd90 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=90:d=0.1,fade=out:st=95:d=0.0:alpha=1[Text1];[Text1]scale=w=iw/1:h=ih/1[Text1e];[0:v][Text1e]overlay=x=0:y=main_h-{4}+{3}:shortest=1[video1];[2:v]fade=in:st=95:d=0.1,fade=out:st=100:d=0.5[Text2];[Text2]scale=w=iw/1:h=ih/1[Text2e];[video1][Text2e]overlay=x=0 :y=main_h-{4}+{3}:shortest=1 [video2];[3:v]fade=in:st=0:d=0.01,fade=out:st=540:d=0.5[banner];[banner]scale=w=iw/1:h=ih/1[bannere];[video2][bannere]overlay=x=0 :y=main_h-{4}:shortest=1\" -y {1}{2}.mp4";
        string video_cmd120 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=120:d=0.1,fade=out:st=125:d=0.0:alpha=1[Text1];[Text1]scale=w=iw/1:h=ih/1[Text1e];[0:v][Text1e]overlay=x=0:y=main_h-{4}+{3}:shortest=1[video1];[2:v]fade=in:st=125:d=0.1,fade=out:st=130:d=0.5[Text2];[Text2]scale=w=iw/1:h=ih/1[Text2e];[video1][Text2e]overlay=x=0 :y=main_h-{4}+{3}:shortest=1 [video2];[3:v]fade=in:st=0:d=0.01,fade=out:st=540:d=0.5[banner];[banner]scale=w=iw/1:h=ih/1[bannere];[video2][bannere]overlay=x=0 :y=main_h-{4}:shortest=1\" -y {1}{2}.mp4";
        string video_cmd150 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=150:d=0.1,fade=out:st=155:d=0.0:alpha=1[Text1];[Text1]scale=w=iw/1:h=ih/1[Text1e];[0:v][Text1e]overlay=x=0:y=main_h-{4}+{3}:shortest=1[video1];[2:v]fade=in:st=155:d=0.1,fade=out:st=160:d=0.5[Text2];[Text2]scale=w=iw/1:h=ih/1[Text2e];[video1][Text2e]overlay=x=0 :y=main_h-{4}+{3}:shortest=1 [video2];[3:v]fade=in:st=0:d=0.01,fade=out:st=540:d=0.5[banner];[banner]scale=w=iw/1:h=ih/1[bannere];[video2][bannere]overlay=x=0 :y=main_h-{4}:shortest=1\" -y {1}{2}.mp4";
        string video_cmd180 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=180:d=0.1,fade=out:st=185:d=0.0:alpha=1[Text1];[Text1]scale=w=iw/1:h=ih/1[Text1e];[0:v][Text1e]overlay=x=0:y=main_h-{4}+{3}:shortest=1[video1];[2:v]fade=in:st=185:d=0.1,fade=out:st=190:d=0.5[Text2];[Text2]scale=w=iw/1:h=ih/1[Text2e];[video1][Text2e]overlay=x=0 :y=main_h-{4}+{3}:shortest=1 [video2];[3:v]fade=in:st=0:d=0.01,fade=out:st=540:d=0.5[banner];[banner]scale=w=iw/1:h=ih/1[bannere];[video2][bannere]overlay=x=0 :y=main_h-{4}:shortest=1\" -y {1}{2}.mp4";
        string video_cmd210 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=210:d=0.1,fade=out:st=215:d=0.0:alpha=1[Text1];[Text1]scale=w=iw/1:h=ih/1[Text1e];[0:v][Text1e]overlay=x=0:y=main_h-{4}+{3}:shortest=1[video1];[2:v]fade=in:st=215:d=0.1,fade=out:st=220:d=0.5[Text2];[Text2]scale=w=iw/1:h=ih/1[Text2e];[video1][Text2e]overlay=x=0 :y=main_h-{4}+{3}:shortest=1 [video2];[3:v]fade=in:st=0:d=0.01,fade=out:st=540:d=0.5[banner];[banner]scale=w=iw/1:h=ih/1[bannere];[video2][bannere]overlay=x=0 :y=main_h-{4}:shortest=1\" -y {1}{2}.mp4";
        string video_cmd240 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=240:d=0.1,fade=out:st=245:d=0.0:alpha=1[Text1];[Text1]scale=w=iw/1:h=ih/1[Text1e];[0:v][Text1e]overlay=x=0:y=main_h-{4}+{3}:shortest=1[video1];[2:v]fade=in:st=245:d=0.1,fade=out:st=250:d=0.5[Text2];[Text2]scale=w=iw/1:h=ih/1[Text2e];[video1][Text2e]overlay=x=0 :y=main_h-{4}+{3}:shortest=1 [video2];[3:v]fade=in:st=0:d=0.01,fade=out:st=540:d=0.5[banner];[banner]scale=w=iw/1:h=ih/1[bannere];[video2][bannere]overlay=x=0 :y=main_h-{4}:shortest=1\" -y {1}{2}.mp4";
        string video_cmd270 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=270:d=0.1,fade=out:st=275:d=0.0:alpha=1[Text1];[Text1]scale=w=iw/1:h=ih/1[Text1e];[0:v][Text1e]overlay=x=0:y=main_h-{4}+{3}:shortest=1[video1];[2:v]fade=in:st=275:d=0.1,fade=out:st=280:d=0.5[Text2];[Text2]scale=w=iw/1:h=ih/1[Text2e];[video1][Text2e]overlay=x=0 :y=main_h-{4}+{3}:shortest=1 [video2];[3:v]fade=in:st=0:d=0.01,fade=out:st=540:d=0.5[banner];[banner]scale=w=iw/1:h=ih/1[bannere];[video2][bannere]overlay=x=0 :y=main_h-{4}:shortest=1\" -y {1}{2}.mp4";
        string video_cmd300 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=300:d=0.1,fade=out:st=305:d=0.0:alpha=1[Text1];[Text1]scale=w=iw/1:h=ih/1[Text1e];[0:v][Text1e]overlay=x=0:y=main_h-{4}+{3}:shortest=1[video1];[2:v]fade=in:st=305:d=0.1,fade=out:st=310:d=0.5[Text2];[Text2]scale=w=iw/1:h=ih/1[Text2e];[video1][Text2e]overlay=x=0 :y=main_h-{4}+{3}:shortest=1 [video2];[3:v]fade=in:st=0:d=0.01,fade=out:st=540:d=0.5[banner];[banner]scale=w=iw/1:h=ih/1[bannere];[video2][bannere]overlay=x=0 :y=main_h-{4}:shortest=1\" -y {1}{2}.mp4";
        string video_cmd330 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=330:d=0.1,fade=out:st=335:d=0.0:alpha=1[Text1];[Text1]scale=w=iw/1:h=ih/1[Text1e];[0:v][Text1e]overlay=x=0:y=main_h-{4}+{3}:shortest=1[video1];[2:v]fade=in:st=335:d=0.1,fade=out:st=340:d=0.5[Text2];[Text2]scale=w=iw/1:h=ih/1[Text2e];[video1][Text2e]overlay=x=0 :y=main_h-{4}+{3}:shortest=1 [video2];[3:v]fade=in:st=0:d=0.01,fade=out:st=540:d=0.5[banner];[banner]scale=w=iw/1:h=ih/1[bannere];[video2][bannere]overlay=x=0 :y=main_h-{4}:shortest=1\" -y {1}{2}.mp4";


        //0 - Path
        //1 - tmp_name
        //3 - start time
        //4 - start time + 5 sec
        //5 - start time + 15 sec
        //6 - start time + 1
        // __________________________
        //|       |  NAME            |     
        //|PROFILE|__________________|  
        //|  PIC  |  HEADLINE        | 
        //|_______|__________________|
        //|                          |
        //| TEXT 1                   |
        //| TEXT 2                   |
        //|__________________________|

        public void ConvertIOS(string tmp_name)
        {
            string path = "C:\\Temp\\";
            string out_path = "C:\\VidOut\\";

            Logger.Write("FFMpeg Conversion IOS Start: " + string.Format(ios_video_cmd, path, out_path, tmp_name));
            runningFFMPEG(string.Format(ios_video_cmd, path, out_path, tmp_name));

            File.Delete(path + tmp_name );
            Logger.Write("Deleted");

        }
        public void Convert(string tmp_name, string text_1, string text_2, string name, string headline, string photo_url, int duration, bool resize = false)
        {

            string path = "C:\\Temp\\";
            string out_path = "C:\\VidOut\\";
            string fileName_text = tmp_name + "_text.png";
            string fileName_text2 = tmp_name + "_text2.png";
            string fileName_head = tmp_name + "_head.png";
            string temp = "wwww wwww wwww wwww ww";
            string maxtemp;
            //string photoName = tmp_name + "_photo.png";
            Bitmap b_text1, b_text2;
            Logger.Write(name);

            //if (headline != null)
            //{
            //    if (name != null)
            //    {
            //        temp = headline.Length > name.Length ? headline : name;
            //    }
            //    else
            //    {
            //        temp = headline;
            //    }
            //}
            maxtemp = temp;
            if (text_1.Length > 40)
            {
                string p1, p2;
                int index = text_1.Substring(0, 40).LastIndexOf(' ');
                if (index > 35 && index < 40)
                {
                    index = 0;
                }
                if (index <= 0)
                {
                    p1 = text_1.Substring(0, 39);
                    p2 = text_1.Substring(39);
                }
                else
                {
                    if (text_1.Substring(index + 1).Length > 42)
                    {
                        p1 = text_1.Substring(0, 39);
                        p2 = text_1.Substring(39);
                    }
                    else
                    {
                        p1 = text_1.Substring(0, index);
                        p2 = text_1.Substring(index + 1);
                    }
                }
                Logger.Write("Text1: " + p1);
                Logger.Write("Text1: " + p2);
                string text = p1.Length > p2.Length ? p1 : p2;
                //temp = (temp.Length > p1.Length && temp.Length > p2.Length) ?
                //                  temp :
                //                    (p1.Length > p2.Length) ?
                //                    p1 : p2;
                //if (headline != null)
                //{
                //    temp = temp.Length > headline.Length ? temp : headline;
                //}
                b_text1 = string.IsNullOrWhiteSpace(text_1) ? new Bitmap(1, 1) :
                EditImage.MergeAbove(
                    EditImage.ConvertTextToBitmap(p1, Color.Black, Color.White, text + "wwwww", resize),
                     EditImage.ConvertTextToBitmap(p2, Color.Black, Color.White, text + "wwwww", resize));
            }
            else
            {
                b_text1 = string.IsNullOrWhiteSpace(text_1) ? new Bitmap(1, 1) : EditImage.ConvertTextToBitmap(text_1, Color.Black, Color.White, text_1 + "wwww", resize);
                Logger.Write("Text1: lss  " + text_1);
            }


            if (text_2.Length > 40)
            {
                string p1, p2;
                int index = text_2.Substring(0, 40).LastIndexOf(' ');
                if (index > 35 && index < 40)
                {
                    index = 0;
                }
                if (index <= 0)
                {
                    p1 = text_2.Substring(0, 39);
                    p2 = text_2.Substring(39);
                }
                else
                {
                    if (text_2.Substring(index + 1).Length > 42)
                    {
                        p1 = text_2.Substring(0, 39);
                        p2 = text_2.Substring(39);
                    }
                    else
                    {
                        p1 = text_2.Substring(0, index);
                        p2 = text_2.Substring(index + 1);
                    }
                }
                string text = p1.Length > p2.Length ? p1 : p2;
                Logger.Write("Text2: " + p1);
                Logger.Write("Text2: " + p2);

                //temp = (temp.Length > p1.Length && temp.Length > p2.Length) ?
                //    temp :
                //      (p1.Length > p2.Length) ?
                //      p1 : p2;

                //if (headline != null)
                //{
                //    temp = temp.Length > headline.Length ? temp : headline;
                //}

                b_text2 = string.IsNullOrWhiteSpace(text_2) ? new Bitmap(1, 1) :
                EditImage.MergeAbove(
                    EditImage.ConvertTextToBitmap(p1, Color.Black, Color.White, text + "wwwww", resize),
                     EditImage.ConvertTextToBitmap(p2, Color.Black, Color.White, text + "wwwww", resize));
            }
            else
            {
                b_text2 = string.IsNullOrWhiteSpace(text_2) ? b_text1 : EditImage.ConvertTextToBitmap(text_2, Color.Black, Color.White, text_2, resize);
                Logger.Write("Text2 ls: " + text_2);
            }

            Logger.Write("headline:  " + headline);
            //  Bitmap bt = EditImage.ConvertTextToBitmap(template_headline_string, Color.Gray, Color.White, template_headline_string);

            //temp = maxtemp;
            Bitmap b_headline = string.IsNullOrWhiteSpace(headline) ?
                 new Bitmap(1, 1)
                : EditImage.ConvertTextToBitmap(headline, Color.Gray, Color.White, temp, false);

            Logger.Write("headline is  " + headline);

            if (!string.IsNullOrWhiteSpace(name))
            {
                Bitmap b_name = EditImage.ConvertTextToBitmap(name.PadRight(50), Color.White, Color.Black, temp, false, string.IsNullOrEmpty(headline));
                b_headline = EditImage.MergeAbove(b_name, b_headline);

                //EditImage.ResizeImage(photoName, path, b_addAndName.Height + 1);
                if (!string.IsNullOrWhiteSpace(photo_url))
                {
                    Bitmap photo = EditImage.URLToBitmap(photo_url, string.IsNullOrWhiteSpace(headline) ? b_name.Height : b_headline.Height);
                    b_headline = EditImage.MergeBeside(photo, b_headline, resize);
                }
            }
            string cmd = string.Empty;

            b_headline.Save(path + fileName_head, ImageFormat.Png);
            b_text1.Save(path + fileName_text, ImageFormat.Png);
            b_text2.Save(path + fileName_text2, ImageFormat.Png);
            int offset = (int)(b_headline.Height * 1.2 + b_text1.Height);
            if (!resize)
            {
                if (text_1.Length < 40)
                    offset = (int)(b_headline.Height * 1.2 + b_text1.Height * 2.5);
                else
                    offset = (int)(b_headline.Height * 1.2 + b_text1.Height * 1.5);
            }
            if (string.IsNullOrEmpty(name))
            {
                //  offset = (int)(offset * 2);
                runningFFMPEG(string.Format(video_cmd_without_name, path, out_path, tmp_name, 0, 5, 15, b_headline.Height, offset));
                cmd = video_cmd_without_name;
            }
            else
            {
                if (string.IsNullOrEmpty(text_1))
                {
                    offset = (int)(offset * 1.5);
                }
                Logger.Write("FFMpeg Conversion 1 Start: " + string.Format(video_cmd, path, out_path, tmp_name, 0, 5, 15, b_headline.Height, offset));
                runningFFMPEG(string.Format(video_cmd, path, out_path, tmp_name, 0, 5, 15, b_headline.Height, offset));
                cmd = video_cmd;
            }

            Logger.Write("Duration: " + duration);
            int t = 30;
            duration -= 30;
            Logger.Write("Duration: " + duration);
            int i = 0;
            while (duration > 0)
            {
                File.Copy(out_path + tmp_name + ".mp4", path + tmp_name + ".mp4", true);
                switch (i)
                {
                    case 0:
                        Logger.Write("FFMpeg Conversion 2 Start: " + string.Format(video_cmd30, path, out_path, tmp_name, b_headline.Height, offset));
                        runningFFMPEG(string.Format(video_cmd30, path, out_path, tmp_name, b_headline.Height, offset));
                        i++;
                        break;
                    case 1:
                        Logger.Write("FFMpeg Conversion 3 Start: " + string.Format(video_cmd60, path, out_path, tmp_name, b_headline.Height, offset));
                        runningFFMPEG(string.Format(video_cmd60, path, out_path, tmp_name, b_headline.Height, offset));
                        i++;
                        break;
                    case 2:
                        Logger.Write("FFMpeg Conversion 4 Start: " + string.Format(video_cmd90, path, out_path, tmp_name, b_headline.Height, offset));
                        runningFFMPEG(string.Format(video_cmd90, path, out_path, tmp_name, b_headline.Height, offset));
                        i++;
                        break;
                    case 3:
                        Logger.Write("FFMpeg Conversion 5 Start: " + string.Format(video_cmd120, path, out_path, tmp_name, b_headline.Height, offset));
                        runningFFMPEG(string.Format(video_cmd120, path, out_path, tmp_name, b_headline.Height, offset));
                        i++;
                        break;
                    case 4:
                        Logger.Write("FFMpeg Conversion 5 Start: " + string.Format(video_cmd150, path, out_path, tmp_name, b_headline.Height, offset));
                        runningFFMPEG(string.Format(video_cmd150, path, out_path, tmp_name, b_headline.Height, offset));
                        i++;
                        break;
                    case 5:
                        Logger.Write("FFMpeg Conversion 5 Start: " + string.Format(video_cmd180, path, out_path, tmp_name, b_headline.Height, offset));
                        runningFFMPEG(string.Format(video_cmd180, path, out_path, tmp_name, b_headline.Height, offset));
                        i++;
                        break;
                    case 6:
                        Logger.Write("FFMpeg Conversion 5 Start: " + string.Format(video_cmd210, path, out_path, tmp_name, b_headline.Height, offset));
                        runningFFMPEG(string.Format(video_cmd210, path, out_path, tmp_name, b_headline.Height, offset));
                        i++;
                        break;
                    case 7:
                        Logger.Write("FFMpeg Conversion 5 Start: " + string.Format(video_cmd240, path, out_path, tmp_name, b_headline.Height, offset));
                        runningFFMPEG(string.Format(video_cmd240, path, out_path, tmp_name, b_headline.Height, offset));
                        i++;
                        break;
                    case 8:
                        Logger.Write("FFMpeg Conversion 5 Start: " + string.Format(video_cmd270, path, out_path, tmp_name, b_headline.Height, offset));
                        runningFFMPEG(string.Format(video_cmd270, path, out_path, tmp_name, b_headline.Height, offset));
                        i++;
                        break;
                    case 9:
                        Logger.Write("FFMpeg Conversion 5 Start: " + string.Format(video_cmd300, path, out_path, tmp_name, b_headline.Height, offset));
                        runningFFMPEG(string.Format(video_cmd300, path, out_path, tmp_name, b_headline.Height, offset));
                        i++;
                        break;
                    case 10:
                        Logger.Write("FFMpeg Conversion 5 Start: " + string.Format(video_cmd330, path, out_path, tmp_name, b_headline.Height, offset));
                        runningFFMPEG(string.Format(video_cmd330, path, out_path, tmp_name, b_headline.Height, offset));
                        i++;
                        break;
                    default:
                        duration = -10;
                        break;

                }
                duration -= 30;
            }

            /*
            for (var i = 0; i < duration; i = i + 30) {
                Logger.Write("FFMpeg Conversion Start: " + string.Format(video_cmd, path, out_path, tmp_name, i, i + 5, i + 15, i + 1));

                if (i > 0) {

                }

                runningFFMPEG(string.Format(video_cmd, path, out_path, tmp_name, i, i+5, i+15, i+1));
            }
             * */


            File.Delete(path + tmp_name + ".mp4");
            File.Delete(path + fileName_text);
            File.Delete(path + fileName_text2);
            File.Delete(path + fileName_head);
        }
        private void runningFFMPEG(string cmd)
        {
            ExecuteAsync(cmd);
        }
        static Process process = null;
        static void ExecuteAsync(string cmd)
        {

            try
            {
                process = new Process();
                ProcessStartInfo info = new ProcessStartInfo();

                Logger.Write("BEGIN: ");
                info.WorkingDirectory = @"C:\ffmpeg\bin\";
                info.FileName = "ffmpeg.exe";
                info.Arguments = cmd;

                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.RedirectStandardError = true;
                info.RedirectStandardOutput = true;

                process.StartInfo = info;

                process.EnableRaisingEvents = true;
                process.ErrorDataReceived +=
                    new DataReceivedEventHandler(process_ErrorDataReceived);
                process.OutputDataReceived +=
                    new DataReceivedEventHandler(process_OutputDataReceived);
                process.Exited += new EventHandler(process_Exited);

                Logger.Write(cmd);
                Logger.Write("FFMpeg Conversion Process Start: ");
                process.Start();
                //

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
            }
            catch (Exception e)
            {
                Logger.Write("Failed FFMPEG:");
                Logger.Write(e.Message);
                if (process != null) process.Dispose();
            }
        }

        static int lineCount = 0;
        static void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Logger.Write(string.Format("Input line: ({0:m:s:fff}) {1}", DateTime.Now, e.Data));
        }

        static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Logger.Write("Output Data Received.");
        }

        static void process_Exited(object sender, EventArgs e)
        {
            process.Dispose();
            Logger.Write("Bye bye!");
        }

    }

    public class EditImage
    {
        public static void ResizeImage(string i_name, string i_path, int size)
        {
            Bitmap original = (Bitmap)Image.FromFile(i_path + i_name);
            Bitmap resized = new Bitmap(original, new Size(size, size));
            resized.Save(i_path + "_resize.png");
        }

        public static Bitmap MergeAbove(Bitmap image1, Bitmap image2)
        {
            Bitmap bitmap = new Bitmap(Math.Max(image1.Width, image2.Width), image1.Height + image2.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(image1, 0, 0);
                g.DrawImage(image2, 0, image1.Height);
            }
            return bitmap;
        }
        public static Bitmap MergeBeside(Bitmap image1, Bitmap image2, bool resize_)
        {
            Bitmap bitmap = new Bitmap(image1.Width + image2.Width, Math.Max(image1.Height, image2.Height));
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(image1, 0, 0);
                // g.DrawImage(image2, image1.Width - image1.Width / 5, 0);
                g.DrawImage(image2, image1.Width, 0);
            }

            if (resize_)
            {
                Bitmap resized = new Bitmap(bitmap, new Size((int)(bitmap.Width * 0.5), (int)(bitmap.Height * 0.5)));
                return resized;
            }

            return bitmap;
        }
        public static Bitmap ConvertTextToBitmap(string i_text, Color i_backColor, Color i_textColor, string Template, bool resize_, bool doubleSize = false)
        {
            Bitmap objBmpImage = new Bitmap(1, 1);
            int intWidth = 0;
            int intHeight = 0;

            // Create the Font object for the image text drawing.
            Font objFont = new Font("Roboto", 34, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);

            // Create a graphics object to measure the text's width and height.
            Graphics objGraphics = Graphics.FromImage(objBmpImage);

            // This is where the bitmap size is determined.
            intWidth = (int)objGraphics.MeasureString(Template == null ? i_text : Template, objFont, new PointF(0, 0), new StringFormat(StringFormatFlags.MeasureTrailingSpaces)).Width + 12;
            intHeight = (int)objGraphics.MeasureString(Template == null ? i_text : Template, objFont).Height + 3;

            if (doubleSize)
                intHeight *= 2;

            // Create the bmpImage again with the correct size for the text and font.
            objBmpImage = new Bitmap(objBmpImage, new Size(intWidth, intHeight));

            // Add the colors to the new bitmap.
            objGraphics = Graphics.FromImage(objBmpImage);

            // Set Background color
            objGraphics.Clear(i_backColor);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            if (doubleSize)
            {
                Logger.Write(string.Format("intHeight: {0} intWidth: {1}", intHeight, intWidth));
                Logger.Write(string.Format("x: {0} y: {1}", intHeight / 4, intHeight / 4));
                objGraphics.DrawString(i_text, objFont, new SolidBrush(i_textColor), 4, intHeight / 4);
            }
            else
                objGraphics.DrawString(i_text, objFont, new SolidBrush(i_textColor), 1, 0);
            objGraphics.Flush();
            Logger.Write("Done drawing");

            if (resize_)
            {
                Bitmap resized = new Bitmap(objBmpImage, new Size((int)(objBmpImage.Width * 0.5), (int)(objBmpImage.Height * 0.5)));
                return resized;
            }

            return objBmpImage;
        }
        public static Bitmap URLToBitmap(string url, int size)
        {
            HttpWebRequest wreq;
            HttpWebResponse wresp;
            Stream mystream;
            Bitmap bmp;

            bmp = null;
            mystream = null;
            wresp = null;
            try
            {
                wreq = (HttpWebRequest)WebRequest.Create(url);
                wreq.AllowWriteStreamBuffering = true;

                wresp = (HttpWebResponse)wreq.GetResponse();

                if ((mystream = wresp.GetResponseStream()) != null)
                {
                    bmp = new Bitmap(mystream);
                    bmp = new Bitmap(bmp, size, size);
                }
            }
            finally
            {
                if (mystream != null)
                    mystream.Close();

                if (wresp != null)
                    wresp.Close();
            }
            return (bmp);

        }
    }
}
