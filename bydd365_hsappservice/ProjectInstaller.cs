﻿using System.ComponentModel;
using System.Configuration;

namespace bydd365_hsappservice
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            var config = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
            string Company = config.AppSettings.Settings["Company"].Value;
            if (config.AppSettings.Settings["ServiceName"] != null)
                this.serviceInstaller1.ServiceName = Company + config.AppSettings.Settings["ServiceName"].Value;
            if (config.AppSettings.Settings["ServiceDisplayName"] != null)
                this.serviceInstaller1.DisplayName = Company + config.AppSettings.Settings["ServiceDisplayName"].Value;
            if (config.AppSettings.Settings["ServiceDescription"] != null)
                this.serviceInstaller1.Description = Company + config.AppSettings.Settings["ServiceDescription"].Value;
        }
    }
}
