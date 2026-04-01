using API.Areas.MobilApi.Models.Database;
using API.Areas.MobilApi.Models.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Areas.MobilApi.Helper
{
    public class Utils
    {
        public static void SendEmail(string receiverEmail, string subject, string body, List<Attachment>? attachmentList, bool IsBodyHtml, byte processType = 0)
        {
            body += "  <br><br><p style=\"font-size: 8px;\">2021  © Neosinerji Bilgi Teknolojileri A.Ş.</p>";
            string fromEMailName = "DaskJet.com";
            MailMessage message = new MailMessage();
            message.From = new MailAddress("neosinerji@neoonline.com.tr", fromEMailName);
            message.To.Add(new MailAddress(receiverEmail));


            message.Subject = subject;
            message.Body = body;
            if (attachmentList != null)
            {
                foreach (var item in attachmentList)
                {
                    message.Attachments.Add(item);
                }
            }
            if (IsBodyHtml)
            {
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                message.AlternateViews.Add(htmlView);

            }
            SmtpClient mySmtpClient = new SmtpClient("212.58.2.42");
            System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential("neosinerji@neoonline.com.tr", "Nsnrj01032016");
            mySmtpClient.Port = 587;
            mySmtpClient.EnableSsl = false;
            mySmtpClient.UseDefaultCredentials = false;
            mySmtpClient.Credentials = myCredential;
            mySmtpClient.Send(message);
            message.Dispose();
            mySmtpClient.Dispose();
        }
        public static string ReadFile(string path)
        {
            var res = System.IO.File.ReadAllText(path);
            return res;
        }
        public static void WriteFile(string path, string content)
        {
            System.IO.File.WriteAllText(path, content);
        }
        public static void AppendFile(string path, string content)
        {
            System.IO.File.AppendAllText(path, content);
        }
        public static void WriteFile(SmsItem smsItem)
        {
            var currentDate = currentLocalTime();
            System.IO.File.WriteAllText(Directory.GetCurrentDirectory() + "/areas/mobilapi/files/smscode/" + currentDate + "_" + smsItem.toPhone.Replace("+", "") + "_" + smsItem.fromPhone + "_" + Guid.NewGuid().ToString().Substring(10) + ".json", smsItem.body + "_" + smsItem.date);
        }
        public static void WriteFile2(SmsItem smsItem)
        {
            var data = prevData(smsItem);
            smsItem.date = getTRDateTime();
            data.Add(smsItem);
            var res = JsonConvert.SerializeObject(data);
            System.IO.File.WriteAllText(Directory.GetCurrentDirectory() + "/areas/mobilapi/files/smscode/smscodefile_" + smsItem.toPhone + ".json", res);
        }
        public static List<SmsItem> prevData(SmsItem smsItem, bool isUpdate = false)
        {
            List<SmsItem> data = new List<SmsItem>(), result = new List<SmsItem>();
            var path = Directory.GetCurrentDirectory() + "/areas/mobilapi/files/smscode/smscodefile_" + smsItem.toPhone + ".json";
            if (!File.Exists(path))
                foreach (var item in Directory.GetFiles(Directory.GetCurrentDirectory() + "/areas/mobilapi/files/smscode/"))
                {
                    if (item.Contains(smsItem.toPhone))
                    {
                        path = item;
                        break;
                    }
                }
            if (File.Exists(path))
            {
                data = JsonConvert.DeserializeObject<List<SmsItem>>(ReadFile(path));
                if (data == null)
                    data = new List<SmsItem>();
                else
                {
                    var tempDate = getTRDateTime().AddSeconds(-45);
                    result = data.Where(x => x.date >= tempDate).ToList();
                    if (isUpdate)
                    {
                        data = result;
                        data = data.Where(x =>
                        {
                            if (x.fromPhone == smsItem.fromPhone && x.toPhone == smsItem.toPhone)
                                return false;
                            return true;
                        }).ToList();
                        updateData(data, path);
                    }
                }
            }

            return result;
        }
        public static void updateData(List<SmsItem> data, string path)
        {
            var res = JsonConvert.SerializeObject(data);
            System.IO.File.WriteAllText(path, res);
        }
        static string dateFormatText = "yyyy.MM.dd";
        static string currentLocalTime()
        {
            var info = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);
            var milisecond = DateTime.Now.Millisecond.ToString();

            var res = localTime.Date.ToString(dateFormatText).Replace("\\", "").Replace("/", "").Replace(".", "") + "" + localTime.DateTime.ToString("HH:mm:ss").Replace(":", "") + "" + (milisecond.Length == 1 ? "00" + milisecond : milisecond.Length == 2 ? "0" + milisecond : milisecond);
            return decimal.Parse(res).ToString();
        }
        public static void WriteErrorLog(string val)
        {
            File.AppendAllText(Directory.GetCurrentDirectory() + "/areas/mobilapi/files/errorlog/error_log.json", val + " " + DateTime.Now);
        }
        public static void DeleteFile(string path)
        {
            System.IO.File.Delete(path);
        }
        public static decimal getOldTimeData(bool isDeleteOldFile = false)
        {
            var info = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);
            var milisecond = DateTime.Now.Millisecond.ToString();
            string res = "";
            if (isDeleteOldFile)
            {
                res = localTime.Date.AddDays(-1).ToString(dateFormatText).Replace("\\", "").Replace("/", "").Replace(".", "") + "" + localTime.DateTime.ToString("HH:mm:ss").Replace(":", "") + "" + (milisecond.Length == 1 ? "00" + milisecond : milisecond.Length == 2 ? "0" + milisecond : milisecond);
            }
            else
            {
                res = localTime.Date.ToString(dateFormatText).Replace("\\", "").Replace("/", "").Replace(".", "") + "" + localTime.DateTime.AddMinutes(-2).ToString("HH:mm:ss").Replace(":", "") + "" + (milisecond.Length == 1 ? "00" + milisecond : milisecond.Length == 2 ? "0" + milisecond : milisecond);

            }
            return decimal.Parse(res);
        }
        public static DateTime getTRDateTime()
        {
            var info = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);

            return localTime.DateTime;
        }

        public static string EncryptRijndael(string text, string salt)
        {
            if (string.IsNullOrEmpty(salt))
            {
                salt = "!082017?";
            }
            var aesAlg = NewRijndaelManaged(salt);
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        static RijndaelManaged NewRijndaelManaged(string salt)
        {
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            var passwordByte = Encoding.ASCII.GetBytes(salt);
            passwordByte = SHA256.Create().ComputeHash(passwordByte);
            var key = new Rfc2898DeriveBytes(passwordByte, saltBytes, 1000);

            var aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            return aesAlg;
        }

    }
}
