using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using StarshipTheory.ModLib;
using StarshipTheory.ModLib.Resources;
using StarshipTheory.ModLib.GUI;
using Events = StarshipTheory.ModLib.Events;

namespace CostTweaking.Mod
{
    public class CostTweaking : StarshipTheory.ModLib.Mod
    {
        private GameObject _Manager;

        private Dictionary<String, List<TextField>> _fields = new Dictionary<string, List<TextField>>();

        public CostTweaking()
        {
            Events.GameEvents.GameLoaded += GameEvents_GameLoaded;
            Events.GameEvents.GameSaved += GameEvents_GameSaved;
        }

        public override void OnInitialize()
        {
            _Manager = GameObject.Find("Manager");
        }

        public override void OnCreateGUI()
        {
            String[] resources = new String[] { "Metal", "Gold", "Silicon", "CPU", "Power", "TileHP", "Water", "Research" };

            ScrollView view = new ScrollView();

            GUIStyle captionStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            foreach (EntityCost cost in Costs.GetEntityCosts())
            {
                if (!_fields.ContainsKey(cost.Internal_Name))
                    _fields[cost.Internal_Name] = new List<TextField>();

                Group costGroup = new Group(GUIItem.Direction.Vertical) { Tag = cost.Internal_Name };
                costGroup.Items.Add(new Label(cost.Name) { Style = captionStyle });
                foreach (String resource in resources)
                {
                    Group resGroup = new Group(GUIItem.Direction.Horizontal);
                    resGroup.Items.Add(new Label(resource == "TileHP" ? "Structure" : resource));
                    TextField resField = new TextField(cost.GetCost(resource).ToString())
                    {
                        Tag = resource,
                        MaxWidth = 200
                    };
                    _fields[cost.Internal_Name].Add(resField);
                    resField.TextChanged += ResField_TextChanged;

                    resGroup.Items.Add(resField);
                    costGroup.Items.Add(resGroup);
                }

                Button resBtn = new Button("Save")
                {
                    Tag = costGroup
                };
                resBtn.Clicked += ResBtn_Clicked;
                costGroup.Items.Add(resBtn);

                view.Items.Add(costGroup);
            }

            ModWindow.Rect = new Rect(ModWindow.Rect.x, ModWindow.Rect.y, ModWindow.Rect.width, 200);

            ModWindow.Items.Add(view);
        }

        private void ResBtn_Clicked(GUIItem item)
        {
            Button btn = item as Button;
            Group costGroup = btn.Tag as Group;
            String cost_name = costGroup.Tag as String;

            MouseUI mUI = _Manager.GetComponent<MouseUI>()._ModGet_Manager().GetComponent<MouseUI>();

            EntityCost cost = Costs.GetEntityCost(cost_name);
            foreach (GUIItem costItem in costGroup.Items)
            {
                TextField txt = null;
                if (costItem is TextField)
                    txt = costItem as TextField;
                else if(costItem is Group)
                {
                    foreach(GUIItem gItem in ((Group)costItem).Items)
                    {
                        if(gItem is TextField)
                        {
                            txt = gItem as TextField;
                            break;
                        }
                    }
                }

                if(txt != null)
                {
                    String resource = txt.Tag as String;
                    int val;
                    if (int.TryParse(txt.Text, out val))
                    {
                        Debug.Log("Setting " + resource + " of " + cost_name + " to " + val);
                        cost.SetCost(resource, val);
                    }
                    else
                    {
                        txt.Text = cost.GetCost(resource).ToString();
                        Debug.Log("Failed to set " + resource + " of " + cost_name + ": Not a Number");
                    }
                }
            }
            Costs.SetEntityCost(cost);

            if (mUI != null)
                mUI.updateBuildInfomation("ButtonBuild" + cost.Internal_Name);

            ManagerResources res = _Manager.GetComponent<ManagerResources>();
            if (res != null && res._ModGet_TileMap() != null)
                res.updateResourcePanel(res._ModGet_TileMap());

        }

        private void ResField_TextChanged(GUIItem item)
        {
            TextField textField = item as TextField;
        }



        private void GameEvents_GameSaved(object sender, Events.GameEvents.SaveLoadEventArgs e)
        {
            ES2.Save<EntityCost>(Costs.GetEntityCosts(), e.savePath + "?tag=CostTweaking_Costs");
        }

        private void GameEvents_GameLoaded(object sender, Events.GameEvents.SaveLoadEventArgs e)
        {
            String path = e.savePath;
            if (ES2.Exists(path))
            {
                if (ES2.Exists(path + "?tag=CostTweaking_Costs"))
                {
                    EntityCost[] costsData = ES2.LoadArray<EntityCost>(path + "?tag=CostTweaking_Costs");
                    foreach (EntityCost data in costsData)
                    {
                        Costs.SetEntityCost(data);
                        if (_fields.ContainsKey(data.Internal_Name))
                        {
                            foreach (TextField field in _fields[data.Internal_Name])
                                field.Text = data.GetCost(field.Tag as String).ToString();
                        }
                    }
                }
            }
        }
    }
}
