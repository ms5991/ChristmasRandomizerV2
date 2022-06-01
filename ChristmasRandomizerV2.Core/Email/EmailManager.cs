using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ChristmasRandomizerV2.Core.Email
{
    internal class EmailManager : IDisposable
    {
        private const string SUBJECTFORMAT = "Your Christmas {0} Stocking Assignment";
        private const string BODYFORMAT = "Hello {0}, \nYou have been assigned {1} for your stocking this year. Enjoy! \n-Stocking assigner";

        private string _fromAddress;

        private SmtpClient _smtpClient;

        private bool disposedValue;

        public EmailManager(string fromAddress, string password)
        {
            this._fromAddress = fromAddress;

            this._smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(this._fromAddress, password),
                EnableSsl = true,
            };
        }

        public void Notify(Person toNotify, Person has)
        {
            string subject = string.Format(SUBJECTFORMAT, DateTime.UtcNow.Year);

            string body = string.Format(BODYFORMAT, toNotify.Name, has.Name);

            this._smtpClient.Send(this._fromAddress, toNotify.EmailAddress, subject, body);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._smtpClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EmailManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
