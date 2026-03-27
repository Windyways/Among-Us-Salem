using Reactor.Utilities.Attributes;
using UnityEngine;

namespace AmongUsSalem.Roles;


[RegisterInIl2Cpp]
public class ShowRoleIcon(IntPtr ptr) : MonoBehaviour(ptr)
{
    public PlayerControl Owner;
    public SpriteRenderer myRend;

    public static List<ShowRoleIcon> AllRoleIcons = new List<ShowRoleIcon>();

    public void Start()
    {
        myRend = gameObject.GetComponent<SpriteRenderer>();
        AllRoleIcons.Add(this);
    }

    private void Update()
    {
        if ((Owner.HasDied() || Owner.inVent || Owner.HasModifier<InvisibleStatus>())) myRend.Hide();
        else if (!Owner.HasDied() && (HudManagerPatches.LocalVisibilityFlag(PlayerControl.LocalPlayer, Owner) || Owner.AmOwner))
        {
            if (Owner.TryGetModifier<DeepfakeRole>(out var fakeRole) && fakeRole.foolingPlayer == PlayerControl.LocalPlayer)
            {
                var fakeRoleIcon = MiscUtils.AllRoles.FirstOrDefault(x => x is ICustomAURole role && role.RoleName == fakeRole.roleName);
                if (fakeRoleIcon is ICustomAURole customRole) myRend.sprite = customRole.Configuration.Icon.LoadAsset();
            }

            myRend.Show();
            if (AUSPlugin.RoleIconSpot.Value == 0)
            {
                var nameText = Owner.cosmetics.nameText;
                nameText.ForceMeshUpdate();

                var line = nameText.textInfo.lineInfo[0]; // second line (player name)

                transform.localPosition = new Vector3(line.lineExtents.min.x - 0.2f, 1.3f); // - 0.2f 1.05f
            }
            else
            {
                var nameText = Owner.cosmetics.nameText;
                nameText.ForceMeshUpdate();

                var line = nameText.textInfo.lineInfo[1]; // first line (player name)

                transform.localPosition = new Vector3(line.lineExtents.min.x - 0.2f, 1.05f); // - 0.2f 1.05f
            }
        }
        else if (!myRend.IsHidden()) myRend.Hide();
    }

    public static void Add(PlayerControl player)
    {
        if (player.Data.Role is ICustomAURole role)
        {
            var obj = new GameObject("Icon " + role.RoleName);
            obj.AddSpriteRenderer(role.Configuration.Icon.LoadAsset(), 0, 100, player.transform.position, Color.white, Vector3.one);

            // pos is handled in UPDATE.
            obj.transform.localScale = new Vector2(0.15f, 0.15f);

            var icon = obj.AddComponent<ShowRoleIcon>();
            icon.Owner = player;

            obj.transform.SetParent(player.transform);
        }
    }

    public static void ClearAll()
    {
        AllRoleIcons.Clear();
    }

    public void UpdateIcon()
    {
        if (Owner.Data.Role is ICustomAURole role)
            myRend.sprite = role.Configuration.Icon.LoadAsset();
    }
}