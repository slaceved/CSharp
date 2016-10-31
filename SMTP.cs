    using System;
    using System.Text;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Threading;
    using System.ComponentModel;
    using System.IO;

    internal class SMTP
    {
        //{"Asynchronous operations are not allowed in this context. Page starting an asynchronous operation has to have the Async attribute set to true and an asynchronous operation can only be started on a page prior to PreRenderComplete event."}
        
        internal SMTP() { }

        private bool _async;
        public bool SendAsync { get { return _async; } set { _async = value; } }
        private int _retryCounter = 3;
        public int RetryCount { get { return _retryCounter; } set { _retryCounter = value; } }
        private string _retVal = string.Empty;
        public string AsyncResults { get { return _retVal; } set { } }
        
        /// <summary>
        /// Sends emails using Simple mail transfer protocol
        /// </summary>
        /// <param name="msgBody">Body of the message</param>
        /// <param name="msgSubject">Subject of the message</param>
        /// <param name="emailToAddr">Senders email id</param>
        /// <param name="emailFromAddr">Reciepients email id</param>
        public void SendEmail(string msgBody, string msgSubject, string emailToAddr, string emailFromAddr)
        {
            SmtpClient smtpClient = new SmtpClient();
            
            MailMessage msg = new MailMessage(); 
            msg.From = new MailAddress(emailFromAddr);
            if (emailToAddr.IndexOf(',') != -1)
            {
                string[] EmailList = emailToAddr.Split(',');
                for (int iloop = 0; iloop < EmailList.Length; iloop++)
                {
                    msg.To.Add(new MailAddress(EmailList[iloop]));
                }
            }
            else
            {
                msg.To.Add(new MailAddress(emailToAddr));
            }
            msg.Body = msgBody;
            msg.IsBodyHtml = true;
            msg.BodyEncoding = Encoding.UTF8;
            msg.Subject = msgSubject;
            msg.SubjectEncoding = Encoding.UTF8;

            int retryCounter = 0;
            bool sent = false;

            try {
                if (_async) {
                    smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                    string userState = emailToAddr + "-" + msgSubject + "-" + msgBody;
                    smtpClient.SendAsync(msg, userState);
                }
                else { smtpClient.Send(msg); }
                sent = true;
            } catch (System.Net.Sockets.SocketException se) {  //if connection times out retry a max of 3 (default value) times then quit
                (new BizRules.Logger()).WriteToEventLog(
                    string.Format("Socket Error occured. Error Code: {0} Error Message: {1}", se.SocketErrorCode.ToString(),se.Message), 
                    System.Diagnostics.EventLogEntryType.Information);
                
                while (retryCounter < _retryCounter) {   
                    try {
                        if (_async) {
                            //already defined event handler above in 1st attempt
                            string userState = emailToAddr + "-" + msgSubject + "-" + msgBody;
                            smtpClient.SendAsync(msg, userState);
                        }
                        else { smtpClient.Send(msg); }
                        sent = true;
                    } catch (System.Net.Sockets.SocketException se_retry) {
                        retryCounter += 1;
                        (new BizRules.Logger()).WriteToEventLog(
                            string.Format("Email resend try {0}. Error Code: {1} Error Message: {2}", retryCounter.ToString(), 
                            se_retry.SocketErrorCode.ToString(),se_retry.Message), System.Diagnostics.EventLogEntryType.Information);
                        se_retry = null;
                    }
                }

                if (sent == false) {
                    (new BizRules.Logger()).WriteToEventLog(
                        string.Format("Email could not be sent. To: {0} Subject: {1} Body: {2}",emailToAddr,msgSubject,msgBody), 
                        System.Diagnostics.EventLogEntryType.Error);
                    se = null;
                }
            }
            catch (Exception ex)
            {
                (new BizRules.Logger()).WriteToEventLog(
                    string.Format("To: {0} Subject: {1} Body: {2}",emailToAddr,msgSubject,msgBody), System.Diagnostics.EventLogEntryType.Error);
                throw new SubSystemException("Email", string.Format("To: {0} Subject: {1} Body: {2}", emailToAddr, msgSubject, msgBody), ex);
            }
            //finally { /*msg.Dispose();*/ } 
        }

        /// <summary>
        /// Sends emails using Simple mail transfer protocol
        /// </summary>
        /// <param name="msgBody"></param>
        /// <param name="msgSubject"></param>
        /// <param name="emailToAddr"></param>
        /// <param name="emailFromAddr"></param>
        /// <param name="attachments"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        public void SendEmail(string msgBody, string msgSubject, string emailToAddr, string emailFromAddr, MailAttachment[] attachments, string cc,string bcc)
        {
            SmtpClient smtpClient = new SmtpClient();

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(emailFromAddr);            
            if (emailToAddr.IndexOf(',') != -1)
            {
                string[] EmailList = emailToAddr.Split(',');
                for (int iloop = 0; iloop < EmailList.Length; iloop++)
                {
                    msg.To.Add(new MailAddress(EmailList[iloop]));
                }
            }
            else
            {
                msg.To.Add(new MailAddress(emailToAddr));
            }
            msg.Body = msgBody;
            msg.IsBodyHtml = true;
            msg.BodyEncoding = Encoding.UTF8;
            msg.Subject = msgSubject;
            msg.SubjectEncoding = Encoding.UTF8;
            if (cc != null && cc != string.Empty)
            {
                if (cc.IndexOf(',') != -1)
                {
                    string[] EmailList = cc.Split(',');
                    for (int iloop = 0; iloop < EmailList.Length; iloop++)
                    {
                        msg.CC.Add(new MailAddress(EmailList[iloop]));
                    }
                }
                else
                {
                    msg.CC.Add(new MailAddress(cc));
                }
            }
            if (bcc != null && bcc != string.Empty)
            {
                if (bcc.IndexOf(',') != -1)
                {
                    string[] EmailList = bcc.Split(',');
                    for (int iloop = 0; iloop < EmailList.Length; iloop++)
                    {
                        msg.Bcc.Add(new MailAddress(EmailList[iloop]));
                    }
                }
                else
                {
                    msg.Bcc.Add(new MailAddress(bcc));
                }
            }
            if (attachments != null)
            {
                int count = 1;
                foreach (MailAttachment attachment in attachments)
                {
                    Stream stream = new MemoryStream(attachment.Content);
                    Attachment attach = new Attachment(stream, attachment.FileName);
                    count++;
                    msg.Attachments.Add(attach);
                }
            }

            int retryCounter = 0;
            bool sent = false;

            try
            {
                if (_async)
                {
                    smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                    string userState = emailToAddr + "-" + msgSubject + "-" + msgBody;
                    smtpClient.SendAsync(msg, userState);
                }
                else { smtpClient.Send(msg); }
                sent = true;
            }
            catch (System.Net.Sockets.SocketException se)
            {  //if connection times out retry a max of 3 (default value) times then quit
                (new BizRules.Logger()).WriteToEventLog(
                    string.Format("Socket Error occured. Error Code: {0} Error Message: {1}", se.SocketErrorCode.ToString(), se.Message),
                    System.Diagnostics.EventLogEntryType.Information);

                while (retryCounter < _retryCounter)
                {
                    try
                    {
                        if (_async)
                        {
                            //already defined event handler above in 1st attempt
                            string userState = emailToAddr + "-" + msgSubject + "-" + msgBody;
                            smtpClient.SendAsync(msg, userState);
                        }
                        else { smtpClient.Send(msg); }
                        sent = true;
                    }
                    catch (System.Net.Sockets.SocketException se_retry)
                    {
                        retryCounter += 1;
                        (new BizRules.Logger()).WriteToEventLog(
                            string.Format("Email resend try {0}. Error Code: {1} Error Message: {2}", retryCounter.ToString(),
                            se_retry.SocketErrorCode.ToString(), se_retry.Message), System.Diagnostics.EventLogEntryType.Information);
                        se_retry = null;
                    }
                }

                if (sent == false)
                {
                    (new BizRules.Logger()).WriteToEventLog(
                        string.Format("Email could not be sent. To: {0} Subject: {1} Body: {2}", emailToAddr, msgSubject, msgBody),
                        System.Diagnostics.EventLogEntryType.Error);
                    se = null;
                }
            }
            catch (Exception ex)
            {
                (new BizRules.Logger()).WriteToEventLog(
                    string.Format("To: {0} Subject: {1} Body: {2}", emailToAddr, msgSubject, msgBody), System.Diagnostics.EventLogEntryType.Error);
                throw new SubSystemException("Email", string.Format("To: {0} Subject: {1} Body: {2}", emailToAddr, msgSubject, msgBody), ex);
            }
            //finally { /*msg.Dispose();*/ } 
        }

        /// <summary>
        /// This function handles the event thown while sending the emails
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            String token = (string)e.UserState; // Get the unique identifier for this asynchronous operation.

            if (e.Error != null)
            {
                (new BizRules.Logger()).WriteToEventLog(token, System.Diagnostics.EventLogEntryType.Error);
            }
            else this._retVal = "Email send completed without error";
        }

    }