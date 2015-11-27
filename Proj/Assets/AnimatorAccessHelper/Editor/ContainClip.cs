using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;

public class CombineAnimationClip : EditorWindow
{
    private AnimatorController controller;
    private AnimationClip sourceClip;
    private string clipName;

    [MenuItem("Assets/HayanUI/Combine AnimationClip", false, 1777)]
    static void Create()
    {
        if (Selection.activeObject is AnimatorController)
        {
            var window = CombineAnimationClip.GetWindow(typeof(CombineAnimationClip)) as CombineAnimationClip;
            window.controller = Selection.activeObject as AnimatorController;
        }
        else
        {
            EditorUtility.DisplayDialog("Error!!!", "Combine을 실행할 Animator를 선택 후\n실행해주세요.", "Close");
        }
    }

    private void OnGUI()
    {
        List<AnimationClip> clipList = new List<AnimationClip>();
        EditorGUILayout.LabelField("target clip");
        {
            controller = EditorGUILayout.ObjectField(controller, typeof(AnimatorController), false) as AnimatorController;

            if (controller == null)
                return;

            var allAsset = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(controller));
            foreach (var asset in allAsset)
            {
                if (asset is AnimationClip)
                {
                    var removeClip = asset as AnimationClip;
                    if (!clipList.Contains(removeClip))
                    {
                        clipList.Add(removeClip);
                    }
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add Clip");
        {
            EditorGUILayout.BeginVertical("box");
            sourceClip = EditorGUILayout.ObjectField(sourceClip, typeof(AnimationClip), false) as AnimationClip;

            if (sourceClip != null)
            {
                if (GUILayout.Button("Add"))
                {
                    AnimationClip destClip = AnimatorController.AllocateAnimatorClip(sourceClip.name);

                    // Copy Animation Events
                    var animationEvents = AnimationUtility.GetAnimationEvents(sourceClip);
                    for (var eventCount = 0; eventCount < animationEvents.Length; eventCount++)
                    {
                        AnimationUtility.SetAnimationEvents(destClip, animationEvents);
                    }

                    // Copy Animation Curves
                    destClip.ClearCurves();
                    AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(sourceClip, true);
                    foreach (AnimationClipCurveData curveData in curveDatas)
                    {
                        EditorCurveBinding bind = new EditorCurveBinding()
                        {
                            path = curveData.path,
                            type = curveData.type,
                            propertyName = curveData.propertyName
                        };

                        AnimationUtility.SetEditorCurve(destClip, bind, curveData.curve);
                    }

                    // Add Object To Asset
                    AssetDatabase.AddObjectToAsset(destClip, controller);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(controller));
                    AssetDatabase.Refresh();
                }
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Create New Clip");
        {
            EditorGUILayout.BeginVertical("box");

            clipName = EditorGUILayout.TextField(clipName);

            if (clipList.Exists(item => item.name == clipName) || string.IsNullOrEmpty(clipName))
            {
                EditorGUILayout.LabelField("can't create duplicate names or empty");
            }
            else
            {
                if (GUILayout.Button("create"))
                {
                    AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(clipName);
                    AssetDatabase.AddObjectToAsset(animationClip, controller);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(controller));
                    AssetDatabase.Refresh();
                }
            }
            EditorGUILayout.EndVertical();
        }


        if (clipList.Count == 0)
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("remove clip");
        {
            EditorGUILayout.BeginVertical("box");

            foreach (var removeClip in clipList)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(removeClip.name);
                if (GUILayout.Button("remove", GUILayout.Width(100)))
                {
                    Object.DestroyImmediate(removeClip, true);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(controller));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

    }
}

public class ConvertToRotationOnlyAnim
{
    [MenuItem("Assets/Convert To Rotation Animation")]
    static void ConvertToRotationAnimation()
    {
        // Get Selected Animation Clip
        AnimationClip sourceClip = Selection.activeObject as AnimationClip;
        if (sourceClip == null)
        {
            Debug.Log("Please select an animation clip");
            return;
        }

        // Rotation only anim clip will have "_rot" post fix at the end
        const string destPostfix = "_rot";

        string sourcePath = AssetDatabase.GetAssetPath(sourceClip);
        string destPath = Path.Combine(Path.GetDirectoryName(sourcePath), sourceClip.name) + destPostfix + ".anim";

        // first try to open existing clip to avoid losing reference to this animation from other meshes that are already using it.
        AnimationClip destClip = AssetDatabase.LoadAssetAtPath(destPath, typeof(AnimationClip)) as AnimationClip;
        if (destClip == null)
        {
            // existing clip not found.  Let's create a new one
            Debug.Log("creating a new rotation only animation at " + destPath);

            destClip = new AnimationClip();
            destClip.name = sourceClip.name + destPostfix;

            AssetDatabase.CreateAsset(destClip, destPath);
            AssetDatabase.Refresh();

            // and let's load it back, just to make sure it's created?
            destClip = AssetDatabase.LoadAssetAtPath(destPath, typeof(AnimationClip)) as AnimationClip;
        }

        if (destClip == null)
        {
            Debug.Log("cannot create/open the rotation only anim at " + destPath);
            return;
        }

        // clear all the existing curves from destination.
        destClip.ClearCurves();

        // Now copy only rotation curves
        AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves(sourceClip, true);
        foreach (AnimationClipCurveData curveData in curveDatas)
        {
            if (curveData.propertyName.Contains("m_LocalRotation"))
            {
                AnimationUtility.SetEditorCurve(
                    destClip,
                    curveData.path,
                    curveData.type,
                    curveData.propertyName,
                    curveData.curve
                );
            }
        }

        Debug.Log("Hooray! Coverting to rotation-only anim to " + destClip.name + " is done");
    }
}
/*
class CopyAnimationEventsWindow extends EditorWindow
{
    var source : Object;
    var destination : Object;
    var scrollPosition;
 
    @MenuItem ("Window/Copy Animation Events Window")
    static function Init ()
    {
        var window : CopyAnimationEventsWindow = EditorWindow.GetWindow(CopyAnimationEventsWindow);
    }
 
    function OnGUI ()
    {
        EditorGUILayout.BeginVertical();
 
        source = EditorGUILayout.ObjectField(source, Object);
        destination = EditorGUILayout.ObjectField(destination, Object);
 
        if ( typeof(source) != UnityEngine.AnimationClip )
        {
            source = null;
        }
 
        if ( typeof(destination) != UnityEngine.AnimationClip )
        {
            destination = null;
        }
 
        if( GUILayout.Button("Copy") )
        {
            DoCopy();
        }
 
        EditorGUILayout.EndVertical();
    }
 
    function DoCopy()
    {
        if ( source == null || destination == null )
        {
            Debug.Log("Select animation clip.");
            return;
        }
 
        var animationEvents = AnimationUtility.GetAnimationEvents( source ); 
 
        for(var eventCount = 0; eventCount < animationEvents.length; eventCount++)
        {
            AnimationUtility.SetAnimationEvents(destination, animationEvents);
        }
 
    }
}*/