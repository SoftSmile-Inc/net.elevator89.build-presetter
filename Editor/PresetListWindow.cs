﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Elevator89.BuildPresetter.Data;
using Elevator89.BuildPresetter.FolderHierarchy;
using UnityEditor.Build;

namespace Elevator89.BuildPresetter
{
	public class PresetListWindow : EditorWindow
	{
		private static readonly GUILayoutOption[] EmptyLayoutOption = new GUILayoutOption[0];

		private PresetList _presets;

		private Vector2 _scrollPos;
		private Vector2 _scrollPosScenes;
		private Vector2 _scrollPosResources;
		private Vector2 _scrollPosStreamingAssets;

		private object _splitterState = SplitterGUILayout.CreateSplitterState(new float[] { 33f, 33f, 33f }, new int[] { 50, 50, 50 }, null);

		[MenuItem("Build/Build with Acive Preset", false, 100)]
		private static void BuildWithAcivePreset()
		{
			PresetList presets = PresetList.Load();
			Builder.Build(presets.GetPreset(presets.ActivePresetName), false, null);
		}

		[MenuItem("Build/Build and Run with Acive Preset", false, 100)]
		private static void BuildAndRunWithAciveConfiguration()
		{
			PresetList presets = PresetList.Load();
			Builder.Build(presets.GetPreset(presets.ActivePresetName), true, null);
		}

		[MenuItem("Build/Presets...", false, 200)]
		private static void OpenSettings()
		{
			PresetListWindow window = GetWindow<PresetListWindow>(false, "Build Presets", true);
			window.minSize = new Vector2(400.0f, 380.0f);
			window.Show();
		}

		private void OnEnable()
		{
			_presets = PresetList.Load();
		}

