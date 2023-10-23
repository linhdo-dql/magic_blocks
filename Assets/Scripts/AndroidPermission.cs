using UnityEngine;

public class AndroidPermission : MonoBehaviour
{
    void Start()
    {
        RequestStoragePermission();
    }
    public void RequestStoragePermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidRuntimePermissions.Permission[] result = AndroidRuntimePermissions.RequestPermissions("android.permission.READ_EXTERNAL_STORAGE", "android.permission.WRITE_EXTERNAL_STORAGE");

        if (result[0] == AndroidRuntimePermissions.Permission.Granted && result[1] == AndroidRuntimePermissions.Permission.Granted)
        {
            Debug.Log("Quyền truy cập bộ nhớ đã được cấp!");
        }
        else
        {
            Debug.Log("Quyền truy cập bộ nhớ bị từ chối!");
        }
#endif
    }
}