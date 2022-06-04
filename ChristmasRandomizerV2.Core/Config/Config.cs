using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasRandomizerV2.Core.Serialization
{
    internal class ConfigPerson
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
    }

    internal class ConfigPersonHas
    {
        [JsonProperty(PropertyName = "person")]
        public string Person { get; set; }

        [JsonProperty(PropertyName = "has")]
        public string Has { get; set; }
    }

    internal class ConfigRestriction
    {
        [JsonProperty(PropertyName = "person")]
        public string Person { get; set; }

        [JsonProperty(PropertyName = "cannotHave")]
        public string CannotHave { get; set; }
    }

    internal class ConfigEmail
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "smtpServer")]
        public string SmtpServer { get; set; }

        [JsonProperty(PropertyName = "port")]
        public int Port { get; set; }
    }

    internal class Config
    {
        [JsonProperty(PropertyName = "names")]
        public IList<ConfigPerson> Names { get; set; }

        [JsonProperty(PropertyName = "restrictions")]
        public IList<ConfigRestriction> Restrictions { get; set; }

        [JsonProperty(PropertyName = "email")]
        public ConfigEmail Email { get; set; }
    }

    internal class ConfigMapping
    {
        [JsonProperty(PropertyName = "mapping")]
        public IList<ConfigPersonHas> Mapping { get; set; }
    }
}
