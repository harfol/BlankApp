using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Configuration.Models
{
    public class ProjectSettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("projectCollection", IsDefaultCollection = true)]
        public ProjectElementCollection ProjectCollection
        {
            get { return (ProjectElementCollection)this["projectCollection"]; }
            set { this["projectCollection"] = value; }
        }
    }
}
