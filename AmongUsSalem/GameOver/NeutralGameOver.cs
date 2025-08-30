using MiraAPI.GameEnd;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Roles;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Roles.Neutral;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectWorkshop.GameOver;

public sealed class NeutralGameOver : CustomGameOver
{
    private Color _roleColor;
    private string _roleName = "PLACEHOLDER";

    public override bool VerifyCondition(PlayerControl playerControl, NetworkedPlayerInfo[] winners)
    {
        if (winners is not [{ Role: RoleBehaviour role and IOWRole tRole }])
        {
            return false;
        }

        var mainRole = role;

        Logger<ObjectWorkshopPlugin>.Error($"VerifyCondition - mainRole: '{mainRole.NiceName}', IsDead: '{role.IsDead}'");

        if (role.IsDead && role is not PhantomTouRole or HaunterRole)
        {
            mainRole = role.Player.GetRoleWhenAlive();

            Logger<ObjectWorkshopPlugin>.Error($"VerifyCondition - RoleWhenAlive: '{mainRole?.NiceName}'");
        }

        _roleName = mainRole!.NiceName;
        _roleColor = mainRole.TeamColor;

        return tRole.WinConditionMet();
    }

    public override void AfterEndGameSetup(EndGameManager endGameManager)
    {
        endGameManager.BackgroundBar.material.SetColor(ShaderID.Color, _roleColor);

        var text = Object.Instantiate(endGameManager.WinText);
        text.text = $"{_roleName} Wins!";
        text.color = _roleColor;
        GameHistory.WinningFaction = $"<color=#{_roleColor.ToHtmlStringRGBA()}>{_roleName}</color>";

        var pos = endGameManager.WinText.transform.localPosition;
        pos.y = 1.5f;
        pos += Vector3.down * 0.15f;
        text.transform.localScale = new Vector3(1f, 1f, 1f);

        text.transform.position = pos;
        text.text = $"<size=4>{text.text}</size>";
    }
}