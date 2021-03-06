﻿using DefaultNamespace;
using ExitGames.Client.Photon;
using LopapaGames.ScriptableObjects;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PUN
{
    public class GameSceneManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public static GameSceneManager Instance;
        public GameObject playerPrefab;
        public GameObject Authoritative;
        public GameObject _networkLaunchManager;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                Authoritative.SetActive(false);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    SpawnCharacters();
                }
                ResetTasks();
            }
            #if UNITY_EDITOR
            else if(_networkLaunchManager)
            {
                _networkLaunchManager.SetActive(true);
            }
            #endif
        }

        private void SpawnCharacters()
        {           
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(EventsConstants.GAME_STARTED, null, raiseEventOptions, SendOptions.SendReliable);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == EventsConstants.GAME_STARTED)
            {
                SpawnMe();
            }
        }

        public Float Progress;
        public Integer TotalTasks;
        public Integer CurrentTasks;
        private void ResetTasks()
        {
            Hashtable hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
            int totalTasks = (int) hashtable["TotalTasks"];
            
            TotalTasks.Value = totalTasks;
            Progress.Value = 0;
            CurrentTasks.Value = 0;
        }

        public void SpawnMe()
        {
            GameObject spawnedObject = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);

            if (PhotonNetwork.IsMasterClient)
            {
                Authoritative.SetActive(true);
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // SceneStateManager.Instance.RemovePlayer(otherPlayer);
        }

        #region PUN
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        public override void OnLeftRoom()
        {
            Leave();
        }

        private void Leave()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("Scenes/Login");
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Leave();
        }
        #endregion
    }
}
