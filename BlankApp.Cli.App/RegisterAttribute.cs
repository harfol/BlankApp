using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Cli.App
{


    public enum RegisterType
    {
        Tree,
        Func
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class RegisterAttribute : Attribute
    {
        private string _multilevel;
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

        private RegisterType _type;

        public RegisterType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private bool _useNonFuncName;
        public bool UseNonFuncName
        {
            get { return _useNonFuncName; }
            set { _useNonFuncName = value; }
        }

        private bool _useProcessIcon;

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


        public RegisterAttribute(string multilevel, string paraments = "", string icon=null, RegisterType type=RegisterType.Func,  
            bool useNonFuncName =false, bool useProcessIcon =false, string funcName=null, string binName=null)
        {
            this._multilevel = multilevel;
            this._icon = icon;
            this._type = type;
            this._useNonFuncName = useNonFuncName;
            this._paraments = paraments;
            this._useProcessIcon = useProcessIcon;
            this._funcName = funcName;
            this._binName = binName;
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
                BinName = this._binName
            };
        }
        public RegisterAttribute()
        {

        }
    }
}
