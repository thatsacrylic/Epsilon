using Cosmos.System.Graphics;
using Epsilon.Interface;
using Epsilon.Interface.Components;
using Epsilon.Interface.Components.Text;
using Epsilon.System;
using Epsilon.System.Critical.Processing;
using Epsilon.System.Debug;
using Epsilon.System.Resources;
using System;
using System.Drawing;
using System.IO;

namespace Epsilon.Applications.System
{
    public class Login : Process
    {
        string[] logInfo;
        string ustr, pstr;
        Bitmap b = new Bitmap(Files.RawTEPBanner);
        Box username, password;
        Window win;
        Button ok;

        public override void Start()
        {
            base.Start();

            bool loginFileExists = false;
            try
            {
                loginFileExists = File.Exists(ESystem.LoginInfoPath);
                logInfo = loginFileExists
                    ? File.ReadAllLines(ESystem.LoginInfoPath)
                    : Array.Empty<string>();
            }
            catch (Exception ex)
            {
                Log.Warning("Failed to read login information: " + ex.Message);
                logInfo = Array.Empty<string>();
            }

            if (logInfo.Length >= 2)
            {
                ustr = logInfo[0];
                pstr = logInfo[1];
            }
            else
            {
                ustr = logInfo.Length > 0 ? logInfo[0] : string.Empty;
                pstr = string.Empty;

                if (loginFileExists)
                    Log.Warning("Login information is incomplete. Falling back to guest mode.");
            }
            ESystem.CurrentUser = null;

            win = new();
            win.StartAPI(this, false);

            username = new(wData.Position.X, wData.Position.Y + win.tSize + (int)b.Height,
                256, 24, Color.White, Color.Black, "Username", this
            );
            password = new(wData.Position.X, wData.Position.Y + win.tSize + 24 + (int)b.Height,
                256, 24, Color.White, Color.Black, "Password", this
            ); password.Password = true;
            ok = new(wData.Position.X + wData.Position.Width / 2 - 32,
                wData.Position.Y + win.tSize + 1 + 24 * 2 + (int)b.Height + 2,
                64, 24, GUI.colors.btColor, GUI.colors.bthColor, GUI.colors.btcColor, "Login",
                this, delegate () { CheckLogin(); });

            wData.Position.Width = (int)b.Width;
            wData.Position.X = GUI.width / 2 - wData.Position.Width / 2;
            wData.Position.Height = wData.Position.Y + win.tSize + 1 + 24 * 2 + (int)b.Height + 24 + 4 * 2;
            wData.Position.Y = GUI.height / 2 - wData.Position.Height / 2;
        }

        public void CheckLogin()
        {
            if (username.Content.Trim() == ustr.Trim()
                && password.Content.Trim() == pstr.Trim())
            {
                ESystem.CurrentUser = username.Content.Trim();
                Remove();
                ESystem.LogIn();
            }
            else if (username.Content.Trim() == "Guest"
                || username.Content.Trim() == "")
            {
                Remove();
                Manager.Start(new MessageBox
                {
                    wData = new WindowData
                    {
                        Position = new Rectangle(GUI.width / 2 -
                        (("You are in guest mode, any modification you bring will not be retained.".Length
                        * GUI.dFont.Width) + 16) / 2,
                        GUI.height / 2 - (int)(75 / 2),
                        ("You are in guest mode, any modification you bring will not be retained.".Length
                        * GUI.dFont.Width) + 16, 75),
                        Moveable = true
                    },
                    Name = "Guest Mode",
                    Content = "You are in guest mode, any modification you bring will not be retained.",
                    Special = false,
                    Button = true,
                    Action = delegate () { ESystem.LogIn(true, false); }
                });
            }
            else
            {
                foreach (Process p in Manager.pList)
                    if (p.Name == "Error")
                        return;

                Manager.Start(new MessageBox
                {
                    wData = new WindowData
                    {
                        Position = new Rectangle(GUI.width / 2 -
                        (int)(("The login information given is incorrect.".Length * GUI.dFont.Width + 16) / 2),
                                GUI.height / 2 - (int)(75 / 2) + wData.Position.Height + 32,
                                "The login information given is incorrect.".Length * GUI.dFont.Width + 16, 75),
                        Moveable = true
                    },
                    Name = "Error",
                    Content = "The login information given is incorrect.",
                    Special = false,
                    Button = true
                });
            }
        }

        public override void Run()
        {
            base.Run();
            win.DrawB(this); win.DrawT(this, false);
            GUI.canv.DrawImage(b, wData.Position.X, wData.Position.Y + win.tSize + 1);

            username.X = wData.Position.X;
            username.Y = wData.Position.Y + win.tSize + 1 + (int)b.Height;
            username.Update();

            password.X = wData.Position.X;
            password.Y = wData.Position.Y + win.tSize + 1 + 24 + (int)b.Height;
            password.Update();

            ok.X = wData.Position.X + wData.Position.Width / 2 - 32;
            ok.Y = wData.Position.Y + win.tSize + 1 + 24 * 2 + (int)b.Height + 4;
            ok.Update();
        }
    }
}
