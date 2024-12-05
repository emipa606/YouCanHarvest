using RimWorld;
using Verse;

namespace YouCanHarvest;

public class PlantAlertSettingItem : IExposable
{
    public ThingDef def;
    public string defName;
    public bool onlyGrowingZone;

    public PlantAlertSettingItem()
    {
    }

    public PlantAlertSettingItem(ThingDef thingDef, bool growingZone)
    {
        def = thingDef;
        defName = thingDef.defName;
        onlyGrowingZone = growingZone;
    }

    public string Label => def != null ? def.LabelCap.ToString() : defName;

    public bool IsAvailable => def != null;

    public void ExposeData()
    {
        if (Scribe.mode == LoadSaveMode.Saving && def != null)
        {
            defName = def.defName;
        }

        Scribe_Values.Look(ref defName, "defName");
        Scribe_Values.Look(ref onlyGrowingZone, "onlyGrowingZone");
    }

    public bool IsAlertTarget(Thing t)
    {
        if (def != t.def)
        {
            return false;
        }

        if (!onlyGrowingZone)
        {
            return true;
        }

        var zone = t.Map.zoneManager.ZoneAt(t.Position);
        return zone is Zone_Growing;
    }

    public void ResolveDef()
    {
        def = DefDatabase<ThingDef>.GetNamed(defName, false);
    }
}