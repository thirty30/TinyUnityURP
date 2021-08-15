using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TGameTool/HotfixConfig", order = 200)]
public class HotfixConfig : ScriptableObject
{
    public string Version;
    public List<string> CDNURL;
}
