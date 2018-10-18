using System.IO;
using UnityEngine;
using Galaxy.Api;

public class Storage : MonoBehaviour {

	#region Variables
	FileShareListener fileShareListener;
	SharedFileDownloadListener sharedFileDownloadListener;
	SpecificUserDataListener specificUserDataListener;
	#endregion

	#region Behaviors
	void OnEnable () 
	{
		ListenersInit();
	}
	
	void OnDisable () 
	{
		ListenersDispose();
	}
	#endregion

	#region Listener methods
	private void ListenersInit ()
	{
		if (fileShareListener == null) fileShareListener = new FileShareListener();
		if (sharedFileDownloadListener == null) sharedFileDownloadListener = new SharedFileDownloadListener();
		if (specificUserDataListener == null) specificUserDataListener = new SpecificUserDataListener();
	}

	private void ListenersDispose ()
	{
		if (fileShareListener != null) fileShareListener.Dispose();
		if (sharedFileDownloadListener != null) sharedFileDownloadListener.Dispose();
		if (specificUserDataListener != null) specificUserDataListener.Dispose();
	}
	#endregion

	#region Methods

	/* This method reads a specified file from your hard-drive and moves it to local storage
	In normal scenarios you can use Galaxy SDK FileWrite method to write bytes in local storage */
	public void MoveFileToLocalStorage (string pathToInputFile)
	{
		string fileName = null;
		byte[] data = null;

		if (File.Exists(pathToInputFile))
		{
			fileName = Path.GetFileName(pathToInputFile);
			data = File.ReadAllBytes(pathToInputFile);
		}
		else
		{
			Debug.Log("File " + pathToInputFile + " does not exist");
			return;
		}
		
		try
		{
			Debug.Log("Writing file " + fileName + " to local storage");
			GalaxyInstance.Storage().FileWrite(fileName, data, (uint)data.Length);
		} 
		catch (GalaxyInstance.Error e)
		{
			Debug.LogWarning ("Could not write file " + fileName + " to local storage for reason: " + e);
		}
	}

	/* Removes specified file from local storage */
	public void RemoveFileFromLocalStorage (string fileName)
	{
		try
		{
			if (GalaxyInstance.Storage().FileExists(fileName)) 
			{
				GalaxyInstance.Storage().FileDelete(fileName);
			}
			else
			{
				Debug.Log("File " + fileName + " could not be removed because it does not exist");
			} 
		}
		catch (GalaxyInstance.Error e)
		{
			Debug.LogWarning("Could not remove file " + fileName + " for reason " + e);
		}
	}

	/* Uploads specified file from local storage to online storage */
	public void ShareFileFromLocalStorage (string fileName)
	{
		try
		{
			Debug.Log("GalaxyInstance.Storage.FileShare method started");
			GalaxyInstance.Storage().FileShare(fileName);
		} 
		catch (GalaxyInstance.Error e)
		{
			Debug.LogWarning ("Error " + e + " occured during execution of GalaxyInstance.Storage.FileShare method");
		}
	}

	/* Share all files present in the local storage */
	public void ShareAllFilesFromLocalStorage ()
	{
		uint fileCount = GalaxyInstance.Storage().GetFileCount();
		string fileName = null;
		try
		{
			for (uint i = 0; i < fileCount; i++)
			{		
				fileName = GalaxyInstance.Storage().GetFileNameByIndex(i);
				ShareFileFromLocalStorage(fileName);
				Debug.Log("File " + fileName + " share request sent");
			}
		}
		catch (GalaxyInstance.Error e)
		{
			Debug.LogWarning("Sharing one of the files from local storage failed for reason: " + e);
		}
	}

	/* Lists all files present in the local storage */
	public string[] ListAllFilesFromOnlineStorage ()
	{
		uint fileCount = 0;
		string[] nameList;
		try
		{
			fileCount = GalaxyInstance.Storage().GetFileCount();
			nameList = new string[fileCount];
			for (uint i = 0; i < fileCount; i++)
			{
				nameList[i] = GalaxyInstance.Storage().GetFileNameByIndex(i);
			}
			Debug.Log("List of files in storage received.");
			return nameList;
		}
		catch (GalaxyInstance.Error e)
		{
			Debug.LogWarning("Getting file list from storage failed for reason: " + e);
		}
		return null;
	}

