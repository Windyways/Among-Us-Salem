using MiraAPI.Modifiers.Types;
using TownOfUs.Modules.Anims;
using UnityEngine;

namespace AmongUsSalem.Misc;

/// <summary>
/// Shared revive implementation (used by roles like Altruist and Time Lord).
/// This is intentionally modeled after the Altruist revive flow to avoid
/// ghost-role / crewmate-ghost desync issues.
/// </summary>
public static class ReviveUtilities
{
    public static DeadBody? FindDeadBodyIncludingInactive(byte bodyId)
    {
        try
        {
            for (var sceneIdx = 0; sceneIdx < UnityEngine.SceneManagement.SceneManager.sceneCount; sceneIdx++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(sceneIdx);
                if (!scene.isLoaded)
                {
                    continue;
                }

                var rootObjects = scene.GetRootGameObjects();
                foreach (var root in rootObjects)
                {
                    var bodies = root.GetComponentsInChildren<DeadBody>(true);
                    foreach (var body in bodies)
                    {
                        if (body != null && body.ParentId == bodyId)
                        {
                            return body;
                        }
                    }
                }
            }
        }
        catch
        {
            // ignored
        }

        return null;
    }

    private static System.Collections.IEnumerator CoEnsureNoLingeringBody(byte revivedId, float timeoutSeconds)
    {
        const float step = 0.05f;
        var waited = 0f;

        while (waited < timeoutSeconds)
        {
            var body = FindDeadBodyIncludingInactive(revivedId) ??
                       UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == revivedId);
            if (body != null && body.gameObject != null)
            {
                try { UnityEngine.Object.Destroy(body.gameObject); } catch {    }
            }

            waited += step;
            yield return new WaitForSeconds(step);
        }
    }

    public static void RevivePlayer(
        PlayerControl reviver,
        PlayerControl revived,
        Vector2 position,
        RoleBehaviour roleWhenAlive,
        Color flashColor,
        string? revivedOwnerNotificationText,
        string? reviverOwnerNotificationText,
        Sprite? notificationIcon = null)
    {
        if (!revived || revived.Data == null)
        {
            return;
        }

        var inMeetingOrExile = MeetingHud.Instance || ExileController.Instance;

        // Ensure death state is properly synced before revive to prevent desyncs
        // Wait a small amount to ensure any pending death syncs complete
        Coroutines.Start(CoEnsureDeathStateSyncedBeforeRevive(revived, inMeetingOrExile, position, roleWhenAlive, flashColor, 
            revivedOwnerNotificationText, reviverOwnerNotificationText, notificationIcon, reviver));
    }

    public static void AdjustNotification(this LobbyNotificationMessage notification)
    {
        /*if (!_languagesToBold.Contains(DataManager.Settings.Language.CurrentLanguage))
        {
            notification.Text.fontStyle = FontStyles.Bold;
            notification.Text.SetOutlineThickness(0.35f);
        }*/
    }

    /// <summary>
    /// Ensures death state is synced before proceeding with revive to prevent race conditions.
    /// </summary>
    private static System.Collections.IEnumerator CoEnsureDeathStateSyncedBeforeRevive(
        PlayerControl revived,
        bool inMeetingOrExile,
        Vector2 position,
        RoleBehaviour roleWhenAlive,
        Color flashColor,
        string? revivedOwnerNotificationText,
        string? reviverOwnerNotificationText,
        Sprite? notificationIcon,
        PlayerControl? reviver)
    {
        yield return new WaitForSeconds(0.15f);

        if (revived == null || revived.Data == null || revived.Data.Disconnected)
        {
            yield break;
        }

        if (!revived.HasDied())
        {
            yield break;
        }

        GameHistory.ClearMurder(revived);

        revived.Revive();

        if (!inMeetingOrExile)
        {
            revived.transform.position = position;
            if (revived.AmOwner)
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(position);
            }

            if (revived.MyPhysics?.body != null)
            {
                revived.MyPhysics.body.position = position;
                Physics2D.SyncTransforms();
            }
        }

        if (ModCompatibility.IsSubmerged() && PlayerControl.LocalPlayer != null &&
            PlayerControl.LocalPlayer.PlayerId == revived.PlayerId)
        {
            ModCompatibility.ChangeFloor(revived.transform.position.y > -7);
        }

        if (!inMeetingOrExile && revived.AmOwner)
        {
            try
            {
                HudManager.Instance.Chat.gameObject.SetActive(false);
            }
            catch
            {
                // ignored
            }
        }

        revived.ChangeRole((ushort)roleWhenAlive.Role, recordRole: false);

        if (revived.Data.Role is IAnimated animatedRole)
        {
            animatedRole.IsVisible = true;
            animatedRole.SetVisible();
        }

        foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(revived.Data.Role)).OfType<IAnimated>())
        {
            button.IsVisible = true;
            button.SetVisible();
        }

        foreach (var modifier in revived.GetModifiers<GameModifier>().Where(x => x is IAnimated))
        {
            if (modifier is IAnimated animatedMod)
            {
                animatedMod.IsVisible = true;
                animatedMod.SetVisible();
            }
        }

        revived.RemainingEmergencies = 0;
        if (reviver != null)
        {
            reviver.RemainingEmergencies = 0;
        }

        if (!inMeetingOrExile && revived.AmOwner && !string.IsNullOrWhiteSpace(revivedOwnerNotificationText))
        {
            try
            {
                //TouAudio.PlaySound(TouAudio.AltruistReviveSound);
                Coroutines.Start(MiscUtils.CoFlash(flashColor));
                var notif = Helpers.CreateAndShowNotification(
                    $"<b>{flashColor.ToTextColor()}{revivedOwnerNotificationText}</color></b>",
                    Color.white,
                    new Vector3(0f, 1f, -20f),
                    spr: notificationIcon);
                notif.AdjustNotification();
            }
            catch
            {
                // ignored
            }
        }

        if (!inMeetingOrExile && reviver != null && reviver.AmOwner && reviver != revived && !string.IsNullOrWhiteSpace(reviverOwnerNotificationText))
        {
            try
            {
                //TouAudio.PlaySound(TouAudio.AltruistReviveSound);
                Coroutines.Start(MiscUtils.CoFlash(flashColor));
                var notif = Helpers.CreateAndShowNotification(
                    $"<b>{flashColor.ToTextColor()}{reviverOwnerNotificationText}</color></b>",
                    Color.white,
                    new Vector3(0f, 1f, -20f),
                    spr: notificationIcon);
                notif.AdjustNotification();
            }
            catch
            {
                // ignored
            }
        }

        try
        {
            var body = UnityEngine.Object.FindObjectsOfType<DeadBody>()
                .FirstOrDefault(b => b.ParentId == revived.PlayerId);
            if (body != null)
            {
                UnityEngine.Object.Destroy(body.gameObject);
            }
        }
        catch
        {
            // ignored
        }

        Coroutines.Start(CoEnsureNoLingeringBody(revived.PlayerId, 1.0f));
    }
}