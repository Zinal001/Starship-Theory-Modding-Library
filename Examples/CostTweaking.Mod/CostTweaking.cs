using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using StarshipTheory.ModLib;
using StarshipTheory.ModLib.Resources;
using StarshipTheory.ModLib.GUI;

namespace CostTweaking.Mod
{
    public class CostTweaking : AbstractMod
    {
        private GameObject _Manager;

        public override string ModName => "Cost Tweaking";

        public override Version ModVersion => new Version("1.0.0");

        public override void OnInitialize()
        {
            _Manager = GameObject.Find("Manager");
        }

        public override void FirstGUIPass()
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
                Group costGroup = new Group(GUIItem.Direction.Vertical);
                costGroup.Items.Add(new Label(cost.Name) { Style = captionStyle });
                foreach (String resource in resources)
                {
                    Group resGroup = new Group(GUIItem.Direction.Horizontal);
                    resGroup.Items.Add(new Label(resource));
                    TextField resField = new TextField(cost.GetCost(resource).ToString())
                    {
                        Tag = resource,
                        Options = new GUILayoutOption[] { GUILayout.Width(200) }
                    };
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

            ModWindow.Items.Add(view);
        }

        private void ResBtn_Clicked(GUIItem item)
        {
            Button btn = item as Button;
        }

        private void ResField_TextChanged(GUIItem item)
        {
            TextField textField = item as TextField;
        }

        public override void OnGameStarted()
        {
            base.OnGameStarted();
        }
    }
}
