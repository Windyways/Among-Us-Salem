using UnityEngine;

namespace AmongUsSalem.Modifiers;

public sealed class JuryModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Jury";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public bool isJudge => c.IsRole<Prosecutor>(); // Change to Judge later.

    public override void OnMeetingStart()
    {
        Player.RpcRemoveModifier<JuryModifier>();
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.RpcRemoveModifier<JuryModifier>();
    }

    public void AnonymousChatSendPatch(string nameText, string message)
    {
        var chat = HudManager.Instance.Chat;
        message = message.ApplyKeywords();

        var pooledBubble = chat.GetPooledBubble();

        pooledBubble.transform.SetParent(chat.scroller.Inner);
        pooledBubble.transform.localScale = Vector3.one;
        if (!Player.AmOwner) pooledBubble.SetLeft();
        else pooledBubble.SetRight();

        pooledBubble.SetCosmetics(PlayerControl.AllPlayerControls.ToArray().FirstOrDefault().CachedPlayerData);
        pooledBubble.NameText.text = nameText;
        pooledBubble.NameText.color = Color.white;
        pooledBubble.NameText.ForceMeshUpdate(true, true);
        pooledBubble.votedMark.enabled = false;
        pooledBubble.Xmark.enabled = false;
        pooledBubble.TextArea.text = message;
        pooledBubble.TextArea.ForceMeshUpdate(true, true);
        pooledBubble.Background.size = new Vector2(5.52f, 0.2f + pooledBubble.NameText.GetNotDumbRenderedHeight() + pooledBubble.TextArea.GetNotDumbRenderedHeight());
        pooledBubble.MaskArea.size = pooledBubble.Background.size - new Vector2(0, 0.03f);

        pooledBubble.AlignChildren();
        var pos = pooledBubble.NameText.transform.localPosition;
        pooledBubble.NameText.transform.localPosition = pos;
        chat.AlignAllBubbles();
        if (chat is { IsOpenOrOpening: false, notificationRoutine: null })
        {
            chat.notificationRoutine = chat.StartCoroutine(chat.BounceDot());
        }

        if (!chat.IsOpenOrOpening)
        {
            SoundManager.Instance.PlaySound(chat.messageSound, false).pitch = 0.5f + PlayerControl.LocalPlayer.PlayerId / 15f;
            chat.chatNotification.SetUp(PlayerControl.LocalPlayer, message);
        }
    }
}