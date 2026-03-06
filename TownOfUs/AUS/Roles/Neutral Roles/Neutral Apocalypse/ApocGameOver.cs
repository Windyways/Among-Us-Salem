using MiraAPI.GameEnd;
using Reactor.Utilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.ApocalypseRoles;

public sealed class ApocGameOver : CustomGameOver
{
    private Color _roleColor;

    public override bool VerifyCondition(PlayerControl playerControl, NetworkedPlayerInfo[] winners)
    {
        if (winners is not [{ Role: RoleBehaviour role and ICustomAURole tRole }])
        {
            return false;
        }

        var mainRole = role;

        Logger<AUSPlugin>.Error($"VerifyCondition - mainRole: '{mainRole.NiceName}', IsDead: '{role.IsDead}'");

        if (role.IsDead)
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
        text.text = $"The Apocalypse Is Nigh!";
        text.color = _roleColor;
        GameHistory.WinningFaction = $"<color=#{_roleColor.ToHtmlStringRGBA()}>Apocalypse</color>";

        var pos = endGameManager.WinText.transform.localPosition;
        pos.y = 1.5f;
        pos += Vector3.down * 0.15f;
        text.transform.localScale = new Vector3(1f, 1f, 1f);

        text.transform.position = pos;
        text.text = $"<size=4>{text.text}</size>";

        //AUSAssets.PlaySound(AUSAssets.CovenWin_SFX);
    }

    public static bool WinConditionMet(RoleBehaviour role)
    {
        var aliveApoc = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Alignment.NeutralApocalypse));
        if (aliveApoc == 0) return false;

        var result = MiscUtils.GetAlivePlayersToEnd().Count <= aliveApoc && MiscUtils.KillersAliveCount == aliveApoc;
        return result;
    }

    public static bool AnyApocWon(GameOverReason gameOverReason)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.Is(Alignment.NeutralApocalypse) && WinConditionMet(player.Data.Role)) return true;
        }
        return false;
    }
}