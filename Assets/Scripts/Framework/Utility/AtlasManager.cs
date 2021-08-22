using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace TFramework
{
    public class AtlasManager : CSingleton<AtlasManager>
    {
        private Dictionary<string, SpriteAtlas> mDicAtlas = new Dictionary<string, SpriteAtlas>();

        public void LoadAtlas(string aKey, string aAtlasDir)
        {
            if (this.mDicAtlas.ContainsKey(aKey) == true)
            {
                Debug.LogError("Load the same atlas!!");
                return;
            }
            SpriteAtlas atlas = AssetLoader.LoadAsset<SpriteAtlas>(aAtlasDir);
            this.mDicAtlas.Add(aKey, atlas);
        }

        public Sprite GetSprite(string aAtlasName, string aSpriteName)
        {
            if (this.mDicAtlas.ContainsKey(aAtlasName) == false)
            {
                return null;
            }
            SpriteAtlas atlas = this.mDicAtlas[aAtlasName];
            return atlas.GetSprite(aSpriteName);
        }
    }
}

