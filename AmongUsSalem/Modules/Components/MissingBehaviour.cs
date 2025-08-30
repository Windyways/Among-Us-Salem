using Reactor.Utilities.Attributes;
using UnityEngine;

namespace ObjectWorkshop.Modules.Components;

[RegisterInIl2Cpp]
public sealed class MissingBehaviour(IntPtr ip) : MonoBehaviour(ip);