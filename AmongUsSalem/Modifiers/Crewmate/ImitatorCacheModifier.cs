using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Modifiers.Crewmate;

public sealed class ImitatorCacheModifier : BaseModifier, ICachedRole
{
    private MeetingMenu? _meetingMenu;
    private NetworkedPlayerInfo? _selectedPlr;
    public override string ModifierName => "Imitator";
    public override bool HideOnUi => true;
    public bool ShowCurrentRoleFirst => true;

    public bool Visible => Player.AmOwner || PlayerControl.LocalPlayer.HasDied() ||
                           GuardianAngelTouRole.GASeesRoleVisibilityFlag(Player);

    public RoleBehaviour CachedRole => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ImitatorRole>());

    public override void OnActivate()
    {
        base.OnActivate();

        if (Player.AmOwner)
        {
            _meetingMenu = new MeetingMenu(
                Player.Data.Role,
                Click,
                MeetingAbilityType.Toggle,
                TouAssets.ImitateSelectSprite,
                TouAssets.ImitateDeselectSprite,
                IsExempt,
                Color.white)
            {
                Position = new Vector3(-0.40f, 0f, -3f)
            };
        }
    }

    public override void OnMeetingStart()
    {
        if (!Player.IsCrewmate())
        {
            if (ObjectWorkshopPlugin.IsDevBuild) Logger<ObjectWorkshopPlugin>.Error($"Removed Imitator Cache Modifier On Meeting Start");
            ModifierComponent?.RemoveModifier(this);
            return;
        }

        if (Player.AmOwner)
        {
            // _selectedPlr = null;
            _meetingMenu!.GenButtons(MeetingHud.Instance,
                Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>());
            if (_selectedPlr != null)
            {
                _meetingMenu!.Actives[_selectedPlr.PlayerId] = true;
            }
        }
    }

    public void OnVotingComplete()
    {
        if (Player.AmOwner)
        {
            _meetingMenu!.HideButtons();
        }
    }

    public override void OnDeactivate()
    {
        _selectedPlr = null;

        if (Player.AmOwner && _meetingMenu != null)
        {
            _meetingMenu?.Dispose();
            _meetingMenu = null!;
        }
    }

    public void Click(PlayerVoteArea voteArea, MeetingHud __)
    {
        var player = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId);

        if (_selectedPlr == player)
        {
            _selectedPlr = null;
            _meetingMenu!.Actives[voteArea.TargetPlayerId] = false;
            return;
        }

        if (_selectedPlr != null)
        {
            _meetingMenu!.Actives[_selectedPlr.PlayerId] = false;
            _selectedPlr = null;
        }

        _meetingMenu!.Actives[voteArea.TargetPlayerId] = true;
        _selectedPlr = player;
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId);
        var opts = OptionGroupSingleton<ImitatorOptions>.Instance;
        if (player != null && player.Object.GetRoleWhenAlive() is ICrewVariant neutVariant &&
            player.Object.IsNeutral() && opts.ImitateNeutrals &&
            MiscUtils.GetPotentialRoles().Contains(neutVariant.CrewVariant))
        {
            return voteArea.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || !voteArea!.AmDead;
        }

        if (player != null && player.Object.GetRoleWhenAlive() is ICrewVariant impVariant &&
            player.Object.IsImpostor() && opts.ImitateImpostors &&
            MiscUtils.GetPotentialRoles().Contains(impVariant.CrewVariant))
        {
            return voteArea.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || !voteArea!.AmDead;
        }


        if (player != null && !player.Object.IsCrewmate())
        {
            return true;
        }

        if (player != null && player.Object.GetRoleWhenAlive() is JailorRole &&
            !CustomRoleUtils.GetActiveRolesOfType<JailorRole>().Any() && PlayerControl.AllPlayerControls.ToArray()
                .Any(x => x.HasModifier<ImitatorCacheModifier>() && !x.Data.IsDead && !x != Player))
        {
            return true;
        }

        if (player != null && player.Object.GetRoleWhenAlive() is ProsecutorRole &&
            !CustomRoleUtils.GetActiveRolesOfType<ProsecutorRole>().Any() && PlayerControl.AllPlayerControls.ToArray()
                .Any(x => x.HasModifier<ImitatorCacheModifier>() && !x.Data.IsDead && !x != Player))
        {
            return true;
        }

        if (player != null && player.Object.GetRoleWhenAlive() is MayorRole)
        {
            return true;
        }

        if (player != null && player.Object.GetRoleWhenAlive() is PoliticianRole)
        {
            return true;
        }

        if (player != null && player.Object.GetRoleWhenAlive() is ImitatorRole)
        {
            return true;
        }

        return voteArea.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || !voteArea!.AmDead;
    }

    public void UpdateRole()
    {
        if (!Player.IsCrewmate())
        {
            if (ObjectWorkshopPlugin.IsDevBuild) Logger<ObjectWorkshopPlugin>.Error($"Removed Imitator Cache Modifier On Attempt To Update Role");
            ModifierComponent?.RemoveModifier(this);
            return;
        }

        if (_selectedPlr == null || Player.Data.IsDead || !_selectedPlr.IsDead)
        {
            _selectedPlr = null;
            if (Player == null || Player.IsRole<ImitatorRole>())
            {
                return;
            }

            Player.RpcChangeRole(RoleId.Get<ImitatorRole>(), false);
            return;
        }

        var roleWhenAlive = _selectedPlr.Object.GetRoleWhenAlive();
        if (roleWhenAlive is ICrewVariant crewType)
        {
            roleWhenAlive = crewType.CrewVariant;
        }

        if (Player.Data.Role.GetType() != roleWhenAlive.GetType())
        {
            Player.RpcChangeRole((ushort)roleWhenAlive.Role, false);
        }
    }
}