	/* Downloads a file share */
	public void DownloadSharedFileFromUser (string fileName, GalaxyID userID)
	{
		specificUserDataListener.fileName = fileName;
		try
		{
			GalaxyInstance.User().RequestUserData(userID);
		}
		catch (GalaxyInstance.Error e)
		{
			Debug.Log("Could not request user data for reason " + e);
		}
	}
	
	#endregion

	#region Listeners
	public class SpecificUserDataListener : GlobalSpecificUserDataListener
	{
		public string fileName = null;
		public override void OnSpecificUserDataUpdated(GalaxyID userID)
		{
			Debug.Log("User " + userID + " data received");
			DownloadSharedFile(GetSharedFileIDFromUser(userID), userID);
		}

		public ulong GetSharedFileIDFromUser (GalaxyID userID)
		{
			ulong sharedFileID = 0;
			if (fileName == null) return sharedFileID;
			try
			{
				sharedFileID = ulong.Parse(GalaxyInstance.User().GetUserData(fileName, userID));
			}
			catch (GalaxyInstance.Error e)
			{
				Debug.Log("Could not get SharedFileID for file " + fileName + " for user " + userID.ToString() + " for reason " + e);
			}
			fileName = null;
			return sharedFileID;
		}

		public void DownloadSharedFile (ulong sharedFileID, GalaxyID userID)
		{
			if (sharedFileID == 0) return;
			try
			{
				GalaxyManager.Instance.Storage.sharedFileDownloadListener.userID = userID.ToString();
				GalaxyInstance.Storage().DownloadSharedFile(sharedFileID);
			}
			catch (GalaxyInstance.Error e)
			{
				Debug.LogWarning("Could not download shared file for reason " + e);
			}
		}

	}

	public class FileShareListener : GlobalFileShareListener
	{
		public override void OnFileShareSuccess(string fileName, ulong sharedFileID)
		{
			AssignSharedFileIDToUser(fileName, sharedFileID);
			Debug.Log("File " + fileName + " was shared and assigned ID " + sharedFileID);
		}

		public override void OnFileShareFailure(string fileName, FailureReason failureReason)
		{
			Debug.Log("Failed to share file " + fileName + " for reason " + failureReason);
		}

		void AssignSharedFileIDToUser (string fileName, ulong sharedFileID) 
		{
			try
			{
				GalaxyInstance.User().SetUserData(fileName, sharedFileID.ToString());
			}
			catch (GalaxyInstance.Error e)
			{
				Debug.LogWarning("Could not assign file " + fileName + " to user for reason " + e);
			}
		}
	}

	public class SharedFileDownloadListener : GlobalSharedFileDownloadListener 
	{
		public string userID = null;
		public override void OnSharedFileDownloadSuccess(ulong sharedFileID, string fileName)
		{
			Debug.Log("File with ID " + sharedFileID + " downloaded, written with name " + fileName);
			DownloadedSharedFileWrite(sharedFileID, fileName);
			userID = null;
		}

		public override void OnSharedFileDownloadFailure(ulong sharedFileID, FailureReason failureReason)
		{
			Debug.Log("Failed to download file with ID " + sharedFileID + " for reason " + failureReason);
			userID = null;
		}

		void DownloadedSharedFileWrite (ulong sharedFileID, string fileName)
		{	
			try
			{
				uint downloadedSharedFileSize = GalaxyInstance.Storage().GetSharedFileSize(sharedFileID);
				byte[] downloadedSharedFileBuffer = new byte [downloadedSharedFileSize];
				if (userID != null)
				{
					GalaxyInstance.Storage().SharedFileRead(sharedFileID, downloadedSharedFileBuffer, downloadedSharedFileSize);
					string saveFileName = userID + "/" + fileName;
					GalaxyInstance.Storage().FileWrite(saveFileName, downloadedSharedFileBuffer, downloadedSharedFileSize);
					GalaxyInstance.Storage().SharedFileClose(sharedFileID);
					userID = null;
				}
				else
				{
					Debug.LogWarning("Failed to download file with ID " + sharedFileID + " because SharedFileDownloadListener.userID is null");
				}
			}
			catch (GalaxyInstance.Error e)
			{
				Debug.LogWarning(e);
			}
		}
	}
	#endregion
}
