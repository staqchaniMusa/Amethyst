using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// used to re-import as humanoid
/// </summary>
public class MakeHumanAvatar: MonoBehaviour
{
    //avatar null = create from model
    Avatar avatarAsset;

    //assets to re-import --> soldier
    public GameObject[] asset;

    public void ReimportHuman()
    {
#if UNITY_EDITOR
        // for all the assets, re-import 
        for (int ii = 0; ii < asset.Length; ii++)
        {
            //get the asset's path
            string assetPath = AssetDatabase.GetAssetPath(asset[ii]);
            ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            // if it does not exist, do nothing
            if (modelImporter == null)
                return;

            //set the option source avatar to "null"
            modelImporter.sourceAvatar = avatarAsset;

            //set the animation type to humanoid
            modelImporter.animationType = ModelImporterAnimationType.Human;

            // apply modifications
            SerializedObject modelImporterObj = new SerializedObject(modelImporter);
            modelImporterObj.ApplyModifiedProperties();
            modelImporter.SaveAndReimport();
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ImportRecursive);

            Debug.Log("Re-Imported as humanoid: " + assetPath);
        }

#endif
    }

    public bool checkModelType()
    {


        bool result = true;
#if UNITY_EDITOR
        // for all the assets, re-import 
        for (int ii = 0; ii < asset.Length; ii++)
        {
            //get the asset's path
            string assetPath = AssetDatabase.GetAssetPath(asset[ii]);
            ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            // if it does not exist, do nothing
            if (modelImporter == null)
                return false;

            //set the option source avatar to "null"
            result = result && (modelImporter.animationType == ModelImporterAnimationType.Human);
                

        }
#endif
        return result;
    }
}
