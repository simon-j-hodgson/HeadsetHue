using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace HeadsetHue
{

    public partial class Form1 : Form
    {
        static LightStatus lastStatus = new LightStatus();
        static bool pstnUp = false;
        static bool mobileUp = false;
        static bool voipUp = false;

        static CoreAudioDevice device;

        static Form1 form1;

        public Form1()
        {
            form1 = this;
            InitializeComponent();
            device = new CoreAudioController().DefaultCaptureCommunicationsDevice;
        }

        public async Task LightToColor(Color color)
        {
  
            LightStatus led = new LightStatus();

            led.on = true;
            led.sat = (byte) (color.GetSaturation() * 255);
            led.bri = (byte) (color.GetBrightness() * 255);
            led.hue = (ushort) ((color.GetHue() / 360) * 65535);
            led.transitiontime = 0;

            notifyIcon1.BalloonTipText = "Colour: " + color.Name;

            await UpdateLightStatusAsync(led);
        }

        public async Task LightOn()
        {
            lastStatus.on = true;
            await UpdateLightStatusAsync(lastStatus);
        }

        public async Task LightOff()
        {
            lastStatus.on = false;
            await UpdateLightStatusAsync(lastStatus);
        }

        async Task UpdateLightStatusAsync(LightStatus status)
        {

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://192.168.3.12/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await client.PutAsJsonAsync($"/api/o7Wx3vf2pdlbHdmlFNiqlqmJRV5eeISKBeentqMs/lights/34/state", status);
                response.EnsureSuccessStatusCode();
                lastStatus = status;
            }
            catch (Exception ex)
            {
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await LightOff();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            notifyIcon1.Icon = Properties.Resources.headphones_red;
            await LightOn();
        }

       
        private async void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            await LightOff();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (device != null)
            {

                if (device.SessionController.ActiveSessions().Count() > 0)
                {
                    form1.notifyIcon1.Icon = Properties.Resources.headphones_red;
                    form1.LightOn();  
                }
                else
                {
                    form1.notifyIcon1.Icon = Properties.Resources.headphones_white;
                    form1.LightOff();
                }
            }
        }
    }

    public class LightStatus
    {
        public bool on { get; set; }
        public byte sat { get; set; }
        public byte bri { get; set; }
        public UInt16 hue { get; set; }
        public UInt16 transitiontime { get; set; }
    }

}