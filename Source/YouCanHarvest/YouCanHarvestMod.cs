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
    private static readonly Color alternateBackground = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    public readonly YouCanHarvestSettings settings;
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
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(inRect);
        listing_Standard.ColumnWidth = inRect.width / 2 * 0.95f;

        listing_Standard.CheckboxLabeled("YouCanHarvest.IgnoreMarkedPlants".Translate(),
            ref Settings.ignoreMarkedPlants);

        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("YouCanHarvest.CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.Gap();

        Text.Font = GameFont.Medium;
        listing_Standard.Label("YouCanHarvest.AlertTargetPlant".Translate());
        Text.Font = GameFont.Small;

        var rectRadioButton = listing_Standard.GetRect(30f);
        var isSelected = settings.alertTargetPlant == YouCanHarvestSettings.AlertTargetPlant.All;
        if (Widgets.RadioButtonLabeled(rectRadioButton, "YouCanHarvest.AlertTargetPlantAll".Translate(),
                isSelected))
        {
            settings.alertTargetPlant = YouCanHarvestSettings.AlertTargetPlant.All;
        }

        rectRadioButton = listing_Standard.GetRect(30f);
        isSelected = settings.alertTargetPlant == YouCanHarvestSettings.AlertTargetPlant.InGrowingArea;
        if (Widgets.RadioButtonLabeled(rectRadioButton,
                "YouCanHarvest.AlertTargetPlantInGrowingArea".Translate(), isSelected))
        {
            settings.alertTargetPlant = YouCanHarvestSettings.AlertTargetPlant.InGrowingArea;
        }

        rectRadioButton = listing_Standard.GetRect(30f);
        isSelected = settings.alertTargetPlant == YouCanHarvestSettings.AlertTargetPlant.Custom;
        if (Widgets.RadioButtonLabeled(rectRadioButton, "YouCanHarvest.AlertTargetPlantCustom".Translate(),
                isSelected))
        {
            settings.alertTargetPlant = YouCanHarvestSettings.AlertTargetPlant.Custom;
        }

        if (listing_Standard.ButtonText("YouCanHarvest.Reset".Translate(), widthPct: 0.5f))
        {
            // Reset settings
            settings.alertTargetPlant = YouCanHarvestSettings.AlertTargetPlant.All;
            settings.ignoreMarkedPlants = true;
            settings.settingItems.Clear();
        }

        if (Settings.alertTargetPlant == YouCanHarvestSettings.AlertTargetPlant.Custom)
        {
            listing_Standard.NewColumn();
            listing_Standard.Label("YouCanHarvest.TitlePlantDefs".Translate());
            var buttonRect = listing_Standard.GetRect(30f);
            DrawAddPlantDefButton(buttonRect);

            var outRect = listing_Standard.GetRect(inRect.height - (listing_Standard.CurHeight * 1.05f));
            var width = outRect.width * 0.95f;
            var viewRect = new Rect(0f, 0f, width, CalcHeight());
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            var listing_ScrollView = new Listing_Standard();
            listing_ScrollView.Begin(viewRect);
            DrawPlants(listing_ScrollView);
            listing_ScrollView.End();
            Widgets.EndScrollView();
        }

        listing_Standard.End();
    }

    private float CalcHeight()
    {
        var height = 0f;

        Text.Font = GameFont.Small;
        height += 30f * Settings.SettingItems.Count;

        return height;
    }

    private void DrawAddPlantDefButton(Rect rect)
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
            var list2 = new List<FloatMenuOption> { new FloatMenuOption("YouCanHarvest.NoPlantDef".Translate(), null) };
            Find.WindowStack.Add(new FloatMenu(list2));
        }
    }

    private void DrawPlants(Listing_Standard list)
    {
        settings.ResolveItemDef();

        Text.Font = GameFont.Small;
        string deleteDefName = null;
        for (var i = 0; i < Settings.SettingItems.Count; i++)
        {
            var rect = list.GetRect(24f);
            if (DoPlantRow(rect, i))
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

    private bool DoPlantRow(Rect rect, int index)
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