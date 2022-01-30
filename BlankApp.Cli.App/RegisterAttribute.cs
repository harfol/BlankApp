using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Cli.App
{
    public enum LocationType
    {
        Desktop = 0x01,
        File = 0x02,
        Folder = 0x04,
        FileAndFolder = 0x08,
        FolderAndEmpty = 0x10
    }

    public enum RegisterType
    {
        Tree,
        Func
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class RegisterAttribute : Attribute
    {



        private string _multilevel = "";
        public string Multilevel
        {
            get { return _multilevel; }
            set { _multilevel = value; }
        }

        private string _icon;

        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        private RegisterType _type = RegisterType.Func;

        public RegisterType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private bool _useNonFuncName = false;
        public bool UseNonFuncName
        {
            get { return _useNonFuncName; }
            set { _useNonFuncName = value; }
        }

        private bool _useProcessIcon = false;

        public bool UseProcessIcon
        {
            get { return _useProcessIcon; }
            set { _useProcessIcon = value; }
        }


        private string _paraments;

        public string Paraments
        {
            get { return _paraments; }
            set { _paraments = value; }
        }

        private string _funcName;
        public string FuncName
        {
            get { return _funcName; }
            set { _funcName = value; }
        }

        private string _binName;

        public string BinName
        {
            get { return _binName; }
            set { _binName = value; }
        }



        private LocationType _location;

        public LocationType Location
        {
            get { return _location; }
            set { _location = value; }
        }

        private bool _isSingleFunc = false;

        public RegisterAttribute() 
        {
            this._binName = Path.GetFileName( typeof(RegisterAttribute).Assembly.Location );
        }
        

        public RegisterAttribute Clone()
        {
            return new RegisterAttribute()
            {
                FuncName = this._funcName,
                Paraments = this._paraments,
                UseProcessIcon = _useNonFuncName,
                UseNonFuncName = _useNonFuncName,
                Type = this._type,
                Icon = this._icon,
                Multilevel = this._multilevel,
                BinName = this._binName,
                Location = this._location,
            };
        }
    }
}
