using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace YouCanHarvest;

public class YouCanHarvestSettings : ModSettings
{
    public enum AlertTargetPlant
    {
        All,
        InGrowingArea,
        Custom
    }

    public AlertTargetPlant alertTargetPlant = AlertTargetPlant.All;

    private bool canResolve = true;
    public bool ignoreMarkedPlants = true;
    public List<PlantAlertSettingItem> settingItems = [];

    public List<PlantAlertSettingItem> SettingItems
    {
        get
        {
            ResolveItemDef();
            return settingItems;
        }
    }

    public bool IsAlertTarget(Thing t)
    {
        if (ignoreMarkedPlants && t.Map.designationManager.HasMapDesignationOn(t))
        {
            return false;
        }

        if (alertTargetPlant == AlertTargetPlant.All)
        {
            return true;
        }

        if (alertTargetPlant != AlertTargetPlant.InGrowingArea)
        {
            return settingItems.Exists(item => item.IsAlertTarget(t));
        }

        var zone = t.Map.zoneManager.ZoneAt(t.Position);
        return zone is Zone_Growing;
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref alertTargetPlant, "alertTargetPlant");
        Scribe_Values.Look(ref ignoreMarkedPlants, "ignoreMarkedPlants", true);
        Scribe_Collections.Look(ref settingItems, "settingItems", LookMode.Deep);
    }

    public void ResolveItemDef()
    {
        if (!canResolve)
        {
            return;
        }

        foreach (var item in settingItems)
        {
            item.ResolveDef();
        }

        canResolve = settingItems.All(item => !item.IsAvailable);
    }
}