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
using Interop.Plantronics;

namespace HeadsetHue
{

    public partial class Form1 : Form
    {
        static LightStatus lastStatus = new LightStatus();
        static bool pstnUp = false;
        static bool mobileUp = false;
        static bool voipUp = false;

        static Form1 form1;

        public Form1()
        {
            form1 = this;
            InitPlantonics();
            InitializeComponent();
            LightToColor(Color.WhiteSmoke);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            notifyIcon1.Icon = Properties.Resources.headphones_red;
            await LightToColor(Color.Red);
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
            
            HttpResponseMessage response = await client.PutAsJsonAsync($"/api/o7Wx3vf2pdlbHdmlFNiqlqmJRV5eeISKBeentqMs/lights/1/state", status);

            response.EnsureSuccessStatusCode();

            lastStatus = status;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await LightOff();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            notifyIcon1.Icon = Properties.Resources.headphones_yellow;
            await LightToColor(Color.Goldenrod);
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            notifyIcon1.Icon = Properties.Resources.headphones_white;
            await LightToColor(Color.WhiteSmoke);
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            notifyIcon1.Icon = Properties.Resources.headphones_green;
            await LightToColor(Color.LightGreen);
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            notifyIcon1.Icon = Properties.Resources.headphones_purple;
            await LightToColor(Color.Magenta);

        }

        private async void button7_Click(object sender, EventArgs e)
        {
            await LightToColor(Color.Cyan);
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

        static void InitPlantonics()
        {
            ICOMSessionManager sessionManager = new COMSessionManager();
            COMSession session;
            sessionManager.Register("Headset Hue", out session);
            COMDevice device = session.GetActiveDevice();
            ICOMDeviceEvents_Event deviceEvents = device as ICOMDeviceEvents_Event;
            ICOMBaseEvents_Event baseEvents = device as ICOMBaseEvents_Event;
            baseEvents.onBaseEventReceived += BaseEvents_onBaseEventReceived;
        }

        private static void BaseEvents_onBaseEventReceived(COMBaseEventArgs args)
        {
            switch (args.EventType)
            {
                case BaseEventTypeExt.BaseEventTypeExt_MobileLinkEstablished:
                    mobileUp = true;
                    break;
                case BaseEventTypeExt.BaseEventTypeExt_MobileLinkDown:
                    mobileUp = false;
                    break;
                case BaseEventTypeExt.BaseEventTypeExt_PstnLinkEstablished:
                    pstnUp = true;
                    break;
                case BaseEventTypeExt.BaseEventTypeExt_PstnLinkDown:
                    pstnUp = false;
                    break;
                case BaseEventTypeExt.BaseEventTypeExt_VoipLinkEstablished:
                    voipUp = true;
                    break;
                case BaseEventTypeExt.BaseEventTypeExt_VoipLinkDown:
                    voipUp = false;
                    break;
                default:
                    return;
            }

            UpdateLeds();
        }

        private static void UpdateLeds()
        {
            if (voipUp)
            {
                form1.LightToColor(Color.Red);
            }
            else if (mobileUp | pstnUp)
            {
                form1.LightToColor(Color.Magenta);
            }
            else
            {
                form1.LightToColor(Color.WhiteSmoke);
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