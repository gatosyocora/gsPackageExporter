﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gatosyocora
{
    public class gsPackageExporter : EditorWindow
    {
        private const string exportPackageText = "Export Package by gs";

        private static List<UnityEngine.Object> assets;
        private static string[] assetPaths;
        
        private static string ext = "unitypackage";

        private Vector2 scrollPos = Vector2.zero;
        private bool isCreateGitignore = true;

        [MenuItem("Assets/" + exportPackageText, false, 0)]
        private static void Create()
        {
            GetWindow<gsPackageExporter>("gsPackageExporter");
            assetPaths = GetSelectedAssetPathsWithChildFiles();
        }

#if UNITY_EDITOR

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Include Assets");
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, GUI.skin.box))
            {
                scrollPos = scrollView.scrollPosition;

                if (assetPaths != null)
                {
                    foreach (var assetPath in assetPaths)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Label(AssetDatabase.GetCachedIcon(assetPath), GUILayout.Width(20), GUILayout.Height(20));
                            EditorGUILayout.LabelField(Path.GetFileName(assetPath));
                        }
                    }
                }
            }

            isCreateGitignore = EditorGUILayout.ToggleLeft("Create gitignore", isCreateGitignore);
            EditorGUILayout.LabelField("Assets Count", ""+assetPaths.Length);

            if (GUILayout.Button("Export"))
            {
                ExportPackage(ref assetPaths, isCreateGitignore);
            }
        }

#endif

        /// <summary>
        /// 選択されているAssetsフォルダ以下のアセットのパスをすべて取得
        /// </summary>
        /// <returns></returns>
        private static string[] GetSelectedAssetPathsWithChildFiles()
        {
            var assetPaths = new List<string>();

            foreach (var asset in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                var assetPath = AssetDatabase.GetAssetPath(asset);

                // フォルダのとき, フォルダ以下のすべてのファイルを取得
                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    // metaファイルは除外
                    var childFilePaths = Directory.GetFiles(assetPath, "*", SearchOption.AllDirectories).Where(s => !s.EndsWith("meta")).ToArray();
                    assetPaths.AddRange(childFilePaths);
                }
                // ファイルのとき
                else
                {
                    assetPaths.Add(assetPath);
                }
            }

            return assetPaths.ToArray();
        }

        /// <summary>
        /// UnityPackageを出力
        /// </summary>
        /// <param name="exportAssetPaths"></param>
        /// <param name="saveFolderPath"></param>
        /// <param name="isCreateGitignore"></param>
        /// <returns></returns>
        private bool ExportPackage(ref string[] exportAssetPaths, bool isCreateGitignore)
        {
            if (exportAssetPaths.Length <= 0) return false;

            // 保存先を選択
            var saveFilePath = EditorUtility.SaveFilePanel("Save Some Asset", "", "", ext);

            // unitypackageを作成
            AssetDatabase.ExportPackage(exportAssetPaths, saveFilePath, ExportPackageOptions.Interactive);

            // .gitignoreを作成
            if (isCreateGitignore)
            {
                var gitignoreFilePath = Path.GetDirectoryName(saveFilePath) + "\\.gitignore";
                CreateGitignoreToUnityProject(gitignoreFilePath, exportAssetPaths);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveFilePath"></param>
        /// <param name="containFilePaths">Assets/から始まるgit管理するファイルのパス</param>
        /// <returns></returns>
        private bool CreateGitignoreToUnityProject(string saveFilePath, string[] containFilePaths)
        {
            using (var writer = new StreamWriter(saveFilePath))
            {
                var br = Environment.NewLine;

                //一度すべて除外するようにする
                writer.WriteLine("# First all ignore");
                writer.WriteLine("*");

                writer.WriteLine("");

                // 管理下にするファイルは追跡するようにする
                writer.WriteLine("# Not ignore files");
                foreach (var filePath in containFilePaths)
                {
                    var containFilePath = filePath.Replace('\\', '/'); // 追跡したいファイル

                    var folderNames = Path.GetDirectoryName(containFilePath).Split('/');
                    
                    // 最終的に追跡したいファイルまでのフォルダそれぞれを追跡対象にして
                    // それらのフォルダ内のファイルは除外にする
                    for (int i = 0; i < folderNames.Length; i++)
                    {
                        var path = string.Join("/", folderNames.Where((name, index) => index <= i).ToArray());
                        writer.WriteLine("!/" + path + "/");
                        writer.WriteLine("/" + path + "/*");
                    }

                    writer.WriteLine("!/" + containFilePath);
                }
                
                writer.WriteLine("");

                // その他必要そうなファイルは追跡するようにする
                writer.WriteLine("# Other not ignore files");
                writer.WriteLine(   "!LICENSE" + br
                                    + "!README.md" + br
                                    + "!.git*");

                writer.Close();
            }

            return true;
        }

        #region no_use
        /// <summary>
        /// 複数選択されたアセットのパスをすべて取得する
        /// </summary>
        /// <returns></returns>
        private static string[] GetSelectedAssetPaths()
        {
            var selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            return selectedAssets.Select(asset => { return AssetDatabase.GetAssetPath(asset); }).ToArray();
        }

        /// <summary>
        /// 選択されているAssetsフォルダ以下のアセットをすべて取得
        /// </summary>
        /// <returns></returns>
        private static List<UnityEngine.Object> GetSelectedAssets()
        {
            var assets = new List<UnityEngine.Object>();

            foreach (var asset in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                var assetPath = AssetDatabase.GetAssetPath(asset);

                // フォルダのとき
                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    // metaファイルはアセット化できないため除外
                    var childFilePaths = Directory.GetFiles(assetPath, "*", SearchOption.AllDirectories).Where(s => !s.EndsWith("meta")).ToArray();
                    var childAssets = childFilePaths.Select(path => { return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path); });
                    assets.AddRange(childAssets);
                }
                // ファイルのとき
                else
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        private bool ExportPackage(ref List<UnityEngine.Object> assets, string saveFilePath, bool isCreateGitignore)
        {
            // パスに変換
            var exportAssetPaths = assets.Select(asset => { return AssetDatabase.GetAssetPath(asset); }).ToArray();

            return ExportPackage(ref exportAssetPaths, isCreateGitignore);
        }

        #endregion

    }
}