		private void OnGUI()
		{
			_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

			GUILayout.Space(10);

			string[] presetNames = _presets.AvailablePresets.Select(prst => prst.Name).ToArray();

			int selectedPresetIndex = Array.IndexOf(presetNames, _presets.ActivePresetName);
			selectedPresetIndex = EditorGUILayout.Popup("Preset", selectedPresetIndex, presetNames);

			Preset selectedPreset = _presets.GetPreset(presetNames[selectedPresetIndex]);
			_presets.ActivePresetName = selectedPreset.Name;

			BuildMode buildMode = ShowBuildPresetsGuiAndReturnBuildPress(selectedPreset, selectedPreset.AppName);

			GUILayout.Space(5);

			GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
			{
				if (GUILayout.Button("Reload All", GUILayout.ExpandWidth(false)))
				{
					_presets = PresetList.Load();
				}
				if (GUILayout.Button("Save All", GUILayout.ExpandWidth(false)))
				{
					PresetList.Save(_presets);
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.EndScrollView();

			switch (buildMode)
			{
				case BuildMode.Build:
					Builder.Build(selectedPreset, false, null);
					break;
				case BuildMode.BuildAndRun:
					Builder.Build(selectedPreset, true, null);
					break;
				case BuildMode.DoNotBuild:
				default:
					break;
			}
		}

		private BuildMode ShowBuildPresetsGuiAndReturnBuildPress(Preset preset, string header)
		{
			BuildMode buildMode = BuildMode.DoNotBuild;

			EditorGUILayout.BeginVertical("box");
			{
				GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
				{
					EditorGUILayout.LabelField(header);
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Clone", GUILayout.ExpandWidth(false), GUILayout.MaxHeight(15)))
					{
						string[] presetNames = _presets.AvailablePresets.Select(preset => preset.Name).ToArray();

						Preset clone = new Preset
						{
							Name = GenerateName(preset.Name, presetNames),
							AppId = preset.AppId,
							AppName = preset.AppName,
							AppIconPath = preset.AppIconPath,
							BuildDirectory = preset.BuildDirectory,
							BuildFileName = preset.BuildFileName,
							BuildTarget = preset.BuildTarget,
							BuildTargetGroup = preset.BuildTargetGroup,
							ConnectWithProfiler = preset.ConnectWithProfiler,
							DefineSymbols = preset.DefineSymbols,
							DevelopmentBuild = preset.DevelopmentBuild,
							IncludedResources = new List<string>(preset.IncludedResources),
							IncludedStreamingAssets = new AssetsLists()
							{
								Files = preset.IncludedStreamingAssets.Files,
								Folders = preset.IncludedStreamingAssets.Folders
							},
							IncludedScenes = new List<string>(preset.IncludedScenes),
							InitialSceneIndex = preset.InitialSceneIndex,
							UseIncrementalGC = preset.UseIncrementalGC,
							Il2CppCodeGeneration = preset.Il2CppCodeGeneration,
							Il2CppCompilerConfiguration = preset.Il2CppCompilerConfiguration,
							IncrementalIl2CppBuild = preset.IncrementalIl2CppBuild,
							ScriptingImplementation = preset.ScriptingImplementation,
							SkipCopyPdbFiles = preset.SkipCopyPdbFiles
						};

						_presets.AddPreset(clone);
						_presets.ActivePresetName = clone.Name;
					}
					if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false), GUILayout.MaxHeight(15)))
					{
						_presets.RemovePreset(preset.Name);
						_presets.ActivePresetName = _presets.AvailablePresets[0].Name;
					}

				}
				GUILayout.EndHorizontal();

				GUILayout.Space(5);

				bool isActivePreset = preset.Name == _presets.ActivePresetName;

				preset.Name = EditorGUILayout.TextField("Preset Name", preset.Name);
				if (isActivePreset)
				{
					_presets.ActivePresetName = preset.Name;
				}

				GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
				{
					GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
					{
						preset.AppId = EditorGUILayout.TextField("App ID", preset.AppId);
						preset.AppName = EditorGUILayout.TextField("App name", preset.AppName);
						preset.AppIconPath = EditorGUILayout.TextField("App icon path", preset.AppIconPath);
					}
					GUILayout.EndVertical();

					GUI.enabled = false;
					Texture2D iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(preset.AppIconPath);
					iconTexture = (Texture2D)EditorGUILayout.ObjectField(iconTexture, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64), GUILayout.ExpandWidth(false));
					GUI.enabled = true;
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(5);

				GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
				{
					SplitterGUILayout.BeginHorizontalSplit(_splitterState);
					{
						GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
						{
							GUILayout.Label("Initial scene:");
							preset.InitialSceneIndex = EditorGUILayout.Popup(preset.InitialSceneIndex, preset.IncludedScenes.Select(scenePath => scenePath.Replace('/', '\u2215')).ToArray());

							GUILayout.Label("Scenes:");
							_scrollPosScenes = EditorGUILayout.BeginScrollView(_scrollPosScenes, EditorStyles.helpBox);
							{
								string[] allSccenes = Util.FindAllScenesPaths().ToArray();
								HashSet<string> includedScenesPaths = new HashSet<string>(preset.IncludedScenes.Where(scenePath => allSccenes.Contains(scenePath)));

								foreach (string scenePath in allSccenes)
								{
									EditorGUI.BeginChangeCheck();
									bool isSceneIncluded = EditorGUILayout.ToggleLeft(scenePath, includedScenesPaths.Contains(scenePath));
									if (EditorGUI.EndChangeCheck())
									{
										if (isSceneIncluded)
											includedScenesPaths.Add(scenePath);
										else
											includedScenesPaths.Remove(scenePath);
									}
								}
								preset.IncludedScenes = includedScenesPaths.ToList();
							}
							EditorGUILayout.EndScrollView();
						}
						GUILayout.EndVertical();

						GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
						{
							GUILayout.Label("Resources:");

							_scrollPosResources = EditorGUILayout.BeginScrollView(_scrollPosResources, EditorStyles.helpBox);
							{
								string[] allResourcesFolders = Util.FindResourcesFolders(searchIncluded: true, searchExcluded: true).ToArray();
								HashSet<string> includedResourcesFolders = new HashSet<string>(preset.IncludedResources.Where(path => allResourcesFolders.Contains(path)));

								foreach (string resourcesFolder in allResourcesFolders)
								{
									EditorGUI.BeginChangeCheck();
									bool areResourcesIncluded = EditorGUILayout.ToggleLeft(resourcesFolder, includedResourcesFolders.Contains(resourcesFolder));
									if (EditorGUI.EndChangeCheck())
									{
										if (areResourcesIncluded)
											includedResourcesFolders.Add(resourcesFolder);
										else
											includedResourcesFolders.Remove(resourcesFolder);
									}
								}
								preset.IncludedResources = includedResourcesFolders.ToList();
							}
							EditorGUILayout.EndScrollView();
						}
						GUILayout.EndVertical();

						GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
						{
							GUILayout.Label("Streaming assets:");

							// ToDo: Consider using TreeView: https://docs.unity3d.com/Manual/TreeViewAPI.html
							_scrollPosStreamingAssets = EditorGUILayout.BeginScrollView(_scrollPosStreamingAssets, EditorStyles.helpBox, GUILayout.ExpandWidth(true));
							{
								HierarchyAsset streamingAssetsHierarchy = StreamingAssetsUtil.GetStreamingAssetsHierarchyByLists(preset.IncludedStreamingAssets);
								ShowStreamingAssetsFoldersAndFiles(0, parentIsIncluded: false, streamingAssetsHierarchy);
								preset.IncludedStreamingAssets = StreamingAssetsUtil.GetAssetsListsByHierarchy(streamingAssetsHierarchy);
							}
							EditorGUILayout.EndScrollView();
						}
						GUILayout.EndVertical();
					}
					SplitterGUILayout.EndHorizontalSplit();
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(5);

				preset.BuildTargetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup("Target group", preset.BuildTargetGroup);
				preset.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Target", preset.BuildTarget);
				preset.ScriptingImplementation = (ScriptingImplementation)EditorGUILayout.EnumPopup("ScriptingImplementation", preset.ScriptingImplementation);

				preset.BuildDirectory = EditorGUILayout.TextField("Build directory", preset.BuildDirectory);
				preset.BuildFileName = EditorGUILayout.TextField("Build file name", preset.BuildFileName);
				preset.DefineSymbols = EditorGUILayout.TextField("Define symbols", preset.DefineSymbols);

				preset.IncrementalIl2CppBuild = EditorGUILayout.Toggle("Incremental IL2CPP Build", preset.IncrementalIl2CppBuild);
				preset.DevelopmentBuild = EditorGUILayout.Toggle("Development Build", preset.DevelopmentBuild);
				preset.ConnectWithProfiler = EditorGUILayout.Toggle("Connect Profiler", preset.ConnectWithProfiler);

				preset.UseIncrementalGC = EditorGUILayout.Toggle("Use incremental GC", preset.UseIncrementalGC);
				preset.Il2CppCodeGeneration = (Il2CppCodeGeneration)EditorGUILayout.EnumPopup("Il2CppCodeGeneration", preset.Il2CppCodeGeneration);
				preset.Il2CppCompilerConfiguration = (Il2CppCompilerConfiguration)EditorGUILayout.EnumPopup("Il2CppCompilerConfiguration", preset.Il2CppCompilerConfiguration);
				preset.SkipCopyPdbFiles = EditorGUILayout.Toggle("Don't copy PDB files (Windows)", preset.SkipCopyPdbFiles);

				GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
				{
					if (GUILayout.Button("Fill from Settings", GUILayout.ExpandWidth(false)))
					{
						Presetter.FillFromCurrent(preset);
					}

					if (GUILayout.Button("Apply to Settings", GUILayout.ExpandWidth(false)))
					{
						Presetter.SetCurrent(preset);
					}

					GUILayout.FlexibleSpace();

					if (GUILayout.Button("Build", GUILayout.ExpandWidth(false)))
					{
						buildMode = BuildMode.Build;
					}

					if (GUILayout.Button("Build and Run", GUILayout.ExpandWidth(false)))
					{
						buildMode = BuildMode.BuildAndRun;
					}
				}
				GUILayout.EndHorizontal();
			}

			EditorGUILayout.EndVertical();

			return buildMode;
		}

