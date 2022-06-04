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

        public EmailManager(
            string fromAddress,
            string password,
            string smtpServer,
            int port)
        {
            this._fromAddress = fromAddress;

            this._smtpClient = new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(this._fromAddress, password),
                EnableSsl = true,
            };
        }

        /// <summary>
        /// Send email to the toNotify Person
        /// indicating that they have been assigned
        /// the has Person
        /// </summary>
        /// <param name="toNotify"></param>
        /// <param name="has"></param>
        public void Notify(Person toNotify, Person has)
        {
            // format the subject line for the email
            string subject = string.Format(SUBJECTFORMAT, DateTime.UtcNow.Year);

            // format the body of the email
            string body = string.Format(BODYFORMAT, toNotify.Name, has.Name);

            // send the email
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
