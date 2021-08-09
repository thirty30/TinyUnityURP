using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace TFramework.TGUI
{
    public class TGUIHoleImage : Image
    {
        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            Material baseMat = baseMaterial;
            if (this.m_ShouldRecalculateStencil)
            {
                Transform sortOverrideCanvas = MaskUtilities.FindRootSortOverrideCanvas(this.transform);
                this.m_StencilValue = !this.maskable ? 0 : MaskUtilities.GetStencilDepth(this.transform, sortOverrideCanvas);
                this.m_ShouldRecalculateStencil = false;
            }
            Mask component = this.GetComponent<Mask>();
            if (this.m_StencilValue > 0 && ((UnityEngine.Object)component == (UnityEngine.Object)null || !component.IsActive()))
            {
                Material material = StencilMaterial.Add(baseMat, (1 << this.m_StencilValue) - 1, StencilOp.Keep, CompareFunction.NotEqual, ColorWriteMask.All, (1 << this.m_StencilValue) - 1, 0);
                StencilMaterial.Remove(this.m_MaskMaterial);
                this.m_MaskMaterial = material;
                baseMat = this.m_MaskMaterial;
            }
            return baseMat;
        }
    }
}
