using System.Collections.Generic;
using System.Linq;
using Mlie;
using UnityEngine;
using Verse;

namespace YouCanHarvest;

[StaticConstructorOnStartup]
public class YouCanHarvestMod : Mod
{
    private static string currentVersion;
    private static readonly Color alternateBackground = new(0.2f, 0.2f, 0.2f, 0.5f);
    private readonly YouCanHarvestSettings settings;
    private Vector2 scrollPosition;

    public YouCanHarvestMod(ModContentPack content) : base(content)
    {
        settings = GetSettings<YouCanHarvestSettings>();
        settings.ResolveItemDef();
        scrollPosition = new Vector2(0f, 0f);
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public static YouCanHarvestSettings Settings
    {
        get
        {
            var settings = LoadedModManager.GetMod<YouCanHarvestMod>().settings;
            settings.ResolveItemDef();
            return settings;
        }
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(inRect);
        listingStandard.ColumnWidth = inRect.width / 2 * 0.95f;

        listingStandard.CheckboxLabeled("YouCanHarvest.IgnoreMarkedPlants".Translate(),
            ref Settings.IgnoreMarkedPlants);

        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("YouCanHarvest.CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.Gap();

        Text.Font = GameFont.Medium;
        listingStandard.Label("YouCanHarvest.AlertTargetPlant".Translate());
        Text.Font = GameFont.Small;

        var rectRadioButton = listingStandard.GetRect(30f);
        var isSelected = settings.alertTargetPlant == YouCanHarvestSettings.AlertTargetPlant.All;
        if (Widgets.RadioButtonLabeled(rectRadioButton, "YouCanHarvest.AlertTargetPlantAll".Translate(),
                isSelected))
        {
            settings.alertTargetPlant = YouCanHarvestSettings.AlertTargetPlant.All;
        }

        rectRadioButton = listingStandard.GetRect(30f);
        isSelected = settings.alertTargetPlant == YouCanHarvestSettings.AlertTargetPlant.InGrowingArea;
        if (Widgets.RadioButtonLabeled(rectRadioButton,
                "YouCanHarvest.AlertTargetPlantInGrowingArea".Translate(), isSelected))
        {
            settings.alertTargetPlant = YouCanHarvestSettings.AlertTargetPlant.InGrowingArea;
        }

        rectRadioButton = listingStandard.GetRect(30f);
        isSelected = settings.alertTargetPlant == YouCanHarvestSettings.AlertTargetPlant.Custom;
        if (Widgets.RadioButtonLabeled(rectRadioButton, "YouCanHarvest.AlertTargetPlantCustom".Translate(),
                isSelected))
        {
            settings.alertTargetPlant = YouCanHarvestSettings.AlertTargetPlant.Custom;
        }

        if (listingStandard.ButtonText("YouCanHarvest.Reset".Translate(), widthPct: 0.5f))
        {
            // Reset settings
            settings.alertTargetPlant = YouCanHarvestSettings.AlertTargetPlant.All;
            settings.IgnoreMarkedPlants = true;
            settings.settingItems.Clear();
        }

        if (Settings.alertTargetPlant == YouCanHarvestSettings.AlertTargetPlant.Custom)
        {
            listingStandard.NewColumn();
            listingStandard.Label("YouCanHarvest.TitlePlantDefs".Translate());
            var buttonRect = listingStandard.GetRect(30f);
            drawAddPlantDefButton(buttonRect);

            var outRect = listingStandard.GetRect(inRect.height - (listingStandard.CurHeight * 1.05f));
            var width = outRect.width * 0.95f;
            var viewRect = new Rect(0f, 0f, width, calcHeight());
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            var listing_ScrollView = new Listing_Standard();
            listing_ScrollView.Begin(viewRect);
            drawPlants(listing_ScrollView);
            listing_ScrollView.End();
            Widgets.EndScrollView();
        }

        listingStandard.End();
    }

    private static float calcHeight()
    {
        var height = 0f;

        Text.Font = GameFont.Small;
        height += 30f * Settings.SettingItems.Count;

        return height;
    }

    private void drawAddPlantDefButton(Rect rect)
    {
        if (!Widgets.ButtonText(rect, "YouCanHarvest.AddPlantDef".Translate()))
        {
            return;
        }

        var plantDefs = new List<ThingDef>();
        plantDefs.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(thingDef =>
                !settings.settingItems.Exists(item => item.def == thingDef) &&
                thingDef.plant is { Harvestable: true, isStump: false })
            .OrderBy(plantDef => plantDef.label));

        if (!plantDefs.NullOrEmpty())
        {
            var listFloatMenu = new List<FloatMenuOption>();
            foreach (var plantDef in plantDefs)
            {
                listFloatMenu.Add(new FloatMenuOption(plantDef.LabelCap,
                    delegate
                    {
                        settings.settingItems.Add(new PlantAlertSettingItem(plantDef, false));
                        settings.settingItems = settings.settingItems.OrderBy(item => item.Label).ToList();
                    }, plantDef));
            }

            Find.WindowStack.Add(new FloatMenu(listFloatMenu));
        }
        else
        {
            var list2 = new List<FloatMenuOption> { new("YouCanHarvest.NoPlantDef".Translate(), null) };
            Find.WindowStack.Add(new FloatMenu(list2));
        }
    }

    private void drawPlants(Listing_Standard list)
    {
        settings.ResolveItemDef();

        Text.Font = GameFont.Small;
        string deleteDefName = null;
        for (var i = 0; i < Settings.SettingItems.Count; i++)
        {
            var rect = list.GetRect(24f);
            if (doPlantRow(rect, i))
            {
                deleteDefName = Settings.SettingItems[i].defName;
            }

            list.Gap(6f);
        }

        if (deleteDefName != null)
        {
            Settings.settingItems.RemoveAll(item => item.defName == deleteDefName);
        }
    }

    private static bool doPlantRow(Rect rect, int index)
    {
        var returnValue = false;

        var item = Settings.SettingItems[index];
        if (Mouse.IsOver(rect))
        {
            Widgets.DrawHighlight(rect);
        }
        else
        {
            if (index % 2 != 0)
            {
                Widgets.DrawBoxSolid(rect.ExpandedBy(2f), alternateBackground);
            }
        }

        GUI.BeginGroup(rect);

        var widgetRow = new WidgetRow(0f, 0f);
        widgetRow.Icon(item.def.uiIcon);
        var color = GUI.color;
        if (!item.IsAvailable)
        {
            GUI.color = Color.red;
        }

        var rectLabel = widgetRow.Label(item.Label, 260f);
        if (!item.IsAvailable)
        {
            TooltipHandler.TipRegion(rectLabel, "YouCanHarvest.AddedByUnloadMod".Translate());
            GUI.color = color;
        }

        widgetRow.ToggleableIcon(ref item.onlyGrowingZone, ContentFinder<Texture2D>.Get("UI/Buttons/ShowZones"),
            "YouCanHarvest.ToggleOnlyGrowingZone".Translate());
        if (widgetRow.ButtonIcon(ContentFinder<Texture2D>.Get("UI/Buttons/Delete")))
        {
            returnValue = true;
        }

        GUI.EndGroup();

        return returnValue;
    }

    public override string SettingsCategory()
    {
        return "You Can Harvest!";
    }
}