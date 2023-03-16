using System.Collections.Generic;
using Modding;
using Satchel.BetterMenus;
using SFCore.Utils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Linq;

public class AbsRadConfigurableAttacks : Mod, ICustomMenuMod, ILocalSettings<LocalSettings> {
    private Menu menuRef, firstPhaseMenu, platformPhaseMenu = null;
    public static AbsRadConfigurableAttacks instance;
    private PlayMakerFSM attackChoicesFSM = null;
    public static Dictionary<string, float> firstPhaseDefaults = new Dictionary<string, float>() {
        { "nailSweepRight", 0.5f },
        { "nailSweepLeft", 0.5f },
        { "nailSweepTop", 0.75f },
        { "eyeBeams", 1f },
        { "beamSweepLeft", 0.75f },
        { "beamSweepRight", 0.75f },
        { "nailFan", 1f },
        { "orbs", 1f },
    };
    public static Dictionary<string, float> platformPhaseDefaults = new Dictionary<string, float>(){
        { "nailSweep", 0.5f },
        { "eyeBeams", 1f },
        { "beamSweepLeft", 0.75f },
        { "beamSweepRight", 0.75f },
        { "nailFan", 1f },
        { "orbs", 1f },
    };
    
    private static Dictionary<float, int> weightToIndex = new Dictionary<float, int>() {
        { 0f, 0 },
        { 0.25f, 1 },
        { 0.5f, 2 },
        { 0.75f, 3 },
        { 1f, 4 }
    };

    private static Dictionary<int, float> indexToWeight = new Dictionary<int, float>() {
        { 0, 0f },
        { 1, 0.25f },
        { 2, 0.5f },
        { 3, 0.75f },
        { 4, 1f }
    };

    public AbsRadConfigurableAttacks() : base("AbsRad Configurable Attacks") { 
        instance = this;
    }

    public static LocalSettings localSettings { get; private set; } = new();
    public void OnLoadLocal(LocalSettings s) => localSettings = s;
    public LocalSettings OnSaveLocal() => localSettings;

    public override void Initialize() {
        Log("Initializing");

        On.PlayMakerFSM.OnEnable += OnFsmEnable;

        Log("Initialized");
    }

    private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
        orig(self);

