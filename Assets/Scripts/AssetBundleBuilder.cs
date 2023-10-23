using UnityEditor;
#if UNITY_EDITOR
public class AssetBundleBuilder
{
    [MenuItem("Build/Build AssetBundle")]
    public static void BuildAssetBundle()
    {
        // Đường dẫn đến nơi lưu trữ AssetBundle
        string outputPath = "Assets/AssetBundles/Datas";

        // Xây dựng AssetBundle từ các tài nguyên đã chuẩn bị
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
#endif