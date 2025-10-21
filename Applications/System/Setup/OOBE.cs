using Cosmos.System.Graphics;
using Epsilon.Interface;
using Epsilon.Interface.Components;
using Epsilon.Interface.Components.Text;
using Epsilon.System;
using Epsilon.System.Critical.Processing;
using Epsilon.System.Resources;
using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Epsilon.Applications.System.Setup
{
    // Out of Box Experience
    public class OOBE : Process
    {
        Window win;
        Box username, password, c_password;
        Bitmap b = new Bitmap(Files.RawTEPBanner);
        Button ok;

        public override void Start()
        {
            base.Start();
            win = new();
            win.StartAPI(this, false);

            username = new(wData.Position.X, wData.Position.Y + win.tSize + (int)b.Height,
                256, 24, Color.White, Color.Black, "Username", this
            );
            password = new(wData.Position.X, wData.Position.Y + win.tSize + 24 + (int)b.Height,
                256, 24, Color.White, Color.Black, "Password", this
            ); password.Password = true;
            c_password = new(wData.Position.X, wData.Position.Y + win.tSize + 24*2 + (int)b.Height,
                256, 24, Color.White, Color.Black, "Confirm Password", this
            ); c_password.Password = true;

            ok = new(wData.Position.X + wData.Position.Width / 2 - 32,
                wData.Position.Y + win.tSize + 1 + 24 * 3 + (int)b.Height + 2,
                64, 24, GUI.colors.btColor, GUI.colors.bthColor, GUI.colors.btcColor, "OK",
                this, delegate ()
                {
                    try
                    {
                        if (username.Content.Trim() != "Guest")
                        {
                            string[] lines = new string[2];
                            lines[0] = username.Content.Trim();
                            lines[1] = password.Content.Trim();
                            File.WriteAllLines(ESystem.LoginInfoPath,
                                lines);
                            ESystem.CurrentUser = lines[0];
                            Remove();
                            ESystem.LogIn();
                        } else
                            Manager.Start(new MessageBox
                            {
                                wData = new WindowData
                                {
                                    Position = new Rectangle(GUI.width / 2 -
                                (int)((("Please choose another username.").Length * GUI.dFont.Width + 16) / 2),
                                GUI.height / 2 - (int)(75 / 2) + wData.Position.Height + 32,
                                ("Please choose another username.").Length * GUI.dFont.Width + 16, 75),
                                    Moveable = true
                                },
                                Name = "Error",
                                Content = "Please choose another username.",
                                Special = false,
                                Button = true
                            });
                    }
                    catch (Exception ex)
                    {
                        foreach (Process p in Manager.pList)
                            if (p.Name == "Error")
                                return;

                        Manager.Start(new MessageBox
                        {
                            wData = new WindowData
                            {
                                Position = new Rectangle(GUI.width / 2 -
                                (int)((("Failed to create user information: " + ex).Length * GUI.dFont.Width + 16) / 2),
                                GUI.height / 2 - (int)(75 / 2) + wData.Position.Height + 32, 
                                ("Failed to create user information: " + ex).Length * GUI.dFont.Width + 16, 75),
                                Moveable = true
                            },
                            Name = "Error",
                            Content = "Failed to create user information: " + ex,
                            Special = false,
                            Button = true
                        });
                    }
                });

            wData.Position.Width = (int)b.Width;
            wData.Position.Height = wData.Position.Y + win.tSize + 1 + 24 * 3 + (int)b.Height + 24 + 4 * 2;
            wData.Position.X = GUI.width / 2 - wData.Position.Width / 2;
            wData.Position.Y = GUI.height / 2 - wData.Position.Height / 2;
        }

        public override void Run()
        {
            win.DrawB(this); win.DrawT(this, false);
            GUI.canv.DrawImage(b, wData.Position.X, wData.Position.Y + win.tSize+1);

            username.X = wData.Position.X;
            username.Y = wData.Position.Y + win.tSize+1 + (int)b.Height;
            username.Update();

            password.X = wData.Position.X;
            password.Y = wData.Position.Y + win.tSize+1 + 24 + (int)b.Height;
            password.Update();

            c_password.X = wData.Position.X;
            c_password.Y = wData.Position.Y + win.tSize+1 + 24*2 + (int)b.Height;
            c_password.Update();

            ok.X = wData.Position.X + wData.Position.Width / 2 - 32;
            ok.Y = wData.Position.Y + win.tSize + 1 + 24 * 3 + (int)b.Height + 4;

            if (c_password.Content != password.Content)
                GUI.canv.DrawString("Passwords do not match.", GUI.dFont,
                    Color.LightGray, wData.Position.X + 2,
                    wData.Position.Y + win.tSize + 1 + 24 * 3 + (int)b.Height + 2);
            else if (username.Content == "")
                GUI.canv.DrawString("Username field must be filled.", GUI.dFont,
                    Color.LightGray, wData.Position.X + 2,
                    wData.Position.Y + win.tSize + 1 + 24 * 3 + (int)b.Height + 2);
            else ok.Update();
        }
    }
}
