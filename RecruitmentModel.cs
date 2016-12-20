﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nmfs.Agepro.CoreLib
{
    /// <summary>
    /// Generalized, abstract representation of various AGEPRO Recruitment Models
    /// </summary>
    abstract public class RecruitmentModel : INotifyPropertyChanged
    {
        public int recruitModelNum;
        public int recruitCategory;
        public abstract void ReadRecruitmentModel(StreamReader sr);
        public abstract List<string> WriteRecruitmentDataModelData();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Looks up the name of the AGEPRO Recruitment Model. Unimpmented at this time. 
        /// </summary>
        /// <param name="modelType">Desginated AGEPRO Model Numnber</param>
        /// <returns>String containng the model name</returns>
        public string GetRecruitModelName(int modelType)
        {
            //TODO:Use Dictionary to get Model Name  

            return "";
        }

        //DanRigby 
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }
    }
}
