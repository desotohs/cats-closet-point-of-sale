using System;
using System.Configuration.Install;
using System.ComponentModel;

namespace CatsCloset.Main {
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer {
        public ProjectInstaller() {
            InitializeComponent();
        }
    }
}
