using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ModCommon.Util;
using SFCore;
using AnyRadiance;
using UnityEngine;

internal class Abs : MonoBehaviour
{
	private int HP = 2000;

	private GameObject _spikeMaster;

	private GameObject _spikeTemplate;

	private GameObject _spikeClone;

	private GameObject _spikeClone2;

	private GameObject _spikeClone3;

	private GameObject _spikeClone4;

	private GameObject _spikeClone5;

	private GameObject _beamsweeper;

	private GameObject _beamsweeper2;

	private GameObject _knight;

	private HealthManager _hm;

	private PlayMakerFSM _attackChoices;

	private PlayMakerFSM _attackCommands;

	private PlayMakerFSM _control;

	private PlayMakerFSM _phaseControl;

	private PlayMakerFSM _spikeMasterControl;

	private PlayMakerFSM _beamsweepercontrol;

	private PlayMakerFSM _beamsweeper2control;

	private PlayMakerFSM _spellControl;

	private int CWRepeats = 0;

	private bool fullSpikesSet = false;

	private bool disableBeamSet = false;

	private bool arena2Set = false;

	private bool onePlatSet = false;

	private bool platSpikesSet = false;

	private const int fullSpikesHealth = 750;

	private const int onePlatHealth = 500;

	private const int platSpikesHealth = 500;

	private const float nailWallDelay = 0.8f;

	private void Awake()
	{
		Log("Added AbsRad MonoBehaviour");
		_hm = base.gameObject.GetComponent<HealthManager>();
		_attackChoices = base.gameObject.LocateMyFSM("Attack Choices");
		_attackCommands = base.gameObject.LocateMyFSM("Attack Commands");
		_control = base.gameObject.LocateMyFSM("Control");
		_phaseControl = base.gameObject.LocateMyFSM("Phase Control");
		_spikeMaster = GameObject.Find("Spike Control");
		_spikeMasterControl = _spikeMaster.LocateMyFSM("Control");
		_spikeTemplate = GameObject.Find("Radiant Spike");
		_beamsweeper = GameObject.Find("Beam Sweeper");
		_beamsweeper2 = Object.Instantiate(_beamsweeper);
		_beamsweeper2.AddComponent<BeamSweeperClone>();
		_beamsweepercontrol = _beamsweeper.LocateMyFSM("Control");
		_beamsweeper2control = _beamsweeper2.LocateMyFSM("Control");
		_knight = GameObject.Find("Knight");
		_spellControl = _knight.LocateMyFSM("Spell Control");
	}

