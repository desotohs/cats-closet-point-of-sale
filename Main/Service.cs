using System;
using System.ServiceProcess;
using System.Threading;

namespace CatsCloset.Main {
    partial class Service : ServiceBase {
        private Thread ServiceThread;

        public Service() {
            InitializeComponent();
        }

        private void ServiceMain() {
            try {
                Program.RunServer(false);
            } catch ( ThreadInterruptedException ) {
            }
        }

        protected override void OnStart(string[] args) {
            ServiceThread = new Thread(ServiceMain);
            ServiceThread.Start();
        }

        protected override void OnStop() {
            ServiceThread.Interrupt();
            ServiceThread.Join();
        }
    }
}
