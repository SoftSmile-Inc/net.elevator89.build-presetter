using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Elevator89.BuildPresetter.Data
{
	[Serializable]
	public class Preset
	{
		[SerializeField]
		public string Name;

		[SerializeField]
		public string AppId;

		[SerializeField]
		public string AppName;

		[SerializeField]
		public string AppIconPath;

		[SerializeField]
		public BuildTargetGroup BuildTargetGroup;

		[SerializeField]
		public BuildTarget BuildTarget;

		[SerializeField]
		public ScriptingImplementation ScriptingImplementation;

		[SerializeField]
		public bool IncrementalIl2CppBuild;

		[SerializeField]
		public List<string> IncludedScenes = new List<string>();

		[SerializeField]
		public int InitialSceneIndex;

		[SerializeField]
		public List<string> IncludedResources = new List<string>();

		[SerializeField]
		public AssetsLists IncludedStreamingAssets;

		[SerializeField]
		public string BuildDirectory;

		[SerializeField]
		public string BuildFileName;

		[SerializeField]
		public string DefineSymbols;

		[SerializeField]
		public bool DevelopmentBuild;

		[SerializeField]
		public bool ConnectWithProfiler;

		[SerializeField]
		public bool UseIncrementalGC;

		[SerializeField]
		public Il2CppCompilerConfiguration Il2CppCompilerConfiguration;

		[SerializeField]
		public Il2CppCodeGeneration Il2CppCodeGeneration;

		[SerializeField]
		public bool SkipCopyPdbFiles;
	}
}

