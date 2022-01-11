using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Cli
{

    public enum StatuTypes
    {
        Txt,
        Pdf,
        Cover,
        Catalog,
        Archive,
        Article,
        SQL,
        Other
    }

    public enum CompleteTypes
    {
        Finish,
        Debug,
        SubFunc
    }

    [AttributeUsage(AttributeTargets.Class |AttributeTargets.Constructor |AttributeTargets.Field |AttributeTargets.Method |AttributeTargets.Property,AllowMultiple = true)]
    public class StatusAttribute : Attribute
    {

        private StatuTypes _type;
        public StatuTypes Type { get => _type;  }


        private CompleteTypes _complete;



        private string _example;
        public string Example { get => _example; }
        public CompleteTypes Complete { get => _complete;  }
        public StatusAttribute(StatuTypes type, CompleteTypes complete, string example = "")
        {
            this._type = type;
            this._complete = complete;
            this._example = example;
        }
    }
}
