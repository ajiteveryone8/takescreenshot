// Decompiled with JetBrains decompiler
// Type: TakeScreenShot.Program
// Assembly: TakeScreenShot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E5A75721-56D5-4985-9077-A2998E44E1D2
// Assembly location: C:\Users\NEUROEQUILIBRIUM\Desktop\Take Screen Shot\TakeScreenShot.exe

using System;
using System.Windows.Forms;

namespace TakeScreenShot
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new ScreenshotApp());
    }
  }
}