	private void Start()
	{
		Log("Changing fight variables...");
		_hm.hp += HP;
		_phaseControl.FsmVariables.GetFsmInt("P2 Spike Waves").Value += 1750;
		_phaseControl.FsmVariables.GetFsmInt("P3 A1 Rage").Value += 1000;
		_phaseControl.FsmVariables.GetFsmInt("P4 Stun1").Value += 1000;
		_phaseControl.FsmVariables.GetFsmInt("P5 Acend").Value += 1000;
		FsmutilExt.GetAction<SetHP>(_control, "Scream", 7).hp = 2000;
		_spikeClone = Object.Instantiate(_spikeTemplate);
		_spikeClone.transform.SetPositionX(58f);
		_spikeClone.transform.SetPositionY(153.8f);
		_spikeClone2 = Object.Instantiate(_spikeTemplate);
		_spikeClone2.transform.SetPositionX(57.5f);
		_spikeClone2.transform.SetPositionY(153.8f);
		_spikeClone3 = Object.Instantiate(_spikeTemplate);
		_spikeClone3.transform.SetPositionX(57f);
		_spikeClone3.transform.SetPositionY(153.8f);
		_spikeClone4 = Object.Instantiate(_spikeTemplate);
		_spikeClone4.transform.SetPositionX(58.5f);
		_spikeClone4.transform.SetPositionY(153.8f);
		_spikeClone5 = Object.Instantiate(_spikeTemplate);
		_spikeClone5.transform.SetPositionX(59f);
		_spikeClone5.transform.SetPositionY(153.8f);
		_spikeClone.LocateMyFSM("Control").SendEvent("DOWN");
		_spikeClone2.LocateMyFSM("Control").SendEvent("DOWN");
		_spikeClone3.LocateMyFSM("Control").SendEvent("DOWN");
		_spikeClone4.LocateMyFSM("Control").SendEvent("DOWN");
		_spikeClone5.LocateMyFSM("Control").SendEvent("DOWN");
		FsmutilExt.GetAction<Wait>(_attackCommands, "Orb Antic", 0).time = 0.1f;
		FsmutilExt.GetAction<SetIntValue>(_attackCommands, "Orb Antic", 1).intValue = 12;
		FsmutilExt.GetAction<RandomInt>(_attackCommands, "Orb Antic", 2).min = 10;
		FsmutilExt.GetAction<RandomInt>(_attackCommands, "Orb Antic", 2).max = 14;
		FsmutilExt.GetAction<Wait>(_attackCommands, "Orb Summon", 2).time = 0.1f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "Orb Pause", 0).time = 0.1f;
		FsmutilExt.GetAction<Wait>(_attackChoices, "Orb Recover", 0).time = 0.5f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "CW Repeat", 0).time = -0.5f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "CCW Repeat", 0).time = -0.5f;
		FsmutilExt.GetAction<FloatAdd>(_attackCommands, "CW Restart", 2).add = -10f;
		FsmutilExt.GetAction<FloatAdd>(_attackCommands, "CCW Restart", 2).add = 10f;
		FsmutilExt.RemoveAction(_attackCommands, "CW Restart", 1);
		FsmutilExt.RemoveAction(_attackCommands, "CCW Restart", 1);
		FsmutilExt.GetAction<Wait>(_attackChoices, "Beam Sweep L", 0).time = 0.5f;
		FsmutilExt.GetAction<Wait>(_attackChoices, "Beam Sweep R", 0).time = 0.5f;
		FsmutilExt.ChangeTransition(_attackChoices, "A1 Choice", "BEAM SWEEP R", "Beam Sweep L");
		FsmutilExt.ChangeTransition(_attackChoices, "A2 Choice", "BEAM SWEEP R", "Beam Sweep L 2");
		FsmutilExt.GetAction<Wait>(_attackChoices, "Beam Sweep L 2", 0).time = 5.05f;
		FsmutilExt.GetAction<Wait>(_attackChoices, "Beam Sweep R 2", 0).time = 5.05f;
		FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Beam Sweep L 2", 1).sendEvent = "BEAM SWEEP L";
		FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Beam Sweep R 2", 1).sendEvent = "BEAM SWEEP R";
		FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 1", 9).delay = 0.3f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "EB 1", 10).time = 0.5f;
		FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 2", 9).delay = 0.3f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "EB 2", 10).time = 0.5f;
		FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 3", 9).delay = 0.3f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "EB 3", 10).time = 0.5f;
		FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 4", 4).delay = 0.3f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "EB 4", 5).time = 0.5f;
		FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 5", 5).delay = 0.3f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "EB 5", 6).time = 0.5f;
		FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 6", 5).delay = 0.3f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "EB 6", 6).time = 0.5f;
		FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 7", 8).delay = 0.3f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "EB 7", 9).time = 0.5f;
		FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 8", 8).delay = 0.3f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "EB 8", 9).time = 0.5f;
		FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 9", 8).delay = 0.3f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "EB 9", 9).time = 0.5f;
		FsmutilExt.GetAction<SendEventByName>(_attackCommands, "Aim", 10).delay = 1f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "Aim", 11).time = 0.5f;
		FsmutilExt.GetAction<Wait>(_attackCommands, "Eb Extra Wait", 0).time = 0.05f;
		FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Nail Top Sweep", 1).delay = 0.35f;
		FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Nail Top Sweep", 2).delay = 0.7f;
		FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Nail Top Sweep", 3).delay = 1.05f;
		FsmutilExt.GetAction<Wait>(_attackChoices, "Nail Top Sweep", 4).time = 2.3f;
		FsmutilExt.GetAction<Wait>(_control, "Rage Comb", 0).time = 0.35f;
		FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Nail L Sweep", 1).delay = 0.25f;
		FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Nail L Sweep", 1).delay = 1.85f;
		FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Nail L Sweep", 2).delay = 3.45f;
		FsmutilExt.GetAction<Wait>(_attackChoices, "Nail L Sweep", 3).time = 5f;
		FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Nail R Sweep", 1).delay = 0.25f;
		FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Nail R Sweep", 1).delay = 1.85f;
		FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Nail R Sweep", 2).delay = 3.45f;
		FsmutilExt.GetAction<Wait>(_attackChoices, "Nail R Sweep", 3).time = 5f;
		AddNailWall("Nail L Sweep", "COMB R", 1.3f, 1);
		AddNailWall("Nail R Sweep", "COMB L", 1.3f, 1);
		AddNailWall("Nail L Sweep", "COMB R", 2.9f, 1);
		AddNailWall("Nail R Sweep", "COMB L", 2.9f, 1);
		AddNailWall("Nail L Sweep 2", "COMB R2", 1f, 1);
		AddNailWall("Nail R Sweep 2", "COMB L2", 1f, 1);
        SFCore.Utils.FsmUtil.ChangeTransition(_attackChoices, "A1 Choice", "NAIL TOP SWEEP", "Nail L Sweep");
        SFCore.Utils.FsmUtil.ChangeTransition(_attackChoices, "A1 Choice", "EYE BEAMS", "Nail R Sweep");
        SFCore.Utils.FsmUtil.ChangeTransition(_attackChoices, "A1 Choice", "BEAM SWEEP L", "Nail L Sweep");
        SFCore.Utils.FsmUtil.ChangeTransition(_attackChoices, "A1 Choice", "BEAM SWEEP R", "Nail R Sweep");
        SFCore.Utils.FsmUtil.ChangeTransition(_attackChoices, "A1 Choice", "ORBS", "Nail L Sweep");
        SFCore.Utils.FsmUtil.ChangeTransition(_attackChoices, "A1 Choice", "NAIL FAN", "Nail L Sweep");
		Log("fin.");
	}

	private void Update()
	{
		if (_attackCommands.FsmVariables.GetFsmBool("Repeated").Value)
		{
			switch (CWRepeats)
			{
			case 1:
				CWRepeats = 2;
				break;
			case 0:
				CWRepeats = 1;
				_attackCommands.FsmVariables.GetFsmBool("Repeated").Value = false;
				break;
			}
		}
		else if (CWRepeats == 2)
		{
			CWRepeats = 0;
		}
		if (_beamsweepercontrol.ActiveStateName == _beamsweeper2control.ActiveStateName)
		{
			string activeStateName = _beamsweepercontrol.ActiveStateName;
			string text = activeStateName;
			if (text != null)
			{
				if (!(text == "Beam Sweep L"))
				{
					if (text == "Beam Sweep R")
					{
						_beamsweeper2control.ChangeState(GetFsmEventByName(_beamsweeper2control, "BEAM SWEEP L"));
					}
				}
				else
				{
					_beamsweeper2control.ChangeState(GetFsmEventByName(_beamsweeper2control, "BEAM SWEEP R"));
				}
			}
		}
		if (_hm.hp < _phaseControl.FsmVariables.GetFsmInt("P2 Spike Waves").Value - 750 && !fullSpikesSet)
		{
			fullSpikesSet = true;
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Spikes Left", 0).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Spikes Left", 1).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Spikes Left", 2).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Spikes Left", 3).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Spikes Left", 4).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Spikes Right", 0).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Spikes Right", 1).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Spikes Right", 2).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Spikes Right", 3).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Spikes Right", 4).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Wave L", 2).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Wave L", 3).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Wave L", 4).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Wave L", 5).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Wave L", 6).sendEvent = "UP";
			FsmutilExt.GetAction<WaitRandom>(_spikeMasterControl, "Wave L", 7).timeMin = 0.1f;
			FsmutilExt.GetAction<WaitRandom>(_spikeMasterControl, "Wave L", 7).timeMax = 0.1f;
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Wave R", 2).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Wave R", 3).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Wave R", 4).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Wave R", 5).sendEvent = "UP";
			FsmutilExt.GetAction<SendEventByName>(_spikeMasterControl, "Wave R", 6).sendEvent = "UP";
			FsmutilExt.GetAction<WaitRandom>(_spikeMasterControl, "Wave R", 7).timeMin = 0.1f;
			FsmutilExt.GetAction<WaitRandom>(_spikeMasterControl, "Wave R", 7).timeMax = 0.1f;
			_spikeMasterControl.SetState("Spike Waves");
			FsmutilExt.GetAction<Wait>(_attackCommands, "Orb Summon", 2).time = 0.1f;
			FsmutilExt.GetAction<SetIntValue>(_attackCommands, "Orb Antic", 1).intValue = 12;
			FsmutilExt.GetAction<RandomInt>(_attackCommands, "Orb Antic", 2).min = 10;
			FsmutilExt.GetAction<RandomInt>(_attackCommands, "Orb Antic", 2).max = 14;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 1", 2).delay = 0.3f;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 1", 3).delay = 0.5f;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 1", 8).delay = 0.5f;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 1", 9).delay = 0.5f;
			FsmutilExt.GetAction<Wait>(_attackCommands, "EB 1", 10).time = 0.5f;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 2", 3).delay = 0.3f;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 2", 4).delay = 0.5f;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 2", 8).delay = 0.5f;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 2", 9).delay = 0.5f;
			FsmutilExt.GetAction<Wait>(_attackCommands, "EB 2", 10).time = 0.5f;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 3", 3).delay = 0.3f;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 3", 4).delay = 0.5f;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 3", 8).delay = 0.5f;
			FsmutilExt.GetAction<SendEventByName>(_attackCommands, "EB 3", 9).delay = 0.5f;
			FsmutilExt.GetAction<Wait>(_attackCommands, "EB 3", 10).time = 0.5f;
		}
		if (_hm.hp < _phaseControl.FsmVariables.GetFsmInt("P3 A1 Rage").Value + 30 && !disableBeamSet)
		{
			disableBeamSet = true;
			FsmutilExt.ChangeTransition(_attackChoices, "A1 Choice", "BEAM SWEEP L", "Orb Wait");
			FsmutilExt.ChangeTransition(_attackChoices, "A1 Choice", "BEAM SWEEP R", "Eye Beam Wait");
		}
		if (_attackChoices.FsmVariables.GetFsmInt("Arena").Value == 2 && !arena2Set)
		{
			Modding.Logger.Log("[Ultimatum Radiance] Starting Phase 2");
			arena2Set = true;
			FsmutilExt.GetAction<SetIntValue>(_attackCommands, "Orb Antic", 1).intValue = 12;
			FsmutilExt.GetAction<RandomInt>(_attackCommands, "Orb Antic", 2).min = 10;
			FsmutilExt.GetAction<RandomInt>(_attackCommands, "Orb Antic", 2).max = 14;
			FsmutilExt.GetAction<Wait>(_attackCommands, "Orb Summon", 2).time = 0.1f;
			FsmutilExt.GetAction<SetPosition>(_beamsweepercontrol, "Beam Sweep L", 3).x = 89f;
			FsmutilExt.GetAction<iTweenMoveBy>(_beamsweepercontrol, "Beam Sweep L", 5).vector = new Vector3(-75f, 0f, 0f);
			FsmutilExt.GetAction<iTweenMoveBy>(_beamsweepercontrol, "Beam Sweep L", 5).time = 3f;
			FsmutilExt.GetAction<SetPosition>(_beamsweepercontrol, "Beam Sweep R", 4).x = 32.6f;
			FsmutilExt.GetAction<iTweenMoveBy>(_beamsweepercontrol, "Beam Sweep R", 6).vector = new Vector3(75f, 0f, 0f);
			FsmutilExt.GetAction<iTweenMoveBy>(_beamsweepercontrol, "Beam Sweep R", 6).time = 3f;
			FsmutilExt.GetAction<SetPosition>(_beamsweeper2control, "Beam Sweep L", 2).x = 89f;
			FsmutilExt.GetAction<iTweenMoveBy>(_beamsweeper2control, "Beam Sweep L", 4).vector = new Vector3(-75f, 0f, 0f);
			FsmutilExt.GetAction<iTweenMoveBy>(_beamsweeper2control, "Beam Sweep L", 4).time = 3f;
			FsmutilExt.GetAction<SetPosition>(_beamsweeper2control, "Beam Sweep R", 3).x = 32.6f;
			FsmutilExt.GetAction<iTweenMoveBy>(_beamsweeper2control, "Beam Sweep R", 5).vector = new Vector3(75f, 0f, 0f);
			FsmutilExt.GetAction<iTweenMoveBy>(_beamsweeper2control, "Beam Sweep R", 5).time = 3f;
		}
		if (!(base.gameObject.transform.position.y >= 150f))
		{
			return;
		}
		if (_hm.hp < _phaseControl.FsmVariables.GetFsmInt("P5 Acend").Value - 500)
		{
			GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat").ChangeState(GetFsmEventByName(GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat"), "SLOW VANISH"));
			if (!onePlatSet)
			{
				onePlatSet = true;
				Log("Removing upper right platform");
				FsmutilExt.GetAction<Wait>(_attackCommands, "Orb Antic", 0).time = 0.01f;
				FsmutilExt.GetAction<SetIntValue>(_attackCommands, "Orb Antic", 1).intValue = 5;
				FsmutilExt.GetAction<RandomInt>(_attackCommands, "Orb Antic", 2).min = 4;
				FsmutilExt.GetAction<RandomInt>(_attackCommands, "Orb Antic", 2).max = 6;
				FsmutilExt.GetAction<Wait>(_attackCommands, "Orb Summon", 2).time = 0.01f;
				FsmutilExt.GetAction<Wait>(_attackCommands, "Orb Pause", 0).time = 0.01f;
				FsmutilExt.GetAction<Wait>(_attackChoices, "Orb Recover", 0).time = 0.1f;
			}
		}
		if (_hm.hp < _phaseControl.FsmVariables.GetFsmInt("P5 Acend").Value - 500 - 500)
		{
			_spikeClone.LocateMyFSM("Control").SendEvent("UP");
			_spikeClone2.LocateMyFSM("Control").SendEvent("UP");
			_spikeClone3.LocateMyFSM("Control").SendEvent("UP");
			_spikeClone4.LocateMyFSM("Control").SendEvent("UP");
			_spikeClone5.LocateMyFSM("Control").SendEvent("UP");
			if (!platSpikesSet)
			{
				platSpikesSet = true;
				GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat").ChangeState(GetFsmEventByName(GameObject.Find("Radiant Plat Small (10)").LocateMyFSM("radiant_plat"), "SLOW VANISH"));
			}
		}
	}

	private void AddNailWall(string stateName, string eventName, float delay, int index)
	{
		FsmutilExt.InsertAction(_attackChoices, stateName, (FsmStateAction)new SendEventByName
		{
			eventTarget = FsmutilExt.GetAction<SendEventByName>(_attackChoices, "Nail L Sweep", 0).eventTarget,
			sendEvent = eventName,
			delay = delay,
			everyFrame = false
		}, index);
	}

	private static FsmEvent GetFsmEventByName(PlayMakerFSM fsm, string eventName)
	{
		FsmEvent[] fsmEvents = fsm.FsmEvents;
		foreach (FsmEvent fsmEvent in fsmEvents)
		{
			if (fsmEvent.Name == eventName)
			{
				return fsmEvent;
			}
		}
		return null;
	}

	private static void Log(object obj)
	{
		Modding.Logger.Log("[Ultimatum Radiance] " + obj);
	}
}