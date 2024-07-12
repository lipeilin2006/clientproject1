using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using System.IO;

/// <summary>
/// ��Դ�����ܷ�����
/// </summary>
public class FileOffsetEncryption : IEncryptionServices
{
	public EncryptResult Encrypt(EncryptFileInfo fileInfo)
	{
		int offset = 32;
		byte[] fileData = File.ReadAllBytes(fileInfo.FilePath);
		var encryptedData = new byte[fileData.Length + offset];
		Buffer.BlockCopy(fileData, 0, encryptedData, offset, fileData.Length);

		EncryptResult result = new EncryptResult();
		result.Encrypted = true;
		result.EncryptedData = encryptedData;
		return result;
	}
}
