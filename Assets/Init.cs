using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;

public class Init : MonoBehaviour
{
	private void Awake()
	{
		StartCoroutine(InitializeYooAsset());
	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private IEnumerator InitializeYooAsset()
	{
		YooAssets.Initialize();

		//创建默认的资源包
		var package = YooAssets.CreatePackage("DefaultPackage");

		//设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
		YooAssets.SetDefaultPackage(package);

		//设置服务器地址
		string defaultHostServer = "http://127.0.0.1:8000/";
		string fallbackHostServer = "http://127.0.0.1:8000/";
		//设置游戏运行模式为联机模式
		var initParameters = new HostPlayModeParameters();
		//游戏资源查询服务
		initParameters.BuildinQueryServices = new GameQueryServices();
		//游戏资源解密服务
		initParameters.DecryptionServices = new FileOffsetDecryption();
		//资源地址提供服务
		initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
		var initOperation = package.InitializeAsync(initParameters);
		yield return initOperation;

		if (initOperation.Status == EOperationStatus.Succeed)
		{
			Debug.Log("资源包初始化成功！");
		}
		else
		{
			Debug.LogError($"资源包初始化失败：{initOperation.Error}");
		}
	}
	/// <summary>
	/// 资源地址提供服务类
	/// </summary>
	class RemoteServices : IRemoteServices
	{
		private string DefaultHostServer, FallbackHostServer;
		public RemoteServices(string defaultHostServer,string fallbackHostServer)
		{
			DefaultHostServer = defaultHostServer;
			FallbackHostServer = fallbackHostServer;
		}
		public string GetRemoteFallbackURL(string fileName)
		{
			return FallbackHostServer + fileName;
		}

		public string GetRemoteMainURL(string fileName)
		{
			return DefaultHostServer + fileName;
		}
	}

	/// <summary>
	/// 资源文件偏移加载解密类
	/// </summary>
	private class FileOffsetDecryption : IDecryptionServices
	{
		/// <summary>
		/// 同步方式获取解密的资源包对象
		/// 注意：加载流对象在资源包对象释放的时候会自动释放
		/// </summary>
		AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
		{
			managedStream = null;
			return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
		}

		/// <summary>
		/// 异步方式获取解密的资源包对象
		/// 注意：加载流对象在资源包对象释放的时候会自动释放
		/// </summary>
		AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
		{
			managedStream = null;
			return AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
		}

		private static ulong GetFileOffset()
		{
			return 32;
		}
	}
}