        if (self.FsmName == "Attack Choices" && attackChoicesFSM == null) {
            attackChoicesFSM = self;
            UpdateWeightsFSM();
            CheckRepititionCap();
        }
    }

    public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

    public bool ToggleButtonInsideMenu => false;

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) {
        firstPhaseMenu ??= new Menu(
            name: "First Phases Attack Weights",
            elements: new Element[] {
                new MenuButton(
                    name: "Reset To Defaults",
                    description: "Also reinstates normal attack repitition limits",
                    submitAction: _ => ResetFirstPhases()
                ),
                new HorizontalOption(
                    name: "Sword Rain",
                    description: "",
                    applySetting: index => ApplySetting(index, "nailSweepTop"),
                    loadSetting: () => LoadSetting("nailSweepTop"),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "firstPhases1"
                ),
                new HorizontalOption(
                    name: "Nail Sweep Right",
                    description: "",
                    applySetting: index => ApplySetting(index, "nailSweepRight"),
                    loadSetting: () => LoadSetting("nailSweepRight"),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "firstPhases2"
                ),
                new HorizontalOption(
                    name: "Nail Sweep Left",
                    description: "",
                    applySetting: index => ApplySetting(index, "nailSweepLeft"),
                    loadSetting: () => LoadSetting("nailSweepLeft"),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "firstPhases3"
                ),
                new HorizontalOption(
                    name: "Beam Burst",
                    description: "",
                    applySetting: index => ApplySetting(index, "eyeBeams"),
                    loadSetting: () => LoadSetting("eyeBeams"),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "firstPhases4"
                ),
                new HorizontalOption(
                    name: "Beam Sweep Left",
                    description: "",
                    applySetting: index => ApplySetting(index, "beamSweepLeft"),
                    loadSetting: () => LoadSetting("beamSweepLeft"),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "firstPhases5"
                ),
                new HorizontalOption(
                    name: "Beam Sweep Right",
                    description: "",
                    applySetting: index => ApplySetting(index, "beamSweepRight"),
                    loadSetting: () => LoadSetting("beamSweepRight"),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "firstPhases6"
                ),
                new HorizontalOption(
                    name: "Sword Burst",
                    description: "",
                    applySetting: index => ApplySetting(index, "nailFan"),
                    loadSetting: () => LoadSetting("nailFan"),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "firstPhases7"
                ),
                new HorizontalOption(
                    name: "Orb Barrage",
                    description: "",
                    applySetting: index => ApplySetting(index, "orbs"),
                    loadSetting: () => LoadSetting("orbs"),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "firstPhases8"
                ),
            }
        );

        platformPhaseMenu ??= new Menu(
            name: "Platform Phase Attack Weights",
            elements: new Element[] {
                new MenuButton(
                    name: "Reset To Defaults",
                    description: "Also reinstates normal attack repitition limits",
                    submitAction: _ => ResetPlatsPhase()
                ),
                new HorizontalOption(
                    name: "Nail Sweep",
                    description: "",
                    applySetting: index => ApplySetting(index, "nailSweep", 2),
                    loadSetting: () => LoadSetting("nailSweep", 2),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "platformPhase1"
                ),
                new HorizontalOption(
                    name: "Beam Burst",
                    description: "",
                    applySetting: index => ApplySetting(index, "eyeBeams", 2),
                    loadSetting: () => LoadSetting("eyeBeams", 2),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "platformPhase2"
                ),
                new HorizontalOption(
                    name: "Beam Sweep Left",
                    description: "",
                    applySetting: index => ApplySetting(index, "beamSweepLeft", 2),
                    loadSetting: () => LoadSetting("beamSweepLeft", 2),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "platformPhase3"
                ),
                new HorizontalOption(
                    name: "Beam Sweep Right",
                    description: "",
                    applySetting: index => ApplySetting(index, "beamSweepRight", 2),
                    loadSetting: () => LoadSetting("beamSweepRight", 2),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "platformPhase4"
                ),
                new HorizontalOption(
                    name: "Sword Burst",
                    description: "",
                    applySetting: index => ApplySetting(index, "nailFan", 2),
                    loadSetting: () => LoadSetting("nailFan", 2),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "platformPhase5"
                ),
                new HorizontalOption(
                    name: "Orb Barrage",
                    description: "",
                    applySetting: index => ApplySetting(index, "orbs", 2),
                    loadSetting: () => LoadSetting("orbs", 2),
                    values: new[] {"0", "0.25", "0.5", "0.75", "1"},
                    Id: "platformPhase6"
                ),
            }
        );
        
        menuRef ??= new Menu(
            name: "AbsRad Attack Weights",
            elements: new Element[] {
                Blueprints.NavigateToMenu(
                    name: "First Phases",
                    description: "",
                    getScreen: () => firstPhaseMenu.GetMenuScreen(menuRef.menuScreen)
                ),
                Blueprints.NavigateToMenu(
                    name: "Platform Phase",
                    description: "",
                    getScreen: () => platformPhaseMenu.GetMenuScreen(menuRef.menuScreen)
                ),
                new MenuButton(
                    name: "Reset To Defaults",
                    description: "Also reinstates normal attack repitition limits",
                    submitAction: _ => ResetAllPhases()
                )
            }
        );
        
        return menuRef.GetMenuScreen(modListMenu);
    }

    private void ResetFirstPhases() {
        localSettings.firstPhase = new Dictionary<string, float>(firstPhaseDefaults);
        UpdateWeightsFSM();
        RemoveFirstPhasesAttackRepititionCap();
        foreach (var num in Enumerable.Range(0, 8)) {
            HorizontalOption elem = firstPhaseMenu.Find($"firstPhases{num+1}") as HorizontalOption;
            elem.Update();
        }
    }

    private void ResetPlatsPhase() {
        localSettings.platformPhase = new Dictionary<string, float>(platformPhaseDefaults);
        UpdateWeightsFSM();
        RemovePlatsAttackRepititionCap();
        foreach (var num in Enumerable.Range(0, 6)) {
            HorizontalOption elem = platformPhaseMenu.Find($"platformPhase{num+1}") as HorizontalOption;
            elem.Update();
        }
    }

    private void ResetAllPhases() {
        localSettings.firstPhase = new Dictionary<string, float>(firstPhaseDefaults);
        localSettings.platformPhase = new Dictionary<string, float>(platformPhaseDefaults);
        UpdateWeightsFSM();
        RemoveFirstPhasesAttackRepititionCap();
        RemovePlatsAttackRepititionCap();
    }

    private int LoadSetting(string key, int phase = 1) {
        if (phase == 1) {
            return weightToIndex[localSettings.firstPhase[key]];
        }
        return weightToIndex[localSettings.platformPhase[key]];
    }

    private void ApplySetting(int index, string key, int phase = 1) {
        if (phase == 1) {
            localSettings.firstPhase[key] = indexToWeight[index];
        } else {
            localSettings.platformPhase[key] = indexToWeight[index];
        }
        UpdateWeightsFSM();
        CheckRepititionCap();
    }

    private void UpdateWeightsFSM() {
        if (attackChoicesFSM == null) return;

        attackChoicesFSM.GetAction<SendRandomEventV3>("A1 Choice", 1).weights = new FsmFloat[]{
            localSettings.firstPhase["nailSweepRight"],
            localSettings.firstPhase["nailSweepLeft"],
            localSettings.firstPhase["nailSweepTop"],
            localSettings.firstPhase["eyeBeams"],
            localSettings.firstPhase["beamSweepLeft"],
            localSettings.firstPhase["beamSweepRight"],
            localSettings.firstPhase["nailFan"],
            localSettings.firstPhase["orbs"]
        };
        attackChoicesFSM.GetAction<SendRandomEventV3>("A2 Choice", 1).weights = new FsmFloat[]{
            localSettings.platformPhase["nailSweep"],
            localSettings.platformPhase["nailFan"],
            localSettings.platformPhase["orbs"],
            localSettings.platformPhase["eyeBeams"],
            localSettings.platformPhase["beamSweepLeft"],
            localSettings.platformPhase["beamSweepRight"],
        };
    }

    private void UpdateWeightsToDefaultsFSM() {
        if (attackChoicesFSM == null) return;

        attackChoicesFSM.GetAction<SendRandomEventV3>("A1 Choice", 1).weights = new FsmFloat[]{
            firstPhaseDefaults["nailSweepRight"],
            firstPhaseDefaults["nailSweepLeft"],
            firstPhaseDefaults["nailSweepTop"],
            firstPhaseDefaults["eyeBeams"],
            firstPhaseDefaults["beamSweepLeft"],
            firstPhaseDefaults["beamSweepRight"],
            firstPhaseDefaults["nailFan"],
            firstPhaseDefaults["orbs"]
        };
        attackChoicesFSM.GetAction<SendRandomEventV3>("A2 Choice", 1).weights = new FsmFloat[]{
            platformPhaseDefaults["nailSweep"],
            platformPhaseDefaults["nailFan"],
            platformPhaseDefaults["orbs"],
            platformPhaseDefaults["eyeBeams"],
            platformPhaseDefaults["beamSweepLeft"],
            platformPhaseDefaults["beamSweepRight"],
        };
    }

    private bool FirstPhaseSettingsAreDefault() {
        foreach (var key in localSettings.firstPhase.Keys) {
            if (!localSettings.firstPhase[key].Equals(firstPhaseDefaults[key])) {
                return false;
            }
        }
        return true;
    }

    private bool PlatsSettingsAreDefault() {
        foreach (var key in localSettings.platformPhase.Keys) {
            if (!localSettings.platformPhase[key].Equals(platformPhaseDefaults[key])) {
                return false;
            }
        }
        return true;
    }

    private void CheckRepititionCap() {
        if (FirstPhaseSettingsAreDefault()) {
            AddFirstPhasesAttackRepititionCap();
        } else {
            RemoveFirstPhasesAttackRepititionCap();
        }

        if (PlatsSettingsAreDefault()) {
            AddPlatsAttackRepititionCap();
        } else {
            RemovePlatsAttackRepititionCap();
        }
    }

    private void RemoveFirstPhasesAttackRepititionCap() {
        if (attackChoicesFSM == null) return;
        SendRandomEventV3 action = attackChoicesFSM.GetAction<SendRandomEventV3>("A1 Choice", 1);
        action.eventMax = new FsmInt[]{10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000};
        action.missedMax = new FsmInt[]{10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000};
    }

    private void AddFirstPhasesAttackRepititionCap() {
        if (attackChoicesFSM == null) return;
        SendRandomEventV3 action = attackChoicesFSM.GetAction<SendRandomEventV3>("A1 Choice", 1);
        action.eventMax = new FsmInt[]{1, 1, 1, 2, 1, 1, 2, 1};
        action.missedMax = new FsmInt[]{12, 12, 12, 10, 12, 12, 10, 12};
    }

    private void RemovePlatsAttackRepititionCap() {
        if (attackChoicesFSM == null) return;
        SendRandomEventV3 action = attackChoicesFSM.GetAction<SendRandomEventV3>("A2 Choice", 1);
        action.eventMax = new FsmInt[]{10000, 10000, 10000, 10000, 10000, 10000};
        action.missedMax = new FsmInt[]{10000, 10000, 10000, 10000, 10000, 10000};
    }

    private void AddPlatsAttackRepititionCap() {
        if (attackChoicesFSM == null) return;
        SendRandomEventV3 action = attackChoicesFSM.GetAction<SendRandomEventV3>("A2 Choice", 1);
        action.eventMax = new FsmInt[]{1, 2, 1, 2, 1, 1};
        action.missedMax = new FsmInt[]{12, 10, 10, 10, 12, 12};
    }
}

public class LocalSettings {
    public Dictionary<string, float> firstPhase = new Dictionary<string, float>(AbsRadConfigurableAttacks.firstPhaseDefaults);
    public Dictionary<string, float> platformPhase = new Dictionary<string, float>(AbsRadConfigurableAttacks.platformPhaseDefaults);
}