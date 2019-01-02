using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace NS.Component.Utility
{
    /// <summary>
    /// 邮件发送服务
    /// </summary>
    public class EmailSendHelper
    {
        /// <summary>
        /// 发送邮件--不带附件
        /// </summary>
        /// <param name="mailTitle">邮件标题</param>
        /// <param name="mailTo">待发送到的目标邮箱地址-收件人人列表：用分号隔开例如A@163.com;B@163.com</param>
        /// <param name="mailFrom">邮件发送邮箱地址</param>
        /// <param name="mailContent">邮件内容</param>
        /// <param name="cc">抄送人列表：用分号隔开例如A@163.com;B@163.com</param>
        public static void SetMessage(string mailTitle, string mailTo, string mailFrom, string mailContent, string cc =null)
        {
            //MailAddress addressFrom = new MailAddress(mailFrom);
            //MailAddress addressTo = new MailAddress(mailTo);
            MailMessage mMessage = new MailMessage();
            mMessage.From = new MailAddress(mailFrom, mailFrom, System.Text.Encoding.GetEncoding(936));

            //收件人
            if (mailTo.Split(';').Length > 1)
            {
                var mailToList = mailTo.Split(';');
                foreach (var mailToItem in mailToList)
                {
                    if (string.IsNullOrEmpty(mailToItem))
                        continue;

                    mMessage.To.Add(mailToItem);
                }
            }
            else
                mMessage.To.Add(mailTo);

            //抄送
            if (!string.IsNullOrEmpty(cc))
            {
                if (cc.Split(';').Length > 1)
                {
                    var ccList = cc.Split(';');
                    foreach (var ccItem in ccList)
                    {
                        if (string.IsNullOrEmpty(ccItem))
                            continue;

                        mMessage.CC.Add(ccItem);
                    }
                }
                else
                    mMessage.CC.Add(cc);
            }

            mMessage.IsBodyHtml = true;//show as HTML

            mMessage.Subject = mailTitle;
            mMessage.SubjectEncoding = System.Text.Encoding.GetEncoding(936);
            mMessage.Body = mailContent;
            mMessage.BodyEncoding = System.Text.Encoding.GetEncoding(936);
            try
            {
                SendMail(mMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            //SmtpClient smtp = new SmtpClient();
            //smtp.EnableSsl = false;
            //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            //var tempMailConfig = NS.Framework.Config.PlatformConfig.ServerConfig.KeyValueSettings.KeyValueItems.Where(pre => pre.Key == "Email").FirstOrDefault();

            //if (string.IsNullOrEmpty(tempMailConfig.Value))
            //    throw new Exception("邮箱服务器未配置");

            //var tempEmailItems = tempMailConfig.Value.Split(';');

            //if (tempEmailItems.Length != 3)
            //    throw new Exception("邮箱服务器配置不正确");

            //var tempSmtp = tempEmailItems[0].Replace("smtp:", "");
            //var tempSendEmailAddress = tempEmailItems[1].Replace("address:", "");
            //var tempSendEmailPassword = tempEmailItems[2].Replace("password:", "");

            //smtp.Host =tempSmtp;
            //smtp.Credentials = new NetworkCredential(tempSendEmailAddress, tempSendEmailPassword);

            //MailMessage mm = new MailMessage();
            //mm.From = new MailAddress(mailFrom, mailFrom, Encoding.GetEncoding(936));
            //mm.To.Add(mailTo);

            //mm.SubjectEncoding = Encoding.GetEncoding(936);
            //mm.Subject = mailTitle;

            //mm.BodyEncoding = Encoding.GetEncoding(936);

            //////普通文本邮件内容，如果对方的收件客户端不支持HTML，这是必需的
            ////string plainTextBody = "如果你邮件客户端不支持HTML格式，或者你切换到“普通文本”视图，将看到此内容";
            ////mm.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plainTextBody, null, "text/plain"));

            //////HTML格式邮件的内容
            //string htmlBodyContent = mailContent;
            //htmlBodyContent += "<a href=\"http://www.zu14.cn/\">真有意思网</a> <img src=\"cid:weblogo\">";   //注意此处嵌入的图片资源
            //AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(htmlBodyContent, null, "text/html");

            //////处理嵌入图片
            ////LinkedResource lrImage = new LinkedResource(@"d:\blogo.gif", "image/gif");
            ////lrImage.ContentId = "weblogo"; //此处的ContentId 对应 htmlBodyContent 内容中的 cid:  ，如果设置不正确，请不会显示图片
            ////htmlBody.LinkedResources.Add(lrImage);

            //mm.AlternateViews.Add(htmlBody);

            ////////要求回执的标志
            //////mm.Headers.Add("Disposition-Notification-To", "接收回执的邮箱@163.com");

            ////////自定义邮件头
            ////mm.Headers.Add("X-Website", "http://www.zu14.cn/");

            ////////针对 LOTUS DOMINO SERVER，插入回执头
            ////mm.Headers.Add("ReturnReceipt", "1");

            //mm.Priority = MailPriority.Normal; //优先级
            ////mm.ReplyTo = new MailAddress("回复邮件的接收地址@yahoo.com.cn", "我自己", Encoding.GetEncoding(936));

            //////如果发送失败，SMTP 服务器将发送 失败邮件告诉我
            //mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            //////异步发送完成时的处理事件
            ////smtp.SendCompleted += smtp_SendCompleted;

            ////Console.WriteLine("开始发送邮件信息");

            //////开始异步发送
            //smtp.Send(mm);
        }

        /// <summary>
        /// 发送邮件--带附件
        /// </summary>
        /// <param name="mailTitle">邮件标题</param>
        /// <param name="mailTo">待发送到的目标邮箱地址</param>
        /// <param name="mailFrom">邮件发送邮箱地址</param>
        /// <param name="mailContent">邮件内容</param>
        /// <param name="af">附件列表集合</param>
        public static void SetMessage(string mailTitle, string mailTo, string mailFrom, string mailContent, System.Collections.Generic.IList<AttachmentFile> af, string cc = null)
        {
            MailMessage mMessage = new MailMessage();

            mMessage.From = new MailAddress(mailFrom, mailFrom, System.Text.Encoding.GetEncoding(936));

            //收件人
            if (mailTo.Split(';').Length > 1)
            {
                var mailToList = mailTo.Split(';');
                foreach (var mailToItem in mailToList)
                {
                    if (string.IsNullOrEmpty(mailToItem))
                        continue;

                    mMessage.To.Add(mailToItem);
                }
            }
            else
                mMessage.To.Add(mailTo);

            //抄送
            if (!string.IsNullOrEmpty(cc))
            {
                if (cc.Split(';').Length > 1)
                {
                    var ccList = cc.Split(';');
                    foreach (var ccItem in ccList)
                    {
                        if (string.IsNullOrEmpty(ccItem))
                            continue;

                        mMessage.CC.Add(ccItem);
                    }
                }
                else
                    mMessage.CC.Add(cc);
            }

            mMessage.IsBodyHtml = true;

            mMessage.Subject = mailTitle;
            mMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            mMessage.Body = mailContent;
            mMessage.BodyEncoding = System.Text.Encoding.UTF8;
            //class AttachmentFile has tow properties:Name(string),Data(byte[])
            foreach (AttachmentFile aFile in af)
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream(aFile.Data);
                System.Net.Mail.Attachment mailAttachment = new Attachment(stream, aFile.Name);
                mMessage.Attachments.Add(mailAttachment);
            }

            try
            {
                SendMail(mMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 内部发送邮件的方法
        /// </summary>
        /// <param name="mailMessage">待发送的邮件消息对象</param>
        private static void SendMail(MailMessage mailMessage)
        {
            var tempMailConfig = NS.Framework.Config.PlatformConfig.ServerConfig.KeyValueSettings.KeyValueItems.Where(pre => pre.Key == "Email").FirstOrDefault();

            if (string.IsNullOrEmpty(tempMailConfig.Value))
                throw new Exception("邮箱服务器未配置");

            var tempEmailItems = tempMailConfig.Value.Split(';');

            if (tempEmailItems.Length != 3)
                throw new Exception("邮箱服务器配置不正确");

            mailMessage.Priority = MailPriority.Normal; //优先级
            //mm.ReplyTo = new MailAddress("回复邮件的接收地址@yahoo.com.cn", "我自己", Encoding.GetEncoding(936));

            ////如果发送失败，SMTP 服务器将发送 失败邮件告诉我
            mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            ////普通文本邮件内容，如果对方的收件客户端不支持HTML，这是必需的
            //string plainTextBody = "如果你邮件客户端不支持HTML格式，或者你切换到“普通文本”视图，将看到此内容";
            //mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plainTextBody, null, "text/plain"));

            //AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(mailMessage.Body, null, "text/html");
            //mailMessage.AlternateViews.Add(htmlBody);

            var tempSmtp = tempEmailItems[0].Replace("smtp:", "");
            var tempSendEmailAddress = tempEmailItems[1].Replace("address:", "");
            var tempSendEmailPassword = tempEmailItems[2].Replace("password:", "");
            System.Net.Mail.SmtpClient smtpClient = new SmtpClient();//or other mailbox
            smtpClient.Host = tempSmtp;
            smtpClient.EnableSsl = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(tempSendEmailAddress, tempSendEmailPassword);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            try
            {
                smtpClient.Send(mailMessage);
                smtpClient.SendCompleted += smtpClient_SendCompleted;
            }
            catch (SmtpException ex)
            {
                throw new Exception("发送邮件的过程中出现错误" + ex.Message);
            }
        }

        static void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {

            }
            else
            {
                if (e.Error == null)
                {

                }
                else
                {
                    throw new Exception("发送邮件失败" + e.Error.Message);
                }
            }
        }
    }

    /// <summary>
    /// 附件信息定义
    /// </summary>
    public class AttachmentFile
    {
        /// <summary>
        /// 文件内容
        /// </summary>
        public byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}
