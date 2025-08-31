using MiraAPI.GameEnd;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using AmongUsSalem.Modules;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.GameOver;

public sealed class DrawGameOver : CustomGameOver
{
    public override bool VerifyCondition(PlayerControl playerControl, NetworkedPlayerInfo[] winners)
    {
        return true;
    }

    public override void AfterEndGameSetup(EndGameManager endGameManager)
    {
        endGameManager.BackgroundBar.material.SetColor(ShaderID.Color, AUSColors.Neutral);

        var text = Object.Instantiate(endGameManager.WinText);
        text.text = "Nobody Wins!";
        text.color = AUSColors.Neutral;
        GameHistory.WinningFaction = $"<color=#{AUSColors.Neutral.ToHtmlStringRGBA()}>Nobody</color>";

        var pos = endGameManager.WinText.transform.localPosition;
        pos.y = 1.5f;
        pos += Vector3.down * 0.15f;
        text.transform.localScale = new Vector3(1f, 1f, 1f);

        text.transform.position = pos;
        text.text = $"<size=4>{text.text}</size>";
    }
}