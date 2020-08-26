using System.IO;
using UnityEngine;
using Galaxy.Api;
using Helpers;

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
	private void ListenersInit()
	{
		Listener.Create(ref fileShareListener);
		Listener.Create(ref sharedFileDownloadListener);
		Listener.Create(ref specificUserDataListener);
	}

	private void ListenersDispose()
	{
		Listener.Dispose(ref fileShareListener);
		Listener.Dispose(ref sharedFileDownloadListener);
		Listener.Dispose(ref specificUserDataListener);
	}
	#endregion

	#region Methods

	/* This method reads a specified file from your hard-drive and copies it to local storage.
	Please note that you can use Galaxy SDK FileWrite method to write bytes from memory to a 
	file in local storage i.e. create new file instead of copying an existing one. */
	public void CopyFileToLocalStorage (string absoluteInputPath)
	{
		string fileName = null;
		byte[] data = null;

		if (File.Exists(absoluteInputPath))
		{
			fileName = Path.GetFileName(absoluteInputPath);
			data = File.ReadAllBytes(absoluteInputPath);
		}
		else
		{
			Debug.Log("File " + absoluteInputPath + " does not exist");
			return;
		}
		
		try
		{
			Debug.Log("Writing file " + absoluteInputPath + " to local storage.");
			GalaxyInstance.Storage().FileWrite(fileName, data, (uint)data.Length);
		} 
		catch (GalaxyInstance.Error e)
		{
			Debug.LogWarning ("Could not write file " + absoluteInputPath + " to local storage for reason: " + e);
		}
	}

	/* Removes specified file from local storage. The file needs to be in your local storage 
	when this method is used. If the file is removed when the game is running this information
	will be shared with cloud storage when the game is closed. Files removed manually when 
	the game is not running will be downloaded again when the game is launched. */
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

	/* Uploads specified file from local storage to online storage. Uploaded file will be then
	available in cloud, meaning it can be shared with other users using SharedFileID (obtained from
	FileShareListener), and will be downloaded on all machines used by this user. */
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

	/* Share all files present in the local storage. Does the same thing as ShareFileFromLocalStorage, 
	but for all files present in local storage. */
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

	/* Lists all files present in the local storage. Files in Cloud Storage that are not present in
	the local storage will NOT be listed. */
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

	/* Gets a SharedFileID for a file with a specified name. This uses the User
	interface to read and return a value of a specified user data key. */
	public ulong GetSharedFileIDFromUser(GalaxyID userID, string fileName)
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

	/* Downloads a shared file by the userID of the file owner and its name
	This function will set the fileName var in the specificUserDataListener,
	set the userID var in the sharedFileDownloadListener, and request data
	of the file owner so that we can grab the fileSharedID */
	public void DownloadSharedFileByUserIdAndFileName(GalaxyID userID, string fileName)
	{
		specificUserDataListener.fileName = fileName;
		sharedFileDownloadListener.userID = userID.ToString();
		try
		{
			GalaxyInstance.User().RequestUserData(userID, specificUserDataListener);
		}
		catch (GalaxyInstance.Error e)
		{
			Debug.Log("Could not request user data for reason " + e);
		}
	}

	/* Download shared file using its SharedFileID */
	public void DownloadSharedFileBySharedFileID(ulong sharedFileID)
	{
		if (sharedFileID == 0) return;
		try
		{
			GalaxyInstance.Storage().DownloadSharedFile(sharedFileID);
		}
		catch (GalaxyInstance.Error e)
		{
			Debug.LogWarning("Could not download shared file for reason " + e);
		}
	}
	
	#endregion

	#region Listeners

	/* This listener waits for a result of the FileShare method. OnFileShareSuccess will fire
	when the file was successfully shared, in such case a sharedFileID will also be provided.
	sharedFileID can be used to share a file with other users. We save the sharedFileID as 
	user data using the User interface SetUserData method, so other users can retrieve this data
	without any action required from us. */
	public class FileShareListener : GlobalFileShareListener
	{
		public override void OnFileShareSuccess(string fileName, ulong sharedFileID)
		{
			try
			{
				GalaxyInstance.User().SetUserData(fileName, sharedFileID.ToString());
			}
			catch (GalaxyInstance.Error e)
			{
				Debug.LogWarning("Could not assign file " + fileName + " to user for reason " + e);
			}
			Debug.Log("File " + fileName + " was shared and assigned ID " + sharedFileID);
		}

		public override void OnFileShareFailure(string fileName, FailureReason failureReason)
		{
			Debug.Log("Failed to share file " + fileName + " for reason " + failureReason);
		}
	}

	/* Listener for the event of a file being downloaded. We use it to read the file, write it in
	local storage, appending the file with the GalaxyID of a user the file was downloaded from, 
	and then closing the file to free up memory.*/
	public class SharedFileDownloadListener : GlobalSharedFileDownloadListener 
	{
		public string userID = "0";
		public override void OnSharedFileDownloadSuccess(ulong sharedFileID, string fileName)
		{
			Debug.Log("Shared file with ID " + sharedFileID + " downloaded. Proceeding to write the file ");
			try
			{
				uint downloadedSharedFileSize = GalaxyInstance.Storage().GetSharedFileSize(sharedFileID);
				byte[] downloadedSharedFileBuffer = new byte [downloadedSharedFileSize];
				string saveFileName = userID + "/" + fileName;
				GalaxyInstance.Storage().SharedFileRead(sharedFileID, downloadedSharedFileBuffer, downloadedSharedFileSize);
				GalaxyInstance.Storage().FileWrite(saveFileName, downloadedSharedFileBuffer, downloadedSharedFileSize);
				Debug.Log("Downloaded shared file written to: " + saveFileName);
			}
			catch (GalaxyInstance.Error e)
			{
				Debug.LogWarning(e);
			}
			finally {
				GalaxyInstance.Storage().SharedFileClose(sharedFileID);
				userID = "0";
			}
		}

		public override void OnSharedFileDownloadFailure(ulong sharedFileID, FailureReason failureReason)
		{
			Debug.Log("Failed to download file with ID " + sharedFileID + " for reason " + failureReason);
			userID = "0";
		}
	}

	/* This specific listener is used in pair with DownloadSharedFileByUserIdAndFileName method, because
	before we can retrieve the sharedFileID from users data we need to request that data first. Once we
	get the requested data we check the sharedFileID for the given userID and file name and download the file.*/
	public class SpecificUserDataListener : ISpecificUserDataListener
	{
		public string fileName = "";
		private ulong sharedFileID = 0;

		public override void OnSpecificUserDataUpdated(GalaxyID userID)
		{
			Debug.Log("User " + userID + " data received");
			sharedFileID = GalaxyManager.Instance.Storage.GetSharedFileIDFromUser(userID, fileName);
			GalaxyManager.Instance.Storage.DownloadSharedFileBySharedFileID(sharedFileID);
			fileName = "";
			sharedFileID = 0;
		}

	}

	#endregion
}
