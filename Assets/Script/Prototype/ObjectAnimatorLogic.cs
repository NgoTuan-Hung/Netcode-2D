using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class ObjectAnimatorLogic : NetworkBehaviour
    {
        private ObjectInfoPacked objectInfoPacked;

        private void Awake() 
        {
            objectInfoPacked = GetComponent<ObjectInfoPacked>();
        }

        [Rpc(SendTo.Server)]
        public void ChangeBoolServerRpc(string boolName, bool value)
        {
            objectInfoPacked.Animator.SetBool(boolName, value);
        }

        // public void ChangeSateServerRpc(string state)
        // {
            
        // }
    }
}