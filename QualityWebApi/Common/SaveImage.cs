using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace QualityWebApi.Common
{
    public class SaveImage
    {
        public SaveImage()
        {

        }

        public string SaveInageForBase(string base64,string savePath,string fileName)
        {
 
            string filePath = savePath +"/" +fileName + ".png";
            byte[] arr = Convert.FromBase64String(base64);
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Write(arr, 0, arr.Length);
                fs.Flush();
            }
            //MemoryStream ms = new MemoryStream(arr);
            //Bitmap bmp = new Bitmap(ms);
            //bmp.Save(@"d:\test.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            ////bmp.Save(@"d:\"test.bmp", ImageFormat.Bmp);  
            ////bmp.Save(@"d:\"test.gif", ImageFormat.Gif);  
            ////bmp.Save(@"d:\"test.png", ImageFormat.Png);  
            //ms.Close();

            
            return filePath;
        }
    }
}
