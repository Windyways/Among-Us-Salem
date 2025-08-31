using System.Text;
using MiraAPI.Events;
using MiraAPI.Modifiers;
using Reactor.Utilities.Extensions;
using AmongUsSalem.Events.TouEvents;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Modifiers.Crewmate;

public sealed class LookoutWatchedModifier(PlayerControl lookout) : BaseModifier
{
    public override string ModifierName => "Watched";
    public override bool HideOnUi => true;

    public PlayerControl Lookout { get; set; } = lookout;
    public List<RoleBehaviour> SeenPlayers { get; set; } = [];

    public override void OnActivate()
    {
        base.OnActivate();

        var touAbilityEvent = new TouAbilityEvent(AbilityType.LookoutWatch, Lookout, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Lookout.AmOwner)
        {
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(AUSColors.Lookout));
        }
    }

    public override void OnMeetingStart()
    {
        if (!Lookout.AmOwner)
        {
            return;
        }

        var title = $"<color=#{AUSColors.Lookout.ToHtmlStringRGBA()}>Lookout Feedback</color>";
        var msg = $"No players interacted with {Player.Data.PlayerName}";

        if (SeenPlayers.Count != 0)
        {
            var message = new StringBuilder($"Roles seen interacting with {Player.Data.PlayerName}:\n");

            SeenPlayers.Shuffle();

            foreach (var role in SeenPlayers)
            {
                message.Append(AUSPlugin.Culture, $"{role.NiceName}, ");
            }

            message = message.Remove(message.Length - 2, 2);

            var final = message.ToString();

            if (string.IsNullOrWhiteSpace(final))
            {
                return;
            }

            msg = final;
        }

        MiscUtils.AddFakeChat(Player.Data, title, msg, false, true);

        SeenPlayers.Clear();
    }
}