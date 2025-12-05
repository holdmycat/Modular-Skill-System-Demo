//------------------------------------------------------------
// File: DynamicParallel.cs
// Created: 2025-12-05
// Purpose: Parallel composite supporting runtime addition and removal of children.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public class DynamicParallel : Parallel
    {
        static readonly ILog log = LogManager.GetLogger(typeof(DynamicParallel));

        [InspectorName("Skill Event Node Type")]
        public eSkillEventNode SkillEventNode;
        
        private List<Node> dynamicChildren;
        
        private List<Node> AllChildren;
        public List<Node> ListAllChildren => AllChildren;
        public Node[] ListChildren => Children;
        
        public DynamicParallel(Policy successPolicy, Policy failurePolicy, eSkillEventNode eventNode, params Node[] initialChildren)
            : base(successPolicy, failurePolicy, initialChildren)
        {
            dynamicChildren = new List<Node>();
            AllChildren = new List<Node>();
            SkillEventNode = eventNode;
        }

        public void AddChild(Node child)
        {
            dynamicChildren.Add(child);
            AllChildren.Clear();
            // Rebuild the children array and related bookkeeping.
            foreach (var variable in Children)
            {
                AllChildren.Add(variable);
            }

            foreach (var variable in dynamicChildren)
            {
                if (AllChildren.Contains(variable))
                {
                    continue;
                }
                AllChildren.Add(variable);
            }
            child.SetParent(this);
            child.SetRoot(RootNode);
            
            this.Children = AllChildren.ToArray();
            this.childrenCount = this.Children.Length;
            this.childrenResults[child] = false;
            //child.Start();
        }

        public void RemoveChild(Node child)
        {
            AllChildren.Clear();
            if (!dynamicChildren.Contains(child))
            {
                log.ErrorFormat("child:{0} not exists in dynamicChildren", child.Name);
                return;
            }
            dynamicChildren.Remove(child);
            
            // Rebuild the children array and related bookkeeping.
            foreach (var variable in Children)
            {
                if (variable == child)
                {
                    continue;
                }
                AllChildren.Add(variable);
            }

            foreach (var variable in dynamicChildren)
            {
                AllChildren.Add(variable);
            }
            //child.SetParent(this);
            //child.SetRoot(RootNode);
            
            this.Children = AllChildren.ToArray();
            this.childrenCount = this.Children.Length;
            //this.childrenResults[child] = false;
        }
        
        protected override void DoStart()
        {
            base.DoStart();
            // Start all dynamically added child nodes.
            // foreach (var child in dynamicChildren)
            // {
            //     if (!child.IsActive)
            //     {
            //         child.Start();
            //     }
            // }
        }
        
        protected override void DoStop()
        {
            base.DoStop();
            // Stop every child node.
            // foreach (var child in dynamicChildren)
            // {
            //     if (child.IsActive)
            //     {
            //         child.Stop();
            //     }
            // }
        }
    }
}
