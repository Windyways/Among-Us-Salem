using System.Linq;
using UnityEngine;

namespace ObjectWorkshop.LifeImprovement;

public static class ObjectExtentions
{
    public static SpriteRenderer AddSpriteRenderer(this GameObject gameObject, Sprite sprite, int sortingOrder, int rendererPriority, Vector3 pos, Color color, Vector3 localScale)
    {
        SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = sortingOrder;
        sr.rendererPriority = rendererPriority;
        sr.transform.position = pos;
        sr.color = color;
        sr.gameObject.transform.localScale = localScale;
        return sr;
    }

    public static void AddBoxCollider2D(this GameObject gameObject, bool isTrigger, bool ignoreCollision)
    {
        BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = isTrigger;
        if (ignoreCollision)
        {
            IgnorePlayerCollisions(boxCollider2D);
        }
    }

    public static void AddPolygonCollider2D(this GameObject gameObject, bool isTrigger, Vector2[] points, bool ignoreCollision)
    {
        PolygonCollider2D polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
        polygonCollider2D.isTrigger = isTrigger;
        polygonCollider2D.points = points;
        if (ignoreCollision) IgnoreInfiltratorCollisions(polygonCollider2D);
    }

    public static void IgnorePetCollisions(Collider2D collider2D)
    {
        foreach (PlayerControl players in PlayerControl.AllPlayerControls)
        {
            PetBehaviour pet = players.cosmetics.currentPet;
            IgnoreCollision(collider2D, pet.GetComponent<CircleCollider2D>());
        }
    }

    public static void IgnorePlayerCollisions(Collider2D collider2D)
    {
        foreach (PlayerControl players in PlayerControl.AllPlayerControls)
        {
            IgnoreCollision(collider2D, players.GetComponent<CircleCollider2D>());
            IgnorePetCollisions(collider2D);
        }
    }

    public static void IgnoreInfiltratorCollisions(Collider2D collider2D)
    {
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            if (player.IsFaction(Faction.Infiltrator, true))
            {
                IgnoreCollision(collider2D, player.GetComponent<CircleCollider2D>());
                IgnorePetCollisions(collider2D);
            }
        }
    }

    public static void IgnoreCollision(Collider2D collider2D, Collider2D targetCollider2D)
    {
        if (targetCollider2D != null)
        {
            Physics2D.IgnoreCollision(collider2D, targetCollider2D, true);
        }
    }

    public static void EnableCollision(Collider2D collider2D, Collider2D targetCollider2D)
    {
        if (targetCollider2D != null)
        {
            Physics2D.IgnoreCollision(collider2D, targetCollider2D, false);
        }
    }

    public static void SetCameraToObject(this GameObject gameObject, PlayerControl player) // Used mainly for Totemist/Aimsman.
    {
        LightSource light = player.lightSource;
        light.transform.SetParent(gameObject.transform);
        light.transform.localPosition = Vector3.zero;
        Camera.main.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Camera.main.transform.position.z);
    }

    public static Rigidbody2D AddRigidBody2D(this GameObject gameObject, float gravityScale, RigidbodyConstraints2D constraints2D)
    {
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.constraints = constraints2D;
        return rb;
    }

    public static Color noColor()
    {
        return new Color(1f, 1f, 1f, 0f);
    }

    public static Color fullColor()
    {
        return new Color(1f, 1f, 1f, 1f);
    }

    public static Color One3rdColor()
    {
        return new Color(1f, 1f, 1f, 0.3f);
    }
}