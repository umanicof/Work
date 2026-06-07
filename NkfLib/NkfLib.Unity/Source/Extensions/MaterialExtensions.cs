using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


namespace NkfLib.Unity
{
    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    public static class MaterialExtensions
    {
        /// <summary>
        /// RenderModeの変更（Standard Sharder Material）
        /// 出典：https://qiita.com/polikeiji/items/e56febcfdf886524352c
        /// </summary>
        /// <param name="self"></param>
        /// <param name="renderingMode"></param>
        public static void SetRenderMode(this Material self, RenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    self.SetOverrideTag("RenderType", "");
                    self.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    self.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    self.SetInt("_ZWrite", 1);
                    self.DisableKeyword("_ALPHATEST_ON");
                    self.DisableKeyword("_ALPHABLEND_ON");
                    self.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    self.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    self.SetOverrideTag("RenderType", "TransparentCutout");
                    self.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    self.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    self.SetInt("_ZWrite", 1);
                    self.EnableKeyword("_ALPHATEST_ON");
                    self.DisableKeyword("_ALPHABLEND_ON");
                    self.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    self.renderQueue = 2450;
                    break;
                case RenderingMode.Fade:
                    self.SetOverrideTag("RenderType", "Transparent");
                    self.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    self.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    self.SetInt("_ZWrite", 0);
                    self.DisableKeyword("_ALPHATEST_ON");
                    self.EnableKeyword("_ALPHABLEND_ON");
                    self.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    self.renderQueue = 3000;
                    break;
                case RenderingMode.Transparent:
                    self.SetOverrideTag("RenderType", "Transparent");
                    self.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    self.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    self.SetInt("_ZWrite", 0);
                    self.DisableKeyword("_ALPHATEST_ON");
                    self.DisableKeyword("_ALPHABLEND_ON");
                    self.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    self.renderQueue = 3000;
                    break;
            }
        }
    }
}