using System.Diagnostics;

namespace TheWorld.Services
{
    public class RealMailService : IMailService
    {
        public bool SendMail(string to, string from, string subject, string body)
        {
            Debug.WriteLine($"Sending 'real' mail: To: {to} Subject: {subject}");
            return true;
        }
    }
}