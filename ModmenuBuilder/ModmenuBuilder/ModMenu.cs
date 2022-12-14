using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Satchel.BetterMenus;
using UnityEngine;

namespace ModMenuBuilder;

public static class ModMenu
{
    private static Menu MenuRef;
    private static MenuScreen MainMenu;

    public static Dictionary<string, MenuScreen> ExtraMenuScreens;
    
    //gets all the fields that have our custom atribute (that also means if you dont add the attribute, it wont be in the menu
    public static FieldInfo[] GSFields = typeof(LocalSettings).GetFields().Where(f => f.GetCustomAttribute<ModMenuElementAttribute>() != null).ToArray();
    public static MethodInfo[] GSMethods = typeof(LocalSettings).GetMethods().Where(f => f.GetCustomAttribute<ModMenuElementAttribute>() != null).ToArray();

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
        Dictionary<string, List<MemberInfo>> menuScreenNameList = new Dictionary<string, List<MemberInfo>>();

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
                menuScreenNameList[menuName] = new List<MemberInfo>() { field };
            }
        }
        foreach (var method in GSMethods)
        {
            //garunteed to be there from the where above
            var menuName = method.GetCustomAttribute<ModMenuElementAttribute>().MenuName;

            if (menuScreenNameList.TryGetValue(menuName, out var list))
            {
                list.Add(method);
            }
            else
            {
                menuScreenNameList[menuName] = new List<MemberInfo>() { method };
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

        foreach (var (menuScreenName, members) in menuScreenNameList)
        {
            var extraMenu = new Menu(menuScreenName);

            foreach (var member in members)
            {
                if (member is FieldInfo field)
                {
                    if (field.FieldType == typeof(float))
                    {
                        var info = field.GetCustomAttribute<ModMenuElementAttribute>();
                        if (info is SliderFloatElementAttribute sliderInfo)
                        {
                            extraMenu.AddElement(new CustomSlider(
                                sliderInfo.ElementName,
                                f => { field.SetValue(Settings, f); },
                                () => (float)field.GetValue(Settings),
                                sliderInfo.MinValue,
                                sliderInfo.MaxValue,
                                false));
                        }
                        else if (info is InputFloatElementAttribute inputInfo)
                        {
                            extraMenu.AddElement(Blueprints.FloatInputField(
                                inputInfo.ElementName,
                                f =>
                                {
                                    f = Mathf.Clamp(f, inputInfo.MinValue, inputInfo.MaxValue);
                                    field.SetValue(Settings, f);
                                    var elem = ((InputField)extraMenu?.Find(inputInfo.ElementName));
                                    if (elem != null)
                                    {
                                        elem.userInput = f.ToString();
                                    }
                                },
                                () => (float)field.GetValue(Settings),
                                inputInfo.DefaultValue,
                                inputInfo.PlaceHolder,
                                inputInfo.CharacterLimit,
                                Id: inputInfo.ElementName
                            ));
                        }
                        else
                        {
                            Modding.Logger.LogError($"Wrong attribute assigned to {field.Name}");
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
                                f => { field.SetValue(Settings, (int)f); },
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
                else if (member is MethodInfo method)
                {
                    var info = method.GetCustomAttribute<ButtonElementAttribute>();
                    if (info == null)
                    {
                        Modding.Logger.LogError($"Wrong attribute assigned to {method.Name}");
                    }
                    else
                    {
                        extraMenu.AddElement(new MenuButton(
                            info.ElementName,
                            info.ElementDesc,
                            _ =>
                            {
                                method.Invoke(Settings, null);
                                extraMenu.Update();
                            }));
                    }
                }
            }

            ExtraMenuScreens[menuScreenName] = extraMenu.GetMenuScreen(MainMenu);
        }

        return MainMenu;
    }
}


