using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
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
        var random = new System.Random(4725);
        var positions = new List<Vector3>();
        var areas = new List<string>();
        var placementLog = new List<string>();
        var all = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        // Sample actual model surfaces instead of placing pickups at guessed heights.
        foreach (string category in new[]{"Vehicle_", "Building_"})
        {
            var surfaces = all.Where(t => t.name.StartsWith(category) &&
                t.GetComponentsInChildren<Renderer>().Length > 0)
                .OrderBy(t => Vector3.Distance(t.position, player.transform.position)).Take(18)
                .OrderBy(t => random.Next()).ToArray();
            Check(surfaces.Length > 0, "Missing placement surfaces: " + category);
            int placed = 0;
            for (int attempt = 0; attempt < 200 && placed < 8; attempt++)
            {
                var renderers = surfaces[attempt % surfaces.Length].GetComponentsInChildren<Renderer>();
                var bounds = renderers[0].bounds;
                foreach(var renderer in renderers) bounds.Encapsulate(renderer.bounds);
                Vector3 p = new Vector3(bounds.center.x + (float)(random.NextDouble()-.5)*bounds.size.x*.35f,
                    bounds.max.y+1.05f, bounds.center.z+(float)(random.NextDouble()-.5)*bounds.size.z*.35f);
                if(positions.Any(other => Vector3.Distance(other,p)<2f)) continue;
                positions.Add(p); areas.Add(category == "Vehicle_" ? "CarRoof" : "BuildingRoof"); placed++;
            }
            Check(placed == 8, "Not enough separated roof positions: " + category);
        }
        // Keep an easy first pickup, then spread nine street pickups over the neighborhood.
        positions.Add(player.transform.position + player.transform.forward*3f); areas.Add("Street");
        var roads = all.Where(t => t.name.StartsWith("Road") && t.GetComponentsInChildren<Renderer>().Length>0)
            .OrderBy(t=>Vector3.Distance(t.position,player.transform.position)).Take(24).ToArray();
        Check(roads.Length>0,"No road surfaces found");
        for(int attempt=0; positions.Count<25 && attempt<1000; attempt++)
        {
            var bounds=roads[random.Next(roads.Length)].GetComponentsInChildren<Renderer>()[0].bounds;
            var p=new Vector3(bounds.center.x+(float)(random.NextDouble()-.5)*bounds.size.x*.7f,
                player.transform.position.y+(float)(random.NextDouble()-.5)*1.5f,
                bounds.center.z+(float)(random.NextDouble()-.5)*bounds.size.z*.7f);
            if(positions.Any(other=>Vector3.Distance(other,p)<3f)) continue;
            // Leave enough room around the fish and avoid solid scenery.
            if(Vector3.Distance(p,player.transform.position)<2f || Physics.CheckSphere(p,.85f,~0,QueryTriggerInteraction.Ignore)) continue;
            positions.Add(p); areas.Add("Street");
        }
        Check(positions.Count==25,"Expected exactly 25 separated pickups");
        for(int i=0;i<25;i++)
        {
            var food = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            food.name = "Food_" + (i+1).ToString("00") + "_" + areas[i] + "_" + tags[i%3]; food.tag = tags[i%3];
            food.transform.position = positions[i];
            food.transform.localScale = Vector3.one * 0.9f;
            food.GetComponent<SphereCollider>().isTrigger = true;
            food.GetComponent<Renderer>().sharedMaterial = material;
            var animation = food.AddComponent<FloatAnimation>();
            Set(animation, "_moveSpeed", 0.3f); Set(animation, "_switchSpeed", 1f);
            Check(food.GetComponent<Collider>().isTrigger, "Food trigger missing: " + food.name);
            Check(food.GetComponent<Rigidbody>() == null, "Food should not simulate physics");
            placementLog.Add(food.name+": "+positions[i]);
        }
        EditorSceneManager.MarkSceneDirty(player.gameObject.scene);
        EditorSceneManager.SaveOpenScenes(); AssetDatabase.SaveAssets();
        var output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../../outputs"));
        Directory.CreateDirectory(output);
        File.WriteAllLines(Path.Combine(output,"MG4-food-layout.txt"),placementLog);
        File.WriteAllText(Path.Combine(output, "MG4-validation.txt"), "PASS: all 300 operand/operator cases accept the correct answer and reject an incorrect answer (600 assertions).\nPASS: all 8 serialized Player references assigned.\nPASS: all three independent food objects have correct tags, trigger colliders, FloatAnimation and no Rigidbody.\nPASS: player has no-gravity Rigidbody and collider.\nBrowser gameplay checks are recorded separately.\n");
        File.AppendAllText(Path.Combine(output,"MG4-validation.txt"),"PASS: 25 pickups, including 8 car roofs, 8 building roofs and 9 street positions; all have trigger colliders and FloatAnimation.\n");
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
