﻿using System;
using System.Security;

namespace Drexel.Configurables.Sample
{
    public class DemoConfigurable
    {
        public const string ExpectedUsername = "ExpectedUsername";
        public const string ExpectedPasswordPlaintext = "ExpectedPassword!123";

        public static SecureString ExpectedPassword = DemoConfigurable.ExpectedPasswordPlaintext.ToSecureString();
        public static Uri ExpectedWebsite = new Uri("https://www.expected.com");

        private readonly Uri website;
        private readonly string username;
        private readonly SecureString password;

        public DemoConfigurable(Uri website, string username, SecureString password)
        {
            this.website = website;
            this.username = username;
            this.password = password;

            this.IsConnected = false;
        }

        public bool IsConnected { get; private set; }

        public void Connect()
        {
            // [~11]. Pretend to connect to a website.
            // We know what we expect the website, username, and password to be. If any of them are wrong,
            // then throw an exception.
            this.IsConnected = false;
            if (this.website != DemoConfigurable.ExpectedWebsite)
            {
                throw new InvalidOperationException($"Failed to connect to website '{this.website}'.");
            }
            else if (this.username != DemoConfigurable.ExpectedUsername
                || !this.password.IsEqual(DemoConfigurable.ExpectedPassword))
            {
                throw new InvalidOperationException($"Unrecognized username/password.");
            }
            else
            {
                this.IsConnected = true;
            }
        }
    }
}
