using API.Models.Database;
using API.Models.Inputs.Policy;
using API.Models.Responses;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.IdentityModel.Tokens;
using SigortaDefterimV2API.Controllers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SigortaDefterimV2API.Services
{
    public class UtilityService
    {

        string damageimagepath = Directory.GetCurrentDirectory() + "\\Damageimages", soundpath = Directory.GetCurrentDirectory() + "\\Sound", pdfpath = Directory.GetCurrentDirectory() + "\\Pdf", userpath = Directory.GetCurrentDirectory() + "\\Userphoto";
        int processType = 0;
        /// <summary>
        ///  0:damageimage,1:sound,2:pdf,3:userimage 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public UtilityService(int processType)
        {
            this.processType = processType;
            switch (processType)
            {
                case 0:
                    if (!Directory.Exists(damageimagepath))
                        Directory.CreateDirectory(damageimagepath);
                    break;
                case 1:
                    if (!Directory.Exists(soundpath))
                        Directory.CreateDirectory(soundpath);
                    break;
                case 2:
                    if (!Directory.Exists(pdfpath))
                        Directory.CreateDirectory(pdfpath);
                    break;
                case 3:
                    if (!Directory.Exists(userpath))
                        Directory.CreateDirectory(userpath);
                    break;
                default:
                    break;
            }

        }
        public UtilityService() { }
        public static void SendEmail(string receiverEmail, string subject, string body, List<Attachment>? attachmentList, bool IsBodyHtml, List<MobilAcente> mailList = null, byte processType = 0)
        {
            body += "  <br><br><p style=\"font-size: 8px;\">2021  © Neosinerji Bilgi Teknolojileri A.Ş.</p>";
            string fromEMailName = "SİGORTADEFTERİM.com";
            MailMessage message = new MailMessage();
            message.From = new MailAddress("neosinerji@neoonline.com.tr", fromEMailName);
            if (!string.IsNullOrEmpty(receiverEmail))
                message.To.Add(new MailAddress(receiverEmail));
            else
            {
                message.To.Add(new MailAddress("sigortadefterim@neoonline.com.tr"));
                if (processType != 0)
                {
                    if (mailList != null)
                        foreach (var item in mailList)
                        {
                            message.To.Add(new MailAddress(item.BildirimEmail));
                        }
                }
            }

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
        public static void FeedBackMail(UserResponse userResponse, bool IsBodyHtml, bool isWebSite = false)
        {

            string fromEMailName = !isWebSite ? "Müşteri Önerisi" : "sigortadefterim.com İletişim";
            MailMessage message = new MailMessage();
            message.From = new MailAddress("neosinerji@neoonline.com.tr", fromEMailName);
            message.To.Add(new MailAddress("sigortadefterim@neoonline.com.tr"));


            message.Subject = !isWebSite ? "Müşteri Önerisi" : "sigortadefterim.com İletişim";
            if (!isWebSite) message.Body += "Kullanıcı Bilgileri<br>";
            message.Body += "Ad Soyad: " + userResponse.adsoyad;
            message.Body += "<br>E-Mail: " + userResponse.email + "<br><br>";
            if (!isWebSite) message.Body += "Konu: " + userResponse.subject + "<br>";
            message.Body += "Mesaj: " + userResponse.message;
            if (IsBodyHtml)
            {
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message.Body, null, "text/html");
                message.AlternateViews.Add(htmlView);

            }
            SmtpClient mySmtpClient = new SmtpClient("mail.neoonline.com.tr");//  212.58.2.42
            //SmtpClient mySmtpClient = new SmtpClient("212.58.20.131");//  212.58.2.42
            System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential("neosinerji@neoonline.com.tr", "Nsnrj01032016");
            mySmtpClient.Port = 587;
            mySmtpClient.EnableSsl = false;
            mySmtpClient.UseDefaultCredentials = false;
            mySmtpClient.Credentials = myCredential;
            var res=mySmtpClient.SendMailAsync(message);
            res.Wait();
            message.Dispose();
            mySmtpClient.Dispose();
        }
        public string[] fileUrl(string base64)
        {
            filename = Guid.NewGuid().ToString() + (processType == 1 ? ".mp3" : processType == 2 ? ".pdf" : ".png");
            filepath = (processType == 1 ? soundpath : processType == 2 ? pdfpath : processType == 3 ? userpath : damageimagepath) + "\\" + filename;

            var bytearray = Convert.FromBase64String(base64);
            File.WriteAllBytes(filepath, bytearray);
            resultStringList = new string[2];
            resultStringList[0] = "https://sigortadefterimv2api.azurewebsites.net/" + (processType == 1 ? "Sound" : processType == 2 ? "Pdf" : processType == 3 ? "Userphoto" : "Damageimages") + "/" + filename;
            resultStringList[1] = filepath;
            return resultStringList;
        }
        public void deleteUserPhoto(string filename)
        {
            filename = filename.Replace("https://sigortadefterimv2api.azurewebsites.net/Userphoto/", "");
            if (filename == "noavatar.png") return;
            filepath = userpath + "\\" + filename;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
        }
        PdfPTable pdfPTable;
        PdfPCell pdfPCell;
        Paragraph paragraph;
        Document document;
        string filename = "";
        string filepath = "";
        string[] resultStringList;
        iTextSharp.text.Font fontNormal;
        iTextSharp.text.Font fontBaslik;

        public iTextSharp.text.pdf.PdfWriter PdfWriter_GetInstance(iTextSharp.text.Document document, System.IO.FileStream filestrm)
        {
            iTextSharp.text.pdf.PdfWriter pdfwr = null;

            for (int repeat = 0; repeat < 6; repeat++)
            {
                try
                {
                    pdfwr = iTextSharp.text.pdf.PdfWriter.GetInstance(document, filestrm);
                    break; //created, then exit loop
                }
                catch // instantiation of PdfWriter failed, then pause
                {
                    System.Threading.Thread.Sleep(300);
                }
            }
            if (pdfwr == null)
            {
                throw new Exception("iTextSharp PdfWriter was not instantiated");
            }

            return pdfwr;
        }
        /// <summary>
        /// 0:teklif talebi,1:hasar bildirimi,2:Yenileme talebi
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string[] createPdf(PdfPolicy _pdfPolicy, byte processType)//0:teklif al
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            iTextSharp.text.pdf.BaseFont STF_Helvetica_Turkish = iTextSharp.text.pdf.BaseFont.CreateFont(Directory.GetCurrentDirectory() + "\\font\\arial.ttf", "windows-1254", BaseFont.EMBEDDED);

            fontNormal = new iTextSharp.text.Font(STF_Helvetica_Turkish, 12, iTextSharp.text.Font.NORMAL);
            fontBaslik = new iTextSharp.text.Font(STF_Helvetica_Turkish, 18, iTextSharp.text.Font.BOLD);
           
            iTextSharp.text.Image myImage; 
            filename = "TalepNo_" + _pdfPolicy.TeklifIslemNo.ToString() + "_" + Guid.NewGuid().ToString() + ".pdf";
            filepath = pdfpath + "\\" + filename;
 
            using (FileStream stream = new FileStream(filepath, FileMode.Create))
            {
                document = new Document(PageSize.LETTER, 30, 30, 30, 20);
                PdfWriter.GetInstance(document, stream);
                document.Open();

                myImage = iTextSharp.text.Image.GetInstance(Directory.GetCurrentDirectory() + @"\Images\logosmall.png");

                pdfPTable = new PdfPTable(2);
                pdfPTable.WidthPercentage = 90f;
                pdfPCell = new PdfPCell(myImage);
                pdfPCell.HorizontalAlignment = 0;
                pdfPCell.VerticalAlignment = 1;
                pdfPCell.FixedHeight = 25;
                pdfPCell.Border = 0;
                pdfPTable.AddCell(pdfPCell);

                pdfPCell = new PdfPCell(new Phrase(UtilityService.EditDateTime(DateTime.Now.ToShortDateString())));
                pdfPCell.HorizontalAlignment = 2;
                pdfPCell.Border = 0;

                pdfPTable.AddCell(pdfPCell);

                document.Add(pdfPTable);

                pdfPTable = new PdfPTable(1);
                pdfPTable.SpacingBefore = 15;
                pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.BransAdi + " SİGORTASI " + (processType == 0 ? "TEKLİF TALEBİ" : processType == 1 ? "HASAR BİLDİRİMİ" : "YENİLEME TALEBİ"), fontBaslik));
                pdfPCell.HorizontalAlignment = 1;
                pdfPCell.Border = 0;
                pdfPTable.AddCell(pdfPCell);
                document.Add(pdfPTable);

                document.Add(new Paragraph("    Sayın " + (!string.IsNullOrEmpty(_pdfPolicy.AcenteUnvani) ? _pdfPolicy.AcenteUnvani : _pdfPolicy.Sirket) + ", SİGORTADEFTERİM.com kullanıcısı tarafından aşağıda detayları bulunan " + (processType == 0 ? "Teklif Talebini" : processType == 1 ? "Hasar Bildirimini" : "Yenileme Talebini") + " acilen değerlendirerek, " + (processType == 1 ? "gerekli işlemlerin başlatılması hususunu" : " farklı sigorta şirketlerinden alınmış tekliflerinizi maildeki linke tıklayarak ilgili uygulama üzerinden göndermenizi") + " önemle bilgilerinize sunarız.\n\nSaygılarımızla\n\nSİGORTADEFTERİM.com", fontNormal) { SpacingBefore = 10 });

                if (processType != 0)
                {
                    pdfPTable = new PdfPTable(1);
                    pdfPTable.SpacingBefore = 10;
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.isAcente ? _pdfPolicy.AcenteUnvani : _pdfPolicy.Sirket, fontNormal));
                    pdfPCell.HorizontalAlignment = 1;
                    pdfPTable.AddCell(pdfPCell);

                    if (!string.IsNullOrEmpty(_pdfPolicy.AcenteLogo) && _pdfPolicy.isAcente)
                    {
                        myImage = iTextSharp.text.Image.GetInstance(_pdfPolicy.AcenteLogo);
                        pdfPCell = new PdfPCell(myImage);
                        pdfPCell.HorizontalAlignment = 1;
                        pdfPCell.VerticalAlignment = 1;
                        pdfPCell.Padding = 5;
                        pdfPCell.FixedHeight = 70;
                    }
                    else if (!string.IsNullOrEmpty(_pdfPolicy.SirketLogo) && !_pdfPolicy.isAcente)
                    {
                        myImage = iTextSharp.text.Image.GetInstance(_pdfPolicy.SirketLogo);
                        pdfPCell = new PdfPCell(myImage);
                        pdfPCell.HorizontalAlignment = 1;
                        pdfPCell.VerticalAlignment = 1;
                        pdfPCell.Padding = 5;
                        pdfPCell.FixedHeight = 70;
                    }
                    else
                    {
                        pdfPCell = new PdfPCell(new Phrase("", fontNormal));

                    }
                    pdfPTable.AddCell(pdfPCell);

                    document.Add(pdfPTable);
                }
                paragraph = new Paragraph("Poliçe Bilgileri");
                paragraph.Alignment = 1;
                paragraph.Font.Size = 17;
                paragraph.SpacingBefore = 10;
                document.Add(paragraph);

                pdfPTable = new PdfPTable(2);
                pdfPTable.SpacingBefore = 10;

                pdfPCell = new PdfPCell(new Phrase("SİGORTADEFTERİM.com Talep No", fontNormal));
                pdfPTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.TeklifIslemNo.ToString(), fontNormal));
                pdfPTable.AddCell(pdfPCell);

                if (processType != 0)
                {
                    pdfPCell = new PdfPCell(new Phrase("Sigorta Şirketi", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Sirket, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Poliçe Numarası/Yenileme No", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.PoliceNumarasi + "/" + _pdfPolicy.YenilemeNo, fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                }

                pdfPCell = new PdfPCell(new Phrase(!string.IsNullOrEmpty(_pdfPolicy.BaslangicTarihi) ? "Başlangıç Tarihi" : "Seyahat Gidiş Tarihi", fontNormal));
                pdfPTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(!string.IsNullOrEmpty(_pdfPolicy.BaslangicTarihi) ? _pdfPolicy.BaslangicTarihi : _pdfPolicy.SeyahatGidisTarihi, fontNormal));
                pdfPTable.AddCell(pdfPCell);

                pdfPCell = new PdfPCell(new Phrase(!string.IsNullOrEmpty(_pdfPolicy.BitisTarihi) ? "Bitiş Tarihi" : "Seyahat Dönüş Tarihi", fontNormal));
                pdfPTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(!string.IsNullOrEmpty(_pdfPolicy.BitisTarihi) ? _pdfPolicy.BitisTarihi : _pdfPolicy.SeyahatDonusTarihi, fontNormal));
                pdfPTable.AddCell(pdfPCell);

                pdfPCell = new PdfPCell(new Phrase("Branş Adı", fontNormal));
                pdfPTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.BransAdi, fontNormal));
                pdfPTable.AddCell(pdfPCell);

                pdfPCell = new PdfPCell(new Phrase("TCKN/VKN", fontNormal));
                pdfPTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.KimlikNo, fontNormal));
                pdfPTable.AddCell(pdfPCell);

                pdfPCell = new PdfPCell(new Phrase("Adı Soyadı", fontNormal));
                pdfPTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.AdSoyad, fontNormal));
                pdfPTable.AddCell(pdfPCell);

                selectUrun(_pdfPolicy);

                if (processType == 1)
                {
                    pdfPCell = new PdfPCell(new Phrase("Konum Bilgisi", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(string.IsNullOrEmpty(_pdfPolicy.Konum) ? "Konum bilgisi bulunamadı" : _pdfPolicy.Konum, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    //Chunk portText = new Chunk(string.IsNullOrEmpty(_pdfPolicy.Konum) ? "" : "Konuma gitmek için tıklayınız", fontNormal);
                    //if (!string.IsNullOrEmpty(_pdfPolicy.Konum))
                    //    portText.SetAnchor(new Uri(_pdfPolicy.Konum));
                    //pdfPCell = new PdfPCell(new Phrase(portText));
                    //pdfPTable.AddCell(pdfPCell);
                }
                pdfPCell = new PdfPCell(new Phrase("Açıklama", fontNormal));
                pdfPTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Aciklama, fontNormal));
                pdfPTable.AddCell(pdfPCell);

                document.Add(pdfPTable);

                pdfPTable = new PdfPTable(2);
                pdfPTable.WidthPercentage = 90f;
                pdfPTable.SpacingBefore = 75;
                pdfPCell = new PdfPCell(new Phrase("2020 - © Neosinerji", fontNormal));
                pdfPCell.HorizontalAlignment = 0;
                pdfPCell.Border = 0;
                pdfPTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase("www.sigortadefterim.com", fontNormal));
                pdfPCell.Border = 0;
                pdfPCell.HorizontalAlignment = 2;
                pdfPTable.AddCell(pdfPCell);

                document.Add(pdfPTable);

                document.Close();
            }
            //pdfWriter.Close();
            resultStringList = new string[2];
            resultStringList[0] = "https://sigortadefterimv2api.azurewebsites.net/Pdf/" + filename;
            resultStringList[1] = filepath;

            return resultStringList;
        }

        void selectUrun(PdfPolicy _pdfPolicy)
        {
            switch (_pdfPolicy.BransKodu)
            {
                case 1:
                case 2:
                    pdfPCell = new PdfPCell(new Phrase("Araç Plakası", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Plaka, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Tescil Belge No(Ruhsat No)", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.RuhsatSeriKodu + _pdfPolicy.RuhsatSeriNo, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("ASBIS NO", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.AsbisNo, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Araç Markası", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Marka, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Araç Tipi", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Tip, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Araç Model Yılı", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.ModelYili.ToString(), fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Kullanım Tarzı", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.AracKullanimTarzi, fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    break;
                case 4://saglik
                    pdfPCell = new PdfPCell(new Phrase("Meslek", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Meslek, fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    break;
                case 11://dask
                    pdfPCell = new PdfPCell(new Phrase("İl", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Il, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("İlçe", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Ilce, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Bina Kat Sayısı", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.BinaKatSayisi.ToString(), fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Bina Yapım Yılı", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(getYapimYili(_pdfPolicy.BinaYapimYili), fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Bina Kullanım Şekli", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(getBinaKullanimSekli(_pdfPolicy.BinaKullanimSekli), fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Daire Brüt(m²)", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.DaireBrut.ToString(), fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Bina Yapı Tarzı", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(getYapiTarzi(_pdfPolicy.BinaYapiTarzi), fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Adres", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Adres, fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    break;
                case 21:
                    pdfPCell = new PdfPCell(new Phrase("Seyahat Edilen Ülke Tipi", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.SeyahatUlkeTipi, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Seyahat Edilen Ülke", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.SeyahatUlke, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Seyahat Eden Kişi Sayısı", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.SeyahatEdenKisiSayisi.ToString(), fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    break;
                case 22:
                    pdfPCell = new PdfPCell(new Phrase("İl", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Il, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("İlçe", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Ilce, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Eşya Bedeli", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.EsyaBedeli.ToString(), fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Bina Bedeli", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.BinaBedeli.ToString(), fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Adres", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Adres, fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    break;
                default:
                    pdfPCell = new PdfPCell(new Phrase("İl", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Il, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("İlçe", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Ilce, fontNormal));
                    pdfPTable.AddCell(pdfPCell);

                    pdfPCell = new PdfPCell(new Phrase("Adres", fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(_pdfPolicy.Adres, fontNormal));
                    pdfPTable.AddCell(pdfPCell);
                    break;
            }
        }
        string getYapiTarzi(int index)
        {
            List<string> itemList = new List<string> {
                  "ÇELİK, BETORNARME, KARKAS",
                  "YIĞMA KAGİR",
                  "DİĞER"
            };

            return itemList[index];
        }
        string getYapimYili(int index)
        {
            List<string> itemList = new List<string> {
                  "1975 - ÖNCESİ",
                  "1976 - 1996",
                  "1997 - 1999",
                  "2000 - 2006",
                  "2007 - SONRASI"
            };

            return itemList[index];
        }
        string getBinaKullanimSekli(int index)
        {
            List<string> itemList = new List<string> { "MESKEN", "BÜRO", "TİCARETHANE", "DİĞER"
            };

            return itemList[index];
        }
        public static string EditDateTime(string tarih)
        {
            string[] temp = tarih.Replace("-", "/").Replace(".", "/").Split("/");
            tarih = (temp[1].Length == 1 ? "0" + temp[1] : temp[1]) + "/" + (temp[0].Length == 1 ? "0" + temp[0] : temp[0]) + "/" + temp[2];
            return tarih;
        }
        public string createToken(string id, string secret)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            //var myIssuer = "https://localhost:44346";
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, Guid.NewGuid().ToString())
                }),
                Expires = tokenExipres(),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public DateTime tokenExipres()
        {
            DateTime expires = DateTime.UtcNow.AddHours(1);
            DateTime dateTimeNow = DateTime.Now;
            DateTime dateTimeCheck = DateTime.Parse(DateTime.Now.ToShortDateString() + " 15:00:00"); //azure saatine göre ayarlandı TR içn 18 olmalı
            var currentDayNumber = (int)DateTime.Now.DayOfWeek;
            string sonrakigunSaati = " 07:00:00";//sonraki gün bitiş saaati --azure 3 saat geri oldugun için 7 olarak ayarlandı TR için 10 olmalı
            double totalMinutes = 0;
            switch (currentDayNumber)
            {
                case 5:
                    if (dateTimeNow > dateTimeCheck)
                    {
                        totalMinutes = (DateTime.Parse(DateTime.Now.AddDays(3).ToShortDateString() + sonrakigunSaati) - DateTime.Now).TotalMinutes;
                        expires = DateTime.UtcNow.AddMinutes(totalMinutes);
                    }
                    break;
                case 6:
                    totalMinutes = (DateTime.Parse(DateTime.Now.AddDays(2).ToShortDateString() + sonrakigunSaati) - DateTime.Now).TotalMinutes;
                    expires = DateTime.UtcNow.AddMinutes(totalMinutes);
                    break;
                case 7:
                    totalMinutes = (DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString() + sonrakigunSaati) - DateTime.Now).TotalMinutes;
                    expires = DateTime.UtcNow.AddMinutes(totalMinutes);
                    break;
                default:
                    if (dateTimeNow > dateTimeCheck)
                    {
                        totalMinutes = (DateTime.Parse(DateTime.Now.AddDays(1).ToShortDateString() + sonrakigunSaati) - DateTime.Now).TotalMinutes;
                        expires = DateTime.UtcNow.AddMinutes(totalMinutes);
                    }
                    break;
            }
            return expires;
        }
        public bool checkToken(string token, string secret)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string ReadFromFile(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                return File.ReadAllText(FilePath);
            }
            return "";
        }
        public static void ReadFromFile(string FilePath, string Text)
        {
            if (File.Exists(FilePath))
            {
                File.WriteAllText(FilePath, Text);
            }
        }
    }
}
