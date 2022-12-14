using System;

namespace ModMenuBuilder;

//a base attribute class for defining meta data
public abstract class ModMenuElementAttribute : Attribute
{
    public string MenuName;
    public string ElementName;
    public string ElementDesc; //you can remove this if you want

    public ModMenuElementAttribute(string menuName, string elementName, string elementDesc)
    {
        MenuName = menuName;
        ElementName = elementName;
        ElementDesc = elementDesc;
    }
}

public class BoolElementAttribute : ModMenuElementAttribute
{
    public BoolElementAttribute(string menuName, string elementName, string elementDesc) : base(menuName, elementName, elementDesc) { }
}
public class ButtonElementAttribute : ModMenuElementAttribute
{
    public ButtonElementAttribute(string menuName, string elementName, string elementDesc) : base(menuName, elementName, elementDesc) { }
}
public class SliderFloatElementAttribute : ModMenuElementAttribute
{
    public float MinValue;
    public float MaxValue;

    public SliderFloatElementAttribute(string menuName, string elementName, float minValue, float maxValue) :
        base(menuName, elementName, "")
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
}

public class InputFloatElementAttribute : ModMenuElementAttribute
{
    public float MinValue;
    public float MaxValue;
    public float DefaultValue; //when input is empty
    public string PlaceHolder;
    public int CharacterLimit;

    public InputFloatElementAttribute(string menuName, string elementName, float minValue, float maxValue, float defaultValue = 0f, string placeHolder = "", int characterLimit = 7) :
        base(menuName, elementName, "")
    {
        MinValue = minValue;
        MaxValue = maxValue;
        DefaultValue = defaultValue;
        PlaceHolder = placeHolder;
        CharacterLimit = characterLimit;
    }
}
public class SliderIntElementAttribute : ModMenuElementAttribute
{
    public int MinValue;
    public int MaxValue;

    public SliderIntElementAttribute(string menuName, string elementName, int minValue, int maxValue) :
        base(menuName, elementName, "")
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
}