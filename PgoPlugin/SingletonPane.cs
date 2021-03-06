﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;
using System.Windows.Controls;

namespace PgoPlugin
{
    public class SingletonPane<T> where T : new()
    {
        private SingletonPane()
        {

        }

        static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    App.Current.VerifyAccess();
                    _instance = new T();
                }
                return _instance;
            }

        }
    }
}
