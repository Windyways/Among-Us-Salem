using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Modules;

// Code Review: Should be using a MonoBehaviour
public sealed class ScreenFlash : IDisposable
{
    private static readonly List<ScreenFlash> _screenFlashes = new();

    private readonly KillOverlay _overlay;
    private readonly SpriteRenderer _renderer;

    public ScreenFlash()
    {
        _overlay = Object.Instantiate(HudManager.Instance.KillOverlay, HudManager.Instance.transform);
        _overlay.background.color = Color.clear;

        var transform = _overlay.flameParent.transform;
        var flame = transform.GetChild(0).gameObject;

        _renderer = flame.GetComponent<SpriteRenderer>();
        _renderer.sprite = TouAssets.ScreenFlash.LoadAsset();
        _renderer.color = Color.white;

        _screenFlashes.Add(this);

        SetActive(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public static void Clear()
    {
        _screenFlashes.Do(x => x.Destroy());
        _screenFlashes.Clear();
    }

    public bool IsActive()
    {
        if (_overlay != null && _overlay.flameParent != null)
        {
            return _overlay.flameParent.active;
        }

        return false;
    }

    public void SetActive(bool isActive)
    {
        if (_overlay != null && _overlay.flameParent != null)
        {
            _overlay.flameParent.SetActive(isActive);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        if (_overlay != null && _overlay.flameParent != null)
        {
            _overlay.flameParent.transform.localPosition = pos;
        }
    }

    public void SetScale(Vector3 scale)
    {
        if (_overlay != null && _overlay.flameParent != null)
        {
            _overlay.flameParent.transform.localScale = scale;
        }
    }

    public void SetColour(Color color)
    {
        if (_renderer != null)
        {
            _renderer.color = color;
        }
    }

    public void Destroy()
    {
        Dispose();
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_overlay != null)
            {
                Object.Destroy(_overlay);
            }

            if (_renderer != null)
            {
                Object.Destroy(_renderer);
            }
        }
    }
}