using System;
using System.Reflection;
using System.Collections.Generic;
using Reptile;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JudahsSpeedUtils
{
    public class TriggerTools : MonoBehaviour
    {
        public static TriggerTools Instance;

        private Core core;
        private WorldHandler world;
        private Player player;
        private bool coreHasBeenSetup;
        private bool delegateHasBeenSetup = false;

        public bool DisplayTriggerZones = false;
        private GameObject primitive;
        private Material validMaterial;
        private List<GameObject> textObjects = new List<GameObject>();
        private HashSet<GameObject> processedItems = new HashSet<GameObject>();
        private GameplayCamera gamePlayCamera;
        private Canvas canvas;

        private string[] storyObjectNames = new string[]
        {
            "story_prelude",
            "Story_Hideout",
            "story_Downhill",
            "story_Square",
            "Story_Tower",
            "story_Mall",
            "story_Pyramid",
            "storyOsaka"
        };

        public TriggerTools()
        {
            Instance = this;
        }

        public void Update()
        {
            if (!coreHasBeenSetup)
            {
                core = Core.Instance;
                if (core != null)
                {
                    world = WorldHandler.instance;
                    coreHasBeenSetup = world != null;

                    if (!delegateHasBeenSetup)
                    {
                        StageManager.OnStageInitialized += () =>
                        {
                            coreHasBeenSetup = false;
                            GetValidMaterial();
                            canvas = new GameObject().AddComponent<Canvas>();
                            canvas.transform.parent = gameObject.transform;
                            Debug.Log("creating a canvas to hold text objects" + Time.time.ToString());
                            gamePlayCamera = FindAnyObjectByType<GameplayCamera>();
                        };
                        delegateHasBeenSetup = true;
                    }
                }
            }

            if (coreHasBeenSetup)
            {
                player = world.GetCurrentPlayer();

                if (DisplayTriggerZones)
                {
                    //Debug.Log("Displaying Trigger zones " + Time.time.ToString());

                    if (validMaterial == null)
                    {
                        GetValidMaterial();
                        Debug.Log("Grabbing a valid material " + Time.time.ToString());
                    }

                    gamePlayCamera.GetComponent<Camera>().cullingMask = ~0;
                    //Debug.Log("Setting culling mask to everything " + Time.time.ToString());

                    SetupTriggerMeshes();
                    //Debug.Log("Finding and fixing invisible triggers " + Time.time.ToString());

                    canvas.enabled = true;
                    //Debug.Log("Enabling canvas " + Time.time.ToString());


                    CreateTextObjects();
                    //Debug.Log("Finding trigger objects and adding text objects " + Time.time.ToString());

                    UpdateTextObjects();
                    //Debug.Log("Updating text objects " + Time.time.ToString());

                }
                else
                {
                    canvas.enabled = false;
                    //Debug.Log("Disabling Canvas " + Time.time.ToString());

                    gamePlayCamera.GetComponent<Camera>().cullingMask = 841199383; //resets camera culling mask to default
                    //Debug.Log("Setting culling mask to default " + Time.time.ToString());

                }
            }
        }


        public void CreateTextObjects()
        {
            GameObject overlayTextGO;
            TextMeshProUGUI overlayText;
            string EventStrings = "";
            string EventListeners = "";

            foreach (var item in FindStageStoryObjects())
            {
                bool isTriggerObject = item != null && item.layer == 19;

                // Check if this item has already been processed
                if (!processedItems.Contains(item) && isTriggerObject)
                {
                    overlayTextGO = new GameObject();
                    overlayText = overlayTextGO.AddComponent<TextMeshProUGUI>();
                    if (item.TryGetComponent<ProgressObject>(out ProgressObject po))
                    {
                        Type scriptType = typeof(ProgressObject);

                        FieldInfo[] fields = scriptType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                        foreach (FieldInfo field in fields)
                        {
                            // Check if the field is of type UnityEventBase (the base class for UnityEvents)
                            if (typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(field.FieldType))
                            {
                                // Get the field's value (which is the UnityEvent)
                                UnityEngine.Events.UnityEventBase unityEvent = field.GetValue(po) as UnityEngine.Events.UnityEventBase;

                                // Print the field name and the number of listeners (subscribers) to the event
                                EventStrings = $"{field.Name}, {unityEvent.GetPersistentEventCount()}";

                                EventListeners = "";
                                for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
                                {
                                    string targetName = unityEvent.GetPersistentTarget(i).GetType().FullName;
                                    string methodName = unityEvent.GetPersistentMethodName(i);
                                    EventListeners += $"Listener {i + 1}: {targetName}.{methodName} \n";
                                }
                            }
                        }

                        overlayText.text = po.name +
                            "\nIs Active?: " + po.isActive.ToString() +
                            "\nIs Skippable?: " + po.sequenceSkippable.ToString() +
                            "\nObjective Target: " + po.beTargetForObjective.ToString() +
                            "\nEvent: " + EventStrings + " \n" + EventListeners;

                    }
                    overlayText.rectTransform.sizeDelta = new Vector2(600f, 200f);
                    overlayText.transform.position = item.transform.position + new Vector3(0f, 2f, 0f);
                    overlayText.enableAutoSizing = true;
                    overlayText.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                    overlayText.transform.SetParent(canvas.transform);
                    textObjects.Add(overlayTextGO);

                    // Mark this item as processed
                    processedItems.Add(item);
                }
            }
        }

        public void UpdateTextObjects()
        {
            if (textObjects != null)
            {
                foreach (var item in textObjects)
                {
                    item.transform.LookAt(player.transform);
                    item.transform.Rotate(Vector3.up, 180f);
                }
            }
        }

        public void SetupTriggerMeshes()
        {
            foreach (var item in FindStageStoryObjects())
            {
                bool isTriggerObject = item != null && item.layer == 19 && item.TryGetComponent(out Collider coll);

                if (isTriggerObject)
                {
                    if (!item.TryGetComponent(out MeshFilter mf))
                    {
                        mf = item.AddComponent<MeshFilter>();
                        mf.mesh = GetDefaultCubeMesh();
                        Debug.Log("Added MeshFilter to " + item.name + " " + Time.time.ToString());
                    }

                    if (!item.TryGetComponent(out MeshRenderer mr))
                    {
                        mr = item.AddComponent<MeshRenderer>();
                        mr.material = validMaterial;
                        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        Debug.Log("Added MeshRenderer to " + item.name + " " + Time.time.ToString());
                    }
                }
            }
        }

        private GameObject[] FindStageStoryObjects()
        {
            GameObject foundObject = null;

            // Loop through the names and try to find a GameObject with each name
            foreach (string name in storyObjectNames)
            {
                foundObject = GameObject.Find(name);

                // If a GameObject is found, break out of the loop
                if (foundObject != null)
                {
                    //Debug.Log("Found GameObject with name: " + name + " " + Time.time.ToString());
                    break;
                }
            }

            if (foundObject != null)
            {
                // Get all children of the foundObject
                Transform[] childTransforms = foundObject.GetComponentsInChildren<Transform>();
                List<GameObject> children = new List<GameObject>();

                foreach (Transform childTransform in childTransforms)
                {
                    // Skip the parent (foundObject) itself
                    if (childTransform.gameObject != foundObject)
                    {
                        children.Add(childTransform.gameObject);
                    }
                }

                return children.ToArray();
            }
            else
            {
                Debug.LogError("No GameObjects found with any of the specified names. " + Time.time.ToString());
                return null;
            }
        }

        //get a fresh cube mesh and store it
        public Mesh GetDefaultCubeMesh()
        {
            Mesh mf;
            primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mf = primitive.GetComponent<MeshFilter>().mesh;
            Destroy(primitive);
            return mf;
        }

        public void GetValidMaterial()
        {
            var progressObjects = GetAllGameObjects();
            foreach (var item in progressObjects)
            {
                // find a material in the scene that has is transparent and store it
                if (item.TryGetComponent(out MeshRenderer mr) && mr.material.HasFloat("_Mode") && mr.material.GetFloat("_Mode") > 2.9f)
                {
                    validMaterial = mr.material;
                    Debug.Log("Found valid material: " + validMaterial.name + " " + Time.time.ToString());
                    break;
                }
            }
        }

        public GameObject[] GetAllGameObjects()
        {
            return FindObjectsOfType<GameObject>(false);
        }
    }
}
