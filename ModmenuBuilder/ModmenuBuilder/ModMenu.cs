using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Satchel.BetterMenus;

namespace ModMenuBuilder;

public static class ModMenu
{
    private static Menu MenuRef;
    private static MenuScreen MainMenu;

    public static Dictionary<string, MenuScreen> ExtraMenuScreens;
    
    //gets all the fields that have our custom atribute (that also means if you dont add the attribute, it wont be in the menu
    public static FieldInfo[] GSFields = typeof(LocalSettings).GetFields().Where(f => f.GetCustomAttribute<ModMenuElementAttribute>() != null).ToArray();

    //make it use your mods settings
    public static LocalSettings Settings => ModMenuBuilder.LS;

    public static MenuScreen CreateMenuScreen(MenuScreen modListMenu)
    {
        //create a dict for for the buttons in main menu to use to get to sub menus
        ExtraMenuScreens = new Dictionary<string, MenuScreen>();
        
        //create the main menu
        MenuRef = new Menu("ModmenuBuilder", new Element[]
        {
            //you can add any elements you want here that will appear before the menu buttons
        });

        // key: menu name, value: list of fields 
        Dictionary<string, List<FieldInfo>> menuScreenNameList = new Dictionary<string, List<FieldInfo>>();

        //im not too sure about the order on which they are so im gonna loop through myself
        foreach (var field in GSFields)
        {
            //garunteed to be there from the where above
            var menuName = field.GetCustomAttribute<ModMenuElementAttribute>().MenuName;

            if (menuScreenNameList.TryGetValue(menuName, out var list))
            {
                list.Add(field);
            }
            else
            {
                menuScreenNameList[menuName] = new List<FieldInfo>() { field };
            }
        }

        //we first create the buttons in main menu
        foreach (var menuScreenName in menuScreenNameList.Keys)
        {
            MenuRef.AddElement(Blueprints.NavigateToMenu(menuScreenName, 
                "", //you can add desc as a parameter in the atribute if you want
                () => ExtraMenuScreens[menuScreenName]));
        }

        //we create the menu screen
        MainMenu = MenuRef.GetMenuScreen(modListMenu);

        foreach (var (menuScreenName, fields) in menuScreenNameList)
        {
            var extraMenu = new Menu(menuScreenName);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(float))
                {
                    var info = field.GetCustomAttribute<SliderFloatElementAttribute>();
                    if (info == null)
                    {
                        Modding.Logger.LogError($"Wrong attribute assigned to {field.Name}");
                    }
                    else
                    {
                        extraMenu.AddElement(new CustomSlider(
                            info.ElementName,
                    f => { field.SetValue(Settings, f); },
                    () => (float)field.GetValue(Settings),
                            info.MinValue,
                            info.MaxValue, 
                            false));
                    }
                }
                else if (field.FieldType == typeof(int))
                {
                    var info = field.GetCustomAttribute<SliderIntElementAttribute>();
                    if (info == null)
                    {
                        Modding.Logger.LogError($"Wrong attribute assigned to {field.Name}");
                    }
                    else
                    {
                        extraMenu.AddElement(new CustomSlider(
                            info.ElementName,
                    f => { field.SetValue(Settings, (int)f);},
                    () => (int)field.GetValue(Settings),
                            info.MinValue,
                            info.MaxValue, 
                            true));
                    }
                }
                else if (field.FieldType == typeof(bool))
                {
                    var info = field.GetCustomAttribute<BoolElementAttribute>();
                    if (info == null)
                    {
                        Modding.Logger.LogError($"Wrong attribute assigned to {field.Name}");
                    }
                    else
                    {
                        extraMenu.AddElement(Blueprints.HorizontalBoolOption(info.ElementName,
                            info.ElementDesc,
                            (b) => field.SetValue(Settings, b),
                            () => (bool)field.GetValue(Settings)));
                    }
                }
            }

            ExtraMenuScreens[menuScreenName] = extraMenu.GetMenuScreen(MainMenu);
        }

        return MainMenu;
    }
}


