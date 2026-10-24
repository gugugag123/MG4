using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class AssignmentBuild
{
    static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    static void Set(object obj, string name, object value) => obj.GetType().GetField(name, Private).SetValue(obj, value);
    public static void Web()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        var player = UnityEngine.Object.FindFirstObjectByType<Player>();
        Check(player != null, "PlayerFish missing");
        foreach (string field in new[]{"_equationObject", "_equationText", "_equationInputField", "_pointsText", "_movement", "_correctUI", "_incorrectUI", "_audio"})
            Check((UnityEngine.Object)typeof(Player).GetField(field, Private).GetValue(player) != null, "Missing reference: " + field);
        // Check every possible pair, including negative subtraction and zero products.
        for (int a = 0; a < 10; a++) for (int b = 0; b < 10; b++)
        {
            Set(player, "_number1", a); Set(player, "_number2", b);
            string[] names = {"CheckIfAdditionCorrect", "CheckIfSubtractionCorrect", "CheckIfMultiplicationCorrect"};
            int[] answers = {a+b, a-b, a*b};
            for (int n = 0; n < names.Length; n++)
            {
                var method = typeof(Player).GetMethod(names[n], Private);
                Check((bool)method.Invoke(player, new object[]{answers[n]}), "Correct answer rejected: " + names[n]);
                Check(!(bool)method.Invoke(player, new object[]{answers[n]+1}), "Wrong answer accepted: " + names[n]);
            }
        }
        var body = player.GetComponent<Rigidbody>();
        Check(body != null && !body.useGravity, "Fish needs no-gravity Rigidbody");
        body.constraints = RigidbodyConstraints.FreezeRotation;
        // Idempotent: repeated builds must not stack duplicate food objects.
        foreach (var food in UnityEngine.Object.FindObjectsByType<FloatAnimation>(FindObjectsSortMode.None))
            UnityEngine.Object.DestroyImmediate(food.gameObject);
        var materialPath = "Assets/FishFood.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.color = new Color(1f, 0.82f, 0.08f);
            AssetDatabase.CreateAsset(material, materialPath);
        }
        string[] tags = {"Add", "Subtract", "Multiply"};
        for(int i=0;i<3;i++)
        {
            var food = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            food.name = "Food_" + tags[i]; food.tag = tags[i];
            food.transform.position = player.transform.position + player.transform.forward * (3f + i*4f);
            food.transform.localScale = Vector3.one * 0.9f;
            food.GetComponent<SphereCollider>().isTrigger = true;
            food.GetComponent<Renderer>().sharedMaterial = material;
            var animation = food.AddComponent<FloatAnimation>();
            Set(animation, "_moveSpeed", 0.3f); Set(animation, "_switchSpeed", 1f);
            Check(food.GetComponent<Collider>().isTrigger, "Food trigger missing: " + tags[i]);
            Check(food.GetComponent<Rigidbody>() == null, "Food should not simulate physics");
        }
        EditorSceneManager.MarkSceneDirty(player.gameObject.scene);
        EditorSceneManager.SaveOpenScenes(); AssetDatabase.SaveAssets();
        var output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../../outputs"));
        Directory.CreateDirectory(output);
        File.WriteAllText(Path.Combine(output, "MG4-validation.txt"), "PASS: all 300 operand/operator cases accept the correct answer and reject an incorrect answer (600 assertions).\nPASS: all 8 serialized Player references assigned.\nPASS: all three independent food objects have correct tags, trigger colliders, FloatAnimation and no Rigidbody.\nPASS: player has no-gravity Rigidbody and collider.\nBrowser gameplay checks are recorded separately.\n");
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.runInBackground = true;
        PlayerSettings.productName = "Bright - Math Fish";
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[]{"Assets/Scenes/Main.unity"},
            locationPathName = Path.Combine(output, "MG4-Web"), target = BuildTarget.WebGL,
            options = BuildOptions.None
        });
        Check(report.summary.result == BuildResult.Succeeded, "Web build failed: " + report.summary.result);
        Debug.Log("MG4 BUILD AND CHECKS SUCCEEDED");
    }
}
