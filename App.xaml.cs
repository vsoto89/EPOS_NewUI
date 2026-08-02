using System.Windows;
using EPOS_NewUI.Data;

namespace EPOS_NewUI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        SqliteDatabase.Initialize();
    }
}

