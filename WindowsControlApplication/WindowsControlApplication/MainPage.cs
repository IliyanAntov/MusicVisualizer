using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsControlApplication {
    public partial class MainPage : Form {

        private SerialPort port = new SerialPort("COM5", 38400);

        public MainPage() {
            InitializeComponent();
            InitializeCommunication();
        }

        private void InitializeCommunication() {
            port.Open();
        }

        private void button1_Click(object sender, EventArgs e) {
            port.Write("b");
        }

        private void button2_Click(object sender, EventArgs e) {
            port.Write("e");

        }
    }
}
