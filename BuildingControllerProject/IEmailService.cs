namespace BuildingControllerProject
{
    internal interface IEmailService
    {
        /// <summary>
        /// Sends an email to the given emailAddress.
        /// Uses given subject as email subject.
        /// Uses given message as email message.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        public void SendMail(string emailAddress, string subject, string message);
    }
}
