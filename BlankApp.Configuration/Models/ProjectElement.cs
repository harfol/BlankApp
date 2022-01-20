using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Configuration.Models
{
    public class ProjectElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("describe", DefaultValue = "", IsRequired = true)]
        public string Describe
        {
            get { return (string)this["describe"]; }
            set { this["describe"] = value; }
        }

        [ConfigurationProperty("remote", DefaultValue = "", IsRequired = true)]
        public string Remote
        {
            get { return (string)this["remote"]; }
            set { this["remote"] = value; }
        }
    }
}