		private void ShowStreamingAssetsFoldersAndFiles(int nestingLevel, bool parentIsIncluded, HierarchyAsset hierarchyAsset)
		{
			GUIStyle style = new GUIStyle(EditorStyles.label);
			style.contentOffset = new Vector2(nestingLevel * 20f, 0f);

			GUI.enabled = !parentIsIncluded;
			EditorGUI.BeginChangeCheck();

			bool isIncluded = EditorGUILayout.ToggleLeft(hierarchyAsset.Name, hierarchyAsset.IsIncluded, style);

			if (EditorGUI.EndChangeCheck())
				hierarchyAsset.IsIncluded = isIncluded;

			GUI.enabled = true;

			if (hierarchyAsset.Children.Count > 0)
			{
				foreach (HierarchyAsset child in hierarchyAsset.Children)
					ShowStreamingAssetsFoldersAndFiles(nestingLevel + 1, hierarchyAsset.IsIncluded, child);
			}
		}

		private static string GenerateName(string desiredName, string[] otherNames)
		{
			if (otherNames.Contains(desiredName))
			{
				int i = 1;
				string generatedName = desiredName + i;
				while (otherNames.Contains(generatedName))
				{
					i++;
					generatedName = desiredName + i;
				}
				return generatedName;
			}
			return desiredName;
		}
	}
}
