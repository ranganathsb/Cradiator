﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace Cradiator.Model
{
    /// <summary>
    /// Master data over a certain view
    /// so you could show some statistics or whatever besides just the project names and their states
    /// </summary>
    public class ViewData : INotifyPropertyChanged
    {
        private List<ProjectStatus> _projects = new List<ProjectStatus>();
        private decimal _okPercentage;
        private int _AmountOK;
        private int _AmountNotOK;
        private bool _ShowOnlyBroken;
        private Visibility _ShowProjects;
        private Visibility _ShowAllOK;

        private string _name;

        public ViewData()
        {
        }


        public ViewData(string dataViewName, bool showOnlyBroken)
        {
            _name = dataViewName;
            _ShowOnlyBroken = showOnlyBroken;
        }

        /// <summary>
        /// The name of the view, taken from the config
        /// only og importance when ShowOnlyBroken is true and all projects are ok
        /// in that case the smiley is shown, so you know what view is all ok
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                Notify("Name");
            }
        }


        /// <summary>
        /// Just a list and not a bindinglist, because we just overwrite the entire list,
        /// and are not updating individual projects.
        /// Whenever the list of projects is changed by setting a new value,
        /// the other properties will also be recalculated.
        /// </summary>
        public List<Model.ProjectStatus> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                Notify("Projects");
                CalculateProperties();
            }
        }


        public decimal OKPercentage
        {
            get { return _okPercentage; }
            set
            {
                if (_okPercentage == value) return;
                _okPercentage = value;
                Notify("OKPercentage");
            }
        }

        public int AmountOK
        {
            get { return _AmountOK; }
            set
            {
                if (_AmountOK == value) return;
                _AmountOK = value;
                Notify("AmountOK");
            }
        }

        public int AmountNotOK
        {
            get { return _AmountNotOK; }
            set
            {
                if (_AmountNotOK == value) return;
                _AmountNotOK = value;
                Notify("AmountNotOK");
            }
        }

        /// <summary>
        /// Taken from the config and needed for determining to show the smiley or not
        /// </summary>
        public bool ShowOnlyBroken
        {
            get { return _ShowOnlyBroken; }
            set
            {
                if (_ShowOnlyBroken == value) return;
                _ShowOnlyBroken = value;
                Notify("ShowOnlyBroken");
            }
        }

        /// <summary>
        /// Show the projects viewbox
        /// </summary>
        public Visibility ShowProjects
        {
            get { return _ShowProjects; }
            set
            {
                if (_ShowProjects == value) return;
                _ShowProjects = value;
                Notify("ShowProjects");
            }
        }


        /// <summary>
        /// Show the AllOK viewbox aka smiley
        /// </summary>
        public Visibility ShowAllOK
        {
            get { return _ShowAllOK; }
            set
            {
                if (_ShowAllOK == value) return;
                _ShowAllOK = value;
                Notify("ShowAllOK");
            }
        }


        private void CalculateProperties()
        {
            if (this.Projects == null) this.Projects = new List<Cradiator.Model.ProjectStatus>();

            AmountOK = this.Projects.Count(x => x.IsSuccessful);
            AmountNotOK = this.Projects.Count - AmountOK;

            if (this.Projects.Count == 0)
            {
                OKPercentage = 100;
            }
            else
            {
                OKPercentage = AmountOK / this.Projects.Count;
                OKPercentage *= 100;
                OKPercentage = decimal.Round(OKPercentage, 4);
            }


            //when showonly broken is set to true, and everything is ok, show the OK screen
            if (ShowOnlyBroken && OKPercentage == 100)
            {
                ShowAllOK = Visibility.Visible;
                ShowProjects = Visibility.Collapsed;

            }
            else
            {
                ShowAllOK = Visibility.Collapsed;
                ShowProjects = Visibility.Visible;
            }
        }




        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}