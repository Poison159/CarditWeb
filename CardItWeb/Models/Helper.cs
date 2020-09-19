using BarcodeLib;
using CardItWebApp.Database;
using CardItWebApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using ZXing;

namespace CardItWeb.Models
{
    public static class Helper
    {
        public static Token saveAppUserAndToken(AppUser appUser, CardItDbContext db)
        {
            var token = createToken(appUser.Email);
            db.AppUsers.Add(appUser);
            db.Tokens.Add(token);
            db.SaveChanges();
            return token;
        }
        public static Token createToken(string email)
        {
            var tokenString = Guid.NewGuid().ToString();
            var grantDate = DateTime.Now;
            var endDate = grantDate.AddDays(90);
            return new Token(email, tokenString, grantDate, endDate);
        }

        public static byte[] ConvertToByteArray(this Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public static void downloadImage(string imageUrl, string downloadPath ) {
            using (WebClient client = new WebClient()){
                client.DownloadFile(new Uri(downloadPath), imageUrl);
            }
        }

        public static bool createBarcode(string cardNumber)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + @"Content\imgs\" + cardNumber + ".jpg";
            BarcodeWriter write = createWriter(cardNumber);
            if (write != null)
            {
                try
                {
                    Image img = write.Write(cardNumber);
                    img.Save(path, ImageFormat.Jpeg);
                }
                catch (Exception)
                {
                    return false;
                }
                if (!File.Exists(path)) {
                    return false;
                }
                return true;
            }
            else {
                return false;
            }
        }

        internal static void deleteOldBarcode(string cardNUmber)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + @"Content\imgs\" + cardNUmber + ".jpg";
            File.Delete(path);
        }

        private static BarcodeWriter createWriter(string cardNumer)
        {
            BarcodeWriter writer = null;
            if (cardNumer.Trim().Count() == 13)
            {
                writer = new BarcodeWriter() { Format = BarcodeFormat.EAN_13 };
            }
            else if (cardNumer.Trim().Count() == 16 || cardNumer.Trim().Count() == 9)
            {
                writer = new BarcodeWriter() { Format = BarcodeFormat.CODE_128 };
            }
            else if (cardNumer.Trim().Count() == 8)
            {
                writer = new BarcodeWriter() { Format = BarcodeFormat.EAN_8 };
            }
            else if (cardNumer.Trim().Count() == 12)
            {
                writer = new BarcodeWriter() { Format = BarcodeFormat.UPC_A };
            }
            else {
            }
            return writer;
        }
    }
}
