using System;

namespace ModMenuBuilder;

//your gs/ls class doesnt matter whcih it is
public class LocalSettings
{
    [SliderFloatElement("GrubSong Options", "Soul", 0f, 25f)] 
    public int grubsongDamageSoul = 15;
    
    [SliderIntElement("GrubSong Options", "Soul Combo", 0, 10)] 
    public int grubsongDamageSoulCombo = 25;
    
    [SliderIntElement("GrubSong Options", "Soul Weaver", 0, 10)] 
    public int grubsongWeaversongSoul = 3;
    
    [BoolElement("GrubSong Options", "Combo", "")] 
    public bool grubsongComboBool = true;
    
    [SliderFloatElement("StalwartShell Options", "Invul", 0f, 100f)]
    public float stalwartShellInvulnerability = 35f;
    
    [SliderFloatElement("StalwartShell Options", "Recoil", 0f, 25f)]
    public float stalwartShellRecoil = 20f;
    
    [ButtonElement("StalwartShell Options", "Reset Defaults", "")]
    public void Reset()
    {
        stalwartShellRecoil = 20f;
        //no need to call update here. i do it on invoke
    }
}
