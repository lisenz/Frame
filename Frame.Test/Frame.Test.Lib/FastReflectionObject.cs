using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.Test.Lib
{
    public class FastReflectionObject
    {
        private string _Name;
        private int _Age;
        private bool _IsAuto;
        private decimal _Weight;

        public FastReflectionObject()
        {
            this._Name = string.Empty;
            this._Age = -1;
            this._IsAuto = true;
            this._Weight = 0;
        }

        public string Name
        {
            get { return this._Name; }
            set { this._Name = value; }
        }

        public int Age
        {
            get { return this._Age; }
            set { this._Age = value; }
        }

        public bool IsAuto
        {
            get { return this._IsAuto; }
            set { this._IsAuto = value; }
        }

        public decimal Weight
        {
            get { return this._Weight; }
            set { this._Weight = value; }
        }

        public string TestMethod()
        {
            return "TestMethod";
        }

        public string TestMethod1(string name)
        {
            return name;
        }
    }
}
