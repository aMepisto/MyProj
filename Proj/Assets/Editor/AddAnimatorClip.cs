// UTF8 & LF (유티에프:)

using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
static class AddAnimatorClip 
{

    [MenuItem("Assets/AddAnimatorClip", false, 0)]
    static public void Test()
    {
        // Get controller
        var _animator = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

        foreach(var anim in _animator)
        {

            //var controller = AssetDatabase.LoadAssetAtPath(str, typeof(AnimatorController));
            //var code = GenerateCode(controller);
            //AnimationClip animationClip = new AnimationClip();
            //animationClip.name = "SomeClip";        
            //AssetDatabase.AddObjectToAsset(animationClip, anim);
            //// Reimport the asset after adding an object.
            //// Otherwise the change only shows up when saving the project
            //AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animationClip));
            ////a = a as Animator;
            //Debug.LogError((a as Animator).);
        }


        //Object anim = AssetDatabase.LoadAssetAtPath("Assets/Resources/Test.controller", (typeof(Object)));
        //// Add an animation clip to it
        //AnimationClip animationClip = new AnimationClip();
        //animationClip.name = "SomeClip";        
        //AssetDatabase.AddObjectToAsset(animationClip, anim);
        //// Reimport the asset after adding an object.
        //// Otherwise the change only shows up when saving the project
        //AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animationClip));

        //AssetDatabase.DeleteAsset()
    }
}
