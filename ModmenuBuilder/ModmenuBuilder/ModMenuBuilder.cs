using Modding;
using Satchel;

namespace ModMenuBuilder;

public class ModMenuBuilder : Mod, ILocalSettings<LocalSettings>, ICustomMenuMod
{
    internal static ModMenuBuilder Instance;

    public static LocalSettings LS { get; set; } = new();
    public void OnLoadLocal(LocalSettings s) => LS = s;
    public LocalSettings OnSaveLocal() => LS;

    public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();

    public override void Initialize()
    {
        Instance ??= this;
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? _) =>
        ModMenu.CreateMenuScreen(modListMenu);


    public bool ToggleButtonInsideMenu { get; }
}