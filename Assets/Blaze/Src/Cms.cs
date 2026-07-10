using System;
using Blaze.Runtime.Cms;
using UnityEngine;

[Serializable]
public class CustomCmsComponent : CmsComponent
{
    public int a;
    [WithDefiner(typeof(CustomDefiner1))]
    public CmsEntityPfb adad;
}
[Serializable]
public class CustomDefiner1 : CmsEntityDefiner
{
    public override void OnDefine()
    {
        Define<CustomCmsComponent>();
        Define<CustomCmsComponent>();
        Define<CustomCmsComponent>();
        Define<CustomCmsComponent>();
        base.OnDefine();
    }
}
[Serializable]
public class CustomDefiner2 : CustomDefiner1
{
}


// public class Cms : MonoBehaviour
// {
//     public void Awake()
//     {
//         // Content.
//     }
// }