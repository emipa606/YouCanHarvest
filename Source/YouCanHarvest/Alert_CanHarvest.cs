using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace YouCanHarvest;

public class Alert_CanHarvest : Alert
{
    private readonly List<Thing> cacheHarvestablePlants = [];

    public Alert_CanHarvest()
    {
        defaultLabel = "YouCanHarvest.AlertCanHarvestLabel".Translate();
        defaultPriority = AlertPriority.High;
    }

    private List<Thing> HarvestablePlants
    {
        get
        {
            cacheHarvestablePlants.Clear();
            var maps = Find.Maps;
            foreach (var map in maps)
            {
                if (!map.IsPlayerHome || !map.mapPawns.AnyColonistSpawned)
                {
                    continue;
                }

                cacheHarvestablePlants.AddRange(map.listerThings.ThingsInGroup(ThingRequestGroup.Plant)
                    .Where(Validator));
                continue;

                bool Validator(Thing t)
                {
                    var p = t as Plant;
                    return p is { HarvestableNow: true, LifeStage: PlantLifeStage.Mature } && !p.Fogged() &&
                           YouCanHarvestMod.Settings.IsAlertTarget(p);
                }
            }

            return cacheHarvestablePlants;
        }
    }

    public override TaggedString GetExplanation()
    {
        var stringBuilder = new StringBuilder();
        foreach (var group in HarvestablePlants.GroupBy(t => t.def))
        {
            stringBuilder.AppendLine("YouCanHarvest.AlertCanHarvestItem".Translate(group.Key.LabelCap, group.Count()));
        }

        return "YouCanHarvest.AlertCanHarvestDesc".Translate(stringBuilder.ToString());
    }

    public override AlertReport GetReport()
    {
        return AlertReport.CulpritsAre(HarvestablePlants);
    }
}