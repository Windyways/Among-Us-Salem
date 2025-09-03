using MiraAPI.GameEnd;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using AmongUsSalem.Modules;
using AmongUsSalem.Roles;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Roles.Neutral;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.GameOver;

public sealed class TraitorGameOver : CustomGameOver
{
    private Color _roleColor;

    public override bool VerifyCondition(PlayerControl playerControl, NetworkedPlayerInfo[] winners)
    {
        if (winners is not [{ Role: RoleBehaviour role and IAUSRole tRole }])
        {
            return false;
        }

        var mainRole = role;

        Logger<AUSPlugin>.Error($"VerifyCondition - mainRole: '{mainRole.NiceName}', IsDead: '{role.IsDead}'");

        if (role.IsDead && role is not HaunterRole)
        {
            mainRole = role.Player.GetRoleWhenAlive();

            Logger<AUSPlugin>.Error($"VerifyCondition - RoleWhenAlive: '{mainRole?.NiceName}'");
        }

        _roleColor = mainRole.TeamColor;

        return tRole.WinConditionMet();
    }

    public override void AfterEndGameSetup(EndGameManager endGameManager)
    {
        endGameManager.BackgroundBar.material.SetColor(ShaderID.Color, _roleColor);

        var text = Object.Instantiate(endGameManager.WinText);
        text.text = $"Traitor Wins!";
        text.color = _roleColor;
        GameHistory.WinningFaction = $"<color=#{_roleColor.ToHtmlStringRGBA()}>Traitor</color>";

        var pos = endGameManager.WinText.transform.localPosition;
        pos.y = 1.5f;
        pos += Vector3.down * 0.15f;
        text.transform.localScale = new Vector3(1f, 1f, 1f);

        text.transform.position = pos;
        text.text = $"<size=4>{text.text}</size>";
    }
}