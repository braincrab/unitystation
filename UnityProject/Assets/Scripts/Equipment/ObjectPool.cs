﻿using System;
using System.Collections;
using System.Collections.Generic;
using Tilemaps.Scripts.Behaviours.Objects;
using UnityEngine;
using UnityEngine.Networking;
using Items;
using UI;
 using Random = UnityEngine.Random;

namespace Equipment
{
    //For items that are the ownership of players, the items are kept in a pool serverside and sprites references
    //sent to the client UI and playerobj
    public class ObjectPool : MonoBehaviour
    {
        public Dictionary<NetworkIdentity, ItemAttributes> currentObjects =
            new Dictionary<NetworkIdentity, ItemAttributes>();

        public void AddGameObject(GameObject obj)
        {
//            obj.transform.position = transform.position;
//            obj.transform.parent = transform;
            var objTransform = obj.GetComponent<CustomNetTransform>();
            objTransform.DisappearFromWorldServer();
            objTransform.SetParent(transform);
            //SetPosition(transform.position, false);

            NetworkIdentity id = obj.GetComponent<NetworkIdentity>();
            ItemAttributes att = obj.GetComponent<ItemAttributes>();

            if (currentObjects.ContainsKey(id))
            {
                currentObjects.Remove(id);
                currentObjects.Add(id, att);
            }
            else
            {
                currentObjects.Add(id, att);
            }
        }

        public void DestroyGameObject(GameObject gObj)
        {
            DropGameObject(gObj, Vector3.zero);
        }

        //When dropping items etc, remove them from the player equipment pool and place in scene
        public void DropGameObject(GameObject gObj, Vector3 dropPos)
        {
            NetworkIdentity id = gObj.GetComponent<NetworkIdentity>();
            if (!currentObjects.ContainsKey(id))
            {
                Debug.Log("item: " + gObj.name + "was not found in Player Equipment pool");
            }
            else
            {
                if (!dropPos.Equals(Vector3.zero))
                {
                    GameObject o = currentObjects[id].gameObject;
                    DropNow(o, dropPos);
                }
                currentObjects.Remove(id);
                gObj.GetComponent<RegisterTile>().UpdatePosition();
            }
        }

        private static void DropNow(GameObject gObj, Vector3 dropPos)
        {
           // gObj.transform.parent = GameObject.FindGameObjectWithTag("SpawnParent").transform;
           // gObj.transform.position = dropPos;
            var objTransform = gObj.GetComponent<CustomNetTransform>();
//            objTransform.AppearAtPositionServer(dropPos);
            objTransform.ForceDrop(dropPos);
            objTransform.SetParent(null);
        }
    }
}