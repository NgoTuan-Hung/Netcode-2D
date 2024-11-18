using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;
using Unity.Collections;
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
        public void ChangeBoolServerRpc(NativeArray<FixedString32Bytes> boolNames, bool[] values)
        {
            for (int i=0; i<boolNames.Length; i++)
            {
                objectInfoPacked.Animator.SetBool(boolNames[i].ToString(), values[i]);
            }
        }

        // public void ChangeSateServerRpc(string state)
        // {
            
        // }
    }
}