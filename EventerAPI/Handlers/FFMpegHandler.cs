﻿using System;
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

        //string cmd = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=0:d=0.5,fade=out:st=15:d=0.5:alpha=1[watermark1];[0:v][watermark1]overlay=x='if(lte(t,0.5),0.14*(t-0.5)*main_h+overlay_h-10,''if(gte(t,5),nan, 0)'')':y=main_h-60:shortest=1[video1];[2:v] fade=in:st=5:d=0.5,fade=out:st=15:d=0.5[watermark2];[video1][watermark2] overlay=x='if(lte(t,5),nan,''if(gte(t,7),-40*(t-7),0)'')':y=main_h-60:shortest=1 [video2];[3:v] fade=in:st=0:d=0.5,fade=out:st=14:d=0.5[watermark3];[video2][watermark3]overlay=x='if(lte(t,5),0.09*(t-5)*main_h+overlay_h-10,''if(gte(t,7),-40*(t-7), 0)'')':y=main_h-164:shortest=1\"  -y {1}{2}.mp4";
        //string video_cmd = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=0:d=0.2,fade=out:st=5:d=0.2:alpha=1[watermark1];[0:v][watermark1]overlay=x='if(lte(t,{3}.5),0.7*(t-{3}.5)*main_h-10,''if(gte(t,{4}),nan, 0)'')':y=main_h-60:shortest=1[video1];[2:v]fade=in:st=5:d=0.5,fade=out:st=15:d=0.5[watermark2];[video1][watermark2]overlay=x='if(lte(t,{4}),nan,''if(gte(t,{5}),-40*(t-{4}.5),0)'')':y=main_h-60:shortest=1 [video2];[3:v]fade=in:st=0:d=0.5,fade=out:st=15:d=0.5[watermark3];[video2][watermark3] overlay=x='if(lt(t,{3}.5),0.07*(t-{3}.5)*main_h-10,''if(gte(t,{5}),-40*(t-{6}), 0)'')':y=main_h-164:shortest=1\"  -y {1}{2}.mp4";
        //string video_cmd = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=0:d=0.2,fade=out:st=5:d=0.2:alpha=1[Text1]; [Text1] scale=w=iw/2:h=ih/2 [Text1e]; [0:v][Text1e] overlay=x='if(lte(t,{3}.09),0.7*(t-{3}.09),''if(gte(t,{4}),nan, 0)'')':y=main_h-112:shortest=1[video1]; [2:v]fade=in:st=5:d=0.5,fade=out:st=15:d=0.5[Text2]; [Text2] scale=w=iw/2:h=ih/2 [Text2e]; [video1][Text2e]overlay=x='if(lte(t,{4}),nan,''if(gte(t,{5}),-40*(t-{4}.5),0)'')':y=main_h-112:shortest=1 [video2]; [3:v]fade=in:st=0:d=0.5,fade=out:st=15:d=0.5[banner];  [banner] scale=w=iw/2:h=ih/2 [bannere]; [video2][bannere] overlay=x='if(lte(t,{3}.2),0.5*(t-{3}.2)*main_h+overlay_h-10,''if(gte(t,{5}),-40*(t-{6}), 0)'')':y=main_h-164:shortest=1\" -y {1}{2}.mp4";

        //string video_cmd = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \" [1:v]fade=in:st=0:d=0.2,fade=out:st=5:d=0.2:alpha=1[Text1]; [Text1] scale=w=iw/2:h=ih/2 [Text1e]; [0:v][Text1e] overlay=x='if(lte(t,{3}.5),0.7*(t-{3}.5),''if(gte(t,{4}),nan, 0)'')':y=main_h-112:shortest=1[video1]; [2:v]fade=in:st=5:d=0.5,fade=out:st=15:d=0.5[Text2]; [Text2] scale=w=iw/2:h=ih/2 [Text2e]; [video1][Text2e]overlay=x='if(lte(t,{4}),nan,''if(gte(t,{5}),-40*(t-{4}.5),0)'')':y=main_h-112:shortest=1 [video2]; [3:v]fade=in:st=0:d=0.5,fade=out:st=15:d=0.5[banner];  [banner] scale=w=iw/2:h=ih/2 [bannere]; [video2][bannere] overlay=x='if(lte(t,{3}.2),0.5*(t-{3}.2)*main_h+overlay_h-10,''if(gte(t,{5}),-40*(t-{6}), 0)'')':y=main_h-164:shortest=1\" -y {1}{2}.mp4";
        string video_cmd = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \" [1:v]fade=in:st=0:d=0.1,fade=out:st=5:d=0.1:alpha=1[Text1]; [Text1] scale=w=iw/2:h=ih/2 [Text1e]; [0:v][Text1e] overlay=x='if(lte(t,{3}.5),0.07*(t-{3}.5)*main_h-10,''if(gte(t,{4}),nan, 0)'')':y=main_h-120:shortest=1[video1]; [2:v]fade=in:st=5:d=0.5,fade=out:st=15:d=0.5[Text2]; [Text2] scale=w=iw/2:h=ih/2 [Text2e]; [video1][Text2e]overlay=x='if(lte(t,{4}),nan,''if(gte(t,{5}),-40*(t-{4}.5),0)'')':y=main_h-120:shortest=1 [video2]; [3:v]fade=in:st=0:d=0.5,fade=out:st=15:d=0.5[banner]; [banner] scale=w=iw/2:h=ih/2 [bannere]; [video2][bannere] overlay=x='if(lt(t,{3}.5),0.07*(t-{3}.5)*main_h-10,''if(gte(t,{5}),-40*(t-{6}), 0)'')':y=main_h-163:shortest=1\" -y {1}{2}.mp4";


        //string video_cmd30 = "ffmpeg - i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v] fade=in:st={3}:d=0.2,fade=out:st={4}:d=0.2:alpha=1[Text1];[Text1]scale=w=iw/2:h=ih/2 [Text1e];[0:v][Text1e] overlay=x='if(lte(t,{3}),nan, ''if(lte(t,{5}),90*(t-{6}), 0)'')':y=main_h-120:shortest=1[video1];[2:v]fade=in:st={4}:d=0.5,fade=out:st={7}:d=0.5[Text2];[Text2]scale=w=iw/2:h=ih/2 [Text2e];[video1][Text2e]overlay=x='if(lte(t,{4}),nan,0)':y=main_h-120:shortest=1 [video2];[3:v]fade=in:st={3}:d=0.5,fade=out:st={7}:d=0.5[banner];[banner]scale=w=iw/2:h=ih/2[bannere];[video2][bannere]overlay=x='if(lte(t,{3}),nan,''if(lte(t,{5}),90*(t-{6}), 0)'')':y=main_h-164:shortest=1\" -y {1}{2}.mp4";
        string video_cmd30 = "-i {0}{2}.mp4 -loop 1 -i {0}{2}_text.png -loop 1 -i {0}{2}_text2.png -loop 1 -i {0}{2}_head.png -c:a copy -filter_complex \"[1:v]fade=in:st=30:d=0.2,fade=out:st=35:d=0.2:alpha=1[Text1];[Text1]scale=w=iw/2:h=ih/2 [Text1e];[0:v][Text1e]overlay=x='if(lte(t,30),nan,''if(lte(t,31.7),90*(t-31), 0)'')':y=main_h-120:shortest=1[video1];[2:v]fade=in:st=35:d=0.5,fade=out:st=40:d=0.5[Text2];[Text2]scale=w=iw/2:h=ih/2 [Text2e];[video1][Text2e]overlay=x='if(lte(t,35),nan,0)':y=main_h-120:shortest=1 [video2];[3:v]fade=in:st=30:d=0.5,fade=out:st=40:d=0.5[banner];[banner]scale=w=iw/2:h=ih/2 [bannere];[video2][bannere]overlay=x='if(lte(t,30.2),nan,''if(lte(t,30.9),90*(t-31), 0)'')':y=main_h-163:shortest=1\" -y {1}{2}.mp4";
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

        public void Convert(string tmp_name, string text_1, string text_2, string name, string headline, string photo_url, int duration)
        {
            string path = "C:\\Temp\\";
            string out_path = "C:\\VidOut\\";
            string fileName_text = tmp_name + "_text.png";
            string fileName_text2 = tmp_name + "_text2.png";
            string fileName_head = tmp_name + "_head.png";
            //string photoName = tmp_name + "_photo.png";
            Bitmap b_text1, b_text2;
            Logger.Write(name);

            if (text_1.Length > 50)
            {
                string p1, p2;
                int index = text_1.Substring(0, 50).LastIndexOf(' ');
                if (index <= 0)
                {
                    p1 = text_1.Substring(0, 49);
                    p2 = text_1.Substring(49);
                }
                else
                {
                    p1 = text_1.Substring(0, index);
                    p2 = text_1.Substring(index + 1);
                }
                Logger.Write("Text1: " + p1);
                Logger.Write("Text1: " + p2);
                b_text1 = string.IsNullOrWhiteSpace(text_1) ? new Bitmap(1, 1) :
                EditImage.MergeAbove(
                    EditImage.ConvertTextToBitmap(p1, Color.Black, Color.White, template_body_string),
                     EditImage.ConvertTextToBitmap(p2, Color.Black, Color.White, template_body_string));
            }
            else
            {
                b_text1 = string.IsNullOrWhiteSpace(text_1) ? new Bitmap(1, 1) : EditImage.ConvertTextToBitmap(text_1, Color.Black, Color.White, template_body_string);
                Logger.Write("Text1: lss  " + text_1);

            }

            if (text_2.Length > 50)
            {
                string p1, p2;
                int index = text_2.Substring(0, 50).LastIndexOf(' ');
                if (index <= 0)
                {
                    p1 = text_2.Substring(0, 49);
                    p2 = text_2.Substring(49);
                }
                else
                {
                    p1 = text_2.Substring(0, index);
                    p2 = text_2.Substring(index + 1);
                }

                Logger.Write("Text2: " + p1);
                Logger.Write("Text2: " + p2);
                b_text2 = string.IsNullOrWhiteSpace(text_2) ? new Bitmap(1, 1) :
                EditImage.MergeAbove(
                    EditImage.ConvertTextToBitmap(p1, Color.Black, Color.White, template_body_string),
                     EditImage.ConvertTextToBitmap(p2, Color.Black, Color.White, template_body_string));
            }
            else
            {
                b_text2 = string.IsNullOrWhiteSpace(text_2) ? b_text1 : EditImage.ConvertTextToBitmap(text_2, Color.Black, Color.White, template_body_string);
                Logger.Write("Text2 ls: " + text_2);
            }
            Logger.Write("headline:  " + headline);
          //  Bitmap bt = EditImage.ConvertTextToBitmap(template_headline_string, Color.Gray, Color.White, template_headline_string);
            Bitmap b_headline = string.IsNullOrWhiteSpace(headline) ? new Bitmap(1,1) : EditImage.ConvertTextToBitmap(headline, Color.Gray, Color.White, template_headline_string);
            Logger.Write("headline is  " + headline);
            if (!string.IsNullOrWhiteSpace(name))
            {
                Bitmap b_name = EditImage.ConvertTextToBitmap(name.PadRight(50), Color.White, Color.Black, template_headline_string);
                b_headline = EditImage.MergeAbove(b_name, b_headline);

                //EditImage.ResizeImage(photoName, path, b_addAndName.Height + 1);
                if (!string.IsNullOrWhiteSpace(photo_url))
                {
                    b_headline = EditImage.MergeBeside(EditImage.URLToBitmap(photo_url, string.IsNullOrWhiteSpace(headline) ? b_headline.Height * 2 : b_headline.Height), b_headline);
                }
            }

            b_headline.Save(path + fileName_head, ImageFormat.Png);
            b_text1.Save(path + fileName_text, ImageFormat.Png);
            b_text2.Save(path + fileName_text2, ImageFormat.Png);
            Logger.Write("FFMpeg Conversion Start: " + string.Format(video_cmd, path, out_path, tmp_name, 0, 5, 15, 1));
            runningFFMPEG(string.Format(video_cmd, path, out_path, tmp_name, 0, 5, 15, 1));
            Logger.Write("Duration: " + duration);
            int t = 30;
            duration -= 30;
            Logger.Write("Duration: " + duration);

            if (duration > 0)
            {
                //    int t_P5 = t + 5;
                //    float t_p09 = t + 0.9f;
                //    int t_6 = t + 1;
                //    int t10 = t + 10;
                File.Copy(out_path + tmp_name + ".mp4", path + tmp_name + ".mp4", true);
                Logger.Write("FFMpeg Conversion 2 Start: " + string.Format(video_cmd30, path, out_path, tmp_name));
                runningFFMPEG(string.Format(video_cmd30, path, out_path, tmp_name));//, t, t_P5, t_p09, t_6, t10));
                duration -= 30;
                t += 30;
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
        public static Bitmap MergeBeside(Bitmap image1, Bitmap image2)
        {
            Bitmap bitmap = new Bitmap(image1.Width + image2.Width, Math.Max(image1.Height, image2.Height));
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(image1, 0, 0);
                g.DrawImage(image2, image1.Width - image1.Width / 5, 0);
            }
            return bitmap;
        }
        public static Bitmap ConvertTextToBitmap(string i_text, Color i_backColor, Color i_textColor, string Template)
        {
            Bitmap objBmpImage = new Bitmap(1, 1);
            int intWidth = 0;
            int intHeight = 0;

            // Create the Font object for the image text drawing.
            Font objFont = new Font("Roboto", 32, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);

            // Create a graphics object to measure the text's width and height.
            Graphics objGraphics = Graphics.FromImage(objBmpImage);

            // This is where the bitmap size is determined.
            intWidth = (int)objGraphics.MeasureString(Template ?? i_text, objFont, new PointF(0, 0), new StringFormat(StringFormatFlags.MeasureTrailingSpaces)).Width + 12;
            intHeight = (int)objGraphics.MeasureString(Template ?? i_text, objFont).Height + 3;

            // Create the bmpImage again with the correct size for the text and font.
            objBmpImage = new Bitmap(objBmpImage, new Size(intWidth, intHeight));

            // Add the colors to the new bitmap.
            objGraphics = Graphics.FromImage(objBmpImage);

            // Set Background color
            objGraphics.Clear(i_backColor);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            objGraphics.DrawString(i_text, objFont, new SolidBrush(i_textColor), 0, 0);
            objGraphics.Flush();

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
