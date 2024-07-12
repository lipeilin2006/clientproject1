using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

public class BtnScript : MonoBehaviour
{
	public static string packageVersion;
	public Text msgs;
	public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	/// <summary>
	/// 联机获取最新版本号
	/// </summary>
	public void GetVersion()
	{
		StartCoroutine(UpdatePackageVersion());
	}
	/// <summary>
	/// 联机获取该版本号对应的资源包列表
	/// </summary>
	public void GetManifest()
	{
		StartCoroutine(UpdatePackageManifest(packageVersion));
	}
	/// <summary>
	/// 开始下载资源包
	/// </summary>
	public void StartDownload()
	{
		StartCoroutine(Download());
	}
	/// <summary>
	/// 进入游戏
	/// </summary>
	public void EnterGame()
	{
		StartCoroutine(EnterScene());
	}
	private IEnumerator UpdatePackageVersion()
	{
		var package = YooAssets.GetPackage("DefaultPackage");
		var operation = package.UpdatePackageVersionAsync();
		yield return operation;
		if (operation.Status == EOperationStatus.Succeed)
		{
			//更新成功
			packageVersion = operation.PackageVersion;
			msgs.text = $"获取成功，版本号：{packageVersion}";
			Debug.Log($"Updated package Version : {packageVersion}");
		}
		else
		{
			//更新失败
			Debug.LogError(operation.Error);
		}
	}

	private IEnumerator UpdatePackageManifest(string packageVersion)
	{
		// 更新成功后自动保存版本号，作为下次初始化的版本。
		// 也可以通过operation.SavePackageVersion()方法保存。
		bool savePackageVersion = true;
		var package = YooAssets.GetPackage("DefaultPackage");
		var operation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion);
		yield return operation;

		if (operation.Status == EOperationStatus.Succeed)
		{
			//更新成功
			msgs.text = $"资源清单获取成功";
		}
		else
		{
			//更新失败
			Debug.LogError(operation.Error);
		}
	}

	IEnumerator Download()
	{
		int downloadingMaxNum = 10;
		int failedTryAgain = 3;
		var package = YooAssets.GetPackage("DefaultPackage");
		var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

		//没有需要下载的资源
		if (downloader.TotalDownloadCount == 0)
		{
			yield break;
		}

		//需要下载的文件总数和总大小
		int totalDownloadCount = downloader.TotalDownloadCount;
		long totalDownloadBytes = downloader.TotalDownloadBytes;

		//注册回调方法
		downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
		downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
		downloader.OnDownloadOverCallback = OnDownloadOverFunction;
		downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

		//开启下载
		downloader.BeginDownload();
		msgs.text = "下载中";
		yield return downloader;

		//检测下载结果
		if (downloader.Status == EOperationStatus.Succeed)
		{
			//下载成功
			msgs.text = "下载成功";
		}
		else
		{
			//下载失败
		}
	}

	/// <summary>
	/// 使用协程加载热更资源包中的场景
	/// </summary>
	/// <returns></returns>
	IEnumerator EnterScene()
	{
		msgs.text = "正在进入游戏";
		var package = YooAssets.GetPackage("DefaultPackage");

		string location = "Assets/HotUpdate/Scene/Scene1";
		var sceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single;
		bool suspendLoad = false;
		SceneHandle handle = package.LoadSceneAsync(location, sceneMode, suspendLoad);
		yield return handle;
		Debug.Log($"Scene name is {handle.SceneName}");
	}


	void OnDownloadErrorFunction(string filename, string error)
	{
		Debug.LogError($"Error {error} downloading {filename}");
	}
	/// <summary>
	/// 更新下载进度条
	/// </summary>
	/// <param name="totalDownloadCount"></param>
	/// <param name="currentDownloadCount"></param>
	/// <param name="totalDownloadBytes"></param>
	/// <param name="currentDownloadBytes"></param>
	void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
	{
		slider.maxValue = totalDownloadBytes;
		slider.value = currentDownloadBytes;
		Debug.Log($"totalDownloadCount:{totalDownloadCount},currentDownloadCount:{currentDownloadCount},totalDownloadBytes:{totalDownloadBytes},currentDownloadBytes:{currentDownloadBytes}");
	}
	void OnDownloadOverFunction(bool isSucceed)
	{
		Debug.Log(isSucceed ? "Succeed" : "Failed");
	}
	void OnStartDownloadFileFunction(string fileName, long sizeBytes)
	{
		Debug.Log($"Start download {fileName} size :{sizeBytes}");
	}
}
