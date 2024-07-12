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
	/// ������ȡ���°汾��
	/// </summary>
	public void GetVersion()
	{
		StartCoroutine(UpdatePackageVersion());
	}
	/// <summary>
	/// ������ȡ�ð汾�Ŷ�Ӧ����Դ���б�
	/// </summary>
	public void GetManifest()
	{
		StartCoroutine(UpdatePackageManifest(packageVersion));
	}
	/// <summary>
	/// ��ʼ������Դ��
	/// </summary>
	public void StartDownload()
	{
		StartCoroutine(Download());
	}
	/// <summary>
	/// ������Ϸ
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
			//���³ɹ�
			packageVersion = operation.PackageVersion;
			msgs.text = $"��ȡ�ɹ����汾�ţ�{packageVersion}";
			Debug.Log($"Updated package Version : {packageVersion}");
		}
		else
		{
			//����ʧ��
			Debug.LogError(operation.Error);
		}
	}

	private IEnumerator UpdatePackageManifest(string packageVersion)
	{
		// ���³ɹ����Զ�����汾�ţ���Ϊ�´γ�ʼ���İ汾��
		// Ҳ����ͨ��operation.SavePackageVersion()�������档
		bool savePackageVersion = true;
		var package = YooAssets.GetPackage("DefaultPackage");
		var operation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion);
		yield return operation;

		if (operation.Status == EOperationStatus.Succeed)
		{
			//���³ɹ�
			msgs.text = $"��Դ�嵥��ȡ�ɹ�";
		}
		else
		{
			//����ʧ��
			Debug.LogError(operation.Error);
		}
	}

	IEnumerator Download()
	{
		int downloadingMaxNum = 10;
		int failedTryAgain = 3;
		var package = YooAssets.GetPackage("DefaultPackage");
		var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

		//û����Ҫ���ص���Դ
		if (downloader.TotalDownloadCount == 0)
		{
			yield break;
		}

		//��Ҫ���ص��ļ��������ܴ�С
		int totalDownloadCount = downloader.TotalDownloadCount;
		long totalDownloadBytes = downloader.TotalDownloadBytes;

		//ע��ص�����
		downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
		downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
		downloader.OnDownloadOverCallback = OnDownloadOverFunction;
		downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

		//��������
		downloader.BeginDownload();
		msgs.text = "������";
		yield return downloader;

		//������ؽ��
		if (downloader.Status == EOperationStatus.Succeed)
		{
			//���سɹ�
			msgs.text = "���سɹ�";
		}
		else
		{
			//����ʧ��
		}
	}

	/// <summary>
	/// ʹ��Э�̼����ȸ���Դ���еĳ���
	/// </summary>
	/// <returns></returns>
	IEnumerator EnterScene()
	{
		msgs.text = "���ڽ�����Ϸ";
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
	/// �������ؽ�����
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
