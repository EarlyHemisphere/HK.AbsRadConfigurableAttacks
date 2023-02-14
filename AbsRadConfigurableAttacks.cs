using System.Collections.Generic;
using Modding;
using Satchel.BetterMenus;
using SFCore.Utils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

public class AbsRadConfigurableAttacks : Mod, ICustomMenuMod, ILocalSettings<LocalSettings> {
    private Menu menuRef, firstPhaseMenu, platformPhaseMenu;
    public static AbsRadConfigurableAttacks instance;
    private PlayMakerFSM attackChoicesFSM = null;
    public static Dictionary<string, float> firstPhaseDefaults = new Dictionary<string, float>(){
        { "nailSweepRight", 0.5f },
        { "nailSweepLeft", 0.5f },
        { "nailSweepTop", 0.75f },
        { "eyeBeams", 1f },
        { "beamSweepLeft", 0.75f },
        { "beamSweepRight", 0.5f },
        { "nailFan", 1f },
        { "orbs", 1f },
    };
    public static Dictionary<string, float> platformPhaseDefaults = new Dictionary<string, float>(){
        { "nailSweep", 0.5f },
        { "eyeBeams", 1f },
        { "beamSweepLeft", 0.75f },
        { "beamSweepRight", 0.5f },
        { "nailFan", 1f },
        { "orbs", 1f },
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

    public void Unload() {
        On.PlayMakerFSM.OnEnable -= OnFsmEnable;
    }

    private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
        if (self.FsmName == "Attack Choices") {
            if (attackChoicesFSM == null) {
                attackChoicesFSM = self;
                UpdateWeightsFSM();
                CheckRepititionCap();
            }
        }
    }

    public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

    public bool ToggleButtonInsideMenu => false;

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) {
        firstPhaseMenu ??= new Menu(
            name: "First Phases Attack Weights",
            elements: new Element[] {
                new CustomSlider(
                    name: "Nail Sweep Right",
                    storeValue: val => localSettings.firstPhase["nailSweepRight"] = val,
                    loadValue: () => localSettings.firstPhase["nailSweepRight"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Nail Sweep Left",
                    storeValue: val => localSettings.firstPhase["nailSweepLeft"] = val,
                    loadValue: () => localSettings.firstPhase["nailSweepLeft"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Nail Sweep Top",
                    storeValue: val => localSettings.firstPhase["nailSweepTop"] = val,
                    loadValue: () => localSettings.firstPhase["nailSweepTop"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Beam Burst",
                    storeValue: val => localSettings.firstPhase["eyeBeams"] = val,
                    loadValue: () => localSettings.firstPhase["eyeBeams"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Beam Sweep Left",
                    storeValue: val => localSettings.firstPhase["beamSweepLeft"] = val,
                    loadValue: () => localSettings.firstPhase["beamSweepLeft"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Beam Sweep Right",
                    storeValue: val => localSettings.firstPhase["beamSweepRight"] = val,
                    loadValue: () => localSettings.firstPhase["beamSweepRight"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Sword Burst",
                    storeValue: val => localSettings.firstPhase["nailFan"] = val,
                    loadValue: () => localSettings.firstPhase["nailFan"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Orb Barrage",
                    storeValue: val => localSettings.firstPhase["orbs"] = val,
                    loadValue: () => localSettings.firstPhase["orbs"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                )
            }
        );

        platformPhaseMenu ??= new Menu(
            name: "Platform Phase Attack Weights",
            elements: new Element[] {
                new CustomSlider(
                    name: "Nail Sweep",
                    storeValue: val => localSettings.platformPhase["nailSweep"] = val,
                    loadValue: () => localSettings.platformPhase["nailSweep"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Beam Burst",
                    storeValue: val => localSettings.platformPhase["eyeBeams"] = val,
                    loadValue: () => localSettings.platformPhase["eyeBeams"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Beam Sweep Left",
                    storeValue: val => localSettings.platformPhase["beamSweepLeft"] = val,
                    loadValue: () => localSettings.platformPhase["beamSweepLeft"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Beam Sweep Right",
                    storeValue: val => localSettings.platformPhase["beamSweepRight"] = val,
                    loadValue: () => localSettings.platformPhase["beamSweepRight"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Sword Burst",
                    storeValue: val => localSettings.platformPhase["nailFan"] = val,
                    loadValue: () => localSettings.platformPhase["nailFan"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                ),
                new CustomSlider(
                    name: "Orb Barrage",
                    storeValue: val => localSettings.platformPhase["orbs"] = val,
                    loadValue: () => localSettings.platformPhase["orbs"],
                    minValue: 0,
                    maxValue: 1,
                    wholeNumbers: false
                )
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
                )
            }
        );
        
        return menuRef.GetMenuScreen(modListMenu);
    }

    private void UpdateWeight(float weight, string key, int phase = 1) {
        if (phase == 1) {
            localSettings.firstPhase[key] = weight;
        } else {
            localSettings.platformPhase[key] = weight;
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
            localSettings.platformPhase["eyeBeams"],
            localSettings.platformPhase["beamSweepLeft"],
            localSettings.platformPhase["beamSweepRight"],
            localSettings.platformPhase["nailFan"],
            localSettings.platformPhase["orbs"]
        };
    }

    private bool SettingsAreDefault() {
        foreach (var key in localSettings.firstPhase.Keys) {
            if (!localSettings.firstPhase[key].Equals(firstPhaseDefaults[key])) {
                return false;
            }
        }

        foreach (var key in localSettings.platformPhase.Keys) {
            if (!localSettings.platformPhase[key].Equals(platformPhaseDefaults[key])) {
                return false;
            }
        }

        return true;
    }

    private void CheckRepititionCap() {
        if (SettingsAreDefault()) {
            AddAttackRepititionCap();
        } else {
            RemoveAttackRepititionCap();
        }
    }

    private void RemoveAttackRepititionCap() {
        if (attackChoicesFSM == null) {
            return;
        }

        attackChoicesFSM.GetAction<SendRandomEventV3>("A1 Choice", 1).eventMax = new FsmInt[]{10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000};
        attackChoicesFSM.GetAction<SendRandomEventV3>("A1 Choice", 1).eventMax = new FsmInt[]{10000, 10000, 10000, 10000, 10000, 10000};
    }

    private void AddAttackRepititionCap() {
        if (attackChoicesFSM == null) {
            return;
        }

        attackChoicesFSM.GetAction<SendRandomEventV3>("A1 Choice", 1).eventMax = new FsmInt[]{1, 1, 1, 2, 1, 1, 2, 1};
        attackChoicesFSM.GetAction<SendRandomEventV3>("A1 Choice", 1).eventMax = new FsmInt[]{1, 2, 1, 2, 1, 1};
    }
}

public class LocalSettings {
    public Dictionary<string, float> firstPhase = new Dictionary<string, float>(AbsRadConfigurableAttacks.firstPhaseDefaults);
    public Dictionary<string, float> platformPhase = new Dictionary<string, float>(AbsRadConfigurableAttacks.platformPhaseDefaults);
}