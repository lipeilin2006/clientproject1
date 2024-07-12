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

		//����Ĭ�ϵ���Դ��
		var package = YooAssets.CreatePackage("DefaultPackage");

		//���ø���Դ��ΪĬ�ϵ���Դ��������ʹ��YooAssets��ؼ��ؽӿڼ��ظ���Դ�����ݡ�
		YooAssets.SetDefaultPackage(package);

		//���÷�������ַ
		string defaultHostServer = "http://127.0.0.1:8000/";
		string fallbackHostServer = "http://127.0.0.1:8000/";
		//������Ϸ����ģʽΪ����ģʽ
		var initParameters = new HostPlayModeParameters();
		//��Ϸ��Դ��ѯ����
		initParameters.BuildinQueryServices = new GameQueryServices();
		//��Ϸ��Դ���ܷ���
		initParameters.DecryptionServices = new FileOffsetDecryption();
		//��Դ��ַ�ṩ����
		initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
		var initOperation = package.InitializeAsync(initParameters);
		yield return initOperation;

		if (initOperation.Status == EOperationStatus.Succeed)
		{
			Debug.Log("��Դ����ʼ���ɹ���");
		}
		else
		{
			Debug.LogError($"��Դ����ʼ��ʧ�ܣ�{initOperation.Error}");
		}
	}
	/// <summary>
	/// ��Դ��ַ�ṩ������
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
	/// ��Դ�ļ�ƫ�Ƽ��ؽ�����
	/// </summary>
	private class FileOffsetDecryption : IDecryptionServices
	{
		/// <summary>
		/// ͬ����ʽ��ȡ���ܵ���Դ������
		/// ע�⣺��������������Դ�������ͷŵ�ʱ����Զ��ͷ�
		/// </summary>
		AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
		{
			managedStream = null;
			return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
		}

		/// <summary>
		/// �첽��ʽ��ȡ���ܵ���Դ������
		/// ע�⣺��������������Դ�������ͷŵ�ʱ����Զ��ͷ�
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